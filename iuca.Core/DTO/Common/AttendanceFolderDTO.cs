using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Common
{
    public class AttendanceFolderDTO
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Required]
        [Display(Name = "Folder id")]
        public string FolderId { get; set; }

        [Display(Name = "Main spreadsheet id")]
        public string MainSpreadsheetId { get; set; }
    }
}
