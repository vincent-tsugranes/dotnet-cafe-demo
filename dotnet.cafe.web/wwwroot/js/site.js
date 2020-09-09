// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function cafeInitialize() {
    console.log("called");

    /*    if (window.EventSource == undefined) {
            // If not supported  
            document.getElementById('cafeEvents').innerHTML = "Your browser doesn't support Server Sent Events.";
            return;
        } else {
            var source = new EventSource('/api/sse');
            console.log("got source");
    
            source.onopen = function (event) {
                console.log("source open: " + event);
                document.getElementById('cafeEvents').innerHTML += 'Connection Opened.<br>';
            };
    
            source.onerror = function (event) {
                console.log("error event" + event);
                if (event.eventPhase == EventSource.CLOSED) {
                    document.getElementById('cafeEvents').innerHTML += 'Connection Closed.<br>';
                }
            };
    
            source.onmessage = function (event) {
                console.log("message event" + event.data);
                document.getElementById('cafeEvents').innerHTML += event.data + '<br>';
            };
            
        }*/
    var source = new EventSource('/api/sse');

    source.onmessage = function (event) {
        console.log('onmessage: ' + event.data);
    };

    source.onopen = function (event) {
        console.log('onopen');
    };

    source.onerror = function (event) {
        console.log('onerror');
    }
}