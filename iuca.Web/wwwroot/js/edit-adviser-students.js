//Show modal window with student selection 
$('button[id=selectStudentsBtn]').click(function () {
    var departmentGroupId = $(this).parent().find('input[name=departmentGroupId]').val();
    var deanUserId = $(this).parent().find('input[name=deanUserId]').val();

    //Exclude already added students
    var addedStudentsIds = getAddedStudentsIds();

    $.ajax({
        url: "/AdviserStudents/GetStudentsForSelection",
        traditional: true,
        data: {
            "deanUserId": deanUserId,
            "departmentGroupId": departmentGroupId,
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
    $('input[name="studentUserIds"]').each(function () {
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

    var organizationId = $(this).parent().find('input[name=organizationId]').val();
    var selectedStudentsIds = getSelectedStudentsIds();

    $.ajax({
        url: "/AdviserStudents/GetStudentsFromSelection",
        dataType: "text",
        traditional: true,
        data: {
            "organizationId": organizationId,
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

//Remove student row
$(document).on('click', 'button[name=removeStudentBtn]', function () {
    $(this).parent().parent().remove();
    updateNumbers();
});



//Update numbers after students quantity changing
function updateNumbers() {
    var nums = $(".studentNum");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent=(i+1);
    }
}

