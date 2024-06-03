using AutoMapper;
using iuca.Infrastructure.Persistence;
using iuca.Application.Interfaces.Courses;
using iuca.Application.DTO.Courses;
using iuca.Domain.Entities.Courses;
using System;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;

namespace iuca.Application.Services.Courses
{
    public class ProposalCourseService : IProposalCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAnnouncementService _announcementService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public ProposalCourseService(IApplicationDbContext db,
            IMapper mapper,
            IAnnouncementService announcementService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _announcementService = announcementService;
            _userManager = userManager;
        }

        /// <summary>
        /// Set proposal course status by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="status">Proposal status</param>
        public void SetProposalCourseStatus(int proposalCourseId, int status)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException("The proposal course id is 0.", nameof(proposalCourseId));

            if (status <= 0 || status > Enum.GetValues(typeof(enu_ProposalCourseStatus)).Length)
                throw new ArgumentException($"The status with number {status} does not exist.");

            ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .Include(x => x.Course)
                .FirstOrDefault(x => x.Id == proposalCourseId);

            if (proposalCourse == null)
                throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.");

            if (status == proposalCourse.Status)
                throw new InvalidOperationException($"The proposal course already has status {Enum.GetName(typeof(enu_ProposalCourseStatus), status)}.");

            if ((status == (int)enu_ProposalCourseStatus.Pending && proposalCourse.Status == (int)enu_ProposalCourseStatus.Approved) ||
                (status == (int)enu_ProposalCourseStatus.Approved && proposalCourse.Status == (int)enu_ProposalCourseStatus.New) ||
                (status == (int)enu_ProposalCourseStatus.Rejected && proposalCourse.Status == (int)enu_ProposalCourseStatus.New) ||
                (status == (int)enu_ProposalCourseStatus.New && (proposalCourse.Status == (int)enu_ProposalCourseStatus.Approved || proposalCourse.Status == (int)enu_ProposalCourseStatus.Rejected)))
            {
                throw new InvalidOperationException("Invalid status transition.");
            }

            if (status == (int)enu_ProposalCourseStatus.Approved && proposalCourse.Course.DepartmentId != proposalCourse.Proposal.DepartmentId)
                throw new ModelValidationException("This course is from the incorrect department.", "");

            if (status == (int)enu_ProposalCourseStatus.Approved)
                _announcementService.CreateAnnouncements(new int[] { proposalCourseId });

            if (status == (int)enu_ProposalCourseStatus.Rejected)
                _announcementService.DeleteAnnouncement(proposalCourseId);

