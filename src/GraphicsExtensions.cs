using System.Drawing;
using System.Drawing.Drawing2D;

public static class GraphicsExtensions
{
    public static void AddRoundedRectangle(this GraphicsPath path, Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        Size size = new Size(diameter, diameter);
        Rectangle arc = new Rectangle(rect.Location, size);


        path.AddArc(arc, 180, 90);


        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);


        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);


        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
    }

    public static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;


        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);

        path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
   
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);

        path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom - radius);
    
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);

        path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);

        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
  
        path.AddLine(rect.X, rect.Bottom - radius, rect.X, rect.Y + radius);

        path.CloseFigure();
        return path;
    }
} 