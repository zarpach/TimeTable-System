const cardWrapper = document.getElementById("table-wrapper");

//const sortable = new Sortable(draggableTable, {
//    animation: 150,
//    ghostClass: 'ghost',
//    dragoverBubble: true,
//});

$("slotsTable").sortable({
    items: 'td:not(:first)',
    helper: 'clone',
});

function initializeDragAndDrop() {
    var draggedId;
    var droppedOnId;

    var departmentId = $('ul#myTab li a.active').data('department-id'); // Получаем активную вкладку департамента
    var dayOfWeek = $('div#dayOfWeekButtons input').val(); // Получаем активную кнопку дня недели
    var semesterId = $('#semesterSelect').val(); // Получаем значение семестра

    function handleDragStart(e) {
        this.style.opacity = '0.4';
        draggedId = $(this).data('slot-id');
        //console.log($('ul#myTab li a.nav-link.active').data('department-id'));
        //console.log($('#semesterSelect').val());
    }

    function handleDragEnd(e) {
        this.style.opacity = '1';

        slots.forEach(function (item) {
            item.classList.remove('over');
        });
    }

    function handleDragOver(e) {
        e.preventDefault();
        return false;
    }

    function handleDragEnter(e) {
        this.classList.add('over');
    }

    function handleDragLeave(e) {
        this.classList.remove('over');
    }

    function handleDrop(e) {
        //e.preventDefault(); // stops the browser from redirecting.
        droppedOnId = $(this).data('slot-id');

        // if user dropped slot on empty cell
        if (droppedOnId == 0) {
            var newLessonPeriod = $(this).data('lesson-period-id');
            var newDepartmentGroupId = $(this).data('department-group-id');
        }
        
        if (draggedId !== droppedOnId) {
            swapSlots(draggedId, droppedOnId, newLessonPeriod, newDepartmentGroupId);
            
        }
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

    function swapSlots(draggedId, droppedOnId, newLessonPeriod, newDepartmentGroupId) {
        $.ajax({
            type: "POST",
            url: "/Slots/SwapSlots",
            data: {
                draggedId: draggedId,
                droppedOnId: droppedOnId,
                newLessonPeriod: newLessonPeriod,
                newDepartmentGroupId: newDepartmentGroupId
            },
            success: function (response) {
                updateSlotsTable(departmentId, semesterId, dayOfWeek);
            },
            error: function (error) {
                console.error("Error updating server: ", error);
            }
        });
    }
}


// Call the function to initialize drag-and-drop functionality
$(document).ready(function () {
    initializeDragAndDrop();
});
