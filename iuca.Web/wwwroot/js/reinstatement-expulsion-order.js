
$('button[name="deleteReinstatementExpulsionOrderBtn"]').click(function () {
    deleteReinstatementExpulsionOrder(this);
});

function deleteReinstatementExpulsionOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to delete this order?"))
        $.ajax({
            url: "/StudentOrders/DeleteReinstatementExpulsionOrder",
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

$('button[name="applyReinstatementExpulsionOrderBtn"]').click(function () {
    applyReinstatementExpulsionOrder(this);
});

function applyReinstatementExpulsionOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to apply this order?"))
        $.ajax({
            url: "/StudentOrders/ApplyReinstatementExpulsionOrder",
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

$('button[name="cancelReinstatementExpulsionOrderBtn"]').click(function () {
    cancelReinstatementExpulsionOrder(this);
});

function cancelReinstatementExpulsionOrder(btn) {
    var orderId = $(btn).val();

    if (confirm("Are you sure you want to cancel this order?"))
        $.ajax({
            url: "/StudentOrders/CancelReinstatementExpulsionOrder",
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