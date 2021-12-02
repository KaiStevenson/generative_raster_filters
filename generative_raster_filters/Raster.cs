using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace generative_raster_filters
{
    class Raster
    {
        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }
        public static void SaveFile(Bitmap bmp, string path, string name)
        {
            var ms = new FileStream(path + "\\" + name, FileMode.Create, FileAccess.Write);
            bmp.Save(ms, ImageFormat.Png);
            ms.Close();
        }
        //scales point 1 from scale 1 to scale 2
        public static int ScaleResolution(int point1, int scale1, int scale2)
        {
            return (int)(((float)scale2 / (float)scale1) * point1);
        }
    }
    class Filters
    {
        public static Filter[] filtersInfo = new Filter[]
        {
            new Filter("Triangles", "Creates an alternating pattern of triangles", "Number of triangles along the X axis", "Output resolution scale", 20f, 1f)
        };
        public static Filter GetFilter(string filterName)
        {
            var f = filtersInfo.FirstOrDefault(q => q.name == filterName);
            return f;
        }
        public struct Filter
        {
            public Filter(string name, string description, string factor1Description, string factor2Description, float factor1Default, float factor2Default)
            {
                this.name = name;
                this.description = description;
                this.factor1Description = factor1Description;
                this.factor2Description = factor2Description;
                this.factor1Default = factor1Default;
                this.factor2Default = factor2Default;
            }
            public string name;
            public string description;
            public string factor1Description;
            public string factor2Description;
            public float factor1Default;
            public float factor2Default;
        }
        public static Bitmap TriangleFilter(Bitmap bmpI, int factor1, float factor2)
        {
            var bmp = new Bitmap((int)(bmpI.Width * factor2), (int)(bmpI.Height * factor2)); //arbitrary size, smaller is faster but less accurate. size of input only matters for downsample accuracy
            var triangleSize = bmp.Width / factor1;
            var bmpS = Raster.ResizeBitmap(bmpI, bmp.Width / triangleSize, bmp.Height / triangleSize);
            for (int c = 0; c < bmp.Width; c++)
            {
                for (int r = 0; r < bmp.Height; r++)
                {
                    //find parent pixel from downsampled image
                    var sx = Raster.ScaleResolution(c, bmp.Width, bmpS.Width);
                    var sy = Raster.ScaleResolution(r, bmp.Height, bmpS.Height);
                    var pc = bmpS.GetPixel(sx, sy);
                    //find points relative to parent pixel
                    var rtc = c - (sx * triangleSize);
                    var rtr = r - (sy * triangleSize);
                    //determine if rtc, rtr is inside triangle formed by downsampled pixel square
                    //triangle extent from center in either direction at relative y descent
                    var xtxD = rtr / 2f;
                    //triangle extent from center in either direction at relative y ascent
                    var xtxA = (triangleSize - rtr) / 2f;
                    //determine if triangle ought to be downward facing or otherwise
                    var d = (sx + sy) % 2 == 0;
                    if ((Math.Abs(rtc - (triangleSize / 2)) < xtxD && d) || Math.Abs(rtc - (triangleSize / 2)) < xtxA && !d)
                    {
                        bmp.SetPixel(c, r, pc);
                    }
                }
            }
            return bmp;
        }
    }
}