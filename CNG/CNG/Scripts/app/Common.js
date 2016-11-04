function FormatNumber(x) {
    x = parseFloat(x).toFixed(2);
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function RemoveCommas(objName) {
    var lstLetters = objName;
    var lstReplace = lstLetters.replace(/\,/g, '');

    return lstReplace;
}