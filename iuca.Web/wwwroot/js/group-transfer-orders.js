
$('button[name="deleteGroupTransferOrderBtn"]').click(function () {
    deleteGroupTransferOrder(this);
});

function deleteGroupTransferOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to delete this order?"))
        $.ajax({
            url: "/GroupTransferOrders/Delete",
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

$('button[name="applyGroupTransferOrderBtn"]').click(function () {
    applyGroupTransferOrder(this);
});

function applyGroupTransferOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to apply this order?"))
        $.ajax({
            url: "/GroupTransferOrders/Apply",
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

$('button[name="cancelGroupTransferOrderBtn"]').click(function () {
    cancelGroupTransferOrder(this);
});

function cancelGroupTransferOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to cancel this order?"))
        $.ajax({
            url: "/GroupTransferOrders/Cancel",
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
