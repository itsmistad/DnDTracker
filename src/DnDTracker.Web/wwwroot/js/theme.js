var headerBarHeight = $('#main-header').height();
var footerBarHeight = $('#main-footer').height();
var resize = function () {
    var contentContainerHeight = $('#main-content').height();
    var totalHeight = $(document).innerHeight();
    var fillHeight = totalHeight - headerBarHeight - footerBarHeight - 1;
    if (contentContainerHeight < fillHeight)
        $('#main-content').css('height', `${fillHeight}px`);
}
resize();

$(function () {
    $(window).on('resize', resize);
});

notify.initNetwork(() => {
    network.on('HandshakeConfirm', json => {
        switch (json.response) {
        case 'ok':
            console.log(json.message);
            obj.connected = true;
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