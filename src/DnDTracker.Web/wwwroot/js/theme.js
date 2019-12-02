/*
 * Start On-Load Section
 */
// Function Definitions
var headerBarHeight = $('#main-header').height();
var taskBarHeight = $('#main-taskbar').length ? $('#main-taskbar').height() : 0;
var footerBarHeight = $('#main-footer').height();
function resize () {
    var contentContainerHeight = $('#main-content').height();
    var totalHeight = $(document).innerHeight();
    var fillHeight = totalHeight - headerBarHeight - taskBarHeight - footerBarHeight - 1;
    if (contentContainerHeight <= fillHeight) {
        $('#main-content').css('min-height', `${fillHeight}px`);
    }
}
resize();

// Checks the page for any empty required fields and assigns areRequiredFieldsFilled accordingly.
var areRequiredFieldsFilled = () => {
    var result = true;
    $.merge($('input'), $('textarea')).each(function () {
        if ($(this).is(':required') && $(this).val() === '') 
            result = false;
    });
    return result;
};
// Checks the page for any required fields and adds an asterisk to the preceeding header, if it exists.
$.merge($('input'), $('textarea')).each(function () {
    if ($(this).is(':required')) {
        var sibling = $(this).prev();
        if (sibling.is(':header'))
            sibling.append(' <strong style="color: red;">*</strong>');
    }
});

// Notify Handling
notify.initNetwork(() => {
    network.on('HandshakeConfirm', json => {
        switch (json.response) {
        case 'ok':
            console.log(json.message);
            break;
        case 'err':
            console.error(json.message);
            break;
        }
    });
    network.send('Handshake', {
        guid: currentUser.guid
    }, err => {
        console.error(err);
    });
});
notify.initSound('default', '/assets/notify.mp3');

/*
 * Start On-Ready Section
 */
$(function () {
    $(window).on('resize', resize);
});