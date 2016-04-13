﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CongCuDungCu.Code
{
    public static class PhotoUtils
    {
        public static Image Inscribe(Image image, int size)
        {
            return Inscribe(image, size, size);
        }

        public static Image Inscribe(Image image, int width, int height)
        {
            var result = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(result))
            {
                var factor = 1.0 * width / image.Width;
                if (image.Height * factor < height)
                {
                    factor = 1.0 * height / image.Height;
                }
                var size = new Size((int)(width / factor), (int)(height / factor));
                var sourceLocation = new Point((image.Width - size.Width) / 2, (image.Height - size.Height) / 2);

                SmoothGraphics(graphics);
                graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(sourceLocation, size), GraphicsUnit.Pixel);
            }
            return result;
        }

        private static void SmoothGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public static void SaveToJpeg(Image image, Stream output)
        {
            image.Save(output, ImageFormat.Jpeg);
        }

        public static void SaveToJpeg(Image image, string fileName)
        {
            image.Save(fileName, ImageFormat.Jpeg);
        }
    }
}