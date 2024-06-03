
$('#addStudentRegistrationBtn').click(function () {
    $.ajax({
        url: "/StudentCourseRegistrations/SelectStudentsForRegistrationAdding",
        traditional: true,
        data: {
        },
        cache: false,
        success: function (html) {
            showModalWindow(html);
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
});

$(document).on('click', 'button[name="addRegistrationBtn"]', function () {
    var semesterId = $("#semesterId").val();
    var studentUserId = $(this).parent().find("input[name=studentUserId]").val();

    $.ajax({
        url: "/StudentCourseRegistrations/Create",
        method: "POST",
        traditional: true,
        data: {
            "semesterId": semesterId,
            "studentUserId": studentUserId
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Ошибка во время создания регистрации");
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Ошибка во время создания регистрации");
        }
    });
});

function showModalWindow(html) {
    var modalContainer = $("#ModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

function hideModalWindow() {
    var modalContainer = $("#ModalContainer");
    modalContainer.find('.modal').modal('hide');
}

$('input[name="noCreditsLimitation"]').change(function() {
    var registrationId = $(this).parent().parent().find('input[name="registrationId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/SetNoCreditsLimitation",
        data: {
            "registrationId": registrationId,
            "noCreditsLimitation": $(this).prop("checked"),
        },
        success: function (response) {
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});