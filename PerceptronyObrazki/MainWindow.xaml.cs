using AForge.Imaging.Filters;
using AForge.Math.Random;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PerceptronyObrazki
{
    public partial class MainWindow : Window
    {
        public List<StrokeCollection> strokes = new List<StrokeCollection>();
        public List<int> imagePoints = new List<int>();
        public int toLoad = 8;
        public int imageNumber = 0;
        public Random randN = new Random();
        DrawingAttributes attributes = new DrawingAttributes();
        
        public MainWindow()
        {
            InitializeComponent();

            attributes.StylusTip = StylusTip.Rectangle;
            attributes.Width = 1;
            attributes.Height = 1;

            for (int i = 1; i <= toLoad; i++)
            {
                var fs = new FileStream("obrazek" + i.ToString(), FileMode.Open, FileAccess.Read);
                strokes.Add(new StrokeCollection(fs));
                fs.Close();
            }
            drawSpace.Strokes = strokes[0];
            for (int i = 0; i < 2500; i++)
            {
                imagePoints.Add(-1);
            }
        }
        private void TrainClick(object sender, RoutedEventArgs e)
        {
            int t = 0;
            foreach (var stroke in drawSpace.Strokes)
            {
                Geometry sketchGeo = stroke.GetGeometry();
                //Rect strokeBounds = sketchGeo.Bounds;

                for (int y = 0; y < 50; y++)
                {
                    for (int x = 0; x < 50; x++)
                    {
                        Point p = new Point(x, y);

                        if (sketchGeo.FillContains(p))
                        {
                            imagePoints[t] = 1;
                        }
                        t++;
                    }
                }
                t = 0;
            }
            for (int i = 0; i < 2500; i++)
            {
                if (i % 50 == 0)
                {
                    Debug.WriteLine("");
                }
                if (imagePoints[i] == 1)
                {
                    Debug.Write("X");
                }
                else
                {
                    Debug.Write("-");
                }
            }
        }
        private void NoiseClick(object sender, RoutedEventArgs e)
        {
            int d;
            StylusPoint p;
            List<Stroke> l = new List<Stroke>();
            DrawingAttributes attributesDelete = new DrawingAttributes();
            attributesDelete.Color = Colors.White;
            attributesDelete.StylusTip = StylusTip.Rectangle;
            attributesDelete.Width = 1;
            attributesDelete.Height = 1;

            for (int i = 0; i < 40; i++)
            {
                d = randN.Next(2500);
                if (imagePoints[d] == -1)
                {
                    StylusPointCollection points = new StylusPointCollection();
                    
                    imagePoints[d] = 1;
                    p = new StylusPoint(d % 50, d / 50);
                    points.Add(p);
                    Stroke s = new Stroke(points, attributes); 
                    l.Add(s);
                    drawSpace.Strokes.Add(s);
                }
                else
                {
                    StylusPointCollection points = new StylusPointCollection();

                    imagePoints[d] = -1;
                    p = new StylusPoint(d % 50, d / 50);
                    points.Add(p);
                    Stroke s = new Stroke(points, attributesDelete);
                    l.Add(s);
                    drawSpace.Strokes.Add(s);
                }
            }
            
        }
        private void SavePointsClick(object sender, RoutedEventArgs e)
        {
            var fs = new FileStream("obrazek" + (imageNumber + 1).ToString(), FileMode.Create);
            drawSpace.Strokes.Save(fs);
            fs.Close();
        }

        private void LoadPointsClick(object sender, RoutedEventArgs e)
        {
            var fs = new FileStream("obrazek" + (imageNumber + 1).ToString(), FileMode.Open, FileAccess.Read);
            strokes[imageNumber] = new StrokeCollection(fs);
            drawSpace.Strokes = strokes[imageNumber];
            fs.Close();
        }       

        private void AddNewClick(object sender, RoutedEventArgs e)
        {
            var fs = new FileStream("obrazek" + (strokes.Count() + 1).ToString(), FileMode.Create);
            drawSpace.Strokes.Save(fs);
            fs.Close();

            var fs2 = new FileStream("obrazek" + (strokes.Count() + 1).ToString(), FileMode.Open, FileAccess.Read);
            strokes.Add(new StrokeCollection(fs2));
            fs2.Close();

            toLoad++;

        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            if (imageNumber == toLoad - 1)
            {
                imageNumber = -1;
            }
            drawSpace.Strokes = strokes[imageNumber + 1];
            imageNumber++;
            ResetPoints();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            drawSpace.Strokes.Clear();
        }
        private void Clear2Click(object sender, RoutedEventArgs e)
        {
            drawSpace2.Strokes.Clear();
        }
        public void ResetPoints()
        {
            for (int i = 0; i < 2500; i++)
            {
                imagePoints[i] = -1;
            }
        }

    }
}
