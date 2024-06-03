
$(document).on('click', '#searchBtn', searchItems);

function searchItems() {
    var value = $('#searchText').val().toLowerCase();
    $('.course-item').each(function (index) {
        var searchText = $(this).text().toLowerCase();

        if (searchText.indexOf(value) >= 0) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

$(document).on('click', '#clearSearchBtn', clearSearch);

function clearSearch() {
    $('#searchText').val('');
    searchItems();
}

$('button[name="btnAdd"]').click(function () {
    var currentBtn = $(this);
    currentBtn.prop('disabled', true);
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
            var places = response["places"];
            if (currentBtn.hasClass("forAll")) {
                $('.forAll').each(function () {
                    $(this).prop("disabled", true);
                });
            }
            currentBtn.parent().parent().remove();
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
