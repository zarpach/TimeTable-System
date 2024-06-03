$(document).on('click', 'button[name=btnDrop]', function () {
    var currentBtn = $(this);
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
            if (response.success) {
                subtractCourseNumberAndPoints(currentBtn);
                switchToReturnBtn(currentBtn);
            }
            else {
                alert(response.error);
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

$(document).on('click', 'button[name=btnRemove]', function () {
    var currentBtn = $(this);
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
            subtractCourseNumberAndPoints(currentBtn);
            if (currentBtn.hasClass("forAllRemoveBtn")) {
                $('.forAllReturnBtn').each(function () {
                    $(this).prop("disabled", false);
                });
            }
            currentBtn.parent().parent().parent().parent().remove();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});

$(document).on('click', 'button[name=btnReturn]', function () {
    var currentBtn = $(this);
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
            addCourseNumberAndPoints(currentBtn);
            switchToDropBtn(currentBtn);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
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
    var item = $(btn).parent().parent().parent().parent();

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

function addCourseNumberAndPoints(btn) {
    var item = $(btn).parent().parent().parent().parent();

    $('#couseNum').text(parseInt($('#couseNum').text()) + 1);

    if (!item.find('input[name="isAudit"]').prop('checked')) {
        addPoints(item)
    }
}

function addPoints(item) {
    var pts = parseInt($(item).find(".coursePts").text());
    console.log(pts);
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
        url: "/StudentCourseRegistrations/SaveStudentAddDropComment",
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