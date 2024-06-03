
$('#importCoursesBtn').click(function () {
    var btn = $(this);
    spinIcon(btn, true);

    $.ajax({
        url: "/ImportData/ImportCourses",
        type: "POST",
        data: {
            overwrite: false
        },
        cache: false,
        success: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Синхронизация успешно завершена!');
        },
        failure: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        },
        error: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        }
    });
});

$('#importInstructorsBtn').click(function () {
    var btn = $(this);

    spinIcon(btn, true);

    $.ajax({
        url: "/ImportData/ImportInstructors",
        type: "POST",
        data: {
            overwrite: false
        },
        cache: false,
        success: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Синхронизация успешно завершена!');
        },
        failure: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        },
        error: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        }
    });
});

$('#importRegistrationCoursesBtn').click(function () {
    var btn = $(this);
    var semesterId = btn.val();

    spinIcon(btn, true);

    $.ajax({
        url: "/ImportData/ImportAnnouncementSectionsBySemester",
        type: "POST",
        data: {
            overwrite: false,
            semesterId: semesterId
        },
        cache: false,
        success: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Синхронизация успешно завершена!');
        },
        failure: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        },
        error: function (response) {
            spinIcon(btn, false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время синхронизации!');
        }
    });
});

$('button[name="exportRegistrationCourseBtn"]').click(function () {
    if (confirm("Вы действительно хотите экспортировать изменения?"))
        exportRegistrationCourse($(this));
});

function exportRegistrationCourse(btn) {
    var announcementSectionId = btn.parent().find('input[name="announcementSectionId"]').val();

    $(btn).prop('disabled', true);

    $.ajax({
        url: "/ExportData/ExportRegistrationCourseData",
        type: "POST",
        data: {
            announcementSectionId: announcementSectionId
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
        },
        error: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время экспорта!');
        }
    });
}

$('button[name="importRegistrationCourseBtn"]').click(function () {
    if (confirm("Вы действительно хотите импортировать изменения из старой базы?"))
        importRegistrationCourse($(this));
});

function importRegistrationCourse(btn) {
    var courseDetId = btn.parent().find('input[name="courseDetId"]').val();

    $(btn).prop('disabled', true);

    $.ajax({
        url: "/ImportData/ImportAnnouncementSectionData",
        type: "POST",
        data: {
            courseDetId: courseDetId
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время импорта!');
        },
        error: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время импорта!');
        }
    });
}

//$('button[name="deleteRegistrationCourseBtn"]').click(function () {
//    if (confirm("Вы действительно хотите пометить курс на удаление?"))
//        deleteRegistrationCourse($(this));
//});

//function deleteRegistrationCourse(btn) {
//    var courseDetId = btn.parent().find('input[name="courseDetId"]').val();

//    $(btn).prop('disabled', true);

//    $.ajax({
//        url: "/RegistrationCourses/MarkRegistrationCourseDeleted",
//        type: "POST",
//        data: {
//            courseDetId: courseDetId,
//            isDeleted: true
//        },
//        cache: false,
//        success: function (response) {
//            location.reload();
//        },
//        failure: function (response) {
//            $(btn).prop('disabled', false);
//            console.log(response);
//            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время удаления!');
//        },
//        error: function (response) {
//            $(btn).prop('disabled', false);
//            console.log(response);
//            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время удаления!');
//        }
//    });
//}

//$('button[name="undeleteRegistrationCourseBtn"]').click(function () {
//    if (confirm("Вы действительно хотите восстановить курс?"))
//        undeleteRegistrationCourse($(this));
//});

//function undeleteRegistrationCourse(btn) {
//    var courseDetId = btn.parent().find('input[name="courseDetId"]').val();

//    $(btn).prop('disabled', true);

//    $.ajax({
//        url: "/RegistrationCourses/MarkRegistrationCourseDeleted",
//        type: "POST",
//        data: {
//            courseDetId: courseDetId,
//            isDeleted: false
//        },
//        cache: false,
//        success: function (response) {
//            location.reload();
//        },
//        failure: function (response) {
//            $(btn).prop('disabled', false);
//            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время восстановления!');
//        },
//        error: function (response) {
//            $(btn).prop('disabled', false);
//            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время восстановления!');
//        }
//    });
//}

function spinIcon(btn, spin) {
    if (spin)
        $(btn).find('i').addClass('animate-icon rotate');
    else
        $(btn).find('i').removeClass('animate-icon rotate');
    $(btn).prop('disabled', spin);
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

$(document).on('click', '#clearSearchBtn', clearSearch);

function clearSearch() {
    $('#searchText').val('');
    searchItems();
}

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        event.preventDefault();
        event.stopPropagation();
        $("#searchBtn").click();
    }
});