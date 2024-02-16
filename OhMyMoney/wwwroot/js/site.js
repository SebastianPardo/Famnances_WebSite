$(document).ready(function () {
    $('.dropdown-toggle').removeClass('btn-light');
    $('.dropdown-toggle').addClass('btn-outline-secondary');
    $('.dropdown-toggle').addClass('btn-bd-mybutton');
});
function request(options) {
    var getUrl = window.location;
    var baseUrl = getUrl.protocol + "//" + getUrl.host + "/"
    $.ajax({
        url: baseUrl + options.url,
        data: options.data,
        type: options.type,
        success: function (response, status, jqXhr) {
            if (options.callback != undefined) {
                options.callback(response);
            }
        },
        error: function (jqXhr, status, error) {
            if (options.errorMessage === undefined || options.errorMessage === null || options.errorMessage === '') {
                //swal.fire('Error', 'Could NOT get satisfactory answer due to error : ' + error, 'error');
                alert('Could NOT get satisfactory answer due to error : ' + error, 'error')
            }
            else {
                //swal.fire('Error', options.errorMessage, 'error');
                alert(options.errorMessage)
            }
        }
    });
}

function enableFields(...fields) {
    for (const field of fields) {
        if (field.attr('type') === 'checkbox') {
            field.prop('disabled', false);
        }
        else {
            field.removeAttr('disabled');
            if (field.is('select')) {
                field.prop('disabled', false);
                field.selectpicker('refresh');
            }
        }
    }
}

function disableFields(...fields) {
    for (const field of fields) {
        if (field.attr('type') === 'checkbox') {
            field.prop('disabled', true);
        }
        else {
            field.attr('disabled', 'true');
            if (field.is('select')) {
                field.prop('disabled', true);
                field.selectpicker('refresh');
            }
        }
    }
}

let preloader = $('#preloader');
if (preloader) {
    window.addEventListener('load', () => { preloader.hide(); });
    window.addEventListener('submit', () => { preloader.show(); })
}