//Show modal window with courses selection 
$('button[id=selectCoursesBtn]').click(function () {
    var year = $(this).parent().find('input[name=year]').val();
    var season = $(this).parent().find('input[name=season]').val();
    var departmentGroupId = $(this).parent().find('input[name=departmentGroupId]').val();
    var studyCardId = $(this).parent().find('input[name=studyCardId]').val();

    //Exclude already added courses
    var addedCoursesIds = getAddedCoursesIds();

    $.ajax({
        url: "/StudyCards/GetCoursesForSelection",
        traditional: true,
        data: {
            "year": year,
            "season": season,
            "departmentGroupId": departmentGroupId,
            "studyCardId": studyCardId,
            "excludedIds": addedCoursesIds
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
function getAddedCoursesIds() {
    var ids = [];
    $('input[name*="].CyclePartCourseId"]').each(function () {
        ids.push($(this).val());
    });

    return ids;
}

$(document).on('click', '#AddCourses', addCourses);

//Add courses from selection modal form
function addCourses() {

    var studyCardId = $(this).parent().find('input[name=studyCardId]').val();
    var selectedCoursesIds = getSelectedCoursesIds();

    $.ajax({
        url: "/StudyCards/GetCoursesFromSelection",
        dataType: "text",
        traditional: true,
        data: {
            "studyCardId": studyCardId,
            "ids": selectedCoursesIds
        },
        cache: false,
        success: function (html) {
            if (html.length > 4) {
                $("#courseContainer").append(html);

                updateCourseIndexes();
                updateNumbers();
                recalcTotalPoints();
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

//Get ids of selected courses in modal window
function getSelectedCoursesIds() {
    var ids = [];
    $('input[name=SelectCourse]').each(function () {
        if ($(this).prop('checked')) {
            ids.push($(this).parent().find("input[name=CyclePartCourseId]").val());
        }
    });

    return ids;
}

//Remove course row
$(document).on('click', 'button[name=removeCoursesBtn]', function () {
    $(this).parent().parent().remove();
    updateNumbers();
    updateCourseIndexes();
    recalcTotalPoints();
});

// Update indexes courses fields. Indexes must be updated after any change
function updateCourseIndexes() {
    console.log("updating indexes");
    
    var instructors = $('select[name*="].InstructorBasicInfoId"');
    var cyclePartIds = $('input[name*="].CyclePartCourseId"');
    var studyIds = $('input[name*="].StudyCardId"');
    var isVacancy = $('input[name*="].IsVacancy"');

    for (var i = 0; i < instructors.length; i++)
    {
        instructors[i].name = instructors[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        cyclePartIds[i].name = cyclePartIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        studyIds[i].name = studyIds[i].name.replace(/\[(.+)\]/, '[' + i + ']');
        isVacancy[i].name = isVacancy[i].name.replace(/\[(.+)\]/, '[' + i + ']');
    }
}

//Update numbers after courses quantity changing
function updateNumbers(cycleId, part) {
    var nums = $(".courseNum");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent=(i+1);
    }
}

$(document).on('change', 'input[name*="].IsVacancy"]', function () {
    var select = $(this).parent().parent().find('select[name*="].InstructorUserId"]');

    if ($(this).prop('checked') == true) {
        $(this).parent().parent().addClass("red-colored");
        select.val(0);
        select.attr("disabled", true);
    }
    else {
        $(this).parent().parent().removeClass("red-colored");
        select.attr("disabled", false);
    }
});

$(document).ready(function () {
    $('input[name*="].IsVacancy"]').each(function () {
        var select = $(this).parent().parent().find('select[name*="].InstructorUserId"]');

        if ($(this).prop('checked') == true) {
            select.attr("disabled", true);
        }
        else {
            select.attr("disabled", false);
        }
    });
});

function recalcTotalPoints() {
    var pts = 0;
    $('.pts').each(function () {
        pts += parseInt($(this).text());
    });
    $('#totalPoints').text(pts);
}