using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using iuca.Application.Interfaces.Courses;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Instructors;

namespace iuca.Application.Services.Courses
{
    public class SyllabusService : ISyllabusService
    {
        private readonly IApplicationDbContext _db;
        private readonly IRegistrationCourseService _registrationCourseService;
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IAcademicPolicyService _academicPolicyService;
        private readonly ICourseRequirementService _courseRequirementService;
        private readonly ICourseCalendarRowService _courseCalendarRowService;
        private readonly IPolicyService _policyService;

        public SyllabusService(IApplicationDbContext db,
            IRegistrationCourseService registrationCourseService,
            IDepartmentService departmentService,
            IInstructorInfoService instructorInfoService,
            IAcademicPolicyService academicPolicyService,
            ICourseRequirementService courseRequirementService,
            ICourseCalendarRowService courseCalendarRowService,
            IPolicyService policyService)
        {
            _db = db;
            _registrationCourseService = registrationCourseService;
            _departmentService = departmentService;
            _instructorInfoService = instructorInfoService;
            _academicPolicyService = academicPolicyService;
            _courseRequirementService = courseRequirementService;
            _courseCalendarRowService = courseCalendarRowService;
            _policyService = policyService;
        }

        /// <summary>
        /// Get syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <returns>Syllabus</returns>
        public SyllabusDTO GetSyllabusById(int syllabusId)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");

