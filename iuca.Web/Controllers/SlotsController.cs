using System;
using iuca.Application.DTO.Slots;
using iuca.Application.ViewModels.Slots;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Slots;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.Instructors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using iuca.Application.Enums;
using System.Linq;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Exceptions;
using System.Collections.Generic;
using iuca.Application.DTO.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using iuca.Web.Extensions;
using SelectPdf;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using DinkToPdf;

namespace iuca.Web.Controllers
{
    public class SlotsController : Controller
    {
        private readonly ISlotService _slotService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly ILessonPeriodService _lessonPeriodService;
        private readonly ILessonRoomService _lessonRoomService;
        private readonly IAnnouncementService _announcementService;
        private readonly ISemesterService _semesterService;
        

        public SlotsController(
            ISlotService slotService,
            IOrganizationService organizationService,
            IDepartmentService departmentService,
            IDepartmentGroupService departmentGroupService,
            IInstructorInfoService instructorInfoService,
            ILessonPeriodService lessonPeriodService,
            ILessonRoomService lessonRoomService,
            IAnnouncementService announcementService,
            ISemesterService semesterService)
        {
            _slotService = slotService;
            _organizationService = organizationService;
            _departmentService = departmentService;
            _departmentGroupService = departmentGroupService;
            _instructorInfoService = instructorInfoService;
            _lessonPeriodService = lessonPeriodService;
            _lessonRoomService = lessonRoomService;
            _announcementService = announcementService;
            _semesterService = semesterService;
        }

        // Index for rendering a page
        [Authorize]
        public IActionResult Index(string action = null)
        {
            var slotViewModel = PrepareSlotViewModel();
            ViewData["Action"] = action ?? "Create";
            return View(slotViewModel);
        }


        [Authorize]
        public IActionResult Edit(Guid Id)
        {
            var slotToEdit = _slotService.GetSlot(Id);
            var slotViewModel = PrepareSlotViewModel(slotToEdit);
            slotViewModel.SingleSlot.DayOfWeek = slotToEdit.DayOfWeek;
            ViewData["Action"] = "Edit"; // Set ViewData for action type
            return View("Index", slotViewModel);
        }


