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

        public MainWindow()
        {
            InitializeComponent();
            for(int i = 1; i <= toLoad; i++)
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
            for(int i = 0; i < 2500; i++)
            {
                if(i % 50 == 0)
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
        private void MessClick(object sender, RoutedEventArgs e)
        {
            //drawSpace.Strokes
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
