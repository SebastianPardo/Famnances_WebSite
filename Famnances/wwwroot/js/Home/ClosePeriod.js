
document.addEventListener('change', function (e) {
    if (!e.target.classList.contains('move-option')) return;

    const row = e.target.closest('.remainder-row');
    const dropdown = row.querySelector('.saving-select');

    if (e.target.value === 'true') {
        dropdown.classList.remove('d-none');
    } else {
        dropdown.classList.add('d-none');

        const select = dropdown.querySelector('select');
        if (select) select.selectedIndex = 0;
    }
});