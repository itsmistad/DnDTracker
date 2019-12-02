'use strict';

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

    $('#character-name').sync($('#character-summary__name'), '...');

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
            console.log('partial');
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

    function validateModel(model) {
        for (const [key, value] of Object.entries(model)) {
            if (!value || value === "" || value === "...")
                return false;
        }
        return true;
    }

    createBtn.click(function () {
        var model = {
            Name: $('#character-name').val(),
            Proficiencies: $('#character-proficiencies').val(),
            Equipment: $('#character-equipment').val(),
            Gender: $('#character-gender').val(),
            Level: $('#character-level').val(),
            Strength: $('#character-strength').val(),
            Dexterity: $('#character-dexterity').val(),
            Constitution: $('#character-constitution').val(),
            Intelligence: $('#character-intelligence').val(),
            Wisdom: $('#character-wisdom').val(),
            Charisma: $('#character-charisma').val(),
            Class: $('#character-class').val(),
            SubClass: $('.character-subclass-dropdown[current=true]').val(),
            Race: $('#character-race').val(),
            Background: $('#character-background').val(),
            Health: $('#character-health').val()
        };
        if (!validateModel(model)) {
            notify.me({
                class: centerPopupClasses,
                header: 'Oops',
                subheader: 'Looks like you\'re missing some values',
                body: 'Click OK to go back and finish your character creation.',
                buttons: [{
                    text: 'OK',
                    action: () => { },
                    close: true
                }],
                closeButton: false,
                fadeInDuration: 200,
                fadeOutDuration: 300,
                sound: 'default',
                onStartClose: () => {
                    notify.overlay(false);
                }
            });
            notify.overlay(true);
            return;
        }
        var processing = notify.me({
            class: cornerPopupClasses,
            subheader: 'Creating...',
            body: 'Please wait while we create your character.<br/>Do not reload the page.<br/><img src="/assets/spinner.gif" width="80px" style="margin: -20px 0 -40px -30px; user-select: none;"/>',
            forceOnTop: true,
            handleAsStack: true,
            closeButton: false,
            buttons: [],
            fadeInDuration: 200,
            fadeOutDuration: 300,
            targetSelector: '#notify-queue',
            targetMethod: 'prepend',
            sound: 'default'
        });
        network.post('/Character/Save', model, json => {
            processing.close();
            switch (json.response) {
                case "ok":
                    console.log(json.message);
                    notify.me({
                        class: centerPopupClasses,
                        header: 'Creation Successful',
                        subheader: 'Your character was created successfully',
                        body: 'Click OK to head to your character dashboard.',
                        buttons: [{
                            text: 'OK',
                            action: () => window.location.href = '/Character/Dash'
                        }],
                        closeButton: false,
                        fadeInDuration: 200,
                        fadeOutDuration: 300,
                        sound: 'default',
                        onStartClose: () => {
                            notify.overlay(false);
                        }
                    });
                    break;
                case "err":
                    console.error(json.message);
                    notify.me({
                        class: centerPopupClasses,
                        header: 'Creation Failed',
                        subheader: 'Something went wrong',
                        body: 'This doesn\'t usually happen...<br/> Please wait a few minutes and try again.',
                        buttons: [{
                            text: 'Ok',
                            action: () => { },
                            close: true
                        }],
                        closeButton: false,
                        fadeInDuration: 200,
                        fadeOutDuration: 300,
                        sound: 'default',
                        onStartClose: () => {
                            notify.overlay(false);
                        }
                    });
                    break;
            }
            notify.overlay(true);
        });
    });
    $('#roll-ability-scores').click(function (e) {
        e.preventDefault();
        $('#stat-1').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
        $('#stat-2').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
        $('#stat-3').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
        $('#stat-4').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
        $('#stat-5').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
        $('#stat-6').text(roll(4, 6, true).reduce((a, b) => a + b, 0));
    });
    $('#take-standard').click(function (e) {
        e.preventDefault();
        $('#stat-1').text(15);
        $('#stat-2').text(14);
        $('#stat-3').text(13);
        $('#stat-4').text(12);
        $('#stat-5').text(10);
        $('#stat-6').text(8);
    });
    $('#roll-health').click(function (e) {
        e.preventDefault();
        var dN = parseInt($(this).attr('dN'));
        var conModifier = parseInt($('#character-constitution').val());
        var level = parseInt($('#character-level').val());

        var firstLevelHealth = dN + conModifier;
        var laterLevelHealth = firstLevelHealth + ((roll(1, dN)[0] + conModifier) * (level - 1));

        var health = $('#character-health')
        var summaryHealth = $('#character-summary__health');

        if (level > 1) {
            health.val(laterLevelHealth);
            summaryHealth.text(laterLevelHealth);
        }
        else if (level === 1) {
            health.val(firstLevelHealth);
            summaryHealth.text(firstLevelHealth);
        }
    });
    $('#take-standard-health').click(function (e) {
        e.preventDefault();
        var dN = parseInt($('#roll-health').attr('dN'));
        var standard = parseInt($(this).attr('standard'));
        var conModifier = parseInt($('#character-constitution').val());
        var level = parseInt($('#character-level').val());

        var firstLevelHealth = dN + conModifier;
        var laterLevelHealth = firstLevelHealth + ((standard + conModifier) * (level - 1));

        var healthElements = $('#character-health', '#character-summary__health');

        var health = $('#character-health')
        var summaryHealth = $('#character-summary__health');

        if (level > 1) {
            health.val(laterLevelHealth);
            summaryHealth.text(laterLevelHealth);
        }
        else if (level === 1) {
            health.val(firstLevelHealth);
            summaryHealth.text(firstLevelHealth);
        }
    });

    $('#character-class').change(function () {
        var val = $("#character-class option:selected").val();
        var text = $("#character-class option:selected").text();
        $('#character-summary__class').text(text);
        if (text !== 'None') {
            var rollHealth = $('#roll-health');
            var dN = 0;
            var standardRoll = 0;

            switch (text) {
                case 'Barbarian':
                    dN = 12;
                    standardRoll = 7;
                    break;
                case 'Fighter':
                case 'Paladin':
                case 'Ranger':
                    dN = 10;
                    standardRoll = 6;
                    break;
                case 'Bard':
                case 'Cleric':
                case 'Druid':
                case 'Monk':
                case 'Rogue':
                case 'Warlock':
                    dN = 8;
                    standardRoll = 5;
                    break;
                case 'Sorcerer':
                case 'Wizard':
                    dN = 6;
                    standardRoll = 4;
                    break;
            }

            rollHealth.attr('dN', dN);
            $('#take-standard-health').attr('standard', standardRoll);
            rollHealth.find('img').attr('src', `/assets/dice/${dN}.png`);

            $('#character-health-label').show();
            $('#character-health-section').css('display', 'flex');
            $('#character-subclass-label').show();
            $('.character-subclass-dropdown').hide();
            $('.character-subclass-dropdown').removeAttr('current');
            $(`#character-subclass-${val}`).show();
            $(`#character-subclass-${val}`).attr('current', 'true');
        } else {
            $('#character-subclass-label').hide();
            $('.character-subclass-dropdown').hide();
            $('.character-subclass-dropdown').removeAttr('current');
            $('#character-health-label').hide();
            $('#character-health-section').hide();
        }
    });
    $('#character-race').change(function () {
        $('#character-summary__race').text($("#character-race option:selected").text());
    });
    $('#character-background').change(function () {
        $('#character-summary__background').text($("#character-background option:selected").text());
    });
    $('#character-gender').change(function () {
        $('#character-summary__gender').text($("#character-gender option:selected").text());
    });

    function limitMinMax (e) {
        var max = parseInt(e.attr('max'));
        var min = parseInt(e.attr('min'));
        if (e.val() > max) {
            e.val(max);
        }
        else if (e.val() < min) {
            e.val(min);
        }
    }

    $("#character-level").change(function () {
        limitMinMax($(this));
        $('#character-summary__level').text($(this).val());
    });
    $("#character-strength").change(function () {
        limitMinMax($(this));
        $('#character-summary__strength').text($(this).val());
    });
    $("#character-dexterity").change(function () {
        limitMinMax($(this));
        $('#character-summary__dexterity').text($(this).val());
    });
    $("#character-constitution").change(function () {
        limitMinMax($(this));
        $('#character-summary__constitution').text($(this).val());
    });
    $("#character-intelligence").change(function () {
        limitMinMax($(this));
        $('#character-summary__intelligence').text($(this).val());
    });
    $("#character-wisdom").change(function () {
        limitMinMax($(this));
        $('#character-summary__wisdom').text($(this).val());
    });
    $("#character-charisma").change(function () {
        limitMinMax($(this));
        $('#character-summary__charisma').text($(this).val());
    });
    $("#character-health").change(function () {
        limitMinMax($(this));
        $('#character-summary__health').text($(this).val());
    });
});