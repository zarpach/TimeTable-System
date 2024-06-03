//Global variables
var groupedColNum = 3; //Number of colums that will be grouped in row 


//Show modal window with courses selection form for cycle+part
$('button[name=selectCoursesBtn]').click(function () {
    var cyclePartIndex = $(this).parent().find('input[name=cyclePartIndex]').val();
    var cycleName = $(this).parent().find('input[name=cycleName]').val();
    var partName = $(this).parent().find('input[name=partName]').val();

    var cycleId = $(this).parent().find('input[name*="].CycleId"]').val();
    var part = $(this).parent().find('input[name*="].AcademicPlanPart"]').val();

    //Exclude already added courses
    var addedCoursesIds = getAddedCoursesIds();

    $.ajax({
        url: "/AcademicPlans/GetCoursesForSelection",
        traditional: true,
        data: {
            "cyclePartIndex": cyclePartIndex,
            "cycleId": cycleId,
            "cycleName": cycleName,
            "part": part,
            "partName": partName,
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
    $('input[name*="].CourseId"]').each(function () {
        ids.push($(this).val());
    });

    return ids;
}

$(document).on('click', '#AddCourses', addCourses);

//Add courses from selection modal form
function addCourses() {
    var cyclePartIndex = $(this).parent().find('input[name=cyclePartIndex]').val();
    var cycleId = $(this).parent().find('input[name=cycleId]').val();
    var part = $(this).parent().find('input[name=part]').val();

    var selectedCoursesIds = getSelectedCoursesIds();

    $.ajax({
        url: "/AcademicPlans/GetCoursesFromSelection",
        dataType: "text",
        traditional: true,
        data: {
            "cyclePartIndex": cyclePartIndex,
            "cycleId": cycleId,
            "part": part,
            "ids": selectedCoursesIds
        },
        cache: false,
        success: function (html) {
            if (html.length > 4) {
                var row = "row_" + cycleId + "_" + part;
                $("#" + row).after(html);

                updateCourseIndexes(cyclePartIndex, "." + row);

                //Update courses numbers
                updateNumbers(".num_" + cycleId + "_" + part);

                //Set integer only filter for cycle part inputs
                $('.' + row).each(function () {
                    $(this).find('.integer-only').inputFilter(function (value) {
                        return /^[0-9]+$/.test(value);    // Allow digits only, using a RegExp
                    });
                });
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
            ids.push($(this).parent().find("input[name=CourseId]").val());
        }
    });

    return ids;
}

//Remove course row
$(document).on('click', 'button[name=removeCoursesBtn]', function () {
    var cyclePartIndex = $(this).parent().parent().find('input[name=cyclePartIndex]').val();
    var cycleId = $(this).parent().parent().find('input[name=cycleId]').val();
    var part = $(this).parent().parent().find('input[name=part]').val();
    var numClassName = ".num_" + cycleId + "_" + part;

    var currentRow = $(this).parent().parent();

    var guid = $(currentRow).find('input[name*="GroupId"]').val();
    currentRow.remove();

    processRestRowsAfterDetaching(guid, numClassName);
    updateNumbers(numClassName);

    updateCourseIndexes(cyclePartIndex, ".row_" + cycleId + "_" + part);
});

//Recalc total points after required points change for course semester
$(document).on('focusin', 'input[name*="].ReqPts"]', function () {
    $(this).data('val', $(this).val());
}).on('change', 'input[name*="].ReqPts"]', function () {
    var cycleId = $(this).parent().parent().find('input[name*="].CycleId"]').val();

    var cyclePtsId = "span[id=" + "cycle_" + $(this)[0].name.split("].")[1] + "_" + cycleId + "]";
    var totalPtsId = "span[id=" + "total_" + $(this)[0].name.split("].")[1] + "]";

    var prevPointsValue = parseInt($(this).data('val'));
    var currPointsValue = parseInt($(this).val());

    $(cyclePtsId).text($(cyclePtsId).text() - prevPointsValue + currPointsValue);
    $(totalPtsId).text($(totalPtsId).text() - prevPointsValue + currPointsValue);
});

// Update indexes of academic plan courses fields. Indexes must be updated after any change
function updateCourseIndexes(cyclePartIndex, rowClass) {
    console.log("updating indexes");
    var rows = $(rowClass);

    var courseIndex = 0;
    rows.each(function () {
        var inputs = $(this).find('input[name*="CyclePartCourses"]');
        for (var i = 0; i < inputs.length; i++)
        {
            inputs[i].name = inputs[i].name.replace(/\[(.+)\].CyclePartCourses\[(.+)\]/, '[' + cyclePartIndex + '].CyclePartCourses[' + courseIndex + ']');
        }
        courseIndex++;
    });
}

//Update numbers after courses quantity changing
function updateNumbers(className) {
    var nums = $(className);
    var num = 1;
    nums.each(function () {
        if (parseInt($(this)[0].textContent) >= 0)
            $(this)[0].textContent = num++;
    });
}

$("#Year").inputFilter(function (value) {
    return /^[0-9]+$/.test(value);    // Allow digits only, using a RegExp
});

$('.integer-only').inputFilter(function (value) {
    return /^[0-9]+$/.test(value);    // Allow digits only, using a RegExp
});

//Display group button after checkbox change
$(document).on('change', 'input[name*="groupCheckbox"]', function () {
    var inputName = 'input[name="' + $(this)[0].name + '"]';
    var rowId = $(this)[0].name.split(".")[1];

    displayGroupBtn(inputName, rowId);
});

function displayGroupBtn(inputName, rowId) {
    var count = 0;
    $(inputName).each(function () {
        if ($(this).prop("checked"))
            count++;
        
        if (count > 1)
            return false;
    });

    var buttonName = 'button[name="groupRowBtn.' + rowId + '"]';
    if (count > 1)
        $(buttonName).show();
    else 
        $(buttonName).hide();
}

//Group checked rows
$('button[name*=groupRowBtn]').click(function () {

    var groupBtn = $(this);
    var rowId = $(groupBtn)[0].name.split(".")[1];
    var numClassName = ".num_" + rowId.substring(4);
    var rows = [];
    var newGuid = uuidv4();

    $('.' + rowId).each(function () {
        var checkBox = $(this).find('input[name="groupCheckbox.' + rowId + '"]');
        if (checkBox.prop("checked")) {

            //Process all connected rows if row was already grouped
            var guid = $(this).find('input[name*="GroupId"]').val();
            if (guid != '00000000-0000-0000-0000-000000000000') {
                $('input[name*=".GroupId"]').each(function () {
                    if ($(this).val() == guid) {
                        var row = $(this).parent().parent();
                        collectRowForGrouping(row, rows, newGuid);
                    }
                });
            }
            else { //Else group current row
                collectRowForGrouping($(this)[0], rows, newGuid);
            }

            //Uncheck current checkbox
            checkBox.prop("checked", false);
        }

        //Hide group button
        groupBtn.hide();
    });

    groupRows(rows, numClassName, rowId);
    updateNumbers(numClassName);
});

function collectRowForGrouping(row, rows, newGuid) {
    $(row).detach();
    rows.push(row);
    $(row).find('input[name*="GroupId"]').val(newGuid);
}

//Generate new guid
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

//Set rowspan and columns visibility
function groupRows(rows, numClassName, rowId) {
    if (rows.length > 1) {

        //Set rowspan and elements visibility to the first row
        var firstRowTD = $(rows[0]).find("td");
        for (var i = 0; i < groupedColNum; i++) {
            firstRowTD[i].setAttribute("rowspan", rows.length);
        }

        $(rows[0]).find('textarea[name*="GroupName"]')[0].setAttribute("style", "display:block;");
        $(rows[0]).find('button[name*="ungroupRow"]')[0].setAttribute("style", "display:block;");

        //Set visibility for the rest rows in the group
        for (var i = 1; i < rows.length; i++) {

            var rowTD = $(rows[i]).find("td");
            for (var j = 0; j < groupedColNum; j++) {
                rowTD[j].setAttribute("style", "display:none;");
            }

            $(rows[i]).find(numClassName)[0].textContent = -1;
            $(rows[i]).find('button[name*="ungroupRow"]')[0].setAttribute("style", "display:block;");
        }

        //Show rows
        for (var i = (rows.length - 1); i >= 0; i--) {
            $("#" + rowId).after(rows[i]);
        }
    }
}

//Duplicate textarea values to all hidden ones in the group
$(document).on('change', 'textarea[name*=".GroupName"]', function () {
    var text = $(this).val();
    var guid = $(this).parent().parent().find('input[name*="GroupId"]').val();
    if (guid != '00000000-0000-0000-0000-000000000000') {
        $('input[name*=".GroupId"]').each(function () {
            if ($(this).val() == guid) {
                $(this).parent().parent().find('textarea[name*=".GroupName"]').each(function () {
                    $(this).val(text);
                });
            }
        });
    }
});

$(document).on('click', 'button[name*="ungroupRow"]', function () {
    var untgroupBtn = $(this);
    var rowId = $(untgroupBtn)[0].name.split(".")[1];
    var numClassName = ".num_" + rowId.substring(4);

    var currentRow = untgroupBtn.parent().parent();
    currentRow.detach();
    showTdAndSetRowspan(currentRow, 1);
    $("#" + rowId).after(currentRow);

    var guid = $(currentRow).find('input[name*="GroupId"]').val();

    detachRowFromGroup(currentRow, numClassName);
    processRestRowsAfterDetaching(guid, numClassName);
    updateNumbers(numClassName);
});

//Process all rows with group id after detaching any row from group
function processRestRowsAfterDetaching(guid, numClassName) {
    if (guid != '00000000-0000-0000-0000-000000000000') {
        var groupedRows = [];

        //Find all the rest rows with same group id
        $('input[name*=".GroupId"]').each(function () {
            if ($(this).val() == guid) {
                groupedRows.push($(this).parent().parent());
            }
        });

        if (groupedRows.length > 1) {//Set the first row as the main row
            showTdAndSetRowspan(groupedRows[0], groupedRows.length);
            $(groupedRows[0]).find('textarea[name*="GroupName"]')[0].setAttribute("style", "display:block;");
            $(groupedRows[0]).find(numClassName)[0].textContent = 0;

        }
        else if (groupedRows.length == 1) { //Ungroup row if there is only one row left
            showTdAndSetRowspan(groupedRows[0], 1);
            detachRowFromGroup(groupedRows[0], numClassName);
        }
    }
}

//Clear values. Hide textarea and ungroup button
function detachRowFromGroup(row, numClassName) {
    $(row).find('input[name*="GroupId"]').val('00000000-0000-0000-0000-000000000000');
    $(row).find(numClassName)[0].textContent = 0;
    var textArea = $(row).find('textarea[name*="GroupName"]');
    $(textArea[0]).val("");
    textArea[0].setAttribute("style", "display:none;");
    $(row).find('button[name*="ungroupRow"]')[0].setAttribute("style", "display:none;");
}

//Show td and set rowspan for row
function showTdAndSetRowspan(row, rowSpan) {
    var rowTDs = $(row).find("td");

    for (var i = 0; i < groupedColNum; i++) {
        rowTDs[i].setAttribute("style", "display:revert;");
        rowTDs[i].setAttribute("rowspan", rowSpan);
    }
}