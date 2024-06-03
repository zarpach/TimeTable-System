

$('button[name="btnSelect"]').click(function () {
    var currentBtn = $(this);
    currentBtn.prop('disabled', true);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/SelectCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            currentBtn.prop('disabled', false);
            var places = response["places"];
            currentBtn.parent().parent().parent().parent().find('.placesText').text(places);

            var queue = response["queue"];
            if (queue > 0) {
                currentBtn.parent().parent().parent().parent().find('.queueText').text(`(очередь: ${queue})`);
            } else {
                currentBtn.parent().parent().parent().parent().find('.queueText').text('');
            }

            currentBtn.prop('hidden', true);
            currentBtn.parent().find('button[name="btnRemove"]').removeAttr('hidden');

            if (currentBtn.hasClass("forAllSelectBtn")) {
                $('.forAllSelectBtn').each(function () {
                    $(this).prop("disabled", true);
                });
            }

            setColorsAfterAdding(currentBtn);
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

$('button[name="btnRemove"]').click(function () {
    var currentBtn = $(this);
    var studentCourseRegistrationId = currentBtn.parent().find('input[name="studentCourseRegistrationId"]').val();
    var studyCardCourseId = currentBtn.parent().find('input[name="studyCardCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/RemoveCourse",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "studyCardCourseId": studyCardCourseId,
        },
        success: function (response) {
            var places = response["places"];
            currentBtn.parent().parent().parent().parent().find('.placesText').text(places);

            currentBtn.parent().parent().parent().parent().find('.queueText').text('');
            currentBtn.prop('hidden', true);
            currentBtn.parent().find('button[name="btnSelect"]').removeAttr('hidden');

            if (currentBtn.hasClass("forAllRemoveBtn")) {
                $('.forAllSelectBtn').each(function () {
                    $(this).prop("disabled", false);
                });
            }

            setColorsAfterRemoving(currentBtn);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});


function setColorsAfterAdding(btn) {
    var item = $(btn).parent().parent().parent().parent();
    $(item).addClass("regular-bg");
}

function setColorsAfterRemoving(btn) {
    var item = $(btn).parent().parent().parent().parent();
    $(item).removeClass("regular-bg");
    $(item).removeClass("incorrect-bg");
    $(item).removeClass("correct-bg");
    $(item).find(".adviserComment").remove();
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

