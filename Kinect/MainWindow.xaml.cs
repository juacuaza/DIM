using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.IO;

namespace Kinect
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributos

        private Boolean radioButton;
        private KinectSensor sensor;
        //Color
        private byte[] colorPixels;
        private WriteableBitmap colorBitmap;
        //Profundidad
        private DepthImagePixel[] depthPixels;

        private bool ColorOrDepth;


        #endregion


        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Metodos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            try
            {
                if (this.sensor != null)
                {
                    this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                }

                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth,
                    this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);


                if (this.sensor != null)
                {
                    this.sensor.ColorFrameReady += this.SensorColorFrameReady;
                }

                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                this.colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth,
                    this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                if (this.sensor != null)
                {
                    this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                }

                if (this.sensor != null)
                {
                    this.sensor.DepthFrameReady += this.SensorDepthFrameReady;
                }
                // igualar la imagen al sensor
                this.IMG_Kinect.Source = this.colorBitmap;

                try
                {
                    this.sensor.Start();
                    this.LBL_Info.Content = "Kinect emitiendo.";
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }catch(Exception ex)
            {
                LBL_Info.Content = ex.Message;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (ColorOrDepth == true)
                {
                    if (colorFrame != null)
                    {
                        colorFrame.CopyPixelDataTo(this.colorPixels);
                        this.colorBitmap.WritePixels(new Int32Rect(0, 0,
                        this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels, this.colorBitmap.PixelWidth * sizeof(int), 0);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (ColorOrDepth == false)
                {
                    if (depthFrame != null)
                    {
                        depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);
                        int minDepth = depthFrame.MinDepth;
                        int maxDepth = depthFrame.MaxDepth;
                        // Convertir profundidad a RGB
                        int colorPixelIndex = 0;
                        for (int i = 0; i < this.depthPixels.Length; ++i)
                        {
                            short depth = depthPixels[i].Depth;
                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                            this.colorPixels[colorPixelIndex++] = intensity;
                            this.colorPixels[colorPixelIndex++] = intensity;
                            this.colorPixels[colorPixelIndex++] = intensity;
                            ++colorPixelIndex; // no alpha channel RGB
                        }
                        // Copiar pixels RGB en el bitmap
                        this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth,
                        this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CMB_Opciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CMB_Opciones.SelectedIndex == 1)
                ColorOrDepth = true;
            else if (CMB_Opciones.SelectedIndex == 2)
                ColorOrDepth = false;
        }

        #endregion
    }
}
