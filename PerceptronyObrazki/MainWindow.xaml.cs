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
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void SavePointsClick(object sender, RoutedEventArgs e)
        {
            List<Point> pointsInside = new List<Point>();
            foreach (var stroke in drawSpace.Strokes)
            {
                Geometry sketchGeo = stroke.GetGeometry();
                //Rect strokeBounds = sketchGeo.Bounds;

                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 50; y++)
                    {
                        Point p = new Point(x, y);

                        if (sketchGeo.FillContains(p))
                        {
                            pointsInside.Add(p);
                        }
                    }
                }
            }
            var fs = new FileStream("obrazek", FileMode.Create);
            drawSpace.Strokes.Save(fs);
            fs.Close();
        }

        private void LoadPointsClick(object sender, RoutedEventArgs e)
        {
            var fs = new FileStream("obrazek", FileMode.Open, FileAccess.Read);
            StrokeCollection strokes = new StrokeCollection(fs);
            drawSpace.Strokes = strokes;
            fs.Close();
        }       

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            drawSpace.Strokes.Clear();

        }
    }
}
