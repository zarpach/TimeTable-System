
$('select[name=studentGrade]').change(function () {
    var studentCourseId = $(this).parent().find("input[name=studentCourseId]").val();
    var gradeId = $(this).val();

    $.ajax({
        url: "/InstructorCourses/SetStudentGrade",
        type: "POST",
        data: {
            studentCourseId: studentCourseId,
            gradeId: gradeId
        },
        cache: false,
        success: function (response) {
            console.log("Saving succeded");
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
        }
    });
    
});

$('#submitGradeSheetBtn').click(function () {
    var announcementSectionId = $(this).parent().find("input[name=announcementSectionId]").val();

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
                location.reload();
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

$('#returnGradeSheetBtn').click(function () {
    var announcementSectionId = $(this).parent().find("input[name=announcementSectionId]").val();

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
                location.reload();
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