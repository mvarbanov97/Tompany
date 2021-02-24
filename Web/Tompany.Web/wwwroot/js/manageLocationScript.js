function changeState() {
    let item = document.getElementById("stateId").value;
    //console.log(item);
    let input = document.getElementById("stateValue");
    input.value = item;
}

function changeCountry() {
    let item = document.getElementById("countryId").value;
    //console.log(item);
    let input = document.getElementById("countryValue");
    input.value = item;
}

function changeCity() {
    let item = document.getElementById("cityId").value;
    //console.log(item);
    let input = document.getElementById("cityValue");
    input.value = item;
}

function checkCountryCode(field) {
    let code = field.value;
    let pattern = new RegExp('^(\\+{1}\\d{1,3}|\\+{1}\\d{1,4})$');

    if (!pattern.test(code)) {
        alert("Invalid Country Code, example +359, +1, +569 etc.");
        field.value = "";
    }
}