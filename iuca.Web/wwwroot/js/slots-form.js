var departmentId = $('ul#myTab li a.active').data('department-id'); // Получаем активную вкладку департамента
var dayOfWeek = $('div#dayOfWeekButtons input').val(); // Получаем активную кнопку дня недели
var semesterId = $('#semesterSelect').val(); // Получаем значение семестра

$(document).ready(function () {



    // Сделать первую кнопку daysOfWeek активной и сфокусированной по умолчанию
    var firstButton = $('#dayOfWeekButtons label').first();
    firstButton.addClass('active');
    firstButton.addClass('focus');
    firstButton.find('input').focus();


    // Обработчик клика по вкладкам
    $('.nav-link[data-toggle="tab"]').click(function (e) {
        e.preventDefault();

        departmentId = $(this).data('department-id'); // Получаем Id департамента

        updateSlotsTable(departmentId, semesterId, dayOfWeek);
    });

    // Обработчик изменения семестра
    $('#semesterSelect').change(function () {
        semesterId = $(this).val();
        $('#semesterInput').val(semesterId);
        var semesterInput = $('#semesterInput').val()

        updateSlotsTable(departmentId, semesterId, dayOfWeek);
    });

    // Обработчик изменения выбора в элементе <select> (Департамент)
    $(document).on('change', '#departmentSelect', function () {

        var Ids = []; //  Нужен список, так как департаментов может быть несколько
        $(this).find("option:selected").each(function () {
            Ids.push(parseInt($(this).val()))
        });
        console.log(Ids);
        updateSlotForm(Ids, false);
    });

    // Обработчик изменения выбора в элементе <select> (Департамент)
    $(document).on('change', '#printDepartment', function () {

        var Ids = []; //  Нужен список, так как департаментов может быть несколько
        $(this).find("option:selected").each(function () {
            Ids.push(parseInt($(this).val()))
        });
        console.log(Ids);
        updateSlotForm(Ids, true);
    });

    // Обработчик события клика для кнопок
    $(document).on('click', '#dayOfWeekButtons label', function () {
        $('#dayOfWeekButtons label').removeClass('active focus');
        $(this).addClass('active focus');
        $(this).find('input').focus();

        if ($(window).scrollTop() > 0) {
            
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }

        dayOfWeek = $(this).find('input').val();
        updateSlotsTable(departmentId, semesterId, dayOfWeek);
    });



    function updateSlotForm(selectedDepartments, isFromModal) {
        $.ajax({
            type: "GET",
            url: "/Slots/UpdateDepartmentGroupSelect",
            data: { "Ids": selectedDepartments },
            traditional: true,
            success: function (response) {
                if (!isFromModal) {
                    $('#departmentGroupSelect').html(response.departmentGroupOptionsHtml); // Обновляем содержимое select на основе ответа от сервера
                    $('#departmentGroupSelect').multiselect('rebuild');
                } else {
                    $('#printGroup').html(response.departmentGroupOptionsHtml);
                    $('#printGroup').multiselect('rebuild');
                }
                
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});

// Функция для обновления таблицы слотов
function updateSlotsTable(departmentId, semesterId, dayOfWeek) {
    $.ajax({
        type: "GET",
        url: "/Slots/UpdateSlotsTable",
        data: { "departmentId": departmentId, "semesterId": semesterId, "dayOfWeek": dayOfWeek },
        success: function (data) {
            $('#tableContainer').html(data); // Обновляем контейнер таблицы
            initializeDragAndDrop();
        },
        error: function (error) {
            console.log(error);
        }
    });
}



