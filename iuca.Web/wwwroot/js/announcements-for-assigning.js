// Search

$('#searchText').on('input', searchItems);

function searchItems() {
    var value = $('#searchText').val().toLowerCase();

    $('.search-item').each(function (index) {
        var $searchItem = $(this);
        var $name = $searchItem.find('.search-name').text().toLowerCase();
        var $courseId = $searchItem.find('.search-course-id').text().toLowerCase();

        if ($name.indexOf(value) >= 0 || $courseId.indexOf(value) >= 0) {
            $searchItem.show();
        } else {
            $searchItem.hide();
        }
    });
}

// Set student section

function setStudentSection(radio) {
    var newAnnouncementSectionId = $(radio);
    var studentUserId = $(radio).parent().parent().parent().find("#studentUserId");
    var oldAnnouncementSectionId = $(radio).parent().parent().parent().find("#oldAnnouncementSectionId");

    disableOrEnableRadioButtons(radio.name, true);
    $.ajax({
        url: "/RegistrationCourseManagement/SetStudentSection",
        type: "POST",
        data: {
            studentUserId: studentUserId.val(),
            oldAnnouncementSectionId: oldAnnouncementSectionId.val(),
            newAnnouncementSectionId: newAnnouncementSectionId.val()
        },
        cache: false,
        success: function (data) {
            if (data.success) {
                oldAnnouncementSectionId.val(newAnnouncementSectionId.val());
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
            disableOrEnableRadioButtons(radio.name, false);
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while saving.')
        }
    });
}

function disableOrEnableRadioButtons(groupName, isDisabled) {
    var radioButtons = document.querySelectorAll('input[type="radio"][name="' + groupName + '"]');
    radioButtons.forEach(function (radioButton) {
        radioButton.disabled = isDisabled;
    });
}
