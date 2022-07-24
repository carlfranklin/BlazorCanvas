# Blazor Canvas

This demo builds on from Scott Harden's EXCELLENT blog post, [Draw Animated Graphics in the Browser with Blazor WebAssembly](https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/), which uses the OSS [Blazor.Extensions.Canvas](https://github.com/BlazorExtensions/Canvas) component to draw on the canvas, but also includes JavaScript to help with animations, illustrating the real power of the HTML Canvas element. 

I took it one step beyond by encapsulating the JavaScript required to do animations in a Razor Class Library, which I call `AvnCanvasHelper`. At some point, I will create a repo just for AvnCanvasHelper because it can be used to do any kind of Canvas animation.

