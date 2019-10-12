﻿'use strict';

/* 
 * This library is for client-side notification and dialog pop-ups. Pop-ups can be triggered locally or be received from the server.
 * Usage:
 *  notify.me({
 *      // Insert options here (read "defaultOptions")
 *  })
 */

var notify = new function() {

    var obj = {};
    var idPrefix = 'notify-popup-';
    var popupElement = options =>
        `<div id="${options.id}" class="${options.class}" style="display:none;">
    <div class="close">X</div>
    <div class="header">${options.header}</div>
    <div class="subheader">${options.subheader}</div>
    <div class="body">${options.body}</div>
    <div class="buttons"></div>
</div>
`;
    var defaultTopLayer = 999999;
    var defaultNotificationLayer = 2000;
    var defaultOverlayLayer = 1999;
    var defaultOptions = {
        // These shouldn't be touched... usually
        targetSelector: 'body', // What element selector to use when adding the notification using the "targetMethod"
        targetMethod: 'append', // How to add the notification to the "targetSelector" ('prepend', 'append', 'before', and 'after')

        class: 'notify-popup', // The string list of style classes for the element's class attribute
        header: '', // The html header of the notification ('' = disabled and hidden)
        subheader: '', // The html subheader of the notification ('' = disabled and hidden)
        body: '', // The html body of the notification ('' = disabled and hidden)
        closeButton: true, // Toggles the close button
        timeout: 0, // How long (in ms) until the notification closes (0 = disabled)
        fadeOutDuration: 400, // How long (in ms) does the fade out animation last
        fadeInDuration: 800, // How long (in ms) does the fade in animation last
        layer: defaultNotificationLayer, // The z-index layer of the notification
        forceOnTop: false, // Toggles forcing the notification to the top layer
        onStartClose: () => { }, // The event that gets called when the notification starts closing 
        onClose: () => { }, // The event that gets called when the notification is fully closed
        buttons: [
            {
                text: 'OK',
                class: '', // The string list of style classes for the element's class attribute
                action: button => { }, // The event that gets called when the button is clicked
                close: true // Toggles whether the button closes the notification when clicked
            }
        ]
    };

    // Initializes a connection to the NotificationHub if the user is logged in
    obj.initNetwork = () => {
        if (currentUser) {
            network.init('/Notify');
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
            network.on('ReceiveNotification', json => obj.me(json, true));
            network.send('Handshake', {
                guid: currentUser.guid
            }, err => {
                console.error(err);
            });
            network.connect(err => {
                if (err) console.error(err);
            });
        }
    };

    // Enables or disables the notification background overlay.
    obj.overlay = (enable, delay, opacity) => {
        var overlayId = 'notify-overlay';
        var currentOverlay = $('body').find('#' + overlayId);
        if (currentOverlay.length && !enable) {
            if (!delay) delay = defaultOptions.fadeOutDuration;
            currentOverlay.fadeOut(delay, () => {
                currentOverlay.remove();
            });
        } else if (!currentOverlay.length && enable) {
            $('body').append(`<div id="${overlayId}" style="display:none;"></div>`);
            currentOverlay = $('#' + overlayId);
            if (!opacity) opacity = 0.3;
            currentOverlay.css({
                position: 'absolute',
                left: '0',
                top: '0 ',
                height: $(document).height() + 'px',
                width: $(window).width() + 'px',
                'background-color': `rgba(0,0,0,${opacity})`,
                'z-index': defaultOverlayLayer
            });
            if (!delay) delay = defaultOptions.fadeInDuration;
            currentOverlay.fadeIn(delay);
        }
    };

    /*
     * Creates a new notification pop-up.
     * Returns an object with the following properties IF the notification was shown successfully:  
     * - id // The id of the notification element
     * - $ // The jQuery object of the element
     * - options // The original options notify.me was called with
     * - close() // Closes the notifcation
     * 
     * ignoreFunctions is for network use only. Setting this to true will ignore any JS functions passed from the network (prevents JS injection).
     */
    obj.me = (options, ignoreFunctions) => {
        var idSuffix = Math.ceil(Math.random() * 99999);
        var mergedOptions = { ...defaultOptions, ...options };
        for (const [key, value] of Object.entries(defaultOptions)) {
            // If the new option is null/undefined or if the new option is not the same type as the default, override with the default.
            if (mergedOptions[key] == null || typeof mergedOptions[key] !== typeof defaultOptions[key])
                mergedOptions[key] = defaultOptions[key];
        }

        var id = idPrefix + idSuffix;
        var target = $(mergedOptions.targetSelector);

        if (target.length) {
            var elementOptions = {
                id,
                class: mergedOptions.class,
                header: mergedOptions.header,
                subheader: mergedOptions.subheader,
                body: mergedOptions.body
            };
            var element = popupElement(elementOptions);
            switch (options.targetMethod) {
                case 'prepend':
                    target.prepend(element);
                    break;
                case 'before':
                    target.before(element);
                    break;
                case 'after':
                    target.after(element);
                    break;
                case 'append':
                default:
                    target.append(element);
                    break;
            }

            var ret = new function () {
                var result = {};

                result.id = id;
                result.$ = $('#' + id);
                result.options = mergedOptions;
                result.close = () => {
                    if (result.options.timeoutHandle)
                        clearTimeout(result.options.timeoutHandle);
                    if (!ignoreFunctions)
                        mergedOptions.onStartClose();
                    result.$.fadeOut(mergedOptions.fadeOutDuration, function () {
                        $(this).remove();
                        if (!ignoreFunctions)
                            mergedOptions.onClose();
                    });
                };

                return result;
            };

            var jQ = ret.$;
            jQ.css('z-index', mergedOptions.forceOnTop ? defaultTopLayer : mergedOptions.layer);
            if (elementOptions.header === '')
                jQ.find('.header').hide();
            if (elementOptions.subheader === '')
                jQ.find('.subheader').hide();
            if (elementOptions.body === '')
                jQ.find('.body').hide();

            var buttonsElement = jQ.find('.buttons');
            if (buttonsElement.length) {
                mergedOptions.buttons.forEach(_ => {
                    var buttonId = idPrefix + 'button-' + Math.ceil(Math.random() * 99999);
                    buttonsElement.append(`<button id="${buttonId}" class="${_.class}">${_.text}</button>`);
                    $('#' + buttonId).click(function () {
                        if (!ignoreFunctions)
                            _.action($(this));
                        if (_.close)
                            ret.close();
                    });
                });
            }
            if (!mergedOptions.closeButton)
                jQ.find('.close').hide();
            jQ.find('.close').click(function () {
                ret.close();
            });

            jQ.fadeIn(mergedOptions.fadeInDuration, () => {
                if (mergedOptions.timeout > 0)
                    mergedOptions.timeoutHandle = setTimeout(ret.close, mergedOptions.timeout);
            });

            return ret;
        }

        console.error(`Failed to create a notification. Bad selector: ${mergedOptions.targetSelector}`);

        return null;
    };

    return obj;
}