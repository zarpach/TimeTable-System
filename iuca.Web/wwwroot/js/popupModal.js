
var showPopupModal = function (modalType, title, message)
{
    var modal = $('#popupModal');
    modal.removeClass(CONSTS.MODAL_FAIL);
    modal.removeClass(CONSTS.MODAL_SUCCESS);
    modal.addClass(modalType);
    
    modal.find('.modal-title').html(title);
    modal.find('.modal-body').html(message);
    modal.modal('show');
}
