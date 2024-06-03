$('#maxScoreBtn').click(function () {
    $('.midtermItem').each(function () {
        $(this).find('input[name="StudentMidterm.MaxScore"]').val($('#maxScoreValue').val());
        calcGradePercent($(this));
    });
});

$(document).on('focusout', 'input[name="StudentMidterm.Score"]', function () {
    calcGradePercent($(this).parent().parent());
});


$(document).on('focusout', 'input[name="StudentMidterm.MaxScore"]', function () {
    calcGradePercent($(this).parent().parent());
});

function calcGradePercent(item) {
    var percent = 0;
    var score = item.find('input[name="StudentMidterm.Score"]').val();
    var maxScore = item.find('input[name="StudentMidterm.MaxScore"]').val();
    if (maxScore > 0 && score >= 0) {
        percent = Math.floor(score * 100 / maxScore);
    }
    item.find('.gradePercent').text(percent);
}

function saveAdviserComment(btn) {
    var studentMidtermId = $(btn).val();
    var adviserComment = $("#adviserComment_" + studentMidtermId).val();

    $.ajax({
        url: "/StudentMidterms/SetAdviserComment",
        type: "POST",
        data: {
            studentMidtermId: studentMidtermId,
            adviserComment: adviserComment
        },
        cache: false,
        success: function (data) {
            showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully saved.");
        },
        failure: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        }
    });
}
