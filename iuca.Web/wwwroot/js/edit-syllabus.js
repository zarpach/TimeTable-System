
$('#pills-requirements').on('input', function () {
    calculatePointsSum();
});

$('#pills-policies').on('click', '#removeAcademicPolicyBtn', removeAcademicPolicy);
$('#pills-requirements').on('click', '#removeCourseRequirementBtn', removeCourseRequirement);
$('#pills-calendar').on('click', '#removeCourseCalendarRowBtn', removeCourseCalendarRow);

$("#addAcademicPolicyBtn").click(function () {
    if ($("#academicPolicyContainer").children().length < 10)
        addAcademicPolicy();
});

$("#addCourseRequirementBtn").click(function () {
    if ($("#courseRequirementContainer").children().length < 10)
        addCourseRequirement();
});

$("#addCourseCalendarRowBtn").click(function () {
    if ($("#courseCalendarRowContainer").children().length < 100)
        addCourseCalendarRow();
});

// Add academic policy to container
function addAcademicPolicy() {
    $.ajax({
        url: '/Syllabi/EditAcademicPolicy',
        type: 'get',
        cache: false,
        success: function (response) {
            $('#academicPolicyContainer').append(response);
            updateAcademicPolicyIndexes();
        }
    });
    return false;
}

// Add course requirement to container
function addCourseRequirement() {
    var language = $('#Language').val();
    calculatePointsSum();

    $.ajax({
        url: '/Syllabi/EditCourseRequirement',
        type: 'get',
        data: {
            language: language
        },
        cache: false,
        success: function (response) {
            $('#courseRequirementContainer').append(response);
            updateCourseRequirementIndexes();
        }
    });
    return false;
}

// Add course calendar row to container
function addCourseCalendarRow() {
    $.ajax({
        url: '/Syllabi/EditCourseCalendarRow',
        type: 'get',
        cache: false,
        success: function (response) {
            $('#courseCalendarRowContainer').append(response);
            updateCourseCalendarRowIndexes();
        }
    });
    return false;
}

// Remove academic policy from container
function removeAcademicPolicy() {
    $(this).parent().parent().remove();
    updateAcademicPolicyIndexes();
}

// Remove course requirement from container
function removeCourseRequirement() {
    $(this).parent().parent().remove();
    updateCourseRequirementIndexes();
    calculatePointsSum();
}

// Remove course calendar row from container
function removeCourseCalendarRow() {
    $(this).parent().parent().parent().remove();
    updateCourseCalendarRowIndexes();
}

// Regular expression for indexes
const regex = new RegExp("\\[(\\d*)\\]", "g");

// Update indexes of academic policy fields. Indexes must be updated after any change
function updateAcademicPolicyIndexes() {

    var academicPolicyContainer = $('#academicPolicyContainer');
    var ids = academicPolicyContainer.find('input[name*="].Id"]');
    var syllabusIds = academicPolicyContainer.find('input[name*="].SyllabusId"]');
    var names = academicPolicyContainer.find('input[name*="].Name"]');
    var descriptions = academicPolicyContainer.find('textarea[name*="].Description"]');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            syllabusIds[i].name = syllabusIds[i].name.replace(regex, '[' + i + ']');
            names[i].name = names[i].name.replace(regex, '[' + i + ']');
            descriptions[i].name = descriptions[i].name.replace(regex, '[' + i + ']');
        }
    }

}

// Update indexes of course requirement fields. Indexes must be updated after any change
function updateCourseRequirementIndexes() {

    var courseRequirementContainer = $('#courseRequirementContainer');
    var ids = courseRequirementContainer.find('input[name*="].Id"]');
    var syllabusIds = courseRequirementContainer.find('input[name*="].SyllabusId"]');
    var names = courseRequirementContainer.find('select[name*="].Name"]');
    var points = courseRequirementContainer.find('input[name*="].Points"]');
    var descriptions = courseRequirementContainer.find('textarea[name*="].Description"]');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            syllabusIds[i].name = syllabusIds[i].name.replace(regex, '[' + i + ']');
            names[i].name = names[i].name.replace(regex, '[' + i + ']');
            points[i].name = points[i].name.replace(regex, '[' + i + ']');
            descriptions[i].name = descriptions[i].name.replace(regex, '[' + i + ']');
        }
    }

}

// Update indexes of course calendar row fields. Indexes must be updated after any change
function updateCourseCalendarRowIndexes() {

    var courseCalendarRowContainer = $('#courseCalendarRowContainer');
    var ids = courseCalendarRowContainer.find('input[name*="].Id"]');
    var syllabusIds = courseCalendarRowContainer.find('input[name*="].SyllabusId"]');
    var weeks = courseCalendarRowContainer.find('input[name*="].Week"]');
    var dates = courseCalendarRowContainer.find('input[name*="].Date"]');
    var topics = courseCalendarRowContainer.find('textarea[name*="].Topics"]');
    var assignments = courseCalendarRowContainer.find('textarea[name*="].Assignments"]');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            syllabusIds[i].name = syllabusIds[i].name.replace(regex, '[' + i + ']');
            weeks[i].name = weeks[i].name.replace(regex, '[' + i + ']');
            dates[i].name = dates[i].name.replace(regex, '[' + i + ']');
            topics[i].name = topics[i].name.replace(regex, '[' + i + ']');
            assignments[i].name = assignments[i].name.replace(regex, '[' + i + ']');
        }
    }

}

// Update indexes after page loaded for existing rows
$(document).ready(function () {
    updateAcademicPolicyIndexes();
    updateCourseRequirementIndexes();
    updateCourseCalendarRowIndexes();
});

// Calculate requirements point sum 
function calculatePointsSum() {
    var sum = 0;
    $('input[name*="].Points"]').each(function () {
        var value = parseInt($(this).val());
        if (!isNaN(value)) {
            sum += value;
        }
    });
    $('#total-points').text("Total: " + sum);

    if (sum < 100 || sum > 100) {
        $('#total-points').removeClass('text-success').addClass('text-danger');
    } else {
        $('#total-points').removeClass('text-danger').addClass('text-success');
    }
}