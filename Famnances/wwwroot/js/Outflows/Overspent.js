
$(document).ready(function () {

    function updateDropdown() {
        const isSaving = $('input[name="OverSpent.IsSaving"]:checked').val() === 'true';

        $('.pockets').toggleClass('d-none', !isSaving);
        $('.budgets').toggleClass('d-none', isSaving);

        const activeSelect = isSaving
            ? $('.pockets select')
            : $('.budgets select');

        activeSelect.prop('selectedIndex', 0);
    }
    updateDropdown();
    $(document).on('change', 'input[name="OverSpent.IsSaving"]', updateDropdown);
});