
// --------- Edit objects -----------

function approveProposalCourse(btn) {
    var proposalCourseId = $(btn).val();

    $.ajax({
        url: "/Proposals/Approve",
        type: "post",
        data: {
            id: proposalCourseId
        },
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var statusIcon = $(document).find('.statusIcon-' + proposalCourseId);
                var statusText = $(document).find('.statusText-' + proposalCourseId);
                const newStatusText = "Approved";

                statusText.text(newStatusText);
                statusIcon.removeClass('text-danger text-warning').addClass('text-success');

                $(btn).addClass('d-none');
                $(btn).next('button').removeClass('d-none');
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while approving.');
        }
    });
    return false;
}

function rejectProposalCourse(btn) {
    var proposalCourseId = $(btn).val();

    $.ajax({
        url: "/Proposals/Reject",
        type: "post",
        data: {
            id: proposalCourseId
        },
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var statusIcon = $(document).find('.statusIcon-' + proposalCourseId);
                var statusText = $(document).find('.statusText-' + proposalCourseId);
                const newStatusText = "Rejected";

                statusText.text(newStatusText);
                statusIcon.removeClass('text-success text-warning').addClass('text-danger');

                $(btn).addClass('d-none');
                $(btn).prev('button').removeClass('d-none');
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while rejecting.');
        }
    });
    return false;
}
