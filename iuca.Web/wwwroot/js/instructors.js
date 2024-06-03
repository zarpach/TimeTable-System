
$('#refreshInstructorStatesBtn').click(function () {

    $(this).prop('disabled', true);

    $.ajax({
        url: "/InstructorInfo/RefreshInstructorStates",
        type: "POST",
        data: {
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
            $(this).prop('disabled', false);
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
            $(this).prop('disabled', false);
        }
    });
});

$('#importInstructorsBtn').click(function () {
    var btn = $(this);

    spinIcon(btn, true);

    $.ajax({
        url: "/ImportData/ImportInstructors",
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

$('button[name=exportInstructorBtn]').click(function () {
    var btn = $(this);

    $(btn).prop('disabled', true);

    var instructorBasicInfoId = btn.parent().find('input[name=instructorBasicInfoId]').val();

    $.ajax({
        url: "/ExportData/ExportInstructorInfo",
        type: "POST",
        data: {
            instructorBasicInfoId: instructorBasicInfoId
        },
        cache: false,
        success: function (response) {
            if (response.result.success) {
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Экспорт успешно завершен!');
                $(btn).parent().parent().parent().find('.importCode').text(response.importCode);
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!<br>' + response.result.message);
                $(btn).prop('disabled', false);
            }

        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
            $(btn).prop('disabled', false);
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
            $(btn).prop('disabled', false);
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