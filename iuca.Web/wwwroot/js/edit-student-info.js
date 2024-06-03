function studentBasicInfoUpdated() {
    updateStudentBasicInfoId();
}

//Update student basic info id for dependent blocks to check if basic info exists
function updateStudentBasicInfoId() {
    var dbStudentBasicInfoId = $('#dbStudentBasicInfoId').val();
    $('input[name="StudentBasicInfoId"]').each(function () {
        $(this).val(dbStudentBasicInfoId);
    });
}

// Update indexes after page loaded for existing rows
$(document).ready(function () {
    updateStudentBasicInfoId();
});

/***** Language info *****/

$("#addLanguageBtn").click(function () {
    addLanguageRow();
});

$(document).ready(function () {
    $('body').on('click', '#removeLanguageBtn', removeLanguagebRow);
})

// Add language edit row to container
function addLanguageRow() {
    var studentBasicInfoId = $('#StudentBasicInfo_Id').val();
    $.ajax({
        url: "/StudentInfo/GetBlankLanguageEditorRow",
        cache: false,
        data: {
            "studentBasicInfoId": studentBasicInfoId
        },
        success: function (html) {
            $("#languagesContainer").append(html);
            updateLanguagesIndexes();
        }
    });
    return false;
}

// Remove language edit row from container
function removeLanguagebRow() {
    $(this).parent().parent().parent().parent().remove();
    updateLanguagesIndexes();
}

// Update indexes of languge fields. Indexes must be updated after any change
function updateLanguagesIndexes() {
    var studentBasicInfoIds = $('input[id="StudentBasicInfo.StudentLanguages.StudentBasicInfoId"]');
    var languageIds = $('select[id="StudentBasicInfo.StudentLanguages.LanguageId"]');
    
    var length = studentBasicInfoIds.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            studentBasicInfoIds[i].name = studentBasicInfoIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            languageIds[i].name = languageIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}


/***** Parents info *****/

$("#addParentsInfoBtn").click(function () {
    addParentsInfoRow();
});

$(document).ready(function () {
    $('body').on('click', '#removeParentsInfoBtn', removeParentsInfoRow);
})

// Add parents info edit row to container
function addParentsInfoRow() {
    var studentBasicInfoId = $('#StudentBasicInfo_Id').val();
    $.ajax({
        url: "/StudentInfo/GetBlankParentsInfoEditorRow",
        cache: false,
        data: {
            "studentBasicInfoId": studentBasicInfoId
        },
        success: function (html) {
            $("#parentsInfoContainer").append(html);
            updateParentsInfoIndexes();
        }
    });
    return false;
}

// Remove parents info edit row from container
function removeParentsInfoRow() {
    $(this).parent().parent().parent().parent().remove();
    updateParentsInfoIndexes();
}

// Update indexes of parents fields. Indexes must be updated after any change
function updateParentsInfoIndexes() {
    var studentBasicInfoIds = $('input[id="StudentBasicInfo.StudentParentsInfo.StudentBasicInfoId"]');
    var lastNames = $('input[id="StudentBasicInfo.StudentParentsInfo.LastName"]');
    var firstNames = $('input[id="StudentBasicInfo.StudentParentsInfo.FirstName"]');
    var middleNames = $('input[id="StudentBasicInfo.StudentParentsInfo.MiddleName"]');
    var phones = $('input[id="StudentBasicInfo.StudentParentsInfo.Phone"]');
    var workPlaces = $('input[id="StudentBasicInfo.StudentParentsInfo.WorkPlace"]');
    var relations = $('input[id="StudentBasicInfo.StudentParentsInfo.Relation"]');
    var deadYears = $('input[id="StudentBasicInfo.StudentParentsInfo.DeadYear"]');

    var length = studentBasicInfoIds.length;
    console.log(length);

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            studentBasicInfoIds[i].name = studentBasicInfoIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            lastNames[i].name = lastNames[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            firstNames[i].name = firstNames[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            middleNames[i].name = middleNames[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            phones[i].name = phones[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            workPlaces[i].name = workPlaces[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            relations[i].name = relations[i].name.replace(/\[(.+)\]/, '[' + i + ']');
            deadYears[i].name = deadYears[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        }
    }
}

// Update indexes after page loaded for existing rows
$(document).ready(function () {
    updateLanguagesIndexes();
    updateParentsInfoIndexes();
});

$("#IsPrep").change(function () {
    console.log($(this).prop("checked"));
    if ($(this).prop("checked")) {
        $("#PrepDepartmentGroupId").prop("disabled", false);
    }
    else {
        $("#PrepDepartmentGroupId").val(0);
        $("#PrepDepartmentGroupId").prop("disabled", true);
    }
});

/***** Minor info *****/

$(document).ready(function () {
    setMultiselect();
});

function setMultiselect() {
    $('#departmentsSelect').multiselect({
        nonSelectedText: 'None'
    });
}