        [Authorize]
        [HttpPost]
        public IActionResult FormSubmit(Guid Id, SlotDTO slotDTO, string action)
        {
            switch (action)
            {
                case "Create":
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _slotService.Create(slotDTO);
                            return RedirectToAction("Index");
                        }
                        catch (ExistingSlotException slotEx)
                        {
                            TempData["SlotError"] = slotEx.Message;
                        }
                        catch (ModelValidationException ex)
                        {
                            TempData["Error"] = ex.Message;
                        }
                    }
                    break;
                case "Edit":
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _slotService.Edit(Id, slotDTO);
                            TempData["Success"] = action;
                            return RedirectToAction("Index");
                        }
                        catch (ModelValidationException ex)
                        {
                            TempData["Error"] = ex.Message;
                        }
                    }
                    break;
                case "Delete":
                    try
                    {
                        _slotService.Delete(Id);
                        TempData["Success"] = action;
                        return RedirectToAction("Index");
                    }
                    catch (ModelValidationException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                    break;
                default:
                    break;
            }

            var slotViewModel = PrepareSlotViewModel();
            return View("Index", slotViewModel);
        }


        public IActionResult Print(SlotOptionsViewModel model)
        {
            _ = PrepareSlotViewModel();
            var converter = new BasicConverter(new PdfTools());
            string bootstrapCss = "";
            using (var reader = new StreamReader("wwwroot/lib/bootstrap/dist/css/bootstrap.min.css"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    bootstrapCss += line;
                }
            }
           
            string html = GetScheduleHtml(model);

            string htmlToPdf = $@"
            <html>
            <head>
                 <style>{bootstrapCss}</style>
            </head>
            <body>
                {html}
            </body>
            </html>";
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    UseCompression = false,
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
            },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = htmlToPdf,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.8 }
                    }
                }
            };

            byte[] pdfBytes = converter.Convert(doc);

            return File(pdfBytes, "application/pdf", $"Schedule – {DateTime.Now}.pdf");
        }

        public string GetScheduleHtml(SlotOptionsViewModel model)
        {
            var slots = new List<IGrouping<string, SlotDTO>>();
            foreach (int semesterId in model.SelectedSemesterIds)
            {
                foreach (int departmentId in model.SelectedDepartmentIds)
                {
                    foreach (int groupId in model.SelectedGroupIds)
                    {
                        foreach (int dayOfWeek in model.SelectedDayOfWeekIds) {
                            var tempSlots = _slotService.GetSlotsForDepartment(departmentId, semesterId, dayOfWeek, groupId);
                            slots.AddRange(tempSlots);
                        }
                    }
                }
            }

            string htmlResult = this.RenderViewAsync("_SlotsTablePartial", slots, true).GetAwaiter().GetResult(); ;

            return htmlResult;
        }


        private byte[] GeneratePdf(string htmlContent)
        {
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(htmlContent);
            byte[] pdf = doc.Save();
            doc.Close();
            return pdf;
        }

        public JsonResult UpdateDepartmentGroupSelect(int[] Ids)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var departmentGroupOptionsHtml = "";

            foreach (int departmentId in Ids)
            {
                var departmentGroupsByCode = _departmentGroupService.GetDepartmentGroupsByParam(
                    selectedOrganization,
                    departmentId
                );

                var groupedDepartmentGroups = departmentGroupsByCode
                    .GroupBy(g => g.Department.Code)
                    .ToList();

                foreach (var group in groupedDepartmentGroups)
                {
                    departmentGroupOptionsHtml += $"<optgroup label='{group.Key}'>";
                    foreach (var item in group)
                    {
                        departmentGroupOptionsHtml += $"<option value='{item.Id}'>{item.Code}</option>";
                    }
                    departmentGroupOptionsHtml += "</optgroup>";
                }
            }

            return Json(new { departmentGroupOptionsHtml });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetSlotsForMobileApp(int departmentId, int semesterId, int dayOfWeek, int groupId)
        {
            var slotsForMobileApp = _slotService.GetSlotsForDepartment(departmentId, semesterId, dayOfWeek, groupId);
            return Json(new { slotsForMobileApp });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetSlotSearchParameters()
        {
            var selectedOrganization = _organizationService.GetMainOrganization();
            var departments = _departmentService.GetDepartments(selectedOrganization.Id, true)
                .Select(x => new
                {
                    DepartmentId = x.Id,
                    Department = x.Code
                });

            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization.Id)
                .Select(x => new
                {
                    GroupId = x.Id,
                    GroupCode = x.Code,
                    GroupDepartment = x.Department.Code
                });

            return Json(new
            {
                departments,
                departmentGroups
            });
        }


        public IActionResult UpdateSlotsTable(int departmentId, int semesterId, int dayOfWeek)
        {
            var slots = _slotService.GetSlotsForDepartment(departmentId, semesterId, dayOfWeek);
            FillSelectLists(departmentId);

            return PartialView("_SlotsTablePartial", slots.ToList());
        }


        public void SetCookie(string key, string value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Append(key, value, option);
        }

        public string GetCookie(string key)
        {
            string value = Request.Cookies[key];

            return value;
        }

        private void FillSelectLists(int departmentId, SlotDTO slot = null)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var departments = _departmentService.GetDepartments(selectedOrganization, true);
            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization);
            var departmentGroupsByDepartment = _departmentGroupService.GetDepartmentGroupsByParam(selectedOrganization, departmentId: departmentId);
            var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganization, enu_InstructorState.NotSet, null);
            var lessonPeriods = _lessonPeriodService.GetLessonPeriods();
            var lessonRooms = _lessonRoomService.GetLessonRooms();
            var daysOfWeek = enu_SlotDayOfWeek.Monday.ToSelectList();
            var announcements = _announcementService.GetAnnouncements(10, true, true);

            var announcementSections = announcements.SelectMany(a => a.AnnouncementSections);
            var currentSemester = _semesterService.GetCurrentSemester(selectedOrganization);
            var semesters = _semesterService.GetSemesters(selectedOrganization).Reverse();


            ViewBag.AllDepartments = new List<DepartmentDTO>(departments);
            ViewBag.AllLessonPeriods = lessonPeriods;
            ViewBag.CurrentSemester = currentSemester;
            ViewBag.DepartmentGroupsByDepartment = departmentGroupsByDepartment;


            if (slot != null)
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code", slot.Department.Id);
                ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "Code", slot.Group.Id);
                ViewBag.Instructors = new SelectList(instructors, "InstructorUserId", "FullNameEng", slot.InstructorUserId);
                ViewBag.LessonPeriods = new SelectList(lessonPeriods, "Id", "Name", slot.LessonPeriod.Id);
                ViewBag.LessonRooms = new SelectList(lessonRooms, "Id", "RoomName", slot.LessonRoom.Id);
                ViewBag.DaysOfWeek = new SelectList(daysOfWeek, "Value", "Text", slot.DayOfWeek);
                ViewBag.Announcements = new SelectList(announcementSections, "AnnouncementId", "Course.Name", slot.AnnouncementSection.Id);
                ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", slot.Semester.Id);   
            }
            else
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code");
                ViewBag.Instructors = new SelectList(instructors, "InstructorUserId", "FullNameEng");
                ViewBag.LessonPeriods = new SelectList(lessonPeriods, "Id", "Name");
                ViewBag.LessonRooms = new SelectList(lessonRooms, "Id", "RoomName");
                ViewBag.DaysOfWeek = new SelectList(daysOfWeek, "Value", "Text");
                ViewBag.Announcements = new SelectList(announcementSections, "AnnouncementId", "Course.Name");
                ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", currentSemester.Id);
            }
        }

        // Helper method to prepare the SlotViewModel
        private SlotViewModel PrepareSlotViewModel(SlotDTO slotDTO = null)
        {
            var allSlots = _slotService.GetSlotsForDepartment(14, 13, 1).ToList();
            var slotViewModel = new SlotViewModel
            {
                SingleSlot = slotDTO,
                AllSlots = allSlots
            };
            FillSelectLists(14, slotViewModel.SingleSlot);
            return slotViewModel;
        }
    }
}

