$(document).ready(function () {
    $('#departmentSelect').multiselect({
        buttonTextAlignment: 'left',
        nonSelectedText: 'Не выбрано',
        selectAllText: 'Выбрать все',
        numberDisplayed: 7,
        includeSelectAllOption: true,
        buttonWidth: '100%',
        maxHeight: 200
    });

    $('.multiselect-container .multiselect-filter', $('#departmentSelect').parent()).css({
        'position': 'sticky', 'top': '0px', 'z-index': 1,
    });

    $('#departmentGroupSelect').multiselect({
        includeSelectAllOption: true,
        selectAllText: 'Выбрать все',
        enableClickableOptGroups: true,
        enableCollapsibleOptGroups: true,
        nonSelectedText: 'Не выбрано',
        disableIfEmpty: true,
        buttonTextAlignment: 'left',
        disabledText: 'Не выбран департамент',
        buttonWidth: '100%',
        maxHeight: 200
    });

    $('#dayOfWeekSelect, #lessonPeriodSelect, #printSemester, #printDepartment, #printGroup, #printDaysOfWeek').multiselect({
        buttonWidth: '100%',
        maxHeight: 200,
        buttonTextAlignment: 'left',
        nonSelectedText: 'Не выбрано',
    });

    $('#announcementSectionSelect, #instructorUserSelect, #lessonRoomSelect').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        filterPlaceholder: 'Поиск',
        buttonWidth: '100%',
        maxHeight: 200,
        buttonTextAlignment: 'left',
        nonSelectedText: 'Не выбрано',
    });
});
