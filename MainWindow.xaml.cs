//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;
using AxAXVLC;
using AxWMPLib;
using System.Collections.Generic;
using Visifire.Charts;
using Visifire.Commons;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
   
    

    
    public partial class MainWindow : Window
    {

        double lastCordi = 0;
        bool wmpAdded = false;
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 8;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        //VLC
       // AxVLCPlugin vlc;
        AxWindowsMediaPlayer wmp;
        int i = 0;
        double temp = 0;
        List<double> vals;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //this.CreateChart();
           // Console.WriteLine(HttpPost("http://192.168.2.15/candid/backend.php", "_ACTION=_DETAILS&_TAGID=0003396358"));
        }

       

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            wmp = new AxWindowsMediaPlayer();
           
            
            
            
            // If the player is playing, switch to full screen. 
            //if (wmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
           // {
             //   wmp.fullScreen = true;
           // }
            
            
          
            vals = new List<double>();
           
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

           foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

 
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

  
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);



                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {

                    


                    i++;
                    // Skeleton skel = skeletons[1];
                    foreach (Skeleton skel in skeletons)
                    {
                       // RenderClippedEdges(skel, dc);




                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);

                            Joint rightHand = skel.Joints[JointType.HandRight];
                            double rightX = System.Math.Round(rightHand.Position.X, 4);
                            rightX += 1;
                            

                            double disp = Math.Abs(rightX - lastCordi);

                            vals.Add(disp);


                            if (vals.Count > 14)
                            {

                                i = 0;
                                foreach (double v in vals) // Loop through List with foreach
                                {
                                    temp += v;
                                }


                                if (!wmpAdded)
                                {
                                     windowsFormsHost1.Child = wmp;
                                     //wmp.Ctlcontrols.stop();
                                     wmp.URL = @"F:\videos\SEASONS\How I Met Your Mother\rayban.mp4";
                                     wmpAdded = true;
                                     bg.Visibility = Visibility.Hidden;
                                     wmp.uiMode = "none";
                                     // If the player is playing, switch to full screen. 
                                     if (wmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
                                     {
                                         wmp.fullScreen = true;
                                     }
                                }
                                Console.Write(Math.Round(temp, 3));

                                if (Math.Round(temp, 3) > 0.8 && Math.Round(temp, 3) < 1.5)
                                {

                                    Console.WriteLine("FAST");
                                    wmp.settings.rate = 2;
                                }
                                else if (Math.Round(temp, 3) < 0.7)
                                {

                                    Console.WriteLine("SLOWER");
                                    //vlc.playSlower();
                                    wmp.settings.rate = 0.5;
                                }
                                else if (Math.Round(temp, 3) > 1.6)
                                {

                                    Console.WriteLine("SUPER FAST");
                                    //vlc.playSlower();
                                    wmp.settings.rate = 3.0;
                                }
                               

                               
                                temp = 0;
                                vals.Clear();
                            }
                            

                            /*if (i > 500) {
                                wmp.Ctlcontrols.stop();
                                bg.Visibility = Visibility.Hidden;
                               
                                Console.WriteLine("     STOPPING");
                            }
                            */
                           
                          
                           
                            lastCordi = rightX;
             

                        }
                       
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {

     //  Render Joints
            //foreach (Joint joint in skeleton.Joints)
            //{

            Joint joint = skeleton.Joints[JointType.HandRight];

                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;                    
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                    
                }

                if (drawBrush != null)
                {
                    
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            //}
          
           
            
        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }


        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

           
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        public static string HttpPost(string URI, string Parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //req.Proxy = new System.Net.WebProxy(ProxyString, true);
            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        private void CreateChart()
        {
            // Create a Chart element
            Chart chart = new Chart();

            // Set chart width and height
            chart.Width = 400;
            chart.Height = 300;

            // Create new DataSeries
            DataSeries dataSeries = new DataSeries();

            // Number of DataPoints to be generated
            int numberOfDataPoints = 10;

            // To set the YValues of DataPoint
            Random random = new Random();

            // Loop and add a few DataPoints
            for (int loopIndex = 0; loopIndex < numberOfDataPoints; loopIndex++)
            {
                // Create a DataPoint
                DataPoint dataPoint = new DataPoint();

                // Set the YValue using random number
                dataPoint.YValue = random.Next(1, 100);

                // Add DataPoint to DataSeries
                dataSeries.DataPoints.Add(dataPoint);
            }

            // Add DataSeries to Chart
            chart.Series.Add(dataSeries);

            // Add chart to the LayoutRoot for display
            layoutGrid.Children.Add(chart);
        }
 
    }
}