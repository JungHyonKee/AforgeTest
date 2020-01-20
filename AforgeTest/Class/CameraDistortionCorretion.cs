using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AforgeTest;

namespace AforgeTest {
    class CameraDistortionCorretion
    {
        Bitmap Result;
        public double Strength { get; set; } = 0;
        public double Zoom { get; set; } = 1;

        public CameraDistortionCorretion(double strength = 0, double zoom = 1)
        {
            Strength = strength <= 0 ? 0.00001 : strength;
            Zoom = zoom < 1 ? 1 : zoom;
        }

        public Bitmap Apply(Bitmap img, double stren)
        {
            Result = null;
            Size size = img.Size;
            int hWidth = size.Width/2;
            int hHeight = size.Height/2;
            double correctionRadius = Math.Sqrt( size.Width * size.Width + size.Height * size.Height ) / stren;

            Result = img.Process( pxl => 
            {
                //for (int x = 0; x < size.Width; x++)
                Parallel.For( 0, size.Width, x =>
                 {
                     for (int y = 0; y < size.Height; y++)
                     {
                         int newX = x - hWidth;
                         int newY = y - hHeight;
                         double distance = Math.Sqrt( newX * newX + newY * newY );
                         double r = distance / correctionRadius;

                         double theta = r == 0 ? 1 : Math.Atan( r ) / r;

                         int sourceX = (int)(hWidth + theta * newX);
                         int sourceY = (int)(hHeight + theta * newY);

                         pxl[x, y] = pxl[sourceX, sourceY];
                     }
                 } );
            } );

            return Result;
        }
    }
}
