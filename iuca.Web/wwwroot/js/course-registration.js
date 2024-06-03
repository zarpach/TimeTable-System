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
            subtractCourseNumberAndPoints(currentBtn);
            currentBtn.parent().parent().remove();

        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});

$('input[name="isAudit"]').change(function () {
    var item = $(this).parent().parent().parent();
    var studentCourseId = item.find('input[name="studentCourseId"]').val();
    var checked = $(this).prop("checked");

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/MarkAudit",
        data: {
            "studentCourseId": studentCourseId,
            "isAudit": checked,
        },
        success: function (response) {
            if (checked) {
                subtractPoints(item);
            } else {
                addPoints(item);
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});


function subtractCourseNumberAndPoints(btn) {
    var item = $(btn).parent().parent();
    $('#couseNum').text(parseInt($('#couseNum').text()) - 1);

    if (!item.find('input[name="isAudit"]').prop('checked')) {
        subtractPoints(item);
    }
}

function subtractPoints(item) {
    var pts = parseInt($(item).find(".coursePts").text());
    if (item.hasClass('noCreditsCount')) {
        $('#totalNoCreditsCount').text(parseInt($('#totalNoCreditsCount').text()) - pts);
    }
    else {
        $('#totalPoints').text(parseInt($('#totalPoints').text()) - pts);
    }
    
}

function addPoints(item) {
    var pts = parseInt($(item).find(".coursePts").text());
    if (item.hasClass('noCreditsCount')) {
        $('#totalNoCreditsCount').text(parseInt($('#totalNoCreditsCount').text()) + pts);
    }
    else {
        $('#totalPoints').text(parseInt($('#totalPoints').text()) + pts);
    }
}

$('#studentComment').focusout(function () {
    saveStudentComment();
});

function saveStudentComment() {
    var studentCourseRegistrationId = $('#studentCourseRegistrationId').val();
    var comment = $('#studentComment').val();
    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/SaveStudentComment",
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "comment": comment,
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
}

$('#approvalForm').submit(function () {
    saveStudentComment();

    return true; // return false to cancel form action
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