$(document).ready(function () {
    $("#FirstName").change(function () {
        $("#LegalName").val($("#FirstName").val() + " " + $("#LastName").val())
    })

    $("#LastName").change(function () {
        $("#LegalName").val($("#FirstName").val() + " " + $("#LastName").val())
    })

    $('#Country').change(function () {
        fillSelect({
            field: $("#Province"),
            url: 'Users/GetProvincesByCountry?countryId=' + $('#Country').val(),
            value: "id",
            text: "name"
        });
    });

    $('#Province').change(function () {
        fillSelect({
            field: $("#CityId"),
            url: 'Users/GetCitiesByProvince?provinceId=' + $('#Province').val(),
            value: "id",
            text: "name"
        });
    });
});