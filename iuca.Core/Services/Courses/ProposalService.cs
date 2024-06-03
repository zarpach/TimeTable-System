using AutoMapper;
using iuca.Infrastructure.Persistence;
using iuca.Application.Interfaces.Courses;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using System.Collections.Generic;
using iuca.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using iuca.Application.DTO.Users.UserInfo;
using System.Text.Json;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;

namespace iuca.Application.Services.Courses
{
    public class ProposalService : IProposalService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IProposalCourseService _proposalCourseService;
        private readonly ICourseService _courseService;

        public ProposalService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager,
            IProposalCourseService proposalCourseService,
            ICourseService courseService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _proposalCourseService = proposalCourseService;
            _courseService = courseService;
        }

        /// <summary>
        /// Get proposals
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Proposals</returns>
        public IEnumerable<ProposalDTO> GetProposals(int semesterId, IEnumerable<DepartmentDTO> deanDepartments)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            IEnumerable<Proposal> proposals = _db.Proposals
                .Include(x => x.ProposalCourses)
                .Include(x => x.Semester)
                .Include(x => x.Department)
                .Where(x => x.SemesterId == semesterId);

            if (deanDepartments != null && deanDepartments.Any())
                proposals = proposals.Where(x => deanDepartments.Select(x => x.Id).Contains(x.DepartmentId));

            return _mapper.Map<IEnumerable<ProposalDTO>>(proposals);
        }

        /// <summary>
        /// Get proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <returns>Proposal</returns>
        public ProposalDTO GetProposal(int proposalId)
        {
            if (proposalId == 0)
                throw new ArgumentException($"The proposal id is 0.");

            Proposal proposal = _db.Proposals
                .Include(x => x.Department)
                .Include(x => x.Semester)
                .Include(x => x.ProposalCourses)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Department)
                .Include(x => x.ProposalCourses)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Language)
                .FirstOrDefault(x => x.Id == proposalId);
            if (proposal == null)
                throw new ArgumentException($"The proposal with id {proposalId} does not exist.");

            var proposalDTO = _mapper.Map<ProposalDTO>(proposal);

            foreach (var proposalCourseDTO in proposalDTO.ProposalCourses)
            {
                proposalCourseDTO.Instructors = proposalCourseDTO.InstructorsJson
                    .Select(x => new UserDTO
                    {
                        Id = x,
                        FullName = _userManager.GetUserFullName(x)
                    });
            }

            return proposalDTO;
        }

        /// <summary>
        /// Create proposal
        /// </summary>
        /// <param name="proposalDTO">Proposal</param>
        public void CreateProposal(ProposalDTO proposalDTO)
        {
            if (proposalDTO == null)
                throw new ArgumentException($"The proposal is null.");

            var proposal = _db.Proposals.FirstOrDefault(x => x.SemesterId == proposalDTO.SemesterId && 
                x.DepartmentId == proposalDTO.DepartmentId);

            if (proposal != null)
                throw new ModelValidationException("The proposal for these semester and department already exists.", "");

            Proposal newProposal = _mapper.Map<Proposal>(proposalDTO);

            _db.Proposals.Add(newProposal);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="proposalDTO">Proposal</param>
        public void EditProposal(int proposalId, ProposalDTO proposalDTO)
        {
            if (proposalId == 0)
                throw new ArgumentException("The proposal id is 0.", nameof(proposalId));

            if (proposalDTO == null)
                throw new ArgumentException("The proposal is null.", nameof(proposalDTO));

            Proposal proposal = _db.Proposals.Include(x => x.ProposalCourses).FirstOrDefault(x => x.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException($"The proposal with id {proposalId} does not exist.", nameof(proposalId));

            if (proposal.SemesterId != proposalDTO.SemesterId || proposal.DepartmentId != proposalDTO.DepartmentId)
            {
                var existingProposal = _db.Proposals.FirstOrDefault(x => x.Id != proposalId && x.SemesterId == proposalDTO.SemesterId
                    && x.DepartmentId == proposalDTO.DepartmentId);

                if (existingProposal != null)
                    throw new ModelValidationException("A proposal for this semester and department already exists.", "");

                if (proposal.ProposalCourses.Any(x => x.Status == (int)enu_ProposalCourseStatus.Approved ||
                    x.Status == (int)enu_ProposalCourseStatus.Pending))
                    throw new ModelValidationException("This proposal cannot be edited due to approved or pending courses.", "");

                proposal.SemesterId = proposalDTO.SemesterId;
                proposal.DepartmentId = proposalDTO.DepartmentId;
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Edit proposal courses by proposal id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="proposalCourseDTOs">Proposal courses</param>
        public void EditProposalCourses(int proposalId, IEnumerable<ProposalCourseDTO> proposalCourseDTOs)
        {
            if (proposalId == 0)
                throw new ArgumentException("The proposal id is 0.", nameof(proposalId));

            Proposal proposal = _db.Proposals
                .Include(x => x.ProposalCourses)
                .FirstOrDefault(x => x.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException($"The proposal with id {proposalId} does not exist.", nameof(proposalId));

            var existingProposalCourses = proposal.ProposalCourses
                .Where(x => x.Status == (int)enu_ProposalCourseStatus.New || x.Status == (int)enu_ProposalCourseStatus.Rejected)
                .ToList();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (proposalCourseDTOs != null)
                    {
                        var newProposalCourses = proposalCourseDTOs
                            .Where(x => x.Status == (int)enu_ProposalCourseStatus.New || x.Status == (int)enu_ProposalCourseStatus.Rejected)
                            .ToList();

                        foreach (ProposalCourseDTO newProposalCourse in newProposalCourses)
                        {
                            newProposalCourse.ProposalId = proposalId;
                            ProposalCourse existingProposalCourse = existingProposalCourses.FirstOrDefault(x => x.Id == newProposalCourse.Id);

                            if (existingProposalCourse == null)
                                _proposalCourseService.CreateProposalCourse(newProposalCourse);
                            else
                            {
                                if (IsProposalChanged(existingProposalCourse, newProposalCourse))
                                    _proposalCourseService.EditProposalCourse(existingProposalCourse.Id, newProposalCourse);

                                existingProposalCourses.Remove(existingProposalCourse);
                            }
                        }
                    }

                    foreach (var existingProposalCourse in existingProposalCourses)
                        _proposalCourseService.DeleteProposalCourse(existingProposalCourse.Id);

                    transaction.Commit();
                }
                catch (ModelValidationException ex)
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private bool IsProposalChanged(ProposalCourse existingProposalCourse, ProposalCourseDTO newProposalCourse) 
        {
            return (existingProposalCourse.InstructorsJson?.SequenceEqual(newProposalCourse.InstructorsJson) ?? false) ||
                ((existingProposalCourse.YearsOfStudyJson == null && newProposalCourse.YearsOfStudyJson == null) ||
                (existingProposalCourse.YearsOfStudyJson?.SequenceEqual(newProposalCourse.YearsOfStudyJson) ?? false)) ||
                existingProposalCourse.Credits != newProposalCourse.Credits ||
                existingProposalCourse.IsForAll != newProposalCourse.IsForAll ||
                existingProposalCourse.Comment != newProposalCourse.Comment ||
                existingProposalCourse.Schedule != newProposalCourse.Schedule;
        }

        /// <summary>
        /// Delete proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        public void DeleteProposal(int proposalId)
        {
            if (proposalId == 0)
                throw new ArgumentException($"The proposal id is 0.");

            Proposal proposal = _db.Proposals
                .Include(x => x.ProposalCourses)
                .FirstOrDefault(x => x.Id == proposalId);
            if (proposal == null)
                throw new ArgumentException($"The proposal with id {proposalId} does not exist.");

            if (proposal.ProposalCourses != null && proposal.ProposalCourses.Any(x => x.Status == (int)enu_ProposalCourseStatus.Approved ||
                    x.Status == (int)enu_ProposalCourseStatus.Pending))
                throw new ModelValidationException("This proposal cannot be deleted due to approved or pending courses.", "");

            _db.Proposals.Remove(proposal);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="excludedCourseIds">Course ids to exclude</param>
        /// <returns>Course list without excluded course ids</returns>
        public IEnumerable<CourseDTO> GetCoursesForSelection(int organizationId, int[] excludedCourseIds)
        {
            var courses = _courseService.GetCourses(organizationId).Where(x => !excludedCourseIds.Contains(x.Id));

            return courses;
        }

        /// <summary>
        /// Get course from selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="selectedCourseId">Selected course id</param>
        /// <returns>Course</returns>
        public CourseDTO GetCourseFromSelection(int organizationId, int selectedCourseId)
        {
            if (selectedCourseId == 0)
                throw new ArgumentException($"The course id is 0.");

            CourseDTO course = _courseService.GetCourse(organizationId, selectedCourseId);
            if (course == null)
                throw new ArgumentException($"The course with id {selectedCourseId} does not exist.");

            return course;
        }
    }
}
