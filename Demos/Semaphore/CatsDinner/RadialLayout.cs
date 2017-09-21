using System;
using Xamarin.Forms;
using System.Linq;

namespace CatsDinner
{
    public class RadialLayout : Layout<View>
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (!Children.Any(c => c.IsVisible))
                return;
         
            double dimension = Math.Min(width, height);

            double angle = 0;
            double spacing = (360.0 / Children.Count) * (Math.PI / 180);
            double radius = dimension / 2;

            foreach (var child in Children)
            {
                SizeRequest sizeRequest = child.Measure(double.PositiveInfinity, double.PositiveInfinity);

                Size size;
                size = !sizeRequest.Request.IsZero
                    ? sizeRequest.Request
                    : !sizeRequest.Minimum.IsZero
                        ? sizeRequest.Minimum
                        : new Size(100, 100);

                Point centerPoint = new Point(Math.Cos(angle) * radius, -Math.Sin(angle) * radius);
                Point actualPoint = new Point(x + (width / 2 + centerPoint.X) - size.Width / 2, 
                                              y + (height / 2 + centerPoint.Y) - size.Height / 2);

                if (actualPoint.X < x)
                    actualPoint.X = x;
                if (actualPoint.X + size.Width + x > width)
                    actualPoint.X = (width - size.Width);
                if (actualPoint.Y < y)
                    actualPoint.Y = y;
                if (actualPoint.Y + size.Height + y > height)
                    actualPoint.Y = (height - size.Height);

                LayoutChildIntoBoundingRegion(child, 
                    new Rectangle(actualPoint.X, actualPoint.Y, size.Width, size.Height));

                angle += spacing;
            }
        }
    }
}
