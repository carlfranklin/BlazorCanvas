/*
 * Adapted from Scott Harden's EXCELLENT blog post, 
 * "Draw Animated Graphics in the Browser with Blazor WebAssembly"
 * https://swharden.com/blog/2021-01-07-blazor-canvas-animated-graphics/
 */
public class Ball
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double XVel { get; private set; }
    public double YVel { get; private set; }
    public double R { get; private set; }
    public string Color { get; private set; }

    public Ball(double x, double y, double xVel, double yVel, double radius, string color)
    {
        (X, Y, XVel, YVel, R, Color) = (x, y, xVel, yVel, radius, color);
    }

    public void StepForward(double width, double height)
    {
        X += XVel;
        Y += YVel;
        if (X < 0 || X > width)
            XVel *= -1;
        if (Y < 0 || Y > height)
            YVel *= -1;

        if (X < 0)
            X += 0 - X;
        else if (X > width)
            X -= X - width;

        if (Y < 0)
            Y += 0 - Y;
        if (Y > height)
            Y -= Y - height;
    }
}
