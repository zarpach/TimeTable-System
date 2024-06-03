
// --------- Edit objects -----------

$(document).on('click', '#removeProposalCourseBtn', function () {
    $(this).parent().parent().parent().parent().parent().remove();
    updateProposalCourseIndexes();
});

$(document).ready(function () {
    updateProposalCourseIndexes();
});

const regex = new RegExp("\\[(\\d*)\\]", "g");

function updateProposalCourseIndexes() {

    var proposalCourseContainer = $('#proposalCourseContainer');

    var ids = proposalCourseContainer.find('input[name*="].Id"]');
    var courseIds = proposalCourseContainer.find('input[name*="].CourseId"]');
    var credits = proposalCourseContainer.find('input[name*="].Credits"]');
    var isForAllCheckboxes = proposalCourseContainer.find('input[name*="].IsForAll"]:not(:disabled)');
    var statuses = proposalCourseContainer.find('input[name*="].Status"]');
    var comments = proposalCourseContainer.find('input[name*="].Comment"]');
    var schedules = proposalCourseContainer.find('input[name*="].Schedule"]');
    var instructorsJson = proposalCourseContainer.find('select[name*="].InstructorsJson"]:not(:disabled)');
    var yearsOfStudyJson = proposalCourseContainer.find('select[name*="].YearsOfStudyJson"]:not(:disabled)');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            courseIds[i].name = courseIds[i].name.replace(regex, '[' + i + ']');
            credits[i].name = credits[i].name.replace(regex, '[' + i + ']');
            isForAllCheckboxes[i].name = isForAllCheckboxes[i].name.replace(regex, '[' + i + ']');
            statuses[i].name = statuses[i].name.replace(regex, '[' + i + ']');
            comments[i].name = comments[i].name.replace(regex, '[' + i + ']');
            schedules[i].name = schedules[i].name.replace(regex, '[' + i + ']');
            instructorsJson[i].name = instructorsJson[i].name.replace(regex, '[' + i + ']');
            yearsOfStudyJson[i].name = yearsOfStudyJson[i].name.replace(regex, '[' + i + ']');
        }
    }
    updateNums();
}

function updateNums() {
    var nums = $(".proposal-num");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent = "#" + (i + 1);
    }
    return nums.length;
}


// --------- Courses -----------

// отслеживание кнопки выбора курсов
$('#selectCoursesBtn').click(function () {
    selectCourses(this);
});

// выбор курсов
function selectCourses(btn) {
    var semesterId = $('#selectSemester').val();
    var departmentId = $("#DepartmentId").val();
    var excludedCourseIds = getExistingCourses();

    $.ajax({
        url: "/Proposals/GetCoursesForSelection",
        traditional: true,
        data: {
            "semesterId": semesterId,
            departmentId: departmentId,
            "excludedCourseIds": excludedCourseIds
        },
        cache: false,
        success: function (html) {
            showCoursesModalWindow(html);
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

// показать модальное окно с курсами
function showCoursesModalWindow(html) {
    var modalContainer = $("#coursesModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
    searchCourses();
}

// скрыть модальное окно с курсами
function hideCoursesModalWindow() {
    var modalContainer = $("#coursesModalContainer");
    modalContainer.find('.modal').modal('hide');
}

// получить айдишки выбранных курсов
function getAddedCourses() {
    var checkboxes = document.querySelectorAll('#selectCoursesCheckbox');
    var coursesIds = [];

    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            var courseId = parseInt(checkbox.getAttribute('value'));
            coursesIds.push(courseId);
        }
    });

    return coursesIds;
}

// получить айдишки уже добавленных курсов
function getExistingCourses() {
    var inputs = document.querySelectorAll('#proposalCourseId');
    var coursesIds = [];

    inputs.forEach(input => {
        var courseId = parseInt(input.getAttribute('value'));
        coursesIds.push(courseId);
    });

    return coursesIds;
}

// отслеживание кнопки добавления в список выбранных курсов
$(document).on('click', '#addSelectedCoursesBtn', function () {
    $(this).prop("disabled", true);
    var selectedCourseIds = getAddedCourses();
    selectedCourseIds.forEach(courseId => {
        addSelectedCourse(courseId);
    });
});

// добавление в список выбранного курса
function addSelectedCourse(courseId) {
    $.ajax({
        url: "/Proposals/GetCourseFromSelection",
        traditional: true,
        data: {
            "selectedCourseId": courseId
        },
        cache: false,
        success: function (html) {
            $("#proposalCourseContainer").append(html);
            setSelectpicker();
            updateProposalCourseIndexes();
            hideCoursesModalWindow();
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

// на Enter модальное окно не закрывается, а кнопка поиска активируется
$("#coursesModalContainer").on('keydown', function (event) {
    if (event.which === 13) {
        event.preventDefault();
        event.stopPropagation();
        $("#searchCoursesBtn").click();
    }
});

// отслеживание кнопки поиска курсов
$(document).on('click', '#searchCoursesBtn', searchCourses);

// поиск курсов
function searchCourses() {
    var departmentTxt = $("#departments option:selected").text().trim();
    var courseIdTxt = $('#searchByIdText').val().trim();
    var searchCoursesText = $('#searchCoursesText').val().toLowerCase().trim();

    $('.search-item').each(function () {
        var courseId = $(this).find('.searchCourseId').text().trim();
        var departmentCode = $(this).find('.departmentCode').text().trim();
        var searchText = $(this).text().toLowerCase().trim();

        var showItem = true;

        if (departmentTxt != 'All' && departmentTxt.length > 0 && departmentCode !== departmentTxt) {
            showItem = false;
        }

        if (showItem && courseIdTxt.length > 0 && courseId !== courseIdTxt) {
            showItem = false;
        }

        if (showItem && searchCoursesText.length > 0 && searchText.indexOf(searchCoursesText) === -1) {
            showItem = false;
        }

        if (showItem) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

// отслеживание кнопки очистки поиска курсов
$(document).on('click', '#clearSearchCoursesBtn', clearSearchCourses);

// очистить поисковую строку курсов
function clearSearchCourses() {
    $('#searchCoursesText').val('');
    searchCourses();
}

// select с поиском
function setSelectpicker() {
    $('.selectpicker').selectpicker();

    $('.selectpicker').on('loaded.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        countSelectedInstructors($(this));
    });

    $('.selectpicker').on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        countSelectedInstructors($(this));
    });
}

$('.selectpicker').on('loaded.bs.select', function (e, clickedIndex, isSelected, previousValue) {
    countSelectedInstructors($(this));
});

$('.selectpicker').on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
    countSelectedInstructors($(this));
});

function countSelectedInstructors(selectList) {
    var count = $(selectList).find(" :selected").length;
    $(selectList).parent().parent().parent().find('.instructorsCount').text(count);
}

