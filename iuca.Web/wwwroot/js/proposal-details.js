$('button[name="submitProposalCoursesBtn"]').click(function () {
    submitProposalCourses(this);
});

function submitProposalCourses(btn) {
    var proposalId = $(btn).val();

    if (confirm("Are you sure you want to submit new courses for approval?"))
        $.ajax({
            url: "/Proposals/SubmitForApprovalAll",
            type: "post",
            data: {
                id: proposalId
            },
            cache: false,
            success: function (data) {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while submitting.');
            }
        });
    return false;
}

$('button[name="returnProposalCoursesBtn"]').click(function () {
    returnProposalCourses(this);
});

function returnProposalCourses(btn) {
    var proposalId = $(btn).val();

    if (confirm("Are you sure you want to return the submission of pending courses?"))
        $.ajax({
            url: "/Proposals/ReturnFromApprovalAll",
            type: "post",
            data: {
                id: proposalId
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while returning.');
            }
        });
    return false;
}


function submitProposalCourse(btn) {
    $(btn).prop('disabled', true);
    var proposalCourseId = $(btn).val();

    $.ajax({
        url: "/Proposals/SubmitForApproval",
        type: "post",
        data: {
            proposalCourseId: proposalCourseId
        },
        cache: false,
        success: function (response) {
            if (response.success) {
                updateAfterSubmit(btn);
            }
            else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
            }
            $(btn).prop('disabled', false);
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while submitting.');
            $(btn).prop('disabled', false);
        }
    });
    return false;
}

function returnProposalCourse(btn) {
    $(btn).prop('disabled', true);
    var proposalCourseId = $(btn).val();

    $.ajax({
        url: "/Proposals/ReturnFromApproval",
        type: "post",
        data: {
            proposalCourseId: proposalCourseId
        },
        cache: false,
        success: function (response) {
            if (response.success) {
                updateAfterReturn(btn);
            }
            else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
            }
            $(btn).prop('disabled', false);
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while returning.');
            $(btn).prop('disabled', false);
        }
    });
    return false;
}

function updateAfterSubmit(btn) {
    $(btn).addClass("d-none");
    $(btn).next().removeClass("d-none");

    var proposalCourseId = $(btn).val();

    $(document).find('.statusIcon-' + proposalCourseId).removeClass("text-danger").addClass("text-warning");
    $(document).find('.statusText-' + proposalCourseId).text(" Pending ");
    $('#totalForSubmit').text(parseInt($('#totalForSubmit').text()) - 1);
    $('#totalForReturn').text(parseInt($('#totalForReturn').text()) + 1);
}

function updateAfterReturn(btn) {
    $(btn).addClass("d-none");
    $(btn).prev().removeClass("d-none");

    var proposalCourseId = $(btn).val();

    $(document).find('.statusIcon-' + proposalCourseId).removeClass("text-warning").addClass("text-danger");
    $(document).find('.statusText-' + proposalCourseId).text(" New ");
    $('#totalForSubmit').text(parseInt($('#totalForSubmit').text()) + 1);
    $('#totalForReturn').text(parseInt($('#totalForReturn').text()) - 1);
}

function searchItems(select) {
    var value = $(select).val().toLowerCase();

    $('.search-item').each(function (index) {
        var $searchItem = $(this);

        var $yearOfStudy = $searchItem.find('.search-year-of-study').text().toLowerCase();

        if ($yearOfStudy.indexOf(value) >= 0) {
            $searchItem.show();
        } else {
            $searchItem.hide();
        }
    });
}