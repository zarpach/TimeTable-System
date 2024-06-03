
$('#recalcGPABtn').click(function () {
    var btn = $(this);
    spinIcon(btn, true);

    $.ajax({
        url: "/StudentTranscripts/RecalcStudentsGPA",
        type: "POST",
        data: {
            overwrite: false
        },
        cache: false,
        success: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', 'Recalculating finished');
        },
        failure: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'Error occurred while recalculating!');
        },
        error: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'Error occurred while recalculating!');
        }
    });
});

function spinIcon(btn, spin) {
    if (spin)
        $(btn).find('i').addClass('animate-icon rotate');
    else
        $(btn).find('i').removeClass('animate-icon rotate');
    $(btn).prop('disabled', spin);
}