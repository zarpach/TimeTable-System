$(document).ready(function () {
    var slotId;
    var actionType = $('#actionType').val();
    if (actionType === "Edit") {
        $("#createButton").hide();
        $("#updateButton").show();
        $("#deleteButton").show();
    } else {
        $("#createButton").show();
        $("#updateButton").hide();
        $("#deleteButton").hide();
    }

    $('#resetButton').off('click').on('click', function () {
        $('#departmentGroupSelect').multiselect('rebuild');
    })

    window.setTimeout(function () {
        $(".alert-success").fadeTo(2000, 500).slideUp(500, function () {
            $(".alert-success").slideUp(500);
        });
    }, 3000);


});