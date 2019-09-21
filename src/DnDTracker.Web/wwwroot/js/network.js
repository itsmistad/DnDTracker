'use strict';

/* 
 * This library is a wrapper around SignalR and ajax.
 * Usage for SignalR: (in order)
 *  - network.init('/route/to/hub/'); // <-- Once
 *  - network.on('eventName', json => { console.log(JSON.stringify(json)); }); // <-- n times, except duplicates
 *  - network.connect(err => { if (err) console.error(err); }); // <-- Once
 *  - network.send('eventName', json, err => { console.error(err); }); // <-- n times, including duplicates
 *  
 *  Usage for ajax:
 *  - network.post('/route/to/method', json, json => { console.log(JSON.stringify(json)); });
 *  - network.get('/route/to/method', json, json => { console.log(JSON.stringify(json)); });
 *  
 *  "json" is a json object, not a string.
 */

var network = new function() {
    var connection, connected, queue;

    var obj = {};

    obj.init = route => {
        if (!connection)
            connection = new signalR.HubConnectionBuilder().withUrl(`/${route}`).build();
    };

    obj.on = (event, func) => {
        if (connection)
            connection.on(event, json => {
                console.log(`Received ${event}: ${json}`);
                func(JSON.parse(json));
            });
    };

    obj.connect = func => {
        if (connected) return; // Prevents connection duplication.
        console.log('Connecting...');
        connection.start().then(() => {
            console.log('Connected successfully!');
            connected = true;
            func();
            if (queue) {
                for (let i = 0; i < queue.length; i++) {
                    let queueParams = queue.shift();
                    obj.send(queueParams.event, queueParams.json, queueParams.errCallback);
                }
                queue = null;
            }
        }).catch(err => {
            func(err.toString());
        });
    };

    obj.send = (event, json, errCallback) => {
        if (!connected) { // Allows queueing while a connection is being established.
            if (!queue) queue = [];
            queue.push({
                event, json, errCallback
            });
            return;
        }
        var str = JSON.stringify(json);
        console.log(`Sending ${event}: ${str}`);
        connection.invoke(event, str).catch(err => {
            if (errCallback) errCallback(err.toString());
        });
    };

    obj.post = (route, json, func) => {
        $.ajax({
            type: "POST",
            url: route,
            data: JSON.stringify(json),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: response => func(response),
            failure: response => func(response),
            error: response => func(response)
        });  
    };

    obj.get = (route, json, func) => {
        $.ajax({
            type: "GET",
            url: route,
            data: JSON.stringify(json),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: response => func(response),
            failure: response => func(response),
            error: response => func(response)
        });
    };

    return obj;
}