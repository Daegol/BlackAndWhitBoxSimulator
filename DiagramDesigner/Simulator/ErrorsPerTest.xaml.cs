using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace DiagramDesigner.Simulator
{
    /// <summary>
    /// Logika interakcji dla klasy ErrorsPerTest.xaml
    /// </summary>
    public partial class ErrorsPerTest : UserControl
    {
        public ErrorsPerTest(int numberOfTests, IEnumerable<int> whitBoxErrors, IEnumerable<int> blackBoxErrors)
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BlackBox",
                    Values = new ChartValues<int> ( blackBoxErrors )
                },
                new LineSeries
                {
                    Title = "WhiteBox",
                    Values = new ChartValues<int>(whitBoxErrors),
                    PointGeometry = null
                },
            };

            YFormatter = value => value.ToString("C");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
