
$('button[name="deleteProposalBtn"]').click(function () {
    deleteProposal(this);
});

function deleteProposal(btn) {
    var proposalId = $(btn).val();

    if (confirm("Are you sure you want to delete this proposal?"))
        $.ajax({
            url: "/Proposals/Delete",
            type: "post",
            data: {
                id: proposalId
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                location.reload();
                console.log(response);
            }
        });
    return false;
}

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
                var countToSubmit = $(btn).find('.countToSubmit');
                var countToReturn = $(btn).parent().find('.countToReturn');
                countToReturn.text(parseInt(countToSubmit.text()) + parseInt(countToReturn.text()))
                countToSubmit.text('0');

                showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', 'Successsfully submitted.');
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
                var countToSubmit = $(btn).parent().find('.countToSubmit');
                var countToReturn = $(btn).find('.countToReturn');
                countToSubmit.text(parseInt(countToSubmit.text()) + parseInt(countToReturn.text()))
                countToReturn.text('0');

                showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', 'Successsfully returned.');
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while returning.');
            }
        });
    return false;
}

$('button[name="approveProposalCoursesBtn"]').click(function () {
    approveProposalCourses(this);
});

function approveProposalCourses(btn) {
    var proposalId = $(btn).val();

    if (confirm("Are you sure you want to approve all pending courses?"))
        $.ajax({
            url: "/Proposals/ApproveAll",
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
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while approving.');
            }
        });
    return false;
}
