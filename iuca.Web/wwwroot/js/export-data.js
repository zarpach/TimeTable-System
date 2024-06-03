
$('#synchronizeCoursesBtn').click(function () {
    if (confirm("Вы уверены, что хотите выполнить синхронизацию курсов?")) {
        var btn = $(this);
        displaySpinner(btn, true);

        var force = $('#force').prop('checked');
        var semesterId = $('#semesterId').val();

        $.ajax({
            url: "/ExportData/SynchronizeStudentCoursesBySemester",
            type: "POST",
            data: {
                semesterId: semesterId,
                force: force
            },
            cache: false,
            success: function (response) {
                displaySpinner(btn, false);
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Синхронизация успешно завершена', 'Добавлено записей: '
                    + response.addedCoursesQty + '\n' + 'Удалено записей: ' + response.droppedCoursesQty);
            },
            failure: function (response) {
                displaySpinner(btn, false);
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
            },
            error: function (response) {
                displaySpinner(btn, false);
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
            }
        });
    }
});

$('#updateGradesBtn').click(function () {
    var btn = $(this);
    displaySpinner(btn, true);

    var semesterId = $('#semesterId').val();

    $.ajax({
        url: "/ExportData/ExportStudentGrades",
        type: "POST",
        data: {
            semesterId: semesterId
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Обновление оценок завершено');
        },
        failure: function (response) {
            displaySpinner(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления оценок!');
        },
        error: function (response) {
            displaySpinner(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления оценок!');
        }
    });
});

function displaySpinner(btn, show) {
    $(btn).find('.btnTitle').prop('hidden', show);
    $(btn).find('.spinner').prop('hidden', !show);
    $(btn).prop('disabled', show);
}
