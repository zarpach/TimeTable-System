$("#adviserSelect").change(function () {
    var adviserUserId = $(this).val();

    if (adviserUserId == "" || adviserUserId == null) {
        $("#departmentGroupSelect").html('');
    }

    $.ajax({
        type: "GET",
        url: "/GradeManagement/GetAdviserDepartmentGroups",
        data: { "adviserUserId": adviserUserId },
        success: function (response) {

            var items = "<option value >Not selected</option>";
            $.each(response, function (i, departmentGroup) {
                items += "<option value='" + departmentGroup.value + "'>" + departmentGroup.text + "</option>";
            });
            $("#departmentGroupSelect").html(items);
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
});