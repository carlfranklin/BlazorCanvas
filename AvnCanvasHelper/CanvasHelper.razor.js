/*
 * Adapted from Scott Harden's EXCELLENT blog post, 
 * "Draw Animated Graphics in the Browser with Blazor WebAssembly"
 * https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/
 */

/*This is called from the Blazor component's Initialize method*/
export function initRenderJS(instance) {
    // instance is the Blazor component dotnet reference
    window.theInstance = instance;
    // tell the window we want to handle the resize event
    window.addEventListener("resize", resizeCanvasToFitWindow);
    // Call resize now
    resizeCanvasToFitWindow();
    // request an animation frame, telling window to call renderJS
    // https://developer.mozilla.org/en-US/docs/Web/API/window/requestAnimationFrame
    window.requestAnimationFrame(renderJS);
}

/*This is called whenever we have requested an animation frame*/
function renderJS(timeStamp) {
    // Call the blazor component's [JSInvokable] RenderInBlazor method
    theInstance.invokeMethodAsync('RenderInBlazor', timeStamp);
    // request another animation frame
    window.requestAnimationFrame(renderJS);
}

/*This is called whenever the browser (and therefore the canvas) is resized*/
function resizeCanvasToFitWindow() {
    // canvasHolder is the ID of the div that wraps the renderfragment 
    // content(Canvas) in the blazor app
    var holder = document.getElementById('canvasHolder');
    // find the canvas within the renderfragment
    var canvas = holder.querySelector('canvas');
    if (canvas) {
        // resize the canvas
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        // Call the blazor component's [JSInvokable] ResizeInBlazor method
        theInstance.invokeMethodAsync('ResizeInBlazor', canvas.width, canvas.height);
    }
}

