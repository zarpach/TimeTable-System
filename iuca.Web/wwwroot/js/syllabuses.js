
$('button[name="deleteSyllabusBtn"]').click(function () {
    deleteSyllabus(this);
});

$('button[name="submitForApprovalSyllabusBtn"]').click(function () {
    submitForApprovalSyllabus(this);
});

$('button[name="returnFromApprovalSyllabusBtn"]').click(function () {
    returnFromApprovalSyllabus(this);
});

$('button[name="approveSyllabusBtn"]').click(function () {
    approveSyllabus(this);
});

$('button[name="rejectSyllabusBtn"]').click(function () {
    rejectSyllabus(this);
});

function deleteSyllabus(btn) {
    var syllabusId = $(btn).parent().find('input[name="syllabusId"]').val();

    if (confirm("Are you sure you want to delete this syllabus?"))
        $.ajax({
            url: "/Syllabi/Delete",
            type: "post",
            data: {
                id: syllabusId
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while deleting.');
            }
        });
    return false;
}

function submitForApprovalSyllabus(btn) {
    var syllabusId = $(btn).parent().find('input[name="syllabusId"]').val();
    var instructorComment = $(btn).parent().parent().find('textarea[name="instructorComment"]').val();

    if (confirm("Are you sure you want to submit this syllabus for approval?"))
        $.ajax({
            url: "/Syllabi/SubmitForApproval",
            type: "post",
            data: {
                id: syllabusId,
                instructorComment: instructorComment
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while sending.');
            }
        });
    return false;
}

function returnFromApprovalSyllabus(btn) {
    var syllabusId = $(btn).parent().find('input[name="syllabusId"]').val();

    if (confirm("Are you sure you want to return this syllabus from approval?"))
        $.ajax({
            url: "/Syllabi/ReturnFromApproval",
            type: "post",
            data: {
                id: syllabusId
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

function approveSyllabus(btn) {
    var syllabusId = $(btn).parent().find('input[name="syllabusId"]').val();
    var approverComment = $(btn).parent().parent().find('textarea[name="approverComment"]').val();

    if (confirm("Are you sure you want to approve this syllabus?"))
        $.ajax({
            url: "/Syllabi/Approve",
            type: "post",
            data: {
                id: syllabusId,
                approverComment: approverComment
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

function rejectSyllabus(btn) {
    var syllabusId = $(btn).parent().find('input[name="syllabusId"]').val();
    var approverComment = $(btn).parent().parent().find('textarea[name="approverComment"]').val();

    if (confirm("Are you sure you want to reject this syllabus?"))
        $.ajax({
            url: "/Syllabi/Reject",
            type: "post",
            data: {
                id: syllabusId,
                approverComment: approverComment
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while rejecting.');
            }
        });
    return false;
}