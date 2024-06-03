
function displaySpinner(btn, show) {
    $(btn).find('.btnTitle').prop('hidden', show);
    $(btn).find('.spinner').prop('hidden', !show);
    $(btn).prop('disabled', show);
}

// Generate tables
function generateAttendanceTables(btn) {
    displaySpinner($(btn), true);

    if (confirm("Are you sure you want to generate attendance tables?"))
        $.ajax({
            url: "/Attendance/GenerateAttendanceTables",
            type: "get",
            cache: false,
            success: function (data) {
                if (data.success) {
                    showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully generated.");
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while generating.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}

function generateAttendanceTable(btn) {
    displaySpinner($(btn), true);
    var announcementId = $(btn).val();

    if (confirm("Are you sure you want to generate attendance table?"))
        $.ajax({
            url: "/Attendance/GenerateAttendanceTable",
            type: "get",
            data: {
                announcementId: announcementId
            },
            cache: false,
            success: function (data) {
                if (data.success) {
                    showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully generated.");
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while generating.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}

// Parse tables
function parseAttendanceTables(btn) {
    displaySpinner($(btn), true);

    if (confirm("Are you sure you want to parse attendance tables?"))
        $.ajax({
            url: "/Attendance/ParseAttendanceTables",
            type: "get",
            cache: false,
            success: function (data) {
                if (data.success) {
                    showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully parsed.");
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while parsing.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}

function parseAttendanceTable(btn) {
    displaySpinner($(btn), true);
    var announcementId = $(btn).val();

    if (confirm("Are you sure you want to parse attendance table?"))
        $.ajax({
            url: "/Attendance/ParseAttendanceTable",
            type: "get",
            data: {
                announcementId: announcementId
            },
            cache: false,
            success: function (data) {
                if (data.success) {
                    showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully parsed.");
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while parsing.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}

// Delete

function deleteAttendanceSpreadsheet(btn) {
    displaySpinner($(btn), true);
    var announcementId = $(btn).val();

    if (confirm("Are you sure you want to delete attendance spreadsheet?"))
        $.ajax({
            url: "/Attendance/DeleteAttendanceSpreadsheet",
            type: "post",
            data: {
                announcementId: announcementId
            },
            cache: false,
            success: function (data) {
                if (data.success) {
                    showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully deleted.");
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while deleting.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}

function deleteAttendance(btn) {
    displaySpinner($(btn), true);
    var announcementSectionId = $(btn).val();

    if (confirm("Are you sure you want to delete attendance?"))
        $.ajax({
            url: "/Attendance/DeleteAttendance",
            type: "post",
            data: {
                announcementSectionId: announcementSectionId
            },
            cache: false,
            success: function (data) {
                if (data.success) {
                    location.reload();
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
                displaySpinner($(btn), false);
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while deleting.');
                displaySpinner($(btn), false);
            }
        });
    else
        displaySpinner($(btn), false);

    return false;
}