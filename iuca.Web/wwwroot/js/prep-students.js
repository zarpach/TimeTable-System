
$('button[name="saveBtn"]').click(function () {
    var organizationId = $(this).parent().find('input[name=organizationId]').val();
    var studentBasicInfoId = $(this).parent().find('input[name=studentBasicInfoId]').val();
    var prepDepartmentGroupId = $(this).parent().parent().find('select[name="studentInfo.PrepDepartmentGroupId"]').val();

    $.ajax({
        url: "/StudentInfo/SavePrepStudentDepartmentGroup",
        type: "POST",
        data: {
            "organizationId": organizationId,
            "studentBasicInfoId": studentBasicInfoId,
            "prepDepartmentGroupId": prepDepartmentGroupId
        },
        cache: false,
        success: function (response) {
            alert("Успешно сохранено!");
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});
