
const DataTableDisplayLength = 15; // so luong record hien thi moi page
var openMenuStatus = false; // menu dong



function DatePickerProvide() {
    var inputs = $('input[type=text]');
    inputs.each(function () {
        var input = $(this);
        var attr = input.attr('data-provide');
        if (attr !== 'undefined' && attr === 'datepicker') {
            $(input).datepicker({
                format: "dd/mm/yyyy",
                autoclose: true,
                todayHighlight: true,
            });
        }
    })
}

$(document).bind('keydown', function (e) {
    if (e.keyCode === 27 && openMenuStatus === true) {
        $('#close-menu-toggle').click();
    }
})

function dateFormat(d) {
    return (d.getDate() + "").padStart(2, "0") + "/" + ((d.getMonth() + 1) + "").padStart(2, "0") + "/" + d.getFullYear();
}

function ErrorAlert(message) {
    var notify = $('<div class="alert-danger notify-action"><p><i class="fa fa-exclamation-circle"></i> ' + message + '</p></div>');

    $(notify).hide();
    $('#alert-notify').html("");
    $('#alert-notify').prepend(notify);
    notify.fadeIn(200).delay(5000).fadeOut(200);

}

function SuccessAlert(message) {
    var notify = $('<div class="alert-success notify-action"><p><i class="fa fa-check-circle"></i> ' + message + '</p></div>');
    $(notify).hide();
    $('#alert-notify').html("");
    $('#alert-notify').prepend(notify);
    notify.fadeIn(200).delay(500).fadeOut(200);
}


function alphatext(value) {
    var letters = /^[0-9a-zA-Z]+$/;
    if (!value.match(letters)) {
        return false;
    }
    return true;
}

function valiFormatMMYYYY(value) {
    var month = 0, year = 0;
    if (value.length == 5) {
        month = Number(value.substring(0, 1))
        year = Number(value.substring(1, value.length));
    }
    else {
        month = Number(value.substring(0, 2))
        year = Number(value.substring(2, value.length));
    }

    var today = new Date();
    var thisYear = today.getFullYear();
    if ((month < 1 || month > 12) || (year < 1950 || year > thisYear)) {
        return false;
    }
    return true;
}


function valiDateToday(value) {
    var array = value.split('/');
    var day = Number(array[0]), month = Number(array[1]), year = Number(array[2]);
    var today = new Date();
    var dd = Number(String(today.getDate()).padStart(2, '0'));
    var mm = Number(String(today.getMonth() + 1).padStart(2, '0')); //January is 0!
    var yyyy = Number(today.getFullYear());
    if (yyyy > year) {
        return true;
    } else if (yyyy == year) {
        if (mm > month) {
            return true;
        } else if (mm == month) {
            if (dd > day || dd == day) {
                return true;
            }
        }
    } else return false;
}

function openHiddenMenu(menu, open_button, left_size, background_shadow) {
    if (menu.css('left') == left_size) {
        menu.css('left', '0');
        open_button.css("left", '-15px').css('transition', '0.8s');
        background_shadow.addClass('active-overlay');
        background_shadow.click(function () {
            _closeHiddenMenu(menu, background_shadow, left_size, open_button);
        });
        openMenuStatus = true;
    }
}

function _closeHiddenMenu(menu, background_shadow, left_size, open_button) {
    menu.css('left', left_size);
    background_shadow.removeClass('active-overlay');
    open_button.css('left', '0px');
    openMenuStatus = false;
}


function autoFill() {
    var elemQuery = $('input,select');
    if (elemQuery.length !== 0) {
        elemQuery.each(function () {
            var elem = $(this);
            if (elem.get(0).tagName === 'INPUT') {
                if (elem.attr('data-provide') === 'datepicker') {
                    var today = new Date();
                    var dd = String(today.getDate()).padStart(2, '0');
                    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
                    var yyyy = today.getFullYear();

                    today = dd + '/' + mm + '/' + yyyy;
                    elem.attr('value', today);
                }
                else if (elem.attr('readonly') !== 'readonly') {
                    var int = parseInt((Math.random() * 100), 10)
                    elem.attr('value', int);
                }
            }
            else {
                var $options = elem.find('option'),
                    random = ~~(Math.random() * $options.length);
                $options.eq(random).prop('selected', true);
            }
        })
    }
}