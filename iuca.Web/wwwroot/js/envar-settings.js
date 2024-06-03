
$('#saveMaxRegistrationCredits').click(function () {
    var settingsId = $(this).val();
    var maxRegistrationCredits = $('#MaxRegistrationCredits').val();

    $.ajax({
        url: "/EnvarSettings/SetMaxRegistrationCredits",
        type: "POST",
        data: {
            id: settingsId,
            maxRegistrationCredits: maxRegistrationCredits
        },
        cache: false,
        success: function (data) {
            showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully saved.");
        },
        failure: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        }
    });
});

$('#saveDefaultInstructor').click(function () {
    var settingsId = $(this).val();
    var defaultInstructorId = $('#DefaultInstructor').val();

    $.ajax({
        url: "/EnvarSettings/SetDefaultInstructor",
        type: "POST",
        data: {
            id: settingsId,
            defaultInstructorId: defaultInstructorId
        },
        cache: false,
        success: function (data) {
            showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully saved.");
        },
        failure: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        }
    });
});

$('#saveCurrentSemester').click(function () {
    var settingsId = $(this).val();
    var currentSemesterId = $('#CurrentSemester').val();

    $.ajax({
        url: "/EnvarSettings/SetCurrentSemester",
        type: "POST",
        data: {
            id: settingsId,
            semesterId: currentSemesterId
        },
        cache: false,
        success: function (data) {
            showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully saved.");
        },
        failure: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        }
    });
});

$('#saveUpcomingSemester').click(function () {
    var settingsId = $(this).val();
    var upcomingSemesterId = $('#UpcomingSemester').val();

    $.ajax({
        url: "/EnvarSettings/SetUpcomingSemester",
        type: "POST",
        data: {
            id: settingsId,
            semesterId: upcomingSemesterId
        },
        cache: false,
        success: function (data) {
            showPopupModal(CONSTS.MODAL_SUCCESS, "Success", "Successfully saved.");
        },
        failure: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Error during saving.");
        }
    });
});