$('#btnApproveAll').click(function () {
    $('.course-row').each(function () {
        var btn = $(this).find('button[name="btnApprove"]');
        approveCourse($(this), btn);
    });
});


$('button[name="btnApprove"]').click(function () {
    var row = $(this).parent().parent();
    approveCourse(row, $(this));
});

function approveCourse(row, btn) {
    $(row).removeClass("red-row");
    $(row).addClass("green-row");

    if (!$(row).hasClass("processed")) {
        $(row).addClass("processed");
    }

    $(row).find('input[name*="IsApproved"]').val(true);
    $(row).find('.approved-icon').removeClass("fa-times red-colored").addClass("fa-check green-colored");
    $(row).find('span[name="CommentError"]').prop('hidden', true);

    var comment = $(row).find('textarea[name*="Comment"]');
    comment.val("");
    comment.prop("disabled", true);
    
    $(btn).removeClass("btn-outline-success");
    $(btn).addClass("btn-success");
    $(btn).prop("disabled", true);
    $(btn).parent().find('button[name="btnDisapprove"]').removeClass("btn-danger");
    $(btn).parent().find('button[name="btnDisapprove"]').addClass("btn-outline-danger");
    $(btn).parent().find('button[name="btnDisapprove"]').prop("disabled", false);
}

$('#btnDisapproveAll').click(function () {
    $('.course-row').each(function () {
        var btn = $(this).find('button[name="btnDisapprove"]');
        disapproveCourse($(this), btn);
    });
});

$('button[name="btnDisapprove"]').click(function () {
    var row = $(this).parent().parent();
    disapproveCourse(row, $(this));
});

function disapproveCourse(row, btn) {
    $(row).removeClass("green-row");
    $(row).addClass("red-row");

    if (!$(row).hasClass("processed")) {
        $(row).addClass("processed");
    }

    $(row).find('input[name*="IsApproved"]').val(false);
    $(row).find('.approved-icon').removeClass("fa-check green-colored").addClass("fa-times red-colored");

    var comment = $(row).find('textarea[name*="Comment"]');
    comment.prop("disabled", false);

    $(btn).removeClass("btn-outline-danger");
    $(btn).addClass("btn-danger");
    $(btn).prop("disabled", true);
    $(btn).parent().find('button[name="btnApprove"]').removeClass("btn-success");
    $(btn).parent().find('button[name="btnApprove"]').addClass("btn-outline-success");
    $(btn).parent().find('button[name="btnApprove"]').prop("disabled", false);
}

$('#sendBtn').click(function () {
    $('#disapprove').val(false);
    $('#adviserCommentError').prop('hidden', true);
});

$('#cancelBtn').click(function () {
    $('#disapprove').val(true);
});

function isValidForm() {

    if (!checkProcessedCourses() || !checkCoursesComments() || !checkAdviserComment() ||
        !checkApprovedCourses() || !checkMaxCredits()) {
        return false;
    }
}

function checkApprovedCourses() {
    if ($('#disapprove').val() == 'true' && $('.red-row').length == 0) {
        if (!confirm("Все курсы одобрены. Вы уверены, что хотите отправить регистрацию на доработку?"))
            return false;
    }
    return true;
}

function checkProcessedCourses() {
    $('.course-row').each(function () {
        if (!$(this).hasClass == "processed") {
            alert("Не все курсы были одобрены или отклонены");
            return false;
        }
    });
    return true;
}

function checkAdviserComment() {
    var valid = true;
    if ($('#disapprove').val() == 'true') {
        if ($('#adviserComment').val().trim() == "") {
            $('#adviserCommentError').prop('hidden', false);
            valid = false;
        }
        else {
            $('#adviserCommentError').prop('hidden', true);
        }
    }
    return valid;
}

function checkCoursesComments() {
    var valid = true;
    $('input[name*="].IsApproved"]').each(function () {
        if ($(this).val() == "false") {
            var comment = $(this).parent().parent().find('textarea[name*="].Comment"]');
            if (comment.val().trim() == "") {
                $(this).parent().parent().find('span[name=CommentError]').prop('hidden', false);
                valid = false;
            }
            else {
                $(this).parent().parent().find('span[name=CommentError]').prop('hidden', true);
            }
        }
    });

    return valid;
}

function checkMaxCredits() {
    if ($('#noCreditsLimitation').val() == 'False' && $('#disapprove').val() == 'false') {
        var totalCredits = parseInt($('#totalCredits').val());
        var maxCredits = parseInt($('#maxCredits').val());
        if (totalCredits > maxCredits) {
            alert("Количество кредитов превышает " + maxCredits);
            return false;
        }
    }
    return true;
}