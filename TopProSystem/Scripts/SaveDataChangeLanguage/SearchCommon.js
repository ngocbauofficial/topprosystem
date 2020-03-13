$("#changeLanguage").click(function () {
    debugger;
    var values = $('#form-search').serializeArray();
    localStorage.setItem("form", JSON.stringify(values))
})
$(window).on('load', function () {
    if (localStorage.getItem("form") !== null) {
        BindData();
    }
});

function BindData() {
    var x = localStorage.getItem("form");
    result = jQuery.parseJSON(x);
    result.forEach(function (data, index) {
        $('#' + data.name).val(data.value)
    });
    $("#btnSearch").click();
    localStorage.clear();
}