            proposalCourse.Status = status;
            _db.SaveChanges();
        }

        /// <summary>
        /// Set proposal course statuses by proposal id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="status">Proposal status</param>
        public void SetProposalCourseStatuses(int proposalId, int status)
        {
            if (proposalId == 0)
                throw new ArgumentException("The proposal id is 0.", nameof(proposalId));

            if (status <= 0 || status > Enum.GetValues(typeof(enu_ProposalCourseStatus)).Length)
                throw new ArgumentException($"The status with number {status} does not exist.");

            var proposalCourses = _db.ProposalCourses
                .Include(x => x.Proposal)
                .Include(x => x.Course)
                .Where(x => x.ProposalId == proposalId);

            if (status == (int)enu_ProposalCourseStatus.Approved)
            {
                proposalCourses = proposalCourses.Where(x => x.Status == (int)enu_ProposalCourseStatus.Pending && x.Course.DepartmentId == x.Proposal.DepartmentId);
                proposalCourses.ToList().ForEach(course => course.Status = status);

                _announcementService.CreateAnnouncements(proposalCourses.Select(x => x.Id).ToArray());
            }
            else if (status == (int)enu_ProposalCourseStatus.Pending)
            {
                proposalCourses
                    .Where(x => x.Status == (int)enu_ProposalCourseStatus.New || x.Status == (int)enu_ProposalCourseStatus.Rejected)
                    .ToList()
                    .ForEach(course => course.Status = status);
            }
            else if (status == (int)enu_ProposalCourseStatus.New)
            {
                proposalCourses
                    .Where(x => x.Status == (int)enu_ProposalCourseStatus.Pending)
                    .ToList()
                    .ForEach(course => course.Status = status);
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Create proposal course
        /// </summary>
        /// <param name="proposalCourseDTO">Proposal course</param>
        public void CreateProposalCourse(ProposalCourseDTO proposalCourseDTO)
        {
            if (proposalCourseDTO == null)
                throw new ArgumentException("The proposal course is null.");

            ProposalCourse newProposalCourse = _mapper.Map<ProposalCourse>(proposalCourseDTO);

            _db.ProposalCourses.Add(newProposalCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit proposal course by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="proposalCourseDTO">Proposal course</param>
        public void EditProposalCourse(int proposalCourseId, ProposalCourseDTO proposalCourseDTO)
        {
            if (proposalCourseDTO == null)
                throw new ArgumentNullException(nameof(proposalCourseDTO), "The proposal course is null.");

            if (proposalCourseId == 0)
                throw new ArgumentException("The proposal course id is 0.", nameof(proposalCourseId));

            ProposalCourse proposalCourse = _db.ProposalCourses.Find(proposalCourseId);

            if (proposalCourse == null)
                throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.", nameof(proposalCourseId));

            if (proposalCourse.Status == (int)enu_ProposalCourseStatus.Approved)
                throw new ModelValidationException($"Approved proposal courses cannot be edited.", "");

            if (proposalCourse.Status == (int)enu_ProposalCourseStatus.Pending)
                throw new ModelValidationException($"Pending proposal courses cannot be edited.", "");

            proposalCourse.InstructorsJson = proposalCourseDTO.InstructorsJson;
            proposalCourse.YearsOfStudyJson = proposalCourseDTO.YearsOfStudyJson;
            proposalCourse.Credits = proposalCourseDTO.Credits;
            proposalCourse.IsForAll = proposalCourseDTO.IsForAll;
            proposalCourse.Comment = proposalCourseDTO.Comment;
            proposalCourse.Schedule = proposalCourseDTO.Schedule;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete proposal course by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        public void DeleteProposalCourse(int proposalCourseId)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException($"The proposal course id is 0.");

            ProposalCourse proposalCourse = _db.ProposalCourses.Find(proposalCourseId);

            if (proposalCourse == null)
                throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.");

            if (proposalCourse.Status == (int)enu_ProposalCourseStatus.Approved)
                throw new ModelValidationException($"Approved proposal courses cannot be deleted.", "");

            if (proposalCourse.Status == (int)enu_ProposalCourseStatus.Pending)
                throw new ModelValidationException($"Pending proposal courses cannot be deleted.", "");

            _db.ProposalCourses.Remove(proposalCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Submit proposal course for approval
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        public void SubmitProposalCourseForApproval(int proposalCourseId)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException($"The proposal course id is 0.");

            var proposalCourse = _db.ProposalCourses.Find(proposalCourseId);
            if (proposalCourse is null)
                throw new Exception($"Proposal course with id {proposalCourseId} not found");

            if (proposalCourse.Status != (int)enu_ProposalCourseStatus.New &&
                proposalCourse.Status != (int)enu_ProposalCourseStatus.Rejected)
                throw new Exception("Proposal course status is wrong");

            proposalCourse.Status = (int)enu_ProposalCourseStatus.Pending;

            _db.SaveChanges();
        }

        /// <summary>
        /// Return proposal course from approval
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        public void ReturnProposalCourseFromApproval(int proposalCourseId)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException($"The proposal course id is 0.");

            var proposalCourse = _db.ProposalCourses.Find(proposalCourseId);
            if (proposalCourse is null)
                throw new Exception($"Proposal course with id {proposalCourseId} not found");

            if (proposalCourse.Status != (int)enu_ProposalCourseStatus.Pending)
                throw new Exception("Proposal course status is not pending");

            proposalCourse.Status = (int)enu_ProposalCourseStatus.New;

            _db.SaveChanges();
        }

        /// <summary>
        /// Edit proposal course instructors
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="newInstructorIds">Instructor ids</param>
        public void EditProposalCourseInstructors(int proposalCourseId, IEnumerable<string> newInstructorIds)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException("The proposal course id is 0.", nameof(proposalCourseId));

            ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .FirstOrDefault(x => x.Id == proposalCourseId);

            if (proposalCourse == null)
                throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.", nameof(proposalCourseId));

            var existingInstructorIds = proposalCourse.InstructorsJson.ToList();

            var instructorsToDelete = existingInstructorIds.Where(existingId => !newInstructorIds.Contains(existingId)).ToList();
            var instructorsToAdd = newInstructorIds.Where(newId => !existingInstructorIds.Contains(newId)).ToList();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var instructor in instructorsToDelete)
                    {
                        existingInstructorIds.Remove(instructor);
                        _announcementService.RemoveAnnouncementInstructor(proposalCourse.Proposal.SemesterId, proposalCourse.CourseId, instructor);
                    }

                    foreach (var instructor in instructorsToAdd)
                    {
                        existingInstructorIds.Add(instructor);
                    }

                    proposalCourse.InstructorsJson = existingInstructorIds;

                    _db.ProposalCourses.Update(proposalCourse);
                    _db.SaveChanges();

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

        /// <summary>
        /// Replace proposal course instructor
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="previousInstructorId">Previous instructor id</param>
        /// <param name="futureInstructorId">Future instructor id</param>
        public void ReplaceProposalCourseInstructor(int proposalCourseId, string previousInstructorId, string futureInstructorId)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException("The proposal course id is 0.", nameof(proposalCourseId));

            if (string.IsNullOrEmpty(previousInstructorId))
                throw new ArgumentException("The previous instructor id is null.", nameof(previousInstructorId));

            if (string.IsNullOrEmpty(futureInstructorId))
                throw new ArgumentException("The future instructor id is null.", nameof(futureInstructorId));

            using (var transaction = _db.Database.BeginTransaction())
            {
                ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .FirstOrDefault(x => x.Id == proposalCourseId);

                if (proposalCourse == null)
                    throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.", nameof(proposalCourseId));

                var existingInstructorIds = proposalCourse.InstructorsJson.ToList();

                var instructorToReplace = existingInstructorIds.FirstOrDefault(x => string.Equals(x, previousInstructorId));
                if (string.IsNullOrEmpty(instructorToReplace))
                    throw new ArgumentException("This instructor is not in the proposal course.", nameof(instructorToReplace));

                try
                {
                    _announcementService.ReplaceAnnouncementInstructor(proposalCourse.Proposal.SemesterId, proposalCourse.CourseId, previousInstructorId, futureInstructorId);
                    existingInstructorIds.Remove(instructorToReplace);
                    existingInstructorIds.Add(futureInstructorId);

                    proposalCourse.InstructorsJson = existingInstructorIds;

                    _db.ProposalCourses.Update(proposalCourse);
                    _db.SaveChanges();

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
    }
}