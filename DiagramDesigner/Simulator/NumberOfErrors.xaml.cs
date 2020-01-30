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
    /// Logika interakcji dla klasy NumberOfErrors.xaml
    /// </summary>
    public partial class NumberOfErrors : UserControl
    {
        public NumberOfErrors(int blackBoxErrors,int whiteBoxErrors)
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "",
                    Values = new ChartValues<double> { blackBoxErrors, whiteBoxErrors }
                }
            };

            Labels = new[] { "Czarna skrzynka", "Biała skrzynka" };
            Formatter = value => value.ToString("N");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
    }
}
