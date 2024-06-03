
$('#importCoursesBtn').click(function () {
    var btn = $(this);
    spinIcon(btn, true);

    $.ajax({
        url: "/ImportData/ImportCourses",
        type: "POST",
        data: {
            overwrite: false
        },
        cache: false,
        success: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Синхронизация успешно завершена!');
        },
        failure: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        },
        error: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        }
    });
});

$('button[name=exportCourseBtn]').click(function () {
    var btn = $(this);
    $(btn).prop('disabled', true);

    var courseId = $(btn).parent().find('input[name=courseId]').val();

    $.ajax({
        url: "/ExportData/ExportCourse",
        type: "POST",
        data: {
            courseId: courseId
        },
        cache: false,
        success: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Экспорт успешно завершен!');
        },
        failure: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
        },
        error: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
        }
    });
});

function spinIcon(btn, spin) {
    if (spin)
        $(btn).find('i').addClass('animate-icon rotate');
    else
        $(btn).find('i').removeClass('animate-icon rotate');
    $(btn).prop('disabled', spin);
}