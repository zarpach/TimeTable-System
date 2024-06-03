
$(document).ready(function () {
    $('body').on('click', '#removeOtherJobBtn', removeOtherJobRow);
    $('body').on('click', '#addOtherJobBtn', addOtherJobRow);
    $('body').on('click', '#removeEducationBtn', removeEducationRow);
    $('body').on('click', '#addEducationBtn', addEducationRow);
});

function instructorBasicInfoUpdated() {
    updateInstructorBasicInfoId();
    setFormsEnabled();
}

// Add other job edit row to container
function addOtherJobRow() {
    $.ajax({
        url: "/InstructorInfo/GetBlankOtherJobInfoEditorRow",
        cache: false,
        success: function (html) {
            $("#otherJobContainer").append(html);
            updateOtherJobIndexes();
        }
    });
    return false;
}

// Remove other job edit row from container
function removeOtherJobRow() {
    $(this).parent().parent().parent().parent().remove();
    updateOtherJobIndexes();
}

// Update indexes of other job fields. Indexes must be updated after any change
function updateOtherJobIndexes() {
    var otherJobContainer = $('#otherJobContainer');
    var placesEng = otherJobContainer.find('input[name*="].PlaceNameEng"]');
    var placesRus = otherJobContainer.find('input[name*="].PlaceNameRus"]');
    var placesKir = otherJobContainer.find('input[name*="].PlaceNameKir"]');
    var positionsEng = otherJobContainer.find('input[name*="].PositionEng"]');
    var positionsRus = otherJobContainer.find('input[name*="].PositionRus"]');
    var positionsKir = otherJobContainer.find('input[name*="].PositionKir"]');
    var phones = otherJobContainer.find('input[name*="].Phone"]');
    var instructorBasicInfoIds = otherJobContainer.find('input[name*="].InstructorBasicInfoId"]');
    var ids = otherJobContainer.find('input[name*="].Id"]');
    var length = placesEng.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            placesEng[i].name = placesEng[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            placesRus[i].name = placesRus[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            placesKir[i].name = placesKir[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            positionsEng[i].name = positionsEng[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            positionsRus[i].name = positionsRus[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            positionsKir[i].name = positionsKir[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            phones[i].name = phones[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            instructorBasicInfoIds[i].name = instructorBasicInfoIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            ids[i].name = ids[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}

/***** Education info *****/

// Add education edit row to container
function addEducationRow() {
    $.ajax({
        url: "/InstructorInfo/GetBlankEducationInfoEditorRow",
        cache: false,
        success: function (html) {
            $("#educationContainer").append(html);
            updateEducationIndexes();
        }
    });
    return false;
}

// Remove education edit row from container
function removeEducationRow() {
    $(this).parent().parent().parent().parent().remove();
    updateEducationIndexes();
}

// Update indexes of education fields. Indexes must be updated after any change
function updateEducationIndexes() {
    var educationContainer = $('#educationContainer');
    var majorsEng = educationContainer.find('input[name*="].MajorEng"]');
    var majorsRus = educationContainer.find('input[name*="].MajorRus"]');
    var majorsKir = educationContainer.find('input[name*="].MajorKir"]');
    var graduateYear = educationContainer.find('input[name*="].GraduateYear"]');
    var universities = educationContainer.find('select[name*="].UniversityId"]');
    var educationTypes = educationContainer.find('select[name*="].EducationTypeId"]');
    var instructorBasicInfoIds = educationContainer.find('input[name*="].InstructorBasicInfoId"]');
    var ids = educationContainer.find('input[name*="].Id"]');
    var length = majorsEng.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            majorsEng[i].name = majorsEng[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            majorsRus[i].name = majorsRus[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            majorsKir[i].name = majorsKir[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            graduateYear[i].name = graduateYear[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            universities[i].name = universities[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            educationTypes[i].name = educationTypes[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            instructorBasicInfoIds[i].name = instructorBasicInfoIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            ids[i].name = ids[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}

function setReadOnly() {
    if ($("#IsReadOnly").val() == "True")
        $("#infoContainer :input").prop("disabled", true);
}

function setFormsEnabled() {
    var disabled = $('#dbInstructorBasicInfoId').val() == 0;
    $("#orgInfoForm :input").prop("disabled", disabled);
    $("#otherJobInfoForm :input").prop("disabled", disabled);
    $("#educationInfoForm :input").prop("disabled", disabled);
    $("#contactInfoForm :input").prop("disabled", disabled);
}

//Update instructor basic info id for dependent blocks to check if basic info exists
function updateInstructorBasicInfoId() {
    var dbInstrucorBasicInfoId = $('#dbInstructorBasicInfoId').val();
    $('input[name="InstructorBasicInfoId"]').each(function () {
        $(this).val(dbInstrucorBasicInfoId);
    });
}

// Update indexes after page loaded for existing rows
$(document).ready(function () {
    updateInstructorBasicInfoId();
    updateOtherJobIndexes();
    updateEducationIndexes();
    setFormsEnabled();
    setReadOnly();
});