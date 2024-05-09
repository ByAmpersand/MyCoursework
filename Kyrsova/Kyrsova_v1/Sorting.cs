using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyrsova_v1
{
    public class Sorting
    {
        // Метод QuickSort для нашого класу Expenses
        public static List<Expenses> QuickSort(List<Expenses> expenses, string sortBy)
        {
            if (expenses.Count <= 1)
                return expenses;

            int pivotIndex = expenses.Count / 2;
            Expenses pivotExpense = expenses[pivotIndex];

            List<Expenses> left = new List<Expenses>();
            List<Expenses> right = new List<Expenses>();

            for (int i = 0; i < expenses.Count; i++)
            {
                if (i == pivotIndex)
                    continue;

                switch (sortBy)
                {
                    case "Date":
                        if (!string.IsNullOrEmpty(expenses[i].Date) && !string.IsNullOrEmpty(pivotExpense.Date))
                        {
                            DateTime expenseDate, pivotDate;

                            if (DateTime.TryParse(expenses[i].Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out expenseDate) &&
                                DateTime.TryParse(pivotExpense.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out pivotDate))
                            {
                                if (expenseDate < pivotDate)
                                {
                                    left.Add(expenses[i]);
                                }
                                else
                                {
                                    right.Add(expenses[i]);
                                }
                            }
                        }
                        break;

                    case "ExpenseType":
                        if (expenses[i].ExpenseType.CompareTo(pivotExpense.ExpenseType) < 0)
                            left.Add(expenses[i]);
                        else
                            right.Add(expenses[i]);
                        break;

                    case "ExpenseSubtype":
                        if (expenses[i].ExpenseSubtype.CompareTo(pivotExpense.ExpenseSubtype) < 0)
                            left.Add(expenses[i]);
                        else
                            right.Add(expenses[i]);
                        break;

                    case "CurrencyExpense":
                        if (string.Compare(expenses[i].CurrencyExpense, pivotExpense.CurrencyExpense) == -1)
                        {

                            left.Add(expenses[i]);
                        }
                        else
                            right.Add(expenses[i]);
                        break;

                    case "ExpenseAmount":
                        if (expenses[i].CurrencyExpense == "Hryvnia" && pivotExpense.CurrencyExpense == "Hryvnia")
                        {
                            if (expenses[i].ExpenseAmount < pivotExpense.ExpenseAmount)
                            {
                                left.Add(expenses[i]);
                            }
                            else
                            {
                                right.Add(expenses[i]);
                            }
                        }
                        else
                        {
                            double convertedExpenseAmount1 = expenses[i].ExpenseAmount * expenses[i].ExchangeRate;
                            double convertedExpenseAmount2 = pivotExpense.ExpenseAmount * pivotExpense.ExchangeRate;

                            if (convertedExpenseAmount1 < convertedExpenseAmount2)
                            {
                                left.Add(expenses[i]);
                            }
                            else
                            {
                                right.Add(expenses[i]);
                            }
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid sortBy parameter");
                }
            }

            List<Expenses> sortedExpenses = new List<Expenses>();
            sortedExpenses.AddRange(QuickSort(left, sortBy));
            sortedExpenses.Add(pivotExpense);
            sortedExpenses.AddRange(QuickSort(right, sortBy));

            return sortedExpenses;
        }
    }
}