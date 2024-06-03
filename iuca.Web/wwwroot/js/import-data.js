
$('#importGradesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportGrades",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importLanguagesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportLanguages",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importDepartmentsBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportDepartments",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importCoursesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportCourses",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importNationalitiesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportNationalities",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importCountriesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportCountries",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importDepartmentGroupsBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportDepartmentGroups",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importStudentsBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportStudents",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#syncStudentsStatesBtn').click(function () {
    var btn = $(this);

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/SyncStudentsStates",
        type: "POST",
        data: {
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to sync: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to sync: " + response.responseText);
        }
    });
});

$('#syncStudentsGroupsBtn').click(function () {
    var btn = $(this);

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/SyncStudentsGroups",
        type: "POST",
        data: {
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to sync: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to sync: " + response.responseText);
        }
    });
});

$('#importStudentsGradesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportStudentsGrades",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#updateStudentsEmailsBtn').click(function () {
    var btn = $(this);
    var files = document.getElementById("studentsEmailsFile").files;

    var formData = new FormData();
    formData.append('file', files[0]);

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/UpdateStudentEmails",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importUniversitiesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportUniversities",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importTransferCoursesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportTransferCourses",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importEducationTypesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportEducationTypes",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importInstructorsBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportInstructors",
        type: "POST",
        data: {
            overwrite: overwrite
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#updateInstructorsEmailsBtn').click(function () {
    var btn = $(this);
    var files = document.getElementById("instructorsEmailsFile").files;

    var formData = new FormData();
    formData.append('file', files[0]);

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/UpdateInstructorEmails",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importRegistrationCoursesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");
    var semesterId = btn.parent().parent().find("select[name=semesterId]").val();

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportRegistrationCourses",
        type: "POST",
        data: {
            overwrite: overwrite,
            semesterId: semesterId,
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

$('#importStudentCoursesBtn').click(function () {
    var btn = $(this);
    var overwrite = btn.parent().parent().find("input[name=overwriteData]").prop("checked");
    var semesterId = btn.parent().parent().find("select[name=semesterId]").val();

    displaySpinner(btn, true);

    $.ajax({
        url: "/ImportData/ImportStudentCourses",
        type: "POST",
        data: {
            overwrite: overwrite,
            semesterId: semesterId,
        },
        cache: false,
        success: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Success");
        },
        failure: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        },
        error: function (response) {
            displaySpinner(btn, false);
            displayResult(btn, "Failed to import: " + response.responseText);
        }
    });
});

function displaySpinner(btn, show) {
    $(btn).find('.btnTitle').prop('hidden', show);
    $(btn).find('.spinner').prop('hidden', !show);
    $(btn).prop('disabled', show);
}

function displayResult(btn, result) {
    $(btn).parent().parent().find('.importResult').text(result);
}