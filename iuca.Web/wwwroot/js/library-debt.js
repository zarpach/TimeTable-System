
$('input[name="libraryDebtCheck"]').change(function () {
    var semesterId = $(this).parent().find('input[name=semesterId]').val();
    var studentUserId = $(this).parent().find('input[name=studentUserId]').val();
    var noDebt = $(this).prop("checked");

    $.ajax({
        url: "/StudentDebts/SetLibraryDebt",
        type: "POST",
        data: {
            "semesterId": semesterId,
            "studentUserId": studentUserId,
            "noDebt": noDebt
        },
        cache: false,
        success: function (response) {
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});
