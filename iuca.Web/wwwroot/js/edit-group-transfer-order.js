
var studentSelect = document.getElementById("studentSelect");

studentSelect.addEventListener("change", function () {
    UpdateSourceGroupValue(studentSelect);
    UpdatePreviousAdvisorsValue(studentSelect);
});

UpdateSourceGroupValue(studentSelect);
UpdatePreviousAdvisorsValue(studentSelect);

function UpdateSourceGroupValue(studentSelect) {
    var groupInput = document.getElementById("groupInput");

    var selectedStudentId = studentSelect.value;
    $.ajax({
        url: "/GroupTransferOrders/GetStudentGroupCode",
        type: "post",
        data: {
            studentUserId: selectedStudentId
        },
        cache: false,
        success: function (data) {
            groupInput.value = data;
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while updating sourse group value.');
        }
    });
}

function UpdatePreviousAdvisorsValue(studentSelect) {
    var advisersInput = document.getElementById("advisersInput");
    
    var selectedStudentId = studentSelect.value;
    $.ajax({
        url: "/GroupTransferOrders/GetStudentAdviserFullnames",
        type: "post",
        data: {
            studentUserId: selectedStudentId
        },
        cache: false,
        success: function (data) {
            var adviserNames = data.map(function (adviserName) {
                return adviserName;
            }).join(", ");

            advisersInput.value = adviserNames;
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while updating previous advisors value.');
        }
    });
}

