$('#addPrerequisiteBtn').click(function () {
    var excludedCourseids = getExistingCourses();

    $.ajax({
        url: "/Courses/GetCoursesForSelection",
        traditional: true,
        data: {
            "excludedCourseIds": excludedCourseids
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
});

function getExistingCourses() {
    var inputs = document.querySelectorAll('input[name*="].PrerequisiteId"]');
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

function hideModalWindow() {
    var modalContainer = $("#modalContainer");
    modalContainer.find('.modal').modal('hide');
}

$(document).on('click', '#searchBtn', searchItems);

function searchItems() {
    var value = $('#searchText').val().toLowerCase();
    $('.search-item').each(function (index) {
        var searchText = $(this).text().toLowerCase();

        if (searchText.indexOf(value) >= 0) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

$("#modalContainer").on('keydown', function (event) {
    if (event.which === 13) {
        event.preventDefault();
        event.stopPropagation();
        $("#searchBtn").click();
    }
});

$(document).on('click', '#clearSearchBtn', clearSearch);

function clearSearch() {
    $('#searchText').val('');
    searchItems();
}


$(document).on('click', '#addSelectedCoursesBtn', function () {
    var selectedCourseIds = getAddedCourses();
    addSelectedCourses(selectedCourseIds);
});

function getAddedCourses() {
    var checkboxes = document.querySelectorAll('input[name="selectCoursesCheckbox"]');
    var coursesIds = [];

    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            var courseId = parseInt(checkbox.getAttribute('value'));
            coursesIds.push(courseId);
        }
    });

    return coursesIds;
}

function addSelectedCourses(courseIds) {

    $.ajax({
        url: "/Courses/GetCoursesFromSelection",
        traditional: true,
        data: {
            "selectedCourseIds": courseIds
        },
        cache: false,
        success: function (html) {
            $("#coursePrerequisiteContainer").append(html);
            updateCoursePrerequisiteIndexes();
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



$(document).ready(function () {
    $('body').on('click', '#removeCoursePrerequisiteBtn', removeCoursePrerequisiteRow);
})

// Remove course edit row from container
function removeCoursePrerequisiteRow() {
    $(this).parent().parent().parent().remove();
    updateCoursePrerequisiteIndexes();
}

// Update indexes of course fields. Indexes must be updated after any change
function updateCoursePrerequisiteIndexes() {
    var prerequisites = $('input[name*="].PrerequisiteId"]');
    var prerequisiteIds = $('input[name*="].Prerequisite.Id"]');
    var prerequisiteAbbrs = $('input[name*="].Prerequisite.Abbreviation"]');
    var prerequisiteNums = $('input[name*="].Prerequisite.Number"]');
    var prerequisiteNameEngs = $('input[name*="].Prerequisite.NameEng"]');
    var prerequisiteNameRuses = $('input[name*="].Prerequisite.NameRus"]');
    var prerequisiteNameKirs = $('input[name*="].Prerequisite.NameKir"]');
    var prerequisiteImportCodes = $('input[name*="].Prerequisite.ImportCode"]');
    
    var length = prerequisites.length;


    if (length > 0) {
        for (var i = 0; i < length; i++) {
            prerequisites[i].name = prerequisites[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteIds[i].name = prerequisiteIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteAbbrs[i].name = prerequisiteAbbrs[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteNums[i].name = prerequisiteNums[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteNameEngs[i].name = prerequisiteNameEngs[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteNameRuses[i].name = prerequisiteNameRuses[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteNameKirs[i].name = prerequisiteNameKirs[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            prerequisiteImportCodes[i].name = prerequisiteImportCodes[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}
