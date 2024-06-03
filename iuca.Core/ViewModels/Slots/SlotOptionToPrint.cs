using System.Collections.Generic;

public class SlotOptionsViewModel
{
    public List<int> SelectedDepartmentIds { get; set; }
    public List<int> SelectedGroupIds { get; set; }
    public List<int> SelectedDayOfWeekIds { get; set; }
    public List<int> SelectedSemesterIds { get; set; }

    public SlotOptionsViewModel()
    {
        SelectedDepartmentIds = new List<int>();
        SelectedGroupIds = new List<int>();
        SelectedDayOfWeekIds = new List<int>();
        SelectedSemesterIds = new List<int>();
    }
}
