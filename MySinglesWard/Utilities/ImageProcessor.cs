using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace MSW.Utilities
{
    enum ImageProcessorType
    {
        User,
        Company
    }

    /// <summary>
    /// Used in the photo upload process if the photo is too large to fit on the screen during cropping.
    /// </summary>
    static class ImageProcessor
    {
        public static void ResizeImage(string sourceFilePath, string destinationFilePath, int newWidth, int newHeight, bool autoCrop = false)
        {
            //Set up images and drawing objects
            using (Image originalImage = System.Drawing.Image.FromFile(sourceFilePath))
            {
                using (Image interimImage = new Bitmap(newWidth, newHeight))
                {
                    using (Image finalImage = new Bitmap(newWidth, newHeight))
                    {
                        using (Graphics finalGraphic = Graphics.FromImage(finalImage))
                        {

                            finalGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            finalGraphic.SmoothingMode = SmoothingMode.HighQuality;
                            finalGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            finalGraphic.CompositingQuality = CompositingQuality.HighQuality;

                            //Get original image scaled width and height
                            int scaledWidth = newWidth; //default, in case the image is proportional
                            int scaledHeight = newHeight; //default, in case the image is proportional
                            int top = 0;
                            int left = 0;
                            if (autoCrop) //resize cropped (if necessary)
                            {
                                if (originalImage.Height != originalImage.Width)
                                {
                                    double scale = 0;
                                    if (originalImage.Width > originalImage.Height)
                                    {
                                        scale = Convert.ToDouble(newHeight) / Convert.ToDouble(originalImage.Height);
                                    }
                                    else if (originalImage.Height > originalImage.Width)
                                    {
                                        scale = Convert.ToDouble(newWidth) / Convert.ToDouble(originalImage.Width);
                                    }
                                    scaledWidth = Convert.ToInt32(scale * Convert.ToDouble(originalImage.Width));
                                    scaledHeight = Convert.ToInt32(scale * Convert.ToDouble(originalImage.Height));
                                    left = (newWidth - scaledWidth) / 2;
                                    top = (newHeight - scaledHeight) / 2;
                                }
                            }
                            else //resize scaled
                            {
                                if (originalImage.Height != originalImage.Width)
                                {
                                    double scale = 0;
                                    if (originalImage.Width > originalImage.Height)
                                    {
                                        scale = Convert.ToDouble(newWidth) / Convert.ToDouble(originalImage.Width);
                                    }
                                    else if (originalImage.Height > originalImage.Width)
                                    {
                                        scale = Convert.ToDouble(newHeight) / Convert.ToDouble(originalImage.Height);
                                    }
                                    scaledWidth = Convert.ToInt32(scale * Convert.ToDouble(originalImage.Width));
                                    scaledHeight = Convert.ToInt32(scale * Convert.ToDouble(originalImage.Height));
                                    left = (newWidth - scaledWidth) / 2;
                                    top = (newHeight - scaledHeight) / 2;

                                    //Paint white background
                                    finalGraphic.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, newWidth, newHeight));
                                }
                            }
                            //Draw!
                            finalGraphic.DrawImage(originalImage, left, top, scaledWidth, scaledHeight);

                            //Save Image
                            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                            EncoderParameters encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            finalImage.Save(destinationFilePath, info[1], encoderParameters);
                        }
                    }
                }
            }
        }
    }
}