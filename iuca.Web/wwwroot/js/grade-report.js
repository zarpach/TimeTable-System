
$('button[name=saveGradeBtn]').click(function () {
    var row = $(this).parent().parent();

    if (confirm("Вы уверенны, что хотите изменить оценку?") == true) {
        var announcementSectionId = row.find("input[name=announcementSectionId]").val();
        var studentUserId = row.find("input[name=studentUserId]").val();
        var gradeId = row.find('select[name="student.GradeId"]').val();

        $.ajax({
            url: "/GradeManagement/SetStudentGrade",
            type: "POST",
            data: {
                announcementSectionId: announcementSectionId,
                studentUserId: studentUserId,
                gradeId: gradeId
            },
            cache: false,
            success: function (response) {
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Успешно', 'Оценка успешно сохранена!');
            },
            failure: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
            },
            error: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
            }
        });
    } 
});

$('button[name=submitGradeSheetBtn').click(function () {
    var announcementSectionId = $(this).parent().parent().find("input[name=announcementSectionId]").val();
    var btn = $(this);

    $.ajax({
        url: "/InstructorCourses/SubmitGradeSheet",
        type: "POST",
        data: {
            announcementSectionId: announcementSectionId,
        },
        cache: false,
        success: function (response) {
            if (!response.success) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', response.error);
            }
            else {
                afterSubmit(btn);
            }
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        }
    });

});

$('button[name=returnGradeSheetBtn]').click(function () {
    var announcementSectionId = $(this).parent().parent().find("input[name=announcementSectionId]").val();
    var btn = $(this);

    $.ajax({
        url: "/InstructorCourses/ReturnGradeSheet",
        type: "POST",
        data: {
            announcementSectionId: announcementSectionId,
        },
        cache: false,
        success: function (response) {
            if (!response.success) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', response.error);
            }
            else {
                afterReturn(btn);
            }
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        }
    });

});

function afterSubmit(btnSubmit) {
    var btnReturn = btnSubmit.parent().parent().find('button[name=returnGradeSheetBtn]');
    var alert = btnSubmit.parent().parent().find('.alert-danger');
    var title = alert.find('.submitTitle');

    btnSubmit.prop("disabled", true);
    btnReturn.prop("disabled", false);
    alert.removeClass('alert-danger');
    alert.addClass('alert-success');
    title.text('Submitted');
}

function afterReturn(btnReturn) {
    var btnSubmit = btnReturn.parent().parent().find('button[name=submitGradeSheetBtn]');
    var alert = btnReturn.parent().parent().find('.alert-success');
    var title = alert.find('.submitTitle');

    btnReturn.prop("disabled", true);
    btnSubmit.prop("disabled", false);
    alert.removeClass('alert-success');
    alert.addClass('alert-danger');
    title.text('Not submitted');
}