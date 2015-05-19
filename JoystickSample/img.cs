using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using JoystickInterface;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;


namespace JoystickSample
{
    public partial class img : Form
    {

        Bitmap matrice;
        String requestUrl;
        Boolean timerTicker = false;
        Capture capWebcam = null;
        bool blmCaptureInProgress = false;
        Image<Bgr, Byte> imageOriginal;
        Image<Hsv, Byte> imageProcessed;
        static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public img()
        {
            InitializeComponent();
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!timerTicker)
            {
                requestUrl = "http://" + textBox2.Text + ":" + textBox3.Text + "/shot.jpg";
                timer1.Start();
                timerTicker = true;
            }
            else
            {
                timer1.Stop();
                timerTicker = false;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Color red = Color.FromArgb(200, 0, 0), blue = Color.FromArgb(0, 0, 255), now_color;

            try
            {
                for (int i = 0; i < matrice.Height; i++)
                {
                    for (int j = 0; j < matrice.Width; j++)
                    {
                        now_color = matrice.GetPixel(j, i);

                        if (now_color.R < 215 && now_color.B < 215)

                            matrice.SetPixel(j, i, Color.White);



                    }
                }

                pictureBox1.Image = matrice;
                matrice.Save("ImgPrelucrata.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (System.NotImplementedException ex) { }
            catch (System.Net.WebException ex) { }
            catch (System.Runtime.InteropServices.ExternalException ex) { }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                var request = System.Net.WebRequest.Create(requestUrl);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var memoryStream = new MemoryStream())
                {
                    int width = 476;
                    int height = 316;
                    Bitmap sourceBMP = (Bitmap)Bitmap.FromStream(stream);
                    Bitmap result = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(result))
                        g.DrawImage(sourceBMP, 0, 0, width, height);
                    matrice = result;


                    pictureBox1.Image = result;
                }
            }
            catch (System.NotImplementedException ex) { timer1.Stop(); }
            catch (System.Net.WebException ex) { timer1.Stop(); }
        }

        private void img_Load(object sender, EventArgs e)
        {
            try
            {
                capWebcam = new Capture();
            }
            catch (NullReferenceException ex) { return; }
            System.Windows.Forms.Application.Idle += processFrameAndUpdateGUI;
            blmCaptureInProgress = true;
        }

        private void img_FormClosed(object sender, FormClosedEventArgs args)
        {
            if (capWebcam != null)
            {
                capWebcam.Dispose();
            }
        }

        void processFrameAndUpdateGUI(object sender, EventArgs arg)
        {
            imageOriginal = capWebcam.QueryFrame();
            if (imageOriginal == null) return;

            //imageProcessed = new Image<Hsv,byte>(imageOriginal.Bitmap);//imageOriginal.Convert<Hsv, Byte>();
            // 2. Obtain the 3 channels (hue, saturation and value) that compose the HSV image
            Image<Hsv, Byte> hsv = imageOriginal.Convert<Hsv, Byte>();
            Image<Bgr, Byte> imageSmoothed = imageOriginal.PyrDown().PyrUp();
            imageSmoothed._SmoothGaussian(3);
            Image<Gray, Byte> imageGrayColorFiltered = new Image<Gray, Byte>(imageSmoothed.Bitmap);//.InRange(new Bgr(bMin, gMin, rMin), new Bgr(bMax, gMax, rMax));
            imageGrayColorFiltered.PyrDown().PyrUp()._SmoothGaussian(3);

            //imageGrayColorFiltered = imageGrayColorFiltered.Canny(100, 200, 3);
            Image<Gray, Byte> dest_image = new Image<Gray, Byte>(imageGrayColorFiltered.Size);
            double thredshold = CvInvoke.cvThreshold(imageGrayColorFiltered, dest_image, 160, 255, THRESH.CV_THRESH_TOZERO_INV & THRESH.CV_THRESH_OTSU);

            Image<Gray, Byte> imageCanny = imageGrayColorFiltered.Canny(0.1 * thredshold, thredshold);

            double dblRhoRes = 1.0;						//								'distance resolution in pixels
            double dblThetaRes = 4.0 * (Math.PI / 180.0);			//'angle resolution in radians (multiply by PI / 180 converts to radians)
            int intThresholdr = 20;							//					'a line is returned by the function if the corresponding accumulator value is greater than threshold
            double dblMinLineWidth = 10.0;						//				'minimum width of a line
            double dblMinGapBetweenLines = 10.0;

            LineSegment2D[] lines = imageCanny.HoughLinesBinary(
                    dblRhoRes, //Distance resolution in pixel-related units
                    dblThetaRes, //Angle resolution measured in radians.
                    intThresholdr, //threshold
                    dblMinLineWidth, //min Line width
                    dblMinGapBetweenLines //gap between lines
                    )[0]; //Get the lines from the first channel
            textBox2.Text = lines.Length+"";

            foreach (LineSegment2D line in lines)
            {
                imageOriginal.Draw(line, new Bgr(Color.Blue), 2);
            }
            ibProcessed.Image = imageOriginal;
        }


    }
}
