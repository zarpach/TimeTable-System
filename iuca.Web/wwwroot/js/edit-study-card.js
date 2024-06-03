
// Get courses

$('#selectCoursesBtn').click(function () {
    selectCourses(this);
});

function selectCourses(btn) {
    var organizationId = $("#organizationId").val();
    var semesterId = $('#selectSemester').val();
    var departmentGroupId = $('#selectDepartmentGroup').val();
    var excludedCourseids = getExistingCourses();

    $.ajax({
        url: "/StudyCards/GetCoursesForSelection",
        traditional: true,
        data: {
            "semesterId": semesterId,
            "excludedCourseIds": excludedCourseids,
            "departmentGroupId": departmentGroupId
        },
        cache: false,
        success: function (html) {
            showModalWindow(html);
            if (organizationId == 1) {
                $('#onlyOneDepartmentGroup').prop('checked', true);
                hideStudyGroups();
            }
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

function getExistingCourses() {
    var inputs = document.querySelectorAll('#studyCardRegistrationCourseId');
    var coursesIds = [];

    inputs.forEach(input => {
        var courseId = parseInt(input.getAttribute('value'));
        coursesIds.push(courseId);
    });

    return coursesIds;
}

function showModalWindow(html) {
    var modalContainer = $("#modalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

// Add selected courses

$(document).on('click', '#addSelectedCoursesBtn', function () {
    var selectedCourseIds = getAddedCourses();
    selectedCourseIds.forEach(courseId => {
        addSelectedCourse(courseId);
    });
});

function addSelectedCourse(courseId) {

    $.ajax({
        url: "/StudyCards/GetCourseFromSelection",
        traditional: true,
        data: {
            "selectedCourseId": courseId
        },
        cache: false,
        success: function (html) {
            $("#studyCardCourseContainer").append(html);
            updateStudyCardCourseIndexes();
            hideModalWindow();
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

function hideModalWindow() {
    var modalContainer = $("#modalContainer");
    modalContainer.find('.modal').modal('hide');
}

// Update indexes

$(document).ready(function () {
    updateStudyCardCourseIndexes();
});

$(document).on('click', '#removeStudyCardCourseBtn', function () {
    $(this).parent().parent().parent().parent().parent().remove();
    updateStudyCardCourseIndexes();
});

const regex = new RegExp("\\[(\\d*)\\]", "g");

function updateStudyCardCourseIndexes() {

    var studyCardCourseContainer = $('#studyCardCourseContainer');
    var ids = studyCardCourseContainer.find('input[name*="].Id"]');
    var studyCardIds = studyCardCourseContainer.find('input[name*="].StudyCardId"]');
    var registrationCourseIds = studyCardCourseContainer.find('input[name*="].RegistrationCourseId"]');
    var comments = studyCardCourseContainer.find('input[name*="].Comment"]');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            studyCardIds[i].name = studyCardIds[i].name.replace(regex, '[' + i + ']');
            registrationCourseIds[i].name = registrationCourseIds[i].name.replace(regex, '[' + i + ']');
            comments[i].name = comments[i].name.replace(regex, '[' + i + ']');
        }
    }
    selectSemesterDisable();
}

function updateNums() {
    var nums = $(".study-card-num");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent = "#" + (i + 1);
    }
    return nums.length;
}

function selectSemesterDisable() {
    var courseCount = updateNums();
    var selectSemester = document.getElementById('selectSemester');
    var hiddenSelectValue = document.getElementById('hiddenSelectValue');
    if (courseCount >= 1) {
        selectSemester.disabled = true;
        hiddenSelectValue.value = selectSemester.value;
    } else {
        selectSemester.disabled = false;
    }
}

// Search

$(document).on('click', '#searchBtn', searchItems);



$(document).on('click', '#clearSearchBtn', clearSearch);

function clearSearch() {
    $('#searchText').val('');
    searchItems();
}

$(document).on('change', '#onlyOneDepartmentGroup', searchItems);

function searchItems() {
    var isStudyGroupChecked = $('#onlyOneDepartmentGroup').prop('checked');
    var studyGroup = $("#studyGroup").text();

    $('.search-item').each(function (index) {
        var show = true;

        if (isStudyGroupChecked) {
            if (!$(this).hasClass(studyGroup))
                show = false;
        }

        if (show) {
            var value = $('#searchText').val().toLowerCase();
            if (value.length > 0) {
                var searchText = $(this).text().toLowerCase();
                if (searchText.indexOf(value) >= 0) {
                    show = true;
                }
                else {
                    show = false;
                }
            }
        }

        if (show) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}


function hideStudyGroups() {
    var isChecked = $('#onlyOneDepartmentGroup').prop('checked');
    var studyGroup = $("#studyGroup").text();
    if (isChecked) {
        $('.search-item').hide();
        $('.' + studyGroup).show();
    } 
}

$("#modalContainer").on('keydown', function (event) {
    if (event.which === 13) {
        event.preventDefault();
        event.stopPropagation();
        $("#searchBtn").click();
    }
});




