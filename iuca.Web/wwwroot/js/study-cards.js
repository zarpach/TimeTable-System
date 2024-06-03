
$('button[name="deleteStudyCardBtn"]').click(function () {
    deleteStudyCard(this);
});

function deleteStudyCard(btn) {
    var studyCardId = $(btn).val();

    if (confirm("Are you sure you want to delete this study card?"))
        $.ajax({
            url: "/StudyCards/Delete",
            type: "post",
            data: {
                id: studyCardId
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
