//$(document).ready(function(){
//    $('.event').on("dragstart", function (event) {
//        var dt = event.originalEvent.dataTransfer;
//        dt.setData('Text', $(this).attr('id'));
//    });
//$('table td').on("dragenter dragover drop", function (event) {
//    event.preventDefault();
//    if (event.type === 'drop') {
//        var data = event.originalEvent.dataTransfer.getData('Text',$(this).attr('id'));
//        de=$('#'+data).detach();
//        de.appendTo($(this)); 
//        };
//    });
//})

document.addEventListener('DOMContentLoaded', (event) => {

    function handleDragStart(e) {
        this.style.opacity = '0.4';
        
    }

    function handleDragEnd(e) {
        this.style.opacity = '1';

        slots.forEach(function (item) {
            item.classList.remove('slot-td');
        });
    }

    function handleDragOver(e) {
        e.preventDefault();
        return false;
    }

    function handleDragEnter(e) {
        this.classList.add('slot-td');
    }

    function handleDragLeave(e) {
        this.classList.remove('slot-td');
    }

    function handleDrop(e) {
        e.stopPropagation(); // stops the browser from redirecting.
        return false;
    }

    let slots = document.querySelectorAll('.table .slot');
    slots.forEach(function (slot) {
        slot.addEventListener('dragstart', handleDragStart);
        slot.addEventListener('dragover', handleDragOver);
        slot.addEventListener('dragenter', handleDragEnter);
        slot.addEventListener('dragleave', handleDragLeave);
        slot.addEventListener('dragend', handleDragEnd);
        slot.addEventListener('drop', handleDrop);
    });
});
