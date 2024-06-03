using AutoMapper;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Staff;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Staff;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels.Users.Students;
using System;
using iuca.Application.Enums;
using iuca.Application.Models;
using iuca.Application.Converters;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Slots;
using iuca.Application.DTO.Slots;

namespace iuca.Web.Configurations
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Organization, OrganizationDTO>().ReverseMap();
            CreateMap<EnvarSetting, EnvarSettingDTO>().ReverseMap();
            CreateMap<StudyCard, StudyCardDTO>().ReverseMap();
            CreateMap<StudyCardCourse, StudyCardCourseDTO>()
                .ForMember(dest => dest.RegistrationCourseId, opt => opt.MapFrom(src => src.AnnouncementSectionId)).ReverseMap();
            CreateMap<AnnouncementSection, RegistrationCourseDTO>()
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Credits)).ReverseMap();
            CreateMap<DepartmentGroup, DepartmentGroupDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Semester, SemesterDTO>().ReverseMap();
            CreateMap<Course, CourseDTO>().ReverseMap();
            CreateMap<CoursePrerequisite, CoursePrerequisiteDTO>().ReverseMap();
            CreateMap<StudentMinorInfo, StudentMinorInfoDTO>().ReverseMap();
            CreateMap<AcademicLeaveOrder, AcademicLeaveOrderDTO>().ReverseMap();
            CreateMap<ReinstatementExpulsionOrder, ReinstatementExpulsionOrderDTO>().ReverseMap();
            CreateMap<ReinstatementExpulsionOrder, OrdersViewModel>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => DateTime.MinValue))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => DateTime.MinValue));
            CreateMap<AcademicLeaveOrder, OrdersViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)enu_OrderType.AcadLeave));
            CreateMap<Language, LanguageDTO>().ReverseMap();
            CreateMap<CoursePrerequisite, CoursePrerequisiteDTO>().ReverseMap();
            CreateMap<ProposalCourse, ProposalCourseDTO>().ReverseMap();
            CreateMap<Proposal, ProposalDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Announcement, AnnouncementDTO>().ReverseMap();
            CreateMap<AnnouncementSection, AnnouncementSectionDTO>().ReverseMap();
            CreateMap<ExtraInstructor, ExtraInstructorDTO>()
                .ForMember(dest => dest.RegistrationCourseId, opt => opt.MapFrom(src => src.AnnouncementSectionId))
                .ForMember(dest => dest.RegistrationCourse, opt => opt.MapFrom(src => src.AnnouncementSection)).ReverseMap();
            CreateMap<AcademicPolicy, AcademicPolicyDTO>().ReverseMap();
            CreateMap<CourseRequirement, CourseRequirementDTO>().ReverseMap();
            CreateMap<CourseCalendarRow, CourseCalendarRowDTO>().ReverseMap();
            CreateMap<Syllabus, SyllabusDTO>()
                .ForMember(dest => dest.RegistrationCourseId, opt => opt.MapFrom(src => src.AnnouncementSectionId))
                .ForMember(dest => dest.RegistrationCourse, opt => opt.MapFrom(src => src.AnnouncementSection)).ReverseMap();
            CreateMap<StudentCourseTemp, StudentCourseTempDTO>().ReverseMap();
            CreateMap<StudentCourse, StudentCourseDTO>().ReverseMap();
            CreateMap<StudentCourseRegistration, StudentCourseRegistrationDTO>().ReverseMap();
            CreateMap<StudentMidterm, StudentMidtermDTO>().ReverseMap();
            CreateMap<StudentCourseRegistration, StudentCourseRegistrationDTO>().ReverseMap();
            CreateMap<Grade, GradeDTO>().ReverseMap();
            CreateMap<GroupTransferOrder, GroupTransferOrderDTO>().ReverseMap();
            CreateMap<Attendance, AttendanceDTO>().ReverseMap();
            CreateMap<AttendanceClass, AttendanceClassDTO>().ReverseMap();
            CreateMap<AttendanceFolder, AttendanceFolderDTO>().ReverseMap();
            CreateMap<Slot, SlotDTO>().ReverseMap();
            CreateMap<LessonPeriod, LessonPeriodDTO>().ReverseMap();
            CreateMap<LessonRoom, LessonRoomDTO>().ReverseMap();
        }
    }
}
