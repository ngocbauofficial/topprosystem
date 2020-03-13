$("#changeLanguage").click(function () {
    var values = $('.search').val();
    localStorage.setItem("searchvalue", values)
})
$(document).ready(function () {
    if (localStorage.getItem("searchvalue") !== null) {
        var x = localStorage.getItem("searchvalue");
        $('.search').val(x);
        $("#btnSearch").click();
        localStorage.clear();
    }
});

