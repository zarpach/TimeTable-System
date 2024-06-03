
$('#checkAll').change(onCheckAll);

function onCheckAll () {
    var checked = $(event.currentTarget).prop("checked");
    $('input[name="checkRow"]').each(function () {
        $(this).prop("checked", checked)
    });
}

$('input[name *= "].IsDebt"]').change(function () {
    if ($(this).prop('checked')) {
        $(this).val(false);
    } else {
        $(this).val(true);
    }
})

$(document).on('click', '#isDebtBtn', isDebtAction);

function isDebtAction() {
    var checkedRows = $('input[name="checkRow"]:checked').parent().parent();
    checkedRows.each(function () {
        var isDebtInput = $(this).find('input[name*="].IsDebt"]');
        $(isDebtInput).bootstrapToggle('off')
        $(isDebtInput).val(true);
    });
}

$(document).on('click', '#isNotDebtBtn', isNotDebtAction);

function isNotDebtAction() {
    var checkedRows = $('input[name="checkRow"]:checked').parent().parent();
    checkedRows.each(function () {
        var isDebtInput = $(this).find('input[name*="].IsDebt"]');
        $(isDebtInput).bootstrapToggle('on')
        $(isDebtInput).val(false);
    });
}