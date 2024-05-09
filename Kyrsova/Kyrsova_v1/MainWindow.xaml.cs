using Microsoft.Win32;
using System;
using System.IO;
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
using System.Runtime.ConstrainedExecution;
using System.Windows.Shell;

namespace Kyrsova_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
       
        List<Expenses> m_ExpensesList = new List<Expenses>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList.Clear();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Stream MyStream;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                MyStream = openFileDialog.OpenFile();
                if (MyStream.CanRead)
                {
                    MyStream.Close();
                }

                List<Expenses> expensesToAdd = Expenses.getListFromFile(openFileDialog, m_ExpensesList);
                foreach (var i in expensesToAdd)
                {
                    m_ExpensesList.Add(i);
                }
                expensesListView.ItemsSource = null;
                expensesListView.ItemsSource = m_ExpensesList;
            }
            else
            {
                return;
            }
        }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            Stream MyStream;
            SaveFileDialog saveFileDialogFileName = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt", // формат файлу, у який дозволяє зробити вивід
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (saveFileDialogFileName.ShowDialog() == true)
            {
                MyStream = saveFileDialogFileName.OpenFile();
                if (MyStream.CanRead)
                {
                    MyStream.Close();
                }
            }
            else
            {
                return;
            }
            Expenses.setListToFile(saveFileDialogFileName, m_ExpensesList);
        }

        private void SortingByDate_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList = Sorting.QuickSort(m_ExpensesList, "Date");
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;
        }

        private void SortingByCurrency_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList = Sorting.QuickSort(m_ExpensesList, "CurrencyExpense");
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;
        }

        private void SortingByType_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList = Sorting.QuickSort(m_ExpensesList, "ExpenseType");
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;
        }

        private void SortingBySubtype_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList = Sorting.QuickSort(m_ExpensesList, "ExpenseSubtype");
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;
        }

        private void SortingByAmount_Click(object sender, RoutedEventArgs e)
        {
            m_ExpensesList = Sorting.QuickSort(m_ExpensesList, "ExpenseAmount");
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string dateType;
            bool check = TimeSpan.TryParse(TimeTextBox.Text, out TimeSpan time);
            string expenseType;
            string expenseSubtype;
            double expenseAmount;
            string currency;
            double exchangeRate;

            if (!check)
            {
                MessageBox.Show("Error while adding new element", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                dateType = DateTextBox.Text;    
                expenseType = TypeTextBox.Text;
                expenseSubtype = SubtypeTextBox.Text;
                expenseAmount = double.Parse(AmountTextBox.Text);
                currency = CurrencyTextBox.Text;
                exchangeRate = double.Parse(RateTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error while adding new element", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Expenses newExpense = new Expenses
            {
                Number = m_ExpensesList.Count + 1,
                Date = dateType,
                Time = time,
                ExpenseType = expenseType,
                ExpenseSubtype = expenseSubtype,
                ExpenseAmount = expenseAmount,
                CurrencyExpense = currency,
                ExchangeRate = exchangeRate
            };

            m_ExpensesList.Add(newExpense);
            expensesListView.ItemsSource = null;
            expensesListView.ItemsSource = m_ExpensesList;

            DateTextBox.Clear();
            TimeTextBox.Clear();
            TypeTextBox.Clear();
            SubtypeTextBox.Clear();
            AmountTextBox.Clear();
            CurrencyTextBox.Clear();
            RateTextBox.Clear();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (expensesListView.SelectedItem != null && expensesListView.SelectedItem is Expenses selectedExpense)
            {
                var selectedExpenses = expensesListView.SelectedItems.Cast<Expenses>().ToList();
                foreach (var i in selectedExpenses)
                {
                    m_ExpensesList.Remove(i);
                }
                expensesListView.ItemsSource = null;
                expensesListView.ItemsSource = m_ExpensesList;
            }
        }

        private void HighestExpenses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempList = Expenses.get3DatesWithTheHighestPrice(m_ExpensesList);
                Functions newWindow = new Functions(tempList);
                newWindow.Show();
            }
            catch (NoFindingsException)
            {            
                MessageBox.Show("No findings were found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {            
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SmallestExpenses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempList = Expenses.getTypesOfFiveSmallestExpenses(m_ExpensesList);
                Functions newWindow = new Functions(tempList);
                newWindow.Show();
            }
            catch (NoFindingsException)
            {                
                MessageBox.Show("No findings were found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {                
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GapsExpenses_Click(object sender, RoutedEventArgs e)
        {
             WindowGaps newWindow = new WindowGaps(m_ExpensesList);
             newWindow.Show();
        }

        private void FoodExpenses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = Expenses.GetFoodExpenseStatisticsUnder20UAH(m_ExpensesList).ToString();
                MessageBox.Show(result);
            }
            catch (NoFindingsException)
            {
                MessageBox.Show("No findings were found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RateChanged_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempList = Expenses.GetPurchasesOnCurrencyChangeDays(m_ExpensesList);
                Functions newWindow = new Functions(tempList);
                newWindow.Show();
            }
            catch (NoFindingsException)
            {
                MessageBox.Show("No findings were found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
    }
}