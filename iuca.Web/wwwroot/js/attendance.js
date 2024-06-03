

// Student courses attendance modal

function studentCoursesAttendanceModal(studentUserId, semesterId, studentName) {
    $.ajax({
        url: "/Attendance/StudentCoursesAttendance",
        traditional: true,
        data: {
            "studentUserId": studentUserId,
            "semesterId": semesterId,
            "studentName": studentName
        },
        cache: false,
        success: function (html) {
            showModalWindow(html);
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}

function showModalWindow(html) {
    var modalContainer = $("#studentCoursesAttendanceModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

// Search

function searchItems(input) {
    var value = $(input).val().toLowerCase();

    $('.search-item').each(function (index) {
        var $searchItem = $(this);

        var $name = $searchItem.find('.search-name').text().toLowerCase();
        var $courseId = $searchItem.find('.search-course-id').text().toLowerCase();
        var $mark = $searchItem.find('.search-mark').text().toLowerCase();

        if ($name.indexOf(value) >= 0 || $courseId.indexOf(value) >= 0 || $mark.indexOf(value) >= 0) {
            $searchItem.show();
        } else {
            $searchItem.hide();
        }
    });
}

// Graph

function drawChart(chartData) {

    if (chartData.length === 0) {
        console.error('Chart data is empty.');
        return;
    }

    // Создаем DataTable и добавляем столбцы для первого графика
    var data = new google.visualization.DataTable();
    data.addColumn('date', 'Date');
    data.addColumn('number', 'Total classes');
    data.addColumn('number', 'Attended classes');

    // Создаем DataTable и добавляем столбцы для второго графика
    var percentageData = new google.visualization.DataTable();
    percentageData.addColumn('date', 'Date');
    percentageData.addColumn('number', 'Attendance percentage');
    percentageData.addColumn({ type: 'string', role: 'tooltip' });

    // Преобразуем данные и добавляем строки для обоих графиков
    var chartRows = chartData.map(item => [
        new Date(item.Date),
        item.TotalClasses,
        item.BlankOrLateClasses
    ]);
    data.addRows(chartRows);

    var percentageRows = chartData.map(item => [
        new Date(item.Date),
        (item.BlankOrLateClasses / item.TotalClasses) * 100,
        new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric', year: 'numeric' }).format(new Date(item.Date))
        + '\n'
        + (item.BlankOrLateClasses / item.TotalClasses * 100).toFixed(1) + '%'
    ]);
    percentageData.addRows(percentageRows);

    // Фильтрация дат для оси x (каждая 5-я дата)
    var xTicks = chartData.filter((_, index) => (index + 1) % 5 === 0).map(item => new Date(item.Date));

    // Получение значений для оси y для первого графика
    var yTicks = chartData.map(item => item.TotalClasses);

    // Добавление нового значения на ось y для первого графика (максимальное значение + 0.5)
    //var newYTicks = [...yTicks, Math.max(...yTicks) + 0.5];

    // Опции для первого графика
    var options = {
        hAxis: {
            title: 'Date',
            format: 'dd MMM',
            ticks: xTicks
        },
        vAxis: {
            title: 'Number of Classes',
            ticks: yTicks
        },
        series: {
            0: { areaOpacity: 0.5, pointSize: 6, lineDashStyle: [4, 4] },
            1: { areaOpacity: 0.5, pointSize: 3 }
        },
        colors: ['#ff8ff8', '#b734eb']
    };

    // Опции для второго графика
    var percentageOptions = {
        hAxis: {
            title: 'Date',
            format: 'dd MMM',
            ticks: xTicks
        },
        vAxis: {
            title: 'Percentage',
            minValue: 0,
            maxValue: 100,
            format: '#\'%\''
        },
        series: {
            0: { type: 'line', pointSize: 4 } // Новая линия для процента посещения
        },
        colors: ['#ad32fa']
    };

    // Создаем объекты диаграмм и отрисовываем их
    var chartDiv = document.getElementById('chart_div');
    chartDiv.classList.remove("d-none");
    var chart = new google.visualization.AreaChart(chartDiv);
    chart.draw(data, options);

    var percentageChartDiv = document.getElementById('percentage_chart_div');
    percentageChartDiv.classList.remove("d-none");
    var percentageChart = new google.visualization.LineChart(percentageChartDiv);
    percentageChart.draw(percentageData, percentageOptions);
}
