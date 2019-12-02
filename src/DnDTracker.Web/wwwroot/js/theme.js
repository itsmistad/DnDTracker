var headerBarHeight = $('#main-header').height();
var taskBarHeight = $('#main-taskbar').length ? $('#main-taskbar').height() : 0;
var footerBarHeight = $('#main-footer').height();
var resize = function () {
    var contentContainerHeight = $('#main-content').height();
    var totalHeight = $(document).innerHeight();
    var fillHeight = totalHeight - headerBarHeight - taskBarHeight - footerBarHeight - 1;
    if (contentContainerHeight <= fillHeight) {
        $('#main-content').css('min-height', `${fillHeight}px`);
    }
}
resize();

$(function () {
    $(window).on('resize', resize);
});

$.fn.sync = function ($targets, $default) {
    var $els = $targets.add(this);
    $els.on('keyup change', function () {
        var $this = $(this);
        var val = $this.val();
        if (val && val.length > 0)
            val = $this.val();
        else
            val = $default;
        $els.not($this).text(val);
        $els.not($this).val(val);
    });
    return this;
};

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