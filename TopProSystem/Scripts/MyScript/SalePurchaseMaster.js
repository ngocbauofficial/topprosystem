$(function () {
    $('#MACNTRC').change(function () {
        var data = $(this).val();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '010|' + data }).done(function (rs) {
                $('#MACNTRC_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MACNTRC_dl').val('');
        }
    });

    $('#MABUZCD').change(function () {
        var data = $(this).val();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '011|' + data }).done(function (rs) {
                $('#MABUZCD_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MABUZCD_dl').val('');
        }
    });

    $('#MAIDCD').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonMa003', { userid: data }).done(function (rs) {
                $('#MAIDCD_dl').val(rs.MCIDNM);
            })
        } else {
            $('#MAIDCD_dl').val('');
        }
    });

    $('#MASPCTG').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            if (data === 'S') {
                $('#MASPCTG_dl').val('Sales');
            } else {
                $('#MASPCTG_dl').val('Purchase');
            }
        } else {
            $('#MASPCTG_dl').val('');
        }

    });

    $('#MASTDUE').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '001|' + data }).done(function (rs) {
                $('#MASTDUE_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MASTDUE_dl').val('');
        }
    });

    $('#MAPTDUE').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '001|' + data }).done(function (rs) {
                $('#MAPTDUE_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MAPTDUE_dl').val('');
        }
    });

    $('#MASDAYS').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '002|' + data }).done(function (rs) {
                $('#MASDAYS_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MASDAYS_dl').val('');
        }
    });

    $('#MAPDAYS').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '002|' + data }).done(function (rs) {
                $('#MAPDAYS_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MAPDAYS_dl').val('');
        }
    });

    $('#MASCRCD').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '012|' + data }).done(function (rs) {
                $('#MASCRCD_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MASCRCD_dl').val('');
        }
    })

    $('#MAPCRCD').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '012|' + data }).done(function (rs) {
                $('#MAPCRCD_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MAPCRCD_dl').val('');
        }
    });

    $('#MASTXCD').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetTax', { taxcode: data }).done(function (rs) {
                $('#MASTXCD_dl').val(rs.MKTXDL)
            })
        } else {
            $('#MASTXCD_dl').val('');
        }
    });

    $('#MAPTXCD').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetTax', { taxcode: data }).done(function (rs) {
                $('#MAPTXCD_dl').val(rs.MKTXDL)
            })
        } else {
            $('#MAPTXCD_dl').val('');
        }
    });

    $('#MASCALC').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '003|' + data }).done(function (rs) {
                $('#MASCALC_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MASCALC_dl').val('');
        }
    });

    $('#MAPCALC').change(function () {
        var data = $(this).val().trim();
        if (data !== '') {
            $.getJSON('/MasterSetting/Master/GetJsonBySRCode', { codes: '003|' + data }).done(function (rs) {
                $('#MAPCALC_dl').val(rs.MNSRNM);
            })
        } else {
            $('#MAPCALC_dl').val('');
        }
    })
})