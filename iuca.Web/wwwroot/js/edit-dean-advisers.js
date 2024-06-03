//Show modal window with adviser selection 
$('button[id=selectAdvisersBtn]').click(function () {
    var departmentGroupId = $(this).parent().find('input[name=departmentGroupId]').val();

    //Exclude already added advisers
    var addedAdvisersIds = getAddedAdvisersIds();

    $.ajax({
        url: "/Deans/GetAdvisersForSelection",
        traditional: true,
        data: {
            "excludedIds": addedAdvisersIds
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
function getAddedAdvisersIds() {
    var ids = [];
    $('input[name="adviserUserIds"]').each(function () {
        ids.push($(this).val());
    });

    return ids;
}

$(document).on('click', '#AddAdvisers', addAdvisers);

//Add advisers from selection modal form
function addAdvisers() {

    var organizationId = $(this).parent().find('input[name=organizationId]').val();
    var selectedAdvisersIds = getSelectedAdvisersIds();

    $.ajax({
        url: "/Deans/GetAdvisersFromSelection",
        dataType: "text",
        traditional: true,
        data: {
            "organizationId": organizationId,
            "adviserUserIds": selectedAdvisersIds
        },
        cache: false,
        success: function (html) {
            if (html.length > 4) {
                $("#advisersContainer").append(html);
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

//Get ids of selected advisers in modal window
function getSelectedAdvisersIds() {
    var ids = [];
    $('input[name=SelectAdviser]').each(function () {
        if ($(this).prop('checked')) {
            ids.push($(this).parent().find("input[name=AdviserUserId]").val());
        }
    });

    return ids;
}

//Remove adviser row
$(document).on('click', 'button[name=removeAdviserBtn]', function () {
    $(this).parent().parent().remove();
    updateNumbers();
});

//Update numbers after advisers quantity changing
function updateNumbers() {
    var nums = $(".adviserNum");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent=(i+1);
    }
}

