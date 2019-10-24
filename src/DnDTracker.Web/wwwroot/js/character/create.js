function getOffset(jQ) {
    var el = jQ[0];
    var rect = el.getBoundingClientRect(),
        scrollLeft = window.pageXOffset || document.documentElement.scrollLeft,
        scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    return {
        top: rect.top + scrollTop,
        right: rect.right + rect.left + scrollLeft,
        bottom: rect.bottom + rect.top + scrollTop,
        left: rect.left + scrollLeft
    }
}

$(function () {
    var profileImg = $('.character-summary__profile-img');
    var hiddenImgBrowse = $('.character-summary__profile-img-browse');
    var characterSummaryColumn = $('#character-summary');
    var characterSummaryBanner = $('#character-summary .banner');
    var footer = $('#main-footer');
    var header = $('#main-header');
    var taskbar = $('#main-taskbar');
    var buttons = $('.buttons-container');
    var exportBtn = $('.export-button');
    var createBtn = $('.create-button');
    var originalContentHeight = $('#main-content').height();
    var additionalHeight;

    $('.create-character__section-title').click(function () {
        var section = $(this).parent().parent();
        var sectionContent = section.find('.create-character__section-content');
        var duration = 300;
        section.toggleClass('open');
        sectionContent.slideToggle(duration);
        setTimeout(handleScroll, duration);
    });

    characterSummaryColumn.css({
        'justify-content': 'flex-start'
    });
    var resizeBanner = () => characterSummaryBanner.css({
        width: `${characterSummaryColumn.width()}px`,
        position: 'fixed'
    });
    resizeBanner();
    $(window).resize(resizeBanner);

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

    var footerHeight = footer.outerHeight();
    var headerHeight = header.outerHeight() + taskbar.outerHeight();
    var bannerMargin = parseFloat(characterSummaryBanner.css('margin-bottom').replace('px', ''));
    var handleScroll = () => {
        var bottom = $(document).height() - ($(document).scrollTop() + $(window).height());
        var bannerBottom = getOffset(footer).bottom - getOffset(characterSummaryBanner).bottom;
        var scrollTop = $(document).scrollTop();

        if (bannerBottom <= footerHeight) { // If the bottom of the banner is touching the top of the footer
            characterSummaryBanner.css({
                transform: `translateY(${-1 * (headerHeight + (footerHeight - bottom))}px)`
            });
        } else if (scrollTop == 0) { // If we're at the very top of the page
            characterSummaryBanner.css({
                transform: 'unset'
            });
        } else if (scrollTop >= headerHeight) { // If we've scrolled completely past the header
            characterSummaryBanner.css({
                transform: `translateY(${-1 * (headerHeight)}px)`
            });
        } else { // If we've scrolled partially past the header
            characterSummaryBanner.css({
                transform: `translateY(${-1 * (scrollTop)}px)`
            });
        }

        if ($(document).height() === $(window).height() || bottom <= footerHeight)
            buttons.css({
                transform: `translateY(${-1 * (footerHeight - bottom)}px)`
            });
        else if (bottom > footerHeight)
            buttons.css({
                transform: 'unset'
            });
    };
    handleScroll();
    $(document).scroll(handleScroll);
    $(window).resize(handleScroll);
    characterSummaryBanner.show();

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
            targetSelector: '#notify-queue',
            targetMethod: 'prepend',
            sound: 'default'
        });
    });
});