
$('button[name="deleteAcademicLeaveOrderBtn"]').click(function () {
    deleteAcademicLeaveOrder(this);
});

function deleteAcademicLeaveOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to delete this order?"))
        $.ajax({
            url: "/StudentOrders/DeleteAcademicLeaveOrder",
            type: "post",
            data: {
                id: orderId
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

$('button[name="applyAcademicLeaveOrderBtn"]').click(function () {
    applyAcademicLeaveOrder(this);
});

function applyAcademicLeaveOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to apply this order?"))
        $.ajax({
            url: "/StudentOrders/ApplyAcademicLeaveOrder",
            type: "post",
            data: {
                id: orderId
            },
            cache: false,
            success: function (data) {
                window.location.href = data;
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while applying.');
            }
        });
    return false;
}

$('button[name="cancelAcademicLeaveOrderBtn"]').click(function () {
    cancelAcademicLeaveOrder(this);
});

function cancelAcademicLeaveOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to cancel this order?"))
        $.ajax({
            url: "/StudentOrders/CancelAcademicLeaveOrder",
            type: "post",
            data: {
                id: orderId
            },
            cache: false,
            success: function () {
                location.reload();
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while canceling.');
            }
        });
    return false;
}