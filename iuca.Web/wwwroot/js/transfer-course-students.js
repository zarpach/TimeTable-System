
$('select[name=semesterId]').change(function () {
    $("#studentsFromContainer").html("");
    $("#studentsToContainer").html("");
});

//Show modal window with course selection
$('button[id=selectCourseFromBtn]').click(function () {
    showSelectCourseWindow(true);
});

$('button[id=selectCourseToBtn]').click(function () {
    showSelectCourseWindow(false);
});

function showSelectCourseWindow(isFrom) {
    var semesterId = $('select[name=semesterId]').val();

    $.ajax({
        url: "/RegistrationCourseManagement/GetCoursesForSelection",
        traditional: true,
        data: {
            "isFrom": isFrom,
            "semesterId": semesterId
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
}

function showModalWindow(html) {
    var modalContainer = $("#ModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

function hideModalWindow() {
    var modalContainer = $("#ModalContainer");
    modalContainer.find('.modal').modal('hide');
}

$(document).on('click', 'button[name="selectCourseBtn"]', selectCourse);

//Display course students
function selectCourse() {

    var registrationCourseId = $(this).parent().find('input[name="registrationCourseId"]').val();
    var isFrom = $(this).parent().find('input[name="isFrom"]').val();

    $.ajax({
        url: "/RegistrationCourseManagement/GetRegistrationCourseStudents",
        dataType: "text",
        traditional: true,
        data: {
            "registrationCourseId": registrationCourseId,
            "isFrom": isFrom
        },
        cache: false,
        success: function (html) {
            if (isFrom == "true") {
                $("#studentsFromContainer").html(html);
            }
            else {
                $("#studentsToContainer").html(html);
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

$(document).on('click', 'button[name="transferCourseToBtn"]', transferCourseTo);

function transferCourseTo() {
    var courseIdTo = $('input[name=courseIdTo]').val();
    if (courseIdTo != undefined)
    {
        //Hide transfer button and show return btn
        $(this).attr('hidden', true);
        $(this).parent().find('button[name="transferCourseFromBtn"]').attr('hidden', false);

        //Rename student user id
        $(this).parent().parent().find('input[name=studentUserId]').attr('name', 'transferStudentUserId');

        //Move row to another container
        $(this).parent().parent().appendTo('#studentsToContainer')
        updateNumbers("#studentsFromContainer");
        updateNumbers("#studentsToContainer");
    }
}

$(document).on('click', 'button[name="transferCourseFromBtn"]', transferCourseFrom);

function transferCourseFrom() {
    //Hide return button and show transfer btn
    $(this).attr('hidden', true);
    $(this).parent().find('button[name="transferCourseToBtn"]').attr('hidden', false);

    //Rename student user id
    $(this).parent().parent().find('input[name=transferStudentUserId]').attr('name', 'studentUserId');

    //Move row to another container
    $(this).parent().parent().appendTo('#studentsFromContainer')
    updateNumbers("#studentsFromContainer");
    updateNumbers("#studentsToContainer");
}

//Update numbers after students quantity changing
function updateNumbers(container) {
    var nums = $(container).find(".studentNum");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent=(i+1);
    }
}

$(document).on('click', '#saveBtn', saveCourses);

function saveCourses() {

    var courseIdFrom = $('input[name=courseIdFrom]').val();
    var courseIdTo = $('input[name=courseIdTo]').val();

    var transferStudentUserIds = [];
    $('input[name=transferStudentUserId]').each(function () {
        transferStudentUserIds.push($(this).val());
    });

    console.log('courseIdFrom: ' + courseIdFrom);
    console.log('courseIdTo: ' + courseIdTo);
    console.log('transferStudentUserIds: ' + transferStudentUserIds);

    $.ajax({
        url: "/RegistrationCourseManagement/SaveTransferCourseStudents",
        type: "POST",
        data: {
            "courseIdFrom": courseIdFrom,
            "courseIdTo": courseIdTo,
            "transferStudentUserIds": transferStudentUserIds
        },
        cache: false,
        success: function (response) {
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Успешно сохранено');
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
            console.log(response.responseText);
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время сохранения!');
            console.log(response.responseText);
        }
    });
}
