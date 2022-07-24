# Blazor Canvas

This demo builds on from Scott Harden's EXCELLENT blog post, [Draw Animated Graphics in the Browser with Blazor WebAssembly](https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/), which uses the OSS [Blazor.Extensions.Canvas](https://github.com/BlazorExtensions/Canvas) component to draw on the canvas, but also includes JavaScript to help with animations, illustrating the real power of the HTML Canvas element. 

I took it one step beyond by encapsulating the JavaScript required to do animations in a Razor Class Library, which I call `AvnCanvasHelper`. At some point, I will create a repo just for `AvnCanvasHelper` because it can be used to do any kind of Canvas animation.

## AvnCanvasHelper

To use `AvnCanvasHelper`, place the `BECanvas` inside it as child content like so:

```c#
<CanvasHelper 
    @ref="CanvasHelper"
    CanvasResized="CanvasResized" 
    RenderFrame="RenderFrame"
    MouseDown="MouseDown"
    MouseUp="MouseUp"
    MouseMove="MouseMove">

        <BECanvas Width="600" Height="400" @ref="CanvasReference"></BECanvas>

</CanvasHelper>
```

Then, you'll need to hold references to the `AvnCanvasHelper` as well as the Context and the Canvas itself:

```c#
private Canvas2DContext Ctx;
private BECanvasComponent CanvasReference;
private CanvasHelper CanvasHelper;
```

Create your canvas reference in `OnAfterRenderAsync` and initialize the `AvnCanvasHelper`

```c#
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Create the canvas and context
        Ctx = await CanvasReference.CreateCanvas2DAsync();
        // Initialize the helper
        await CanvasHelper.Initialize();
    }
}
```

Now you can handle the `RenderFrame` event to draw the next frame. Here's an example from the demo:

```c#
public async Task RenderFrame(double fps)
{
    // update the Frames Per Second measurement
    FPS = fps;

    // The following code is adapted from Scott Harden's EXCELLENT blog post, 
    // "Draw Animated Graphics in the Browser with Blazor WebAssembly"
    // https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/

    if (BallField.Balls.Count == 0)
        BallField.AddRandomBalls(50);

    BallField.StepForward();

    await this.Ctx.BeginBatchAsync();

    await this.Ctx.ClearRectAsync(0, 0, BallField.Width, BallField.Height);
    await this.Ctx.SetFillStyleAsync("#003366");
    await this.Ctx.FillRectAsync(0, 0, BallField.Width, BallField.Height);

    await this.Ctx.SetFontAsync("26px Segoe UI");
    await this.Ctx.SetFillStyleAsync("#FFFFFF");
    await this.Ctx.FillTextAsync("Blazor Canvas", 10, 30);

    await this.Ctx.SetFontAsync("16px consolas");
    await this.Ctx.FillTextAsync($"FPS: {fps:0.000}", 10, 50);

    await this.Ctx.SetStrokeStyleAsync("#FFFFFF");
    foreach (var ball in BallField.Balls)
    {
        await this.Ctx.BeginPathAsync();
        await this.Ctx.ArcAsync(ball.X, ball.Y, ball.R, 0, 2 * Math.PI, false);
        await this.Ctx.SetFillStyleAsync(ball.Color);
        await this.Ctx.FillAsync();
        await this.Ctx.StrokeAsync();
    }

    await this.Ctx.EndBatchAsync();
}
```

### Methods

`AvnCanvasHelper` exposes the following methods;

| Method     | Description                                                  |
| ---------- | ------------------------------------------------------------ |
| Initialize | Call this in your Blazor app's OnAfterRenderAsync method when firstRender is true |

### Events

`AvnCanvasHelper` exposes the following events:

| Event         | Description                                            |
| ------------- | ------------------------------------------------------ |
| CanvasResized | When the browser, and therefore the canvas, is resized |
| RenderFrame   | When a frame is ready to be drawn                      |
| MouseDown     | The user clicked a mouse button                        |
| MouseUp       | The user released a mouse button                       |
| MouseMove     | The user moved the mouse                               |

### CanvasMouseArgs

The DOM holds a lot of information for the mouse. I've extracted the basic properties into `CanvasMouseArgs` which is passed into the `MouseDown`, `MouseUp`, and `MouseMove` events. For more information check out [https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent](https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent)

```c#
public class CanvasMouseArgs
{
    public int ScreenX { get; set; }
    public int ScreenY { get; set; }
    public int ClientX { get; set; }
    public int ClientY { get; set; }
    public int MovementX { get; set; }
    public int MovementY { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool AltKey { get; set; }
    public bool CtrlKey { get; set; }
    public bool Bubbles { get; set; }
    public int Buttons { get; set; }
    public int Button { get; set; }
}
```

