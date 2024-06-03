
$('#saveRegistrationStateBtn').click(function () {
    if (confirm("Are you sure you want to save registration state?")) {
        var btn = $(this);
        btn.prop('disabled', true);

        var registrationId = $("#registrationId").val();
        var state = $("#registrationState").val();

        $.ajax({
            url: "/StudentCourseRegistrations/SetRegistrationState",
            type: "POST",
            data: {
                registrationId: registrationId,
                state: state
            },
            cache: false,
            success: function (response) {
                btn.prop('disabled', false);
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Статус успешно сохранен!');
            },
            failure: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
                btn.prop('disabled', false);
            },
            error: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
                btn.prop('disabled', false);
            }
        });
    }
});

$('#saveAddDropStateBtn').click(function () {
    if (confirm("Are you sure you want to save add/drop state?")) {
        var btn = $(this);
        btn.prop('disabled', true);

        var registrationId = $("#registrationId").val();
        var state = $("#addDropState").val();

        $.ajax({
            url: "/StudentCourseRegistrations/SetAddDropState",
            type: "POST",
            data: {
                registrationId: registrationId,
                state: state
            },
            cache: false,
            success: function (response) {
                btn.prop('disabled', false);
                showPopupModal(CONSTS.MODAL_SUCCESS, 'Успех', 'Статус успешно сохранен!');
            },
            failure: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
                btn.prop('disabled', false);
            },
            error: function (response) {
                showPopupModal(CONSTS.MODAL_FAIL, 'Ошибка', 'Произошла ошибка во время обновления статусов!');
                btn.prop('disabled', false);
            }
        });
    }
});

$('#addRegularCourseBtn').click(function () {
    displayCoursesForSelection(0);
});

$('#addAddedCourseBtn').click(function () {
    displayCoursesForSelection(1);
});

$('#addDroppedCourseBtn').click(function () {
    displayCoursesForSelection(2);
});

function displayCoursesForSelection(state) {
    var semesterId = $('#semesterId').val();

    $.ajax({
        url: "/StudentCourseRegistrations/GetCoursesForSelection",
        traditional: true,
        data: {
            "semesterId": semesterId,
            "state": state
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

$(document).on('click', 'button[name="selectRegularCourseBtn"]', function () {
    addCourseByAdmin($(this), 0, $("#regularCoursesContainer"));
});

$(document).on('click', 'button[name="selectAddedCourseBtn"]', function () {
    addCourseByAdmin($(this), 1, $("#addedCoursesContainer"));
});

$(document).on('click', 'button[name="selectDroppedCourseBtn"]', function () {
    addCourseByAdmin($(this), 2, $("#droppedCoursesContainer"));
});

function addCourseByAdmin(btn, state, container) {
    var studentCourseRegistrationId = $("#registrationId").val();
    var registrationCourseId = btn.parent().find('input[name="registrationCourseId"]').val();

    $.ajax({
        url: "/StudentCourseRegistrations/AddCourseByAdmin",
        method: "POST",
        traditional: true,
        data: {
            "studentCourseRegistrationId": studentCourseRegistrationId,
            "registrationCourseId": registrationCourseId,
            "state": state
        },
        cache: false,
        success: function (courseRow) {
            $(container).append(courseRow);
            updateNumbers(container);
            if (state != 2) {
                recalcTotalCredits();
            }
            hideModalWindow();
        },
        failure: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Ошибка во время добавления записи");
        },
        error: function (response) {
            showPopupModal(CONSTS.MODAL_FAIL, "Error", "Ошибка во время добавления записи");
        }
    });
}

$(document).on('click', 'button[name="removeCourseBtn"]', function () {
    if (confirm("Are you sure you want remove this course?")) {
        removeCourse($(this));
    }
});

function removeCourse(btn) {

    var studentCourseId = btn.parent().parent().find('input[name="studentCourseId"]').val();

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/RemoveCourseByAdmin",
        data: {
            "studentCourseId": studentCourseId
        },
        success: function (response) {
            btn.parent().parent().parent().parent().remove();
            updateAllNumbers();
            recalcTotalCredits();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

$(document).on('change', 'input[name="isAudit"]', function () {
    if (confirm("Are you sure you want change audit flag?")) {
        markAudit($(this));
    }
});

function markAudit (checkbox) {
    var item = $(checkbox).parent().parent().parent().parent().parent();
    var studentCourseId = item.find('input[name="studentCourseId"]').val();
    var checked = $(checkbox).prop("checked");

    $.ajax({
        type: "POST",
        url: "/StudentCourseRegistrations/MarkAudit",
        data: {
            "studentCourseId": studentCourseId,
            "isAudit": checked,
        },
        success: function (response) {

            if (checked) {
                item.find('.creds').each(function () {
                    $(this).removeClass('countable');
                });
            } else {
                item.find('.creds').each(function () {
                    $(this).addClass('countable');
                });
            }
            recalcTotalCredits();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};

function updateAllNumbers() {
    updateNumbers($('#regularCoursesContainer'));
    updateNumbers($('#addedCoursesContainer'));
    updateNumbers($('#droppedCoursesContainer'));
}

function updateNumbers(container) {
    var nums = $(container).find(".num");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent = (i + 1);
    }
}

function recalcTotalCredits() {
    var totalCredits = 0;
    var creds = $("#creds-container").find(".countable");
    for (var i = 0; i < creds.length; i++) {
        totalCredits += parseInt(creds[i].textContent);
    }
    $('#totalCredits').text(totalCredits);
}