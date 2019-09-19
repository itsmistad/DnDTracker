$(function () {
    var headerBarHeight = $('#main-header').height();
    var footerBarHeight = $('#main-footer').height();
    var resize = function () {
        var contentContainerHeight = $('#main-content').height();
        var totalHeight = $(document).innerHeight();
        var fillHeight = totalHeight - headerBarHeight - footerBarHeight - 1;
        if (contentContainerHeight < fillHeight)
            $('#main-content').css('height', `${fillHeight}px`);
    }
    $(window).on('resize', resize);

    resize();
});