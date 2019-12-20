

function ABPRUPD_Multiply_Result(ABPRUP, EXRT, obj) { // ABPRUPD = ABPRUP * EXRT
    var checking = checkValidVariable(ABPRUP, EXRT);
    if (checking === true) {  //is valid
        $.ajax({
            url: '/MasterSetting/Formula/ABPRUPD_Multiply_Result',
            type: 'POST',
            async: false,
            data: { ABPRUP: ABPRUP, EXRT: EXRT },
            success: function (rs) {
                if (rs !== -1) {
                    obj.val(rs);
                }
            }
        })
    } else {
        obj.val('');
    }
}

function ABPRATD_Multiply_Result(ABPRUPD, ABWT, obj) {//ABPRATD = ABPRUPD * ABWT/1000
    var checking = checkValidVariable(ABPRUPD, ABWT);
    if (checking === true) {  //is valid
        $.ajax({
            url: '/MasterSetting/Formula/ABPRATD_Multiply_Result',
            type: 'POST',
            async: false,
            data: { ABPRUPD: ABPRUPD, ABWT: ABWT },
            success: function (rs) {
                if (rs !== -1) {
                    obj.val(rs);
                }
            }
        })
    } else {
        obj.val('');
    }
}

function ABPTXAD_Multiply_Result(AATXRT, ABPRATD, obj) {//ABPRXATD = AATXRT/100 * ABPRATD
    var checking = checkValidVariable(AATXRT, ABPRATD);
    if (checking === true) {  //is valid
        $.ajax({
            url: '/MasterSetting/Formula/ABPTXAD_Multiply_Result',
            type: 'POST',
            async: false,
            data: { AATXRT: AATXRT, ABPRATD: ABPRATD },
            success: function (rs) {
                if (rs !== -1) {
                    obj.val(rs);
                }
            }
        })
    } else {
        obj.val('');
    }
}


function ABPRAT_Multiply_Result(ABPRUP, ABWT,obj) {//ABPRAT = ABPRUP * ABWT/1000
    var checking = checkValidVariable(ABPRUP, ABWT);
    if (checking === true) {  //is valid
        $.ajax({
            url: '/MasterSetting/Formula/ABPRAT_Multiply_Result',
            type: 'POST',
            async: false,
            data: { ABPRUP: ABPRUP, ABWT: ABWT },
            success: function (rs) {
                if (rs !== -1) {
                    obj.val(rs);
                }
            }
        })
    } else {
        obj.val('');
    }
}


function ABPTXAT_Multiply_Result(AATXRT, ABPRAT,obj) {//ABPTXAT = AATXRT/100 * ABPRAT
    var checking = checkValidVariable(AATXRT, ABPRAT);
    if (checking === true) {  //is valid
        $.ajax({
            url: '/MasterSetting/Formula/ABPTXAT_Multiply_Result',
            type: 'POST',
            async: false,
            data: { AATXRT: AATXRT, ABPRAT: ABPRAT },
            success: function (rs) {
                if (rs !== -1) {
                    obj.val(rs);
                }
            }
        })
    } else {
        obj.val('');
    }
}

function checkValidVariable(x, y) {

    if (x === '' || y === '') return false;
    if (!$.isNumeric(x) && !$.isNumeric(y)) return false;
    return true;
}

