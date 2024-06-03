// Update indexes and numbers for existing rows after page loaded
$(document).ready(function () {
    updateCourseIndexes();
    updateNumbers();
})

// Add transfer course row
$("#addTransferCourseBtn").click(function () {
    addTransferCourseRow();
});

$(document).ready(function () {
    $('body').on('click', 'button[name=removeCourseBtn]', removeCourseRow);
})

// Get blank edit row and append to container
function addTransferCourseRow() {
    var universityId = $('#lastSelectedUniversityId').val();
    var year = $('#lastSelectedCourseYear').val();

    $.ajax({
        url: "/TransferCourses/GetBlankEditTransferCourseRow",
        cache: false,
        data: {
            "universityId": universityId,
            "year": year
        },
        success: function (html) {
            $("#transferCourseContainer").append(html);
            updateCourseIndexes();
            updateNumbers();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    return false;
}

// Remove transfer course edit row from container
function removeCourseRow() {
    $(this).parent().parent().remove();
    updateCourseIndexes();
    updateNumbers();
}

// Update indexes of olympiad fields. Indexes must be updated after any change
function updateCourseIndexes() {
    var universities = $('select[name*="].UniversityId"]');
    var years = $('input[name*="].Year"]');
    var seasons = $('select[name*="].Season"]');
    var namesEng = $('textarea[name*="].NameEng"]');
    var namesRus = $('textarea[name*="].NameRus"]');
    var namesKir = $('textarea[name*="].NameKir"]');
    var cyclePartCourseIds = $('input[name*="].CyclePartCourseId"]');
    var ids = $('input[name*="].Id"]');
    var points = $('input[name*="].Points"]');
    var academicPlans = $('input[name*="].IsAcademicPlanCourse"]');
    var grades = $('select[name*="].GradeId"]');

    var length = universities.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            universities[i].name = universities[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            years[i].name = years[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            seasons[i].name = seasons[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            namesEng[i].name = namesEng[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            namesRus[i].name = namesRus[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            namesKir[i].name = namesKir[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            cyclePartCourseIds[i].name = cyclePartCourseIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            ids[i].name = ids[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            points[i].name = points[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            academicPlans[i].name = academicPlans[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            grades[i].name = grades[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}

//Update numbers after courses quantity changing
function updateNumbers() {
    var nums = $(".numIndex");
    var num = 1;
    nums.each(function () {
        $(this)[0].textContent = num;
        $(this).parent().find('input[name="rowIndex"]').val(num);
        num++;
    });
}

$(document).ready(function () {
    $('body').on('change', 'input[name*="].IsAcademicPlanCourse"]', setCourseElements);
})

//Prepare course elements according to checkbox selected value
function setCourseElements() {
    var checked = $(this).prop("checked");

    //Button
    $(this).parent().find('button[name="selectAcademicPlanCourseBtn"]').prop("disabled", !checked);

    //Name fields
    var nameFields = $(this).parent().parent().find('textarea[name*="].Name"]');
    setNameFieldsEnabled(nameFields, checked);
    if (checked)
        clearNameFields(nameFields);

    //Clear cycle part course id
    $(this).parent().parent().find('input[name*="].CyclePartCourseId"]').val(0);

    //Points field
    var pointsField = $(this).parent().parent().find('input[name*="].Points"]');
    pointsField.prop("readonly", checked)
    if (checked)
        pointsField.val(0);

}

function setNameFieldsEnabled(nameFields, disabled) {
    nameFields.each(function () {
        $(this).prop("readonly", disabled);
    });
}

function clearNameFields(nameFields) {
    nameFields.each(function () {
        $(this).val("");
    });
}


//
$(document).ready(function () {
    $('body').on('click', 'button[name=selectAcademicPlanCourseBtn]', selectCourseForRow);
})

function selectCourseForRow() {
    var rowIndex = $(this).parent().find('input[name="rowIndex"]').val();
    var lastSelectedDepartmentId = $("#lastSelectedDepartmentId").val();
    var lastSelectedYear = $("#lastSelectedYear").val();

    openSelectCoursesWindow(rowIndex, lastSelectedDepartmentId, lastSelectedYear, 0);
}

$(document).ready(function () {
    $('body').on('click', '#filterCourses', filterCourses);
})

function filterCourses() {
    var rowIndex = $(this).parent().parent().parent().find('#selectedRowIndex').val();
    var departmentId = $("#selectedDepartmentId").val();
    var year = $("#selectedYear").val();
    var courseId = $("#selectedCourseId").val();

    hideModalWindow();
    openSelectCoursesWindow(rowIndex, departmentId, year, courseId);
}

//Show modal window with courses selection form
function openSelectCoursesWindow(rowIndex, departmentId, year, courseId) {

    //Exclude already added courses
    var addedCoursesIds = getAddedCoursesIds();

    $.ajax({
        url: "/TransferCourses/GetCoursesForSelection",
        traditional: true,
        data: {
            "rowIndex": rowIndex,
            "departmentId": departmentId,
            "year": year,
            "courseId": courseId,
            "excludedIds": addedCoursesIds
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
};

function showModalWindow(html) {
    var modalContainer = $("#ModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

function hideModalWindow() {
    var modalContainer = $("#ModalContainer");
    modalContainer.find('.modal').modal('hide');
}

//Get already added courses ids
function getAddedCoursesIds() {
    var ids = [];
    $('input[name*="].CyclePartCourseId"]').each(function () {
        ids.push($(this).val());
    });

    return ids;
}

$(document).on('click', 'button[name="selectCourse"]', selectCourse);

//Fill course info from selection modal form
function selectCourse() {

    //Get values from modal window
    var selectedNameEng = $(this).parent().find('input[name="selectedNameEng"]').val();
    var selectedNameRus = $(this).parent().find('input[name="selectedNameRus"]').val();
    var selectedNameKir = $(this).parent().find('input[name="selectedNameKir"]').val();
    var selectedPoints = $(this).parent().find('input[name="selectedPoints"]').val();
    var selectedCyclePartCourseId = $(this).parent().find('input[name="selectedCyclePartCourseId"]').val();
    var selectedRowIndex = $("#selectedRowIndex").val();

    //Set values to row in table
    var row = $(`input[name="rowIndex"][value=${selectedRowIndex}]`).parent().parent();
    $(row).find('textarea[name*="].NameEng"]').val(selectedNameEng);
    $(row).find('textarea[name*="].NameRus"]').val(selectedNameRus);
    $(row).find('textarea[name*="].NameKir"]').val(selectedNameKir);
    $(row).find('input[name*="].CyclePartCourseId"]').val(selectedCyclePartCourseId);
    $(row).find('input[name*="].Points"]').val(selectedPoints);

    //Remember filter from modal window
    $("#lastSelectedDepartmentId").val($("#selectedDepartmentId").val());
    $("#lastSelectedYear").val($("#selectedYear").val());

    hideModalWindow();
};

//Remember last selected university to set it in the next new row
$(document).on('change', 'select[name*="].UniversityId"]', setLastSelectedUniversityId);

function setLastSelectedUniversityId() {
    $("#lastSelectedUniversityId").val($(this).val());
}

//Remember last selected year to set it in the next new row
$(document).on('focusout', 'input[name*="].Year"]', setLastSelectedCourseYear);

function setLastSelectedCourseYear() {
    console.log($(this).val());
    $("#lastSelectedCourseYear").val($(this).val());
}


