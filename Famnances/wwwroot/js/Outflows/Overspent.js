
document.addEventListener('DOMContentLoaded', function () {
    function updateDropdown() {
        const isSaving = document.querySelector('input[name="IsSaving"]:checked').value === 'true';
        document.querySelector('.pockets').classList.toggle('d-none', isSaving);
        document.querySelector('.budgets').classList.toggle('d-none', !isSaving);
        const hiddenSelect = isSaving ? document.querySelector('.pockets select') : document.querySelector('.budgets select');
        if (hiddenSelect) hiddenSelect.selectedIndex = 0;
    }
    updateDropdown();
    document.querySelectorAll('input[name="IsSaving"]').forEach(radio => {
        radio.addEventListener('change', updateDropdown);
    });
});