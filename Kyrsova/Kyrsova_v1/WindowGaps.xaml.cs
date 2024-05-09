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
using System.Windows.Shapes;

namespace Kyrsova_v1
{
    /// <summary>
    /// Interaction logic for WindowGaps.xaml
    /// </summary>
    public partial class WindowGaps : Window
    {
        List<Expenses> tempList = new List<Expenses>();

        public WindowGaps(List<Expenses> mainList)
        {
            tempList = mainList;
            InitializeComponent();
        }

        private void GapsCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string temp = Expenses.AnalysisOfPurchaseStatistics(tempList, GapsDateTextBox.Text);
                OutputLabel.Content = temp;
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Error");
            }
        }
    }
}
