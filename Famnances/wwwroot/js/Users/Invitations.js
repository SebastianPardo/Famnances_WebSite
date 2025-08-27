$("#Guest_Name").on('input', function () {
    if ($(this).val().length < 5) {
        $('#GuestValidationMessage').text('Type at least 5 characters');
    }
})
$("#Guest_Name").autocomplete({
    classes: {
        "ui-autocomplete": "dropdown-menu",
        "ui-menu-item": "dropdown-item",
    },
    source: function (request, response) {
        if (request.term.length > 4) {
            $.ajax({
                url: '/Users/GetGuests?prefix=' + request.term,
                type: "GET",
                success: function (data) {
                    if (data.length == 0) {
                        $('#GuestValidationMessage').text('No users found');
                    }
                    else if (data.length <= 20) {
                        $('#GuestValidationMessage').text('');
                        response($.map(data, function (item) {
                            return {
                                value: item.id,
                                label: item.legalName + ", " + item.account.email
                            }
                        }))
                    } else {
                        $('#GuestValidationMessage').text('Enter additional information to narrow results');
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        }
        else {
            $('#GuestValidationMessage').text('Type at least 5 characters');
        }
    },
    select: function (e, i) {
        $("#Guest_UserId").val(i.item.value)
        $("#Guest_Name").val(i.item.label);
        return false;
    },
    minLength: 5
});