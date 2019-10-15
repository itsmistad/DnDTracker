$(function () {
    var profileImg = $('.character-summary__profile-img');
    var hiddenImgBrowse = $('.character-summary__profile-img-browse');
    hiddenImgBrowse.change(function (event) {
        if (!this.files || !this.files.length) return;
        if (this.files[0].size <= 5242880) { // 5 MB
            var reader = new FileReader();
            reader.onload = function (e) {
                profileImg.attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        } else {
            hiddenImgBrowse.val('');
            notify.me({
                class: centerPopupClasses,
                header: 'Uh oh',
                subheader: 'Image too big',
                body: 'Profile images must be less than 5 MB!',
                onStartClose: () => {
                    notify.overlay(false);
                },
                closeButton: true
            });
            notify.overlay(true);
        }
    });
    profileImg.click(() => {
        hiddenImgBrowse.click();
    });
    $('.character-summary__line-item').text('...');

    var footer = $('.main-footer');
    var buttons = $('.buttons-container');
    var exportBtn = $('#export-button');
    var createBtn = $('#create-button');

    var footerHeight = footer.outerHeight();
    var handleScroll = () => {
        var bottom = $(document).height() - ($(document).scrollTop() + $(window).height());
        if (bottom > footerHeight || $(document).width() >= 1600 && $(document).height() === $(window).height()) {
            if (buttons.css('transform') !== 'none') {
                buttons.css({
                    transform: 'none'
                });
            }
        } else {
            buttons.css({
                transform: `translateY(-${footerHeight - bottom}px)`
            });
        }
    };
    handleScroll();
    $(document).scroll(handleScroll);
    $(window).resize(handleScroll);

    exportBtn.click(function () {
        notify.me({
            class: centerPopupClasses,
            header: 'Test Notification',
            subheader: 'This is a test',
            body: 'This is a test!',
            onStartClose: () => {
                notify.overlay(false);
            },
            closeButton: true
        });
        notify.overlay(true);
    });
    createBtn.click(function () {
        notify.me({
            class: cornerPopupClasses,
            header: 'Oh, hey...',
            subheader: 'This is awkward.',
            body: 'Go back! You shouldn\'t be here!',
            handleAsStack: true,
            buttons: [{
                text: 'Oh, ok',
                action: () => window.location.href = ''
            }],
            fadeInDuration: 200,
            fadeOutDuration: 300,
            targetSelector: '#notification-queue',
            targetMethod: 'prepend',
            sound: 'default'
        });
    });
});