            var syllabus = _db.Syllabi
                .Include(x => x.AcademicPolicies)
                .Include(x => x.CourseRequirements)
                .Include(x => x.CourseCalendar)
                .FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Syllabus, SyllabusDTO>()
                .ForMember(dest => dest.RegistrationCourseId, opt => opt.MapFrom(src => src.AnnouncementSectionId))
                .ForMember(dest => dest.RegistrationCourse, opt => opt.MapFrom(src => src.AnnouncementSection));
                cfg.CreateMap<AcademicPolicy, AcademicPolicyDTO>();
                cfg.CreateMap<CourseRequirement, CourseRequirementDTO>();
                cfg.CreateMap<CourseCalendarRow, CourseCalendarRowDTO>();
            }).CreateMapper();

            return mapper.Map<Syllabus, SyllabusDTO>(syllabus);
        }

        /// <summary>
        /// Get syllabus by registration course id
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Syllabus</returns>
        public SyllabusDTO GetSyllabusByRegistrationCourseId(int registrationCourseId)
        {
            if (registrationCourseId == 0)
                throw new Exception($"The registration course id is 0.");

            var syllabus = _db.Syllabi
                .Include(x => x.AcademicPolicies)
                .Include(x => x.CourseRequirements)
                .Include(x => x.CourseCalendar)
                .FirstOrDefault(x => x.AnnouncementSectionId == registrationCourseId);

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Syllabus, SyllabusDTO>()
                .ForMember(dest => dest.RegistrationCourseId, opt => opt.MapFrom(src => src.AnnouncementSectionId))
                .ForMember(dest => dest.RegistrationCourse, opt => opt.MapFrom(src => src.AnnouncementSection));
                cfg.CreateMap<AcademicPolicy, AcademicPolicyDTO>();
                cfg.CreateMap<CourseRequirement, CourseRequirementDTO>();
                cfg.CreateMap<CourseCalendarRow, CourseCalendarRowDTO>();
            }).CreateMapper();

            return mapper.Map<Syllabus, SyllabusDTO>(syllabus);
        }

        /// <summary>
        /// Get syllabus details by registration course id
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Syllabus details</returns>
        public SyllabusDetailsViewModel GetSyllabusDetails(int selectedOrganizationId, int registrationCourseId)
        {
            if (registrationCourseId == 0)
                throw new Exception($"The registration course id is 0.");

            var model = new SyllabusDetailsViewModel();

            var registrationCourse = _registrationCourseService.GetRegistrationCourse(selectedOrganizationId, registrationCourseId);
            model.SemesterSeason = registrationCourse.Season;
            model.SemesterYear = registrationCourse.Year;
            model.CourseCredits = registrationCourse.Points;
            model.CourseCode = registrationCourse.Course.Abbreviation + " " + registrationCourse.Course.Number;
            model.CourseNameEng = registrationCourse.Course.NameEng;
            model.CourseNameRus = registrationCourse.Course.NameRus;

            if (registrationCourse.Course.CoursePrerequisites != null && registrationCourse.Course.CoursePrerequisites.Any())
            {
                model.CoursePrerequisitesEng = registrationCourse.Course.CoursePrerequisites.Select(x => x.Prerequisite.NameEng).ToList();
                model.CoursePrerequisitesRus = registrationCourse.Course.CoursePrerequisites.Select(x => x.Prerequisite.NameRus).ToList();
            }

            var department = _departmentService.GetDepartment(selectedOrganizationId, registrationCourse.Course.DepartmentId);
            model.DepartmentNameEng = department.NameEng;
            model.DepartmentNameRus = department.NameRus;

            var instructorDetails = _instructorInfoService.GetInstructorDetailsInfo(selectedOrganizationId, registrationCourse.InstructorUserId);
            model.InstructorName = instructorDetails.UserInfo.FullNameEng;
            model.InstructorEmail = instructorDetails.UserInfo.Email;

            model.CoursePolicies = _policyService.GetPolicies();

            return model;
        }

        /// <summary>
        /// Set syllabus status by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="status">Syllabus status</param>
        public void SetSyllabusStatus(int syllabusId, int status)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");
            if (status > Enum.GetValues(typeof(enu_SyllabusStatus)).Length || status <= 0)
                throw new Exception($"The status with number {status} does not exist.");

            var syllabus = _db.Syllabi.FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");

            if (status == (int)enu_SyllabusStatus.Pending)
            {
                if (syllabus.Status == (int)enu_SyllabusStatus.Pending)
                    throw new Exception($"The syllabus is already under approval.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Approved)
                    throw new Exception($"The approved syllabus cannot be submitted for approval.");
            } else if (status == (int)enu_SyllabusStatus.Approved)
            {
                if (syllabus.Status == (int)enu_SyllabusStatus.Approved)
                    throw new Exception($"The syllabus is already approved.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Rejected)
                    throw new Exception($"The rejected syllabus cannot be approved.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Draft)
                    throw new Exception($"The draft syllabus cannot be approved.");
            } else if (status == (int)enu_SyllabusStatus.Rejected)
            {
                if (syllabus.Status == (int)enu_SyllabusStatus.Rejected)
                    throw new Exception($"The syllabus is already rejected.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Draft)
                    throw new Exception($"The draft syllabus cannot be rejected.");
            } else if (status == (int)enu_SyllabusStatus.Draft)
            {
                if (syllabus.Status == (int)enu_SyllabusStatus.Draft)
                    throw new Exception($"The syllabus is already a draft.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Approved)
                    throw new Exception($"The approved syllabus cannot be returned from approval.");
                if (syllabus.Status == (int)enu_SyllabusStatus.Rejected)
                    throw new Exception($"The rejected syllabus cannot be returned from approval.");
            }

            syllabus.Status = status;
            _db.SaveChanges();
        }

        /// <summary>
        /// Set syllabus instructor comment by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="comment">Syllabus instructor comment</param>
        public void SetSyllabusInstructorComment(int syllabusId, string comment)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");

            var syllabus = _db.Syllabi.FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");

            syllabus.InstructorComment = comment;
            _db.SaveChanges();
        }

        /// <summary>
        /// Set syllabus approver comment by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="comment">Syllabus approver comment</param>
        public void SetSyllabusApproverComment(int syllabusId, string comment)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");

            var syllabus = _db.Syllabi.FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");

            syllabus.ApproverComment = comment;
            _db.SaveChanges();
        }

        /// <summary>
        /// Create syllabus
        /// </summary>
        /// <param name="syllabusDTO">Syllabus</param>
        public void CreateSyllabus(SyllabusDTO syllabusDTO)
        {
            if (syllabusDTO == null)
                throw new Exception($"The syllabus is null.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<SyllabusDTO, Syllabus>()
                .ForMember(dest => dest.AnnouncementSectionId, opt => opt.MapFrom(src => src.RegistrationCourseId))
                .ForMember(dest => dest.AnnouncementSection, opt => opt.MapFrom(src => src.RegistrationCourse));
                cfg.CreateMap<AcademicPolicyDTO, AcademicPolicy>();
                cfg.CreateMap<CourseRequirementDTO, CourseRequirement>();
                cfg.CreateMap<CourseCalendarRowDTO, CourseCalendarRow>();
            }).CreateMapper();

            Syllabus syllabus = mapper.Map<SyllabusDTO, Syllabus>(syllabusDTO);

            syllabus.Status = (int)enu_SyllabusStatus.Draft;
            syllabus.Created = DateTime.Now;
            syllabus.Modified = DateTime.Now;

            _db.Syllabi.Add(syllabus);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="syllabusDTO">Syllabus</param>
        public void EditSyllabus(int syllabusId, SyllabusDTO syllabusDTO)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");
            if (syllabusDTO == null)
                throw new Exception($"The syllabus is null.");

            Syllabus syllabus = _db.Syllabi
                .Include(x => x.AcademicPolicies)
                .Include(x => x.CourseRequirements)
                .Include(x => x.CourseCalendar)
                .FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");
            if (syllabus.Status == (int)enu_SyllabusStatus.Approved)
                throw new Exception($"The approved syllabus cannot be edited.");
            if (syllabus.Status == (int)enu_SyllabusStatus.Pending)
                throw new Exception($"The pending syllabus cannot be edited.");

            syllabus.AnnouncementSectionId = syllabusDTO.RegistrationCourseId;
            syllabus.InstructorPhone = syllabusDTO.InstructorPhone;
            syllabus.OfficeHours = syllabusDTO.OfficeHours;
            syllabus.CourseDescription = syllabusDTO.CourseDescription;

            syllabus.Objectives = syllabusDTO.Objectives;
            syllabus.TeachMethods = syllabusDTO.TeachMethods;
            syllabus.PrimaryResources = syllabusDTO.PrimaryResources;
            syllabus.AdditionalResources = syllabusDTO.AdditionalResources;

            syllabus.Link = syllabusDTO.Link;
            syllabus.GradingComment = syllabusDTO.GradingComment;

            syllabus.Language = syllabusDTO.Language;
            syllabus.Modified = DateTime.Now;

            _db.Syllabi.Update(syllabus);
            _db.SaveChanges();

            EditAcademicPolicies(syllabusId, syllabus.AcademicPolicies.ToList(), syllabusDTO.AcademicPolicies);
            EditCourseRequirements(syllabusId, syllabus.CourseRequirements.ToList(), syllabusDTO.CourseRequirements);
            EditCourseCalendarRows(syllabusId, syllabus.CourseCalendar.ToList(), syllabusDTO.CourseCalendar);
        }

        /// <summary>
        /// Delete syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        public void DeleteSyllabus(int syllabusId)
        {
            if (syllabusId == 0)
                throw new Exception($"The syllabus id is 0.");

            Syllabus syllabus = _db.Syllabi.FirstOrDefault(x => x.Id == syllabusId);
            if (syllabus == null)
                throw new Exception($"The syllabus with id {syllabusId} does not exist.");
            if (syllabus.Status == (int)enu_SyllabusStatus.Approved)
                throw new Exception($"The approved syllabus cannot be deleted.");
            if (syllabus.Status == (int)enu_SyllabusStatus.Pending)
                throw new Exception($"The pending syllabus cannot be deleted.");

            _db.Syllabi.Remove(syllabus);
            _db.SaveChanges();
        }

        /// <summary>
        /// Create, update, or delete academic policies
        /// </summary>
        /// <param name="existingAcademicPolicyList">List of existing academic policies</param>
        /// <param name="newAcademicPolicyList">List of new academic policies</param>
        private void EditAcademicPolicies(int syllabusId, List<AcademicPolicy> existingAcademicPolicyList, List<AcademicPolicyDTO> newAcademicPolicyList)
        {
            if (newAcademicPolicyList != null && newAcademicPolicyList.Any())
            {
                foreach (AcademicPolicyDTO academicPolicy in newAcademicPolicyList)
                {
                    academicPolicy.SyllabusId = syllabusId;
                    AcademicPolicy existingPolicy = existingAcademicPolicyList.FirstOrDefault(x => x.Id == academicPolicy.Id);

                    if (existingPolicy == null)
                        _academicPolicyService.CreateAcademicPolicy(academicPolicy);
                    else if (existingPolicy.Name != academicPolicy.Name || 
                        existingPolicy.Description != academicPolicy.Description)
                        _academicPolicyService.EditAcademicPolicy(existingPolicy.Id, academicPolicy);
                }
            }

            if (existingAcademicPolicyList != null && existingAcademicPolicyList.Any())
            {
                foreach (AcademicPolicy academicPolicy in existingAcademicPolicyList)
                {
                    if (newAcademicPolicyList == null || !newAcademicPolicyList.Any(x => x.Id == academicPolicy.Id))
                        _academicPolicyService.DeleteAcademicPolicy(academicPolicy.Id);
                }
            }
        }

        /// <summary>
        /// Create, update, or delete course requirements
        /// </summary>
        /// <param name="existingCourseRequirementList">List of existing course requirements</param>
        /// <param name="newCourseRequirementList">List of new course requirements</param>
        private void EditCourseRequirements(int syllabusId, List<CourseRequirement> existingCourseRequirementList, List<CourseRequirementDTO> newCourseRequirementList)
        {
            if (newCourseRequirementList != null && newCourseRequirementList.Any())
            {
                foreach (CourseRequirementDTO courseRequirement in newCourseRequirementList)
                {
                    courseRequirement.SyllabusId = syllabusId;
                    CourseRequirement existingRequirement = existingCourseRequirementList.FirstOrDefault(x => x.Id == courseRequirement.Id);

                    if (existingRequirement == null)
                        _courseRequirementService.CreateCourseRequirement(courseRequirement);
                    else if (existingRequirement.Name != courseRequirement.Name || 
                        existingRequirement.Description != courseRequirement.Description ||
                        existingRequirement.Points != courseRequirement.Points)
                        _courseRequirementService.EditCourseRequirement(existingRequirement.Id, courseRequirement);
                }
            }
            if (existingCourseRequirementList != null && existingCourseRequirementList.Any())
            {
                foreach (CourseRequirement courseRequirement in existingCourseRequirementList)
                {
                    if (newCourseRequirementList == null || !newCourseRequirementList.Any(x => x.Id == courseRequirement.Id))
                        _courseRequirementService.DeleteCourseRequirement(courseRequirement.Id);
                }
            }
        }

        /// <summary>
        /// Create, update, or delete course calendar rows
        /// </summary>
        /// <param name="existingCourseCalendarRowList">List of existing course calendar rows</param>
        /// <param name="newCourseCalendarRowList">List of new course calendar rows</param>
        private void EditCourseCalendarRows(int syllabusId, List<CourseCalendarRow> existingCourseCalendarRowList, List<CourseCalendarRowDTO> newCourseCalendarRowList)
        {
            if (newCourseCalendarRowList != null && newCourseCalendarRowList.Any())
            {
                foreach (CourseCalendarRowDTO courseCalendarRow in newCourseCalendarRowList)
                {
                    courseCalendarRow.SyllabusId = syllabusId;
                    CourseCalendarRow existingCalendarRow = existingCourseCalendarRowList.FirstOrDefault(x => x.Id == courseCalendarRow.Id);

                    if (existingCalendarRow == null)
                        _courseCalendarRowService.CreateCourseCalendarRow(courseCalendarRow);
                    else if (existingCalendarRow.Week != courseCalendarRow.Week ||
                        existingCalendarRow.Date != courseCalendarRow.Date ||
                        existingCalendarRow.Topics != courseCalendarRow.Topics ||
                        existingCalendarRow.Assignments != courseCalendarRow.Assignments)
                        _courseCalendarRowService.EditCourseCalendarRow(existingCalendarRow.Id, courseCalendarRow);
                }
            }

            if (existingCourseCalendarRowList != null && existingCourseCalendarRowList.Any())
            {
                foreach (CourseCalendarRow courseCalendarRow in existingCourseCalendarRowList)
                {
                    if (newCourseCalendarRowList == null || !newCourseCalendarRowList.Any(x => x.Id == courseCalendarRow.Id))
                        _courseCalendarRowService.DeleteCourseCalendarRow(courseCalendarRow.Id);
                }
            }
        }

    }
}
