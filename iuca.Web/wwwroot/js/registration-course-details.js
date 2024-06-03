
$('button[name="deleteStudentCourseBtn"]').click(function () {
    if (confirm("Вы действительно хотите пометить студента на удаление?"))
        deleteStudentCourse($(this));
});

function deleteStudentCourse(btn) {
    var studentCourseId = btn.parent().find('input[name="studentCourseId"]').val();

    $(btn).prop('disabled', true);

    $.ajax({
        url: "/RegistrationCourses/MarkStudentCourseDeleted",
        type: "POST",
        data: {
            studentCourseId: studentCourseId,
            isDeleted: true
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            $(btn).prop('disabled', false);
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время удаления!');
        },
        error: function (response) {
            $(btn).prop('disabled', false);
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время удаления!');
        }
    });
}

$('button[name="undeleteStudentCourseBtn"]').click(function () {
    if (confirm("Вы действительно хотите восстановить студента?"))
        undeleteStudentCourse($(this));
});

function undeleteStudentCourse(btn) {
    var studentCourseId = btn.parent().find('input[name="studentCourseId"]').val();

    $(btn).prop('disabled', true);

    $.ajax({
        url: "/RegistrationCourses/MarkStudentCourseDeleted",
        type: "POST",
        data: {
            studentCourseId: studentCourseId,
            isDeleted: false
        },
        cache: false,
        success: function (response) {
            location.reload();
        },
        failure: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время восстановления!');
        },
        error: function (response) {
            $(btn).prop('disabled', false);
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время восстановления!');
        }
    });
}

$('button[name="exportRegistrationCoursesBtn"]').click(function () {
    if (confirm("Вы действительно хотите экспортировать изменения?"))
        exportRegistrationCourse($(this));
});

function exportRegistrationCourse(btn) {
    var registrationCourseId = $('#registrationCourseId').val();

    $(btn).prop('disabled', true);

    $.ajax({
        url: "/ExportData/SynchronizeStudentCoursesByRegistrationCourse",
        type: "POST",
        data: {
            registrationCourseId: registrationCourseId
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

//Show modal window with student selection 
$('button[id=selectStudentsBtn]').click(function () {

    var semesterId = $('#semesterId').val();
    //Exclude already added students
    var addedStudentsIds = getAddedStudentsIds();

    console.log("addedStudentsIds: " + addedStudentsIds.length);

    $.ajax({
        url: "/RegistrationCourses/GetStudentsForSelection",
        traditional: true,
        data: {
            "semesterId": semesterId,
            "excludedIds": addedStudentsIds
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

function showModalWindow(html) {
    var modalContainer = $("#ModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

function hideModalWindow() {
    var modalContainer = $("#ModalContainer");
    modalContainer.find('.modal').modal('hide');
}

//Get already added courses ids
function getAddedStudentsIds() {
    var ids = [];
    $('input[name="studentUserId"]').each(function () {
        ids.push($(this).val());
    });

    return ids;
}

$(document).on('click', '#AddStudents', addStudents);

$(document).on('change', '#departmentGroup', departmentGroupFilter);

function departmentGroupFilter() {
    var departmentGroup = $('#departmentGroup').val();
    searchItems(departmentGroup);
}

function searchItems(searchText) {
    var value = searchText.toLowerCase();
    $('.student-item').each(function (index) {
        var searchText = $(this).text().toLowerCase();

        if (searchText.indexOf(value) >= 0) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

$(document).on('click', '#selectAll', selectAll);

function selectAll() {
    var checked = $('#selectAll').prop("checked");
    $('input[name="SelectStudent"]:visible').each(function () {
        $(this).prop("checked", checked);
    });
}

//Add students from selection modal form
function addStudents() {

    var semesterId = $('#semesterId').val();
    var registrationCourseId = $('#registrationCourseId').val();
    var selectedStudentsIds = getSelectedStudentsIds();

    $.ajax({
        url: "/RegistrationCourses/AddStudentsFromSelection",
        dataType: "text",
        traditional: true,
        data: {
            "semesterId": semesterId,
            "registrationCourseId": registrationCourseId,
            "studentUserIds": selectedStudentsIds
        },
        cache: false,
        success: function (html) {
            if (html.length > 4) {
                $("#studentsContainer").append(html);
                updateNumbers();
            }
            hideModalWindow();
        },
        failure: function (response) {
            console.log("failure");
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response);
            console.log(response.responseText);
        }
    });

};

//Get ids of selected students in modal window
function getSelectedStudentsIds() {
    var ids = [];
    $('input[name=SelectStudent]').each(function () {
        if ($(this).prop('checked')) {
            ids.push($(this).parent().find("input[name=StudentUserId]").val());
        }
    });

    return ids;
}

//Update numbers after students quantity changing
function updateNumbers() {
    var nums = $(".studentNum");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent = (i + 1);
    }
}