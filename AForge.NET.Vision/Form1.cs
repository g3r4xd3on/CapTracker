using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using System.Drawing.Imaging;
using System.IO.Ports;

namespace AForge.NET.Vision
{
    public partial class Form1 : Form
    {
        int colorRed;
        int colorGreen;
        int colorBlue;
        Graphics g;

        private FilterInfoCollection kamera;
        private VideoCaptureDevice video;

        public Form1()
        {
            InitializeComponent();
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            kamera = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in kamera)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
                button1.Enabled = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            video = new VideoCaptureDevice(kamera[comboBox1.SelectedIndex].MonikerString);
            video.NewFrame += new NewFrameEventHandler(video_NewFrame);
            video.Start();
        }

        void video_NewFrame(object sender, NewFrameEventArgs e)
        {
            Bitmap image = (Bitmap)e.Frame.Clone();
            Bitmap image1 = (Bitmap)e.Frame.Clone();
            pictureBox1.Image = image;

            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new RGB(Color.FromArgb(colorRed, colorGreen, colorBlue));
            filter.Radius = 100;
            filter.ApplyInPlace(image1);
            nesneTespiti(image1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
            if (video.IsRunning)
            {
                video.SignalToStop();
                //FinalVideo.Stop();
                //FinalVideo = null;
                this.Invalidate();
                g.Dispose();
                pictureBox1.Image = null;
                pictureBox2.Image = null;
            }
        }

        public void nesneTespiti(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Grayscale griFiltre = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = griFiltre.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);
            //
            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;


            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];
                    g = pictureBox1.CreateGraphics();
                    int kalinlik = Convert.ToInt32(numericUpDown1.Value);
                    int xEkseni = objectRect.X + (objectRect.Width / 2);
                    int yEkseni = objectRect.Y + (objectRect.Height / 2);

                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.Black);
                    PointF drawPoint = new PointF(0, 0);

                    if (radioButton4.Checked == true)
                    {
                        if (xEkseni > 450 && xEkseni < 640)
                        {
                            String drawString = "Sola Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("1");
                            }

                        }
                        else if (xEkseni > 190 && xEkseni < 450)
                        {
                            String drawString = "Duruyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                        }
                        else if (xEkseni > 1 && xEkseni < 190)
                        {
                            String drawString = "Sağa Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("2");
                            }
                        }
                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), kalinlik))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                    }
                    if (radioButton5.Checked == true)
                    {
                        if (xEkseni > 450 && xEkseni < 640)
                        {
                            String drawString = "Sola Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("1");
                            }
                        }
                        else if (xEkseni > 190 && xEkseni < 450)
                        {
                            String drawString = "Duruyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        }
                        else if (xEkseni > 1 && xEkseni < 190)
                        {
                            String drawString = "Sağa Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("2");
                            }
                        }
                        using (Pen pen = new Pen(Color.FromArgb(0, 0, 255), kalinlik))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                    }
                    if (radioButton6.Checked == true)
                    {
                        if (xEkseni > 450 && xEkseni < 640)
                        {
                            String drawString = "Sola Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("1");
                            }
                        }
                        else if (xEkseni > 190 && xEkseni < 450)
                        {
                            String drawString = "Duruyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        }
                        else if (xEkseni > 1 && xEkseni < 190)
                        {
                            String drawString = "Sağa Gidiyor" + " " + "[" + "X: " + objectRect.X + "," + "Y: " + objectRect.Y + "]";
                            g.DrawString(drawString, drawFont, drawBrush, drawPoint);

                            if (serialPort1.IsOpen == true)
                            {
                                serialPort1.Write("2");
                            }
                        }
                        using (Pen pen = new Pen(Color.FromArgb(0, 255, 0), kalinlik))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                        this.Invalidate();
                        g.Dispose();
                    }
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            trackBar3.Enabled = false;

            numericUpDownRed.Enabled = true;
            numericUpDownGreen.Enabled = true;
            numericUpDownBlue.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            colorRed = 0;
            colorGreen = 0;
            colorBlue = 200;
            numericUpDownRed.Value = colorRed;
            numericUpDownGreen.Value = colorGreen;
            numericUpDownBlue.Value = colorBlue;
            trackBar1.Value = colorRed;
            trackBar2.Value = colorGreen;
            trackBar3.Value = colorBlue;
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            trackBar3.Enabled = false;
            numericUpDownRed.Enabled = false;
            numericUpDownGreen.Enabled = false;
            numericUpDownBlue.Enabled = false;
        }

        private void numericUpDownRed_ValueChanged(object sender, EventArgs e)
        {
            colorRed = Convert.ToInt32(numericUpDownRed.Value);
            trackBar1.Value = colorRed;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Enabled = true;
            trackBar2.Enabled = true;
            trackBar3.Enabled = true;

            numericUpDownRed.Enabled = false;
            numericUpDownGreen.Enabled = false;
            numericUpDownBlue.Enabled = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            colorRed = trackBar1.Value;
            numericUpDownRed.Value = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            colorGreen = trackBar2.Value;
            numericUpDownGreen.Value = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            colorBlue = trackBar3.Value;
            numericUpDownBlue.Value = trackBar3.Value;
        }

        private void numericUpDownGreen_ValueChanged(object sender, EventArgs e)
        {
            colorGreen = Convert.ToInt32(numericUpDownGreen.Value);
            trackBar2.Value = colorGreen;
        }

        private void numericUpDownBlue_ValueChanged(object sender, EventArgs e)
        {
            colorBlue = Convert.ToInt32(numericUpDownBlue.Value);
            trackBar3.Value = colorBlue;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
            if(video.IsRunning)
            {
                video.SignalToStop();
                //FinalVideo.Stop();
                //FinalVideo = null;
                this.Invalidate();
                g.Dispose();
                pictureBox1.Image = null;
                pictureBox2.Image = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen == false)
            {
                if (string.IsNullOrEmpty(comboBox2.Text))
                {
                    MessageBox.Show("Bir bağlantı noktası seçin!");
                }
                else
                {
                    try
                    {
                            serialPort1.PortName = comboBox2.SelectedItem.ToString();
                            serialPort1.BaudRate = 9600;
                            serialPort1.Open();
                    }
                    catch
                    {
                        MessageBox.Show(comboBox2.SelectedItem.ToString() + " " + "Seri port bağlantısı kurulamadı!");
                    }
                }
            }
        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }
    }
}
