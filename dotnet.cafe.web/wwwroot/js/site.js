// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function cafeInitialize() {
    
    if (window.EventSource === undefined) {
        // If not supported  
        console.log("Your browser doesn't support Server Sent Events.")
        
    } else {
        var source = new EventSource('/api/sse');

        source.onmessage = function (event) {
            console.log('onmessage: ' + event.data);
        };

        source.onopen = function (event) {
            console.log('Connection Open');
        };

        source.onerror = function (event) {
            if (event.eventPhase === EventSource.CLOSED) {
                console.log('Connection Closed');
            }else{
                console.log('Connection Error: ' + event);
            }
        }
    }
    
}