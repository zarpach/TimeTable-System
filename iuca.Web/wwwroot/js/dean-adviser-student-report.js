
$("#deanUserId").change(function () {
    var deanUserId = $(this).val();

    if (deanUserId == "" || deanUserId == null) {
        $("#adviserUserId").html('');
    }

    $.ajax({
        type: "GET",
        url: "/RegistrationCourseReports/GetDeanAdvisers",
        data: { "deanUserId": deanUserId },
        success: function (response) {

            var items = '<option value="">All</option>';
            $.each(response, function (i, adviser) {
                items += "<option value='" + adviser.value + "'>" + adviser.text + "</option>";
            });

            $("#adviserUserId").html(items);

        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });

});