$(document).on('click', 'button[name=btnDrop]', function () {
    var currentBtn = $(this);
    currentBtn.prop("disabled", true);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/DropCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            currentBtn.prop("disabled", false);
            if (response.success) {
                switchToReturnBtn(currentBtn);

                if (currentBtn.hasClass("forAllDropBtn")) {
                    $('.forAllAddBtn').each(function () {
                        $(this).prop("disabled", false);
                    });
                }
            }
            else {
                alert(response.error);
            }
        },
        failure: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        },
        error: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        }
    });
});

$(document).on('click', 'button[name=btnReturn]', function () {
    var currentBtn = $(this);
    currentBtn.prop("disabled", true);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/ReturnCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            currentBtn.prop("disabled", false);
            switchToDropBtn(currentBtn);
            if (currentBtn.hasClass("forAllReturnBtn")) {
                $('.forAllAddBtn').each(function () {
                    $(this).prop("disabled", true);
                });
            }
        },
        failure: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        },
        error: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        }
    });
});
$(document).on('click', 'button[name=btnAdd]', function () {
    var currentBtn = $(this);
    currentBtn.prop("disabled", true);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/AddCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            currentBtn.prop("disabled", false);
            switchToRemoveBtn(currentBtn);

            if (currentBtn.hasClass("forAllAddBtn")) {
                $('.forAllAddBtn').each(function () {
                    $(this).prop("disabled", true);
                });

                $('.forAllReturnBtn').each(function () {
                    $(this).prop("disabled", true);
                });
            }

        },
        failure: function (response) {
            currentBtn.prop('disabled', false);
            alert(response.responseText);
        },
        error: function (response) {
            currentBtn.prop('disabled', false);
            alert(response.responseText);
        }
    });
});

$(document).on('click', 'button[name=btnRemove]', function () {
    var currentBtn = $(this);
    currentBtn.prop("disabled", true);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/RemoveAddedCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            currentBtn.prop("disabled", false);
            switchToAddBtn(currentBtn);

            if (currentBtn.hasClass("forAllRemoveBtn")) {
                $('.forAllAddBtn').each(function () {
                    $(this).prop("disabled", false);
                });

                $('.forAllReturnBtn').each(function () {
                    $(this).prop("disabled", false);
                });
            }
        },
        failure: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        },
        error: function (response) {
            currentBtn.prop("disabled", false);
            alert(response.responseText);
        }
    });
});


function switchToDropBtn(btn) {
    btn.parent().parent().parent().parent().find('.droppedRow').hide();
    btn.parent().find('button[name=btnDrop]').prop("hidden", false);
    btn.prop("hidden", true);
}

function switchToReturnBtn(btn) {
    btn.parent().parent().parent().parent().find('.droppedRow').show();
    btn.parent().find('button[name=btnReturn]').prop("hidden", false);
    btn.prop("hidden", true);
}

function switchToAddBtn(btn) {
    btn.parent().parent().parent().parent().find('.addedRow').hide();
    btn.parent().find('button[name=btnAdd]').prop("hidden", false);
    btn.prop("hidden", true);
}

function switchToRemoveBtn(btn) {
    btn.parent().parent().parent().parent().find('.addedRow').show();
    btn.parent().find('button[name=btnRemove]').prop("hidden", false);
    btn.prop("hidden", true);
}

$('.course-item-name').click(function () {
    var registrationCourseId = $(this).parent().find('input[name=registrationCourseId]').val();
    $.ajax({
        url: "/StudentCourseRegistrations/GetSyllabus",
        traditional: true,
        data: {
            "registrationCourseId": registrationCourseId
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

function showModalWindow(html) {
    var modalContainer = $("#ModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}