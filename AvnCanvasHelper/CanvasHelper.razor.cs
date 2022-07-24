/*
 * Adapted from Scott Harden's EXCELLENT blog post, 
 * "Draw Animated Graphics in the Browser with Blazor WebAssembly"
 * https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/
 */

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Drawing;

namespace AvnCanvasHelper
{
    /// <summary>
    /// CanvasHelper component gives you render and resize callbacks for Canvas animation
    /// </summary>
    public partial class CanvasHelper : ComponentBase, IAsyncDisposable
    {
        /// <summary>
        /// JS Interop module used to call JavaScript
        /// </summary>
        private Lazy<Task<IJSObjectReference>> _moduleTask;
        
        /// <summary>
        /// Used to calculate the frames per second
        /// </summary>
        private DateTime _lastRender;

        /// <summary>
        /// JS Runtime
        /// </summary>
        [Inject]
        protected IJSRuntime _jsRuntime { get; set; }

        /// <summary>
        /// Event called when the browser (and therefore the canvas) is resized
        /// </summary>
        [Parameter]
        public EventCallback<Size> CanvasResized { get; set; }

        /// <summary>
        /// Event called every time a frame can be redrawn
        /// </summary>
        [Parameter]
        public EventCallback<double> RenderFrame { get; set; }

        /// <summary>
        /// Event called on mouse down
        /// </summary>
        [Parameter]
        public EventCallback<CanvasMouseArgs> MouseDown { get; set; }

        /// <summary>
        /// Event called on mouse up
        /// </summary>
        [Parameter]
        public EventCallback<CanvasMouseArgs> MouseUp { get; set; }

        /// <summary>
        /// Event called on mouse move
        /// </summary>
        [Parameter]
        public EventCallback<CanvasMouseArgs> MouseMove { get; set; }


        /// <summary>
        /// Call this in your Blazor app's OnAfterRenderAsync method when firstRender is true
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            // We need to specify the .js file path relative to this code
            _moduleTask = new(() => _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/AvnCanvasHelper/CanvasHelper.razor.js").AsTask());

            // Load the module
            var module = await _moduleTask.Value;

            // Initialize
            await module.InvokeVoidAsync("initRenderJS", DotNetObjectReference.Create(this));
            
            // Dispose the module
            await module.DisposeAsync();
        }

        /// <summary>
        /// Handle the JavaScript event called when the browser/canvas is resized
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task ResizeInBlazor(int width, int height)
        {
            var size = new Size(width, height);
            // Raise the CanvasResized event to the Blazor app
            await CanvasResized.InvokeAsync(size);
        }

        /// <summary>
        /// Handle the JavaScript event when a frame is ready to render
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        [JSInvokable]
        public async ValueTask RenderInBlazor(float timeStamp)
        {
            // calculate frames per second
            double fps = 1.0 / (DateTime.Now - _lastRender).TotalSeconds;
            
            _lastRender = DateTime.Now; // update for the next time 

            // raise the RenderFrame event to the blazor app
            await RenderFrame.InvokeAsync(fps);
        }

        /// <summary>
        /// Handle the JavaScript window.mousedown event
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task OnMouseDown(CanvasMouseArgs args)
        {
            await MouseDown.InvokeAsync(args);
        }

        /// <summary>
        /// Handle the JavaScript window.mouseup event
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task OnMouseUp(CanvasMouseArgs args)
        {
            await MouseUp.InvokeAsync(args);
        }

        /// <summary>
        /// Handle the JavaScript window.mousemove event
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task OnMouseMove(CanvasMouseArgs args)
        {
            await MouseMove.InvokeAsync(args);
        }

        /// <summary>
        /// Dispose of our module resource
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (_moduleTask != null && _moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
