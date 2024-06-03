
$('button[name="deleteAdviser"]').click(function () {
    if (confirm("Are you sure you want delete adviser?")) {
        deleteAdviser($(this));
    }
});

function deleteAdviser(btn) {
    var row = $(btn).parent().parent();
    var deanUserId = $('#deanUserId').val();
    var instructorUserId = $(btn).parent().find('input[name=instructorUserId]').val();

    $.ajax({
        url: "/Deans/DeleteDeanAdviser",
        type: "POST",
        data: {
            "deanUserId": deanUserId,
            "instructorUserId": instructorUserId,
        },
        cache: false,
        success: function (response) {
            row.remove();
            updateNumbers();
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

//Update numbers 
function updateNumbers() {
    var nums = $(".num");
    for (var i = 0; i < nums.length; i++) {
        nums[i].textContent = (i + 1);
    }
}