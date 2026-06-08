let preloader = $('#preloader');
if (preloader) {
    window.addEventListener('load', () => { preloader.hide(); });
    window.addEventListener('submit', () => { preloader.show(); })
}


/////////////////////////////////////////////////////////////////////////


$(document).ready(function () {
    let addIsOpen = false;
    let moreIsOpen = false;
    const footerTop = $('footer').offset().top;
    

    $(window).scroll(function () {
        const scrollBottom = $(window).scrollTop() + $(window).height();
        const footerVisible = scrollBottom >= footerTop;
        $('#nav-wrapper').css('bottom',footerVisible ? $('footer').outerHeight(): '0');
    });

    $('#add-btn').on('click', function () {
        const scrollBottom = $(window).scrollTop() + $(window).height();
        const footerVisible = scrollBottom >= footerTop;

        addIsOpen = !addIsOpen;
        $('#add-panel').css('width', $('.stick-bottom').outerWidth() + 'px');
        $('#add-panel').css('bottom', addIsOpen ? footerVisible ? $('footer').outerHeight() : '0px' : '');
        $('#panel-overlay').css({
            'background': addIsOpen ? 'rgba(0,0,0,0.45)' : 'rgba(0,0,0,0)',
            'pointer-events': addIsOpen ? 'all' : 'none'
        });
        $(this).css('transform', addIsOpen ? 'rotate(45deg)' : 'rotate(0)');
    });

    $('#more-btn').on('click', function () {
        const scrollBottom = $(window).scrollTop() + $(window).height();
        const footerVisible = scrollBottom >= footerTop;

        moreIsOpen = !moreIsOpen;
        $('#more-panel').css('width', $('.stick-bottom').outerWidth() + 'px');
        $('#more-panel').css('bottom', moreIsOpen ? footerVisible ? $('footer').outerHeight() : '0px' : '');
        $('#panel-overlay').css({
            'background': moreIsOpen ? 'rgba(0,0,0,0.45)' : 'rgba(0,0,0,0)',
            'pointer-events': moreIsOpen ? 'all' : 'none'
        });
        $(this).css('transform', moreIsOpen ? 'rotate(45deg)' : 'rotate(0)');
    });

    $('#panel-overlay').on('click', function () {
        isOpen = false;
        $('#add-panel').css('bottom', '');
        $('#more-panel').css('bottom', '');
        $(this).css({ 'background': 'rgba(0,0,0,0)', 'pointer-events': 'none' });
        $('#add-btn').css('transform', 'rotate(0)');
    });

    $('.select2').each(function () {
        const $select = $(this);

        const ph = $select.attr('placeholder') || $select.data('placeholder');
        const options = {
            theme: 'bootstrap-5',
            width: '100%',
            placeholder: ph,
        };

        if ($select.prop('multiple')) {
            options.closeOnSelect = false;
            options.minimumResultsForSearch = Infinity;
        }

        $select.select2(options);

        const instance = $select.data('select2');
        const $rendered = instance.$container.find('.select2-selection__rendered');

        $rendered.attr('data-placeholder', ph);
        function updatePlaceholderState() {
            const val = $select.val();
            const isEmpty = !val || (Array.isArray(val) && val.length === 0);
            $rendered.toggleClass('empty', isEmpty);
        }

        updatePlaceholderState();
        $select.on('change.select2', updatePlaceholderState);

        if ($select.prop('multiple')) {
            $select.on('select2:select select2:unselect', function () {
                const el = $(this);
                setTimeout(() => el.select2('open'), 0);
            });
        }
    });

    $("table").DataTable({
        autowidth: true,
        responsive: true,
        ordering: true,
        order: [],
        lengthChange: false,
        searching: true,
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
