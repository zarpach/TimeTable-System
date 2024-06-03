

function syllabusModal(registrationCourseId) {
    $.ajax({
        url: "/StudentCourseRegistrations/GetSyllabus",
        traditional: true,
        data: {
            "registrationCourseId": registrationCourseId
        },
        cache: false,
        success: function (html) {
            showSyllabusModalWindow(html);
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

function showSyllabusModalWindow(html) {
    var modalContainer = $("#syllabusModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

function attendanceModal(semesterId) {
    $.ajax({
        url: "/Attendance/StudentCoursesAttendance",
        traditional: true,
        data: {
            "semesterId": semesterId
        },
        cache: false,
        success: function (html) {
            showAttendanceModalWindow(html);
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

function showAttendanceModalWindow(html) {
    var modalContainer = $("#attendanceModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}