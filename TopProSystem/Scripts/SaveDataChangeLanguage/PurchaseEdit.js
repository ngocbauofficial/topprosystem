$("#changeLanguage").click(function () {
    localStorage.clear();
    var values = $('#form-data').serializeArray();
    jsonObj = [];
    $('.item-child').each(function (data, index) {

        item = {}
        item[data] = $(index).find(".ABCOAT").attr("id");
        jsonObj.push(item);
    });

    jsonreadonly = [];
    $('.input-readonly').each(function (data, index) {
        item = {}
        item[data] = $(index).attr("name");
        jsonreadonly.push(item);
    });
    localStorage.setItem("form", JSON.stringify(values))
    localStorage.setItem("numberitem", JSON.stringify(jsonObj))
    localStorage.setItem("readonlyitem", JSON.stringify(jsonreadonly))
    localStorage.setItem("global", GLOBAL_INCREMENT)
})
$(document).ready(function () {
    if (localStorage.getItem("form") !== null) {
        $('.item-child').remove();
        var numberitem = localStorage.getItem("numberitem");
        resultnumberitem = jQuery.parseJSON(numberitem);
        resultnumberitem.forEach(function (data, index) {
            CreatePurchaseContractEntry_Item_ChangeLanguage(data[index])
        });
        BindData();
    }
});

function BindData() {
    var x = localStorage.getItem("form");
    result = jQuery.parseJSON(x);
    result.forEach(function (data, index) {
        $('[name=' + data.name + ']').val(data.value);
    });

    debugger;
    var y = localStorage.getItem("readonlyitem");
    resulty = jQuery.parseJSON(y);
    resulty.forEach(function (data, index) {
     //   $('[name=' + data.value + ']').addClass('input-readonly')
        $('#' + data[index]).addClass('input-readonly');
        $('#' + data[index]).attr('readonly', 'readonly');
    });
    $('.CompletetionStatus').each(function (data, index) {
        if($(index).val() == "1") {
            $(index).parent().find('.delete-item').remove();
            $(index).parent().find('.icon').remove();
        }
    });

    $('#AARMTP').change();
    $('#AACRRCD').change();
    var globalnumber = localStorage.getItem("global");
    GLOBAL_INCREMENT = parseInt(globalnumber);
    localStorage.clear();
}
function CreatePurchaseContractEntry_Item_ChangeLanguage(stringitem) {

    var tabindexStep = GLOBAL_INCREMENT * 19;
    $.ajax({
        url: '/Purchase/AddItem',
        type: 'get',
        async: false,
        success: function (rs) {
            var element = $(rs);
            //  element.attr('id', item_n);   // cap nhat id
            var purchase_ctr_no = element.find('input[type=text]').first();
            purchase_ctr_no.attr('value', x() + 1); // cap nhat purchase contract no
            var numberset = stringitem.match(/\d+/);
            if (numberset != null) {
                var childElem = element.find('input,select');

                childElem.each(function () {
                    ChangeID_Name_TabIndex($(this), parseInt(numberset), tabindexStep, true);
                });
            }

            $('.content-bottom').append(element.prop('outerHTML'));

            DisableRawMaterialType($('#AARMTP').val());
            AddUnitbyCurrency();
            // autoFill();
            DatePickerProvide(); // goi lai su kien datepicker
        }

    })
}
