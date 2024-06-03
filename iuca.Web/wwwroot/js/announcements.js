
function setForAllValue(checkbox) {
    var announcementId = $(checkbox).val();
    var isForAll = $(checkbox).prop('checked');

    $.ajax({
        url: "/Announcements/SetForAllValue",
        type: "POST",
        data: {
            announcementId: announcementId,
            isForAll: isForAll
        },
        cache: false,
        success: function (data) {
            if (data.success) {
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', 'Successsfully saved.');
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while saving.')
        }
    });
}

function exportAnnouncementSections() {
    var semesterId = $("#searchSemesterId").val();

    if (confirm("Are you sure you want to export announcement sections to old DB?"))
        $.ajax({
            url: "/ExportData/ExportAnnouncementSections",
            type: "post",
            data: {
                semesterId: semesterId
            },
            cache: false,
            success: function (data) {
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', 'Announcement sections exported');
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while exporting.');
            }
        });
    return false;
}

function activateAllAnnouncement(btn) {
    var announcementIds = $('input[name="announcementId"]').map(function () {
        return $(this).val();
    }).get();

    if (confirm("Are you sure you want to activate all announcements?"))
        $.ajax({
            url: "/Announcements/ActivateAll",
            type: "post",
            data: {
                ids: announcementIds
            },
            cache: false,
            success: function (data) {
                if (data.success) {
                    location.reload();
                } else {
                    showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
                }
            },
            error: function (response) {
                console.log(response);
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while activating.');
            }
        });
    return false;
}

function activateAnnouncement(btn) {
    var announcementId = $(btn).val();
    var container = $('#container-' + announcementId);

    $(btn).prop("disabled", true);
    $.ajax({
        url: "/Announcements/Activate",
        type: "post",
        data: {
            id: announcementId
        },
        cache: false,
        success: function (data) {
            if (data.success) {
                var statusIcon = container.find('#statusIcon');
                var statusText = container.find('#statusText');
                const newStatusText = "Activated";

                statusText.text(newStatusText);
                statusIcon.removeClass('text-danger').addClass('text-success');

                $(btn).addClass('d-none');
                $(btn).next('button').removeClass('d-none');
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
            $(btn).prop("disabled", false);
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while activating.');
            $(btn).prop("disabled", false);
        }
    });
    return false;
}

function deactivateAnnouncement(btn) {
    var announcementId = $(btn).val();
    var container = $('#container-' + announcementId);

    $(btn).prop("disabled", true);
    $.ajax({
        url: "/Announcements/Deactivate",
        type: "post",
        data: {
            id: announcementId
        },
        cache: false,
        success: function (data) {
            if (data.success) {
                var statusIcon = container.find('#statusIcon');
                var statusText = container.find('#statusText');
                const newStatusText = "Deactivated";

                statusText.text(newStatusText);
                statusIcon.removeClass('text-success').addClass('text-danger');

                $(btn).addClass('d-none');
                $(btn).prev('button').removeClass('d-none');
            } else {
                showPopupModal(CONSTS.MODAL_FAIL, 'Error', data.error);
            }
            $(btn).prop("disabled", false);
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while deactivating.');
            $(btn).prop("disabled", false);
        }
    });
    return false;
}

// Search

$('#searchText').on('input', searchItems);

function searchItems() {
    var value = $('#searchText').val().toLowerCase();

    $('.search-item').each(function (index) {
        var $searchItem = $(this);
        var $name = $searchItem.find('.search-name').text().toLowerCase();
        var $courseId = $searchItem.find('.search-course-id').text().toLowerCase();
        var $instructors = $searchItem.find('.search-instructor').text().toLowerCase();
        var $groups = $searchItem.find('.search-group').text().toLowerCase();

        if ($name.indexOf(value) >= 0 || $courseId.indexOf(value) >= 0 || $instructors.indexOf(value) >= 0 || $groups.indexOf(value) >= 0) {
            $searchItem.show();
        } else {
            $searchItem.hide();
        }
    });
}
