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
            url: 'User/GetProvincesByCountry?countryId=' + $('#Country').val(),
            value: "id",
            text: "name"
        });
    });

    $('#Province').change(function () {
        fillSelect({
            field: $("#CityId"),
            url: 'User/GetCitiesByProvince?provinceId=' + $('#Province').val(),
            value: "id",
            text: "name"
        });
    });
});