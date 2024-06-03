
namespace iuca.Domain.Entities.Common
{
    public class AttendanceFolder
    {
        public int Id { get; set; }
        public Semester Semester { get; set; }
        public int SemesterId { get; set; }
        public string FolderId { get; set; }
        public string MainSpreadsheetId { get; set; }
    }
}
