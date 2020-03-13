$("#changeLanguage").click(function () {
    debugger;
    var values = $('form[name="formdata"]').serializeArray();
    localStorage.setItem("form", JSON.stringify(values));
    if ($('.search').length) {
        var values = $('.search').val();
        localStorage.setItem("searchvalue", values)
    }
})
$(document).ready(function () {
    if (localStorage.getItem("form") !== null) {
        BindData();
    }
    if (localStorage.getItem("searchvalue") !== null) {
        var x = localStorage.getItem("searchvalue");
        $('.search').val(x);
        $("#btnSearch").click();
    }
    localStorage.clear();
});

function BindData() {
    var x = localStorage.getItem("form");
    result = jQuery.parseJSON(x);
    result.forEach(function (data, index) {
        $('#' + data.name).val(data.value)
    });
  
}


