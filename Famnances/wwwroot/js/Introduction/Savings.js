$(document).ready(function () {

    toggleFrequentValue();

    $('input[name="Pocket.FrecuentDeposits"]').change(function () {
        toggleFrequentValue();
    });
    function toggleFrequentValue() {
        var isYes = $('input[name="Pocket.FrecuentDeposits"]:checked').val() === "true";

        if (isYes) {
            $('#FecuentValue').slideDown();
        } else {
            $('#Pocket_FrecuentValue').val('');
            $('#FecuentValue').slideUp();
        }
    }
});
