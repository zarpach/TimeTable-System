
$("#addDepartmentBtn").click(function () {
    addDepartmentRow();
});

$(document).ready(function () {
    $('body').on('click', '#removeDepartmentBtn', removeDepartmentRow);
})

// Add department edit row to container
function addDepartmentRow() {
    var organizationId = $('#OrganizationId').val();
    $.ajax({
        url: "/Deans/GetBlankDepartmentEditorRow",
        cache: false,
        data: {
            "organizationId": organizationId,
        },
        success: function (html) {
            $("#departmentsContainer").append(html);
        }
    });
    return false;
}

// Remove department edit row from container
function removeDepartmentRow() {
    $(this).parent().parent().parent().parent().remove();
}



