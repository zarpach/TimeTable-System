
$(document).on('click', '#searchBtn', searchItems);

function searchItems() {
    var value = $('#searchText').val().toLowerCase();
    $('.search-item').each(function (index) {
        var searchText = $(this).text().toLowerCase();

        if (searchText.indexOf(value) >= 0) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

$(document).on('click', '#clearSearchBtn', clearSearch);

function clearSearch() {
    $('#searchText').val('');
    searchItems();
}

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        event.preventDefault();
        event.stopPropagation();
        $("#searchBtn").click();
    }
});
