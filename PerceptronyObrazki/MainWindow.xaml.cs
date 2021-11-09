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
        public List<int> imagePoints2 = new List<int>();
        public List<int> tmpList = new List<int>();
        public int toLoad = 8;
        public int imageNumber = 0;
        public Random randN = new Random();
        public Random randW = new Random();
        public Random randE = new Random();
        DrawingAttributes attributes = new DrawingAttributes();

        public List<List<double>> perceptron = new List<List<double>>();
        public List<List<int>> examples = new List<List<int>>();
        //public List<int> answers = new List<int>();
        //public List<int> example = new List<int>();
        public int maxRounds = 1000;
        public double learn_const = 0.5;
    
        
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
                imagePoints2.Add(-1);
                tmpList.Add(-1);
            }
            FillExamples();
        }

        public void FillExamples()
        {
            for(int i = 0; i < toLoad; i++)
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

                            examples.Add(tmpList);

                            if (sketchGeo.FillContains(p))
                            {
                                examples[toLoad][t] = 1;
                            }
                            t++;
                        }
                    }
                    t = 0;
                }
                NextClick(nextButton, new RoutedEventArgs());
            }
        }

        public List<double> Weights(List<double> w)
        {
            for (int i = 0; i < 2500; i++)
            {
                w.Add(2.0 * randW.NextDouble() - 1.0);
            }
            return w;
        }
        private void TrainClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 2500; i++)
            {
                perceptron[i] = Training(i);
            }
            double sum = 0;
            for (int i = 0; i < 2500; i++)
            {
                for (int j = 1; j < 2500; j++)
                {
                    sum += perceptron[i][j] * imagePoints[j - 1];
                }
                StylusPoint p;
                List<Stroke> l = new List<Stroke>();
                StylusPointCollection points = new StylusPointCollection();
                imagePoints2[i] = 1;
                if (sum > perceptron[i][0])
                {
                    p = new StylusPoint(i % 50, i / 50);
                    points.Add(p);
                    Stroke s = new Stroke(points, attributes);
                    l.Add(s);
                    drawSpace2.Strokes.Add(s);
                }
            }
        }
        public List<double> Training(int perc)
        {
            List<double> w = new List<double>();
            w = Weights(w);
            List<double> pocket = new List<double>();
            pocket.AddRange(w);
            int drawn;
            int T;
            int O;
            int ERR;
            int lifespan = 0;
            int record = 0;
            int round = 0;

            while (round < maxRounds)
            {
                drawn = randE.Next(toLoad);
                double sum = 0;

                if (examples[drawn][perc] == perc)
                {
                    T = 1;
                }
                else
                {
                    T = -1;
                }

                for (int i = 1; i < 2500; i++)
                {
                    sum += w[i] * examples[drawn][i - 1];
                }

                if (sum < w[0])
                {
                    O = -1;
                }
                else
                {
                    O = 1;
                }

                ERR = T - O;
                if (ERR == 0)
                {

                    lifespan++;
                    if (lifespan > record)
                    {
                        record = lifespan;
                        pocket.Clear();
                        pocket.AddRange(w);
                    }
                }
                else
                {
                    for (int i = 1; i < 2500; i++)
                    {
                        w[i] += learn_const * ERR * examples[drawn][i - 1];
                    }
                    w[0] -= learn_const * ERR;
                    lifespan = 0;
                }
                round++;
            }
            perceptron.Add(pocket);

            return pocket;

            //for (int i = 0; i < 2500; i++)
            //{
            //    if (i % 50 == 0)
            //    {
            //        Debug.WriteLine("");
            //    }
            //    if (imagePoints[i] == 1)
            //    {
            //        Debug.Write("X");
            //    }
            //    else
            //    {
            //        Debug.Write("-");
            //    }
            //}

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
