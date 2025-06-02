let preloader = $('#preloader');
if (preloader) {
    window.addEventListener('load', () => { preloader.hide(); });
    window.addEventListener('submit', () => { preloader.show(); })
}

/////////////////////////////////////////////////////////////////////////


$(document).ready(function () {
    $(window).scroll(function () {
        if ($(window).width() > 992) {
            if ($(this).scrollTop() > 50) {
                $('.sticky-top .container').addClass('shadow-sm').css('max-width', '100%');
                $('#menu').addClass('ms-auto');
                $('#logo-img').attr("src", '/images/logos/Logo_2_white.png');
            } else {
                $('.sticky-top .container').removeClass('shadow-sm').css('max-width', '90%');
                $('#menu').removeClass('ms-auto');
                $('#logo-img').attr("src", '/images/logos/no-logo.png');
            }
        } else {
            $('.sticky-top .container').addClass('shadow-sm').css('max-width', '90%');
            $('#menu').addClass('ms-auto');
            $('#logo-img').attr("src", '/images/logos/Logo_2_white.png');
        }
    });
    $('select').select2({ theme: 'bootstrap-5' });
    $("select.no-search").select2({ theme: 'bootstrap-5', minimumResultsForSearch: Infinity })
    $("table").DataTable({
        autowidth:true,
        responsive:true,
        ordering: false,
        lengthChange: false,
        searching: false,
    });
});

function enableFields(...fields) {
    for (const field of fields) {
        if (field.attr('type') === 'checkbox') {
            field.prop('disabled', false);
        }
        else {
            field.removeAttr('disabled');
            if (field.is('select')) {
                field.prop('disabled', false);
                //field.selectpicker('refresh');
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
                //field.selectpicker('refresh');
            }
        }
    }
}

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

function fillSelect(options) {
    enableFields(options.field);
    request({
        url: options.url,
        type: "GET",
        callback: function (response) {
            for (let item of response) {
                options.field.append('<option value="' + item[options.value] + '">' + item[options.text] + '</option>');
            }
        }
    });
}
