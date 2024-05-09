using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kyrsova_v1
{
    public class Expenses
    {
        public int Number { get; set; }
        public string Date { get; set; }
        public TimeSpan Time { get; set; }
        public string ExpenseType { get; set; }
        public string ExpenseSubtype { get; set; }
        public double ExpenseAmount { get; set; }
        public string CurrencyExpense { get; set; }
        public double ExchangeRate { get; set; }

        public Expenses() // конструктор без параметрів 
        {
        }

        // конструктор з параметрами
        public Expenses(int number, string date, TimeSpan time, string expenseType, string expenseSubtype, double expenseAmount, string currencyExpense, double exchangeRate)
        {
            Number = number;
            Date = date;
            Time = time;
            ExpenseType = expenseType;
            ExpenseSubtype = expenseSubtype;
            ExpenseAmount = expenseAmount;
            CurrencyExpense = currencyExpense;
            ExchangeRate = exchangeRate;
        }

        // конструктор копіювання
        public Expenses(Expenses otherExpense)
        {
            Number = otherExpense.Number;
            Date = otherExpense.Date;
            Time = otherExpense.Time;
            ExpenseType = otherExpense.ExpenseType;
            ExpenseSubtype = otherExpense.ExpenseSubtype;
            ExpenseAmount = otherExpense.ExpenseAmount;
            CurrencyExpense = otherExpense.CurrencyExpense;
            ExchangeRate = otherExpense.ExchangeRate;
        }

        public override string ToString() // перевантаження методу для виводу витрат у якості стрічки
        {
            return $"{Date};{Time};{ExpenseType};{ExpenseSubtype};{ExpenseAmount};{CurrencyExpense};{ExchangeRate}";
        }

        // метод зчитування списку з файлу
        public static List<Expenses> getListFromFile(OpenFileDialog openFileDialogFileName, List<Expenses> m_Expenses)
        {
            List<Expenses> expensesToAdd = new List<Expenses>();
            string[] input = File.ReadAllLines(openFileDialogFileName.FileName);
            int line = 0;
            int counter = 1; 

            foreach (string lineData in input)
            {
                string[] inputParts = lineData.Split(';');

                if (inputParts.Length < 7)
                {
                    throw new FileReadException(line);
                }

                if (!string.IsNullOrWhiteSpace(inputParts[0]) &&
                    TimeSpan.TryParse(inputParts[1], out TimeSpan time) &&
                    !string.IsNullOrWhiteSpace(inputParts[2]) &&
                    !string.IsNullOrWhiteSpace(inputParts[3]) &&
                    float.TryParse(inputParts[4], out float expenseAmount) &&
                    !string.IsNullOrWhiteSpace(inputParts[5]) &&
                    float.TryParse(inputParts[6], out float exchangeRate))
                {

                    string date = inputParts[0];
                    string expenseType = inputParts[2];
                    string expenseSubtype = inputParts[3];
                    string currencyExpense = inputParts[5];

                    Expenses expense = new Expenses
                    {
                        Number = m_Expenses.Count + expensesToAdd.Count + 1,
                        Date = date,
                        Time = time,
                        ExpenseType = expenseType,
                        ExpenseSubtype = expenseSubtype,
                        ExpenseAmount = expenseAmount,
                        CurrencyExpense = currencyExpense,
                        ExchangeRate = exchangeRate
                    };

                    expensesToAdd.Add(expense);
                    counter++; 
                }
                else
                {
                    throw new FileReadException(line);
                }
                line++;
            }
            return expensesToAdd;
        }

        // метод вивантаження списку у файл
        public static void setListToFile(SaveFileDialog saveFileDialogFileName, List<Expenses> expenses)  {
            using (StreamWriter writer = new StreamWriter(saveFileDialogFileName.FileName))
            {
                foreach (var i in expenses)
                {
                    writer.WriteLine(i.ToString());
                }
            }
        }

        //Вивести 3 дати, що мають найбільші витрати в доларах США
        public static List<Expenses> get3DatesWithTheHighestPrice(List<Expenses> mainList)
        {
            if (mainList == null)
            {
                throw new EmptyListException();
            }

            if (mainList.Count < 3)
            {
                throw new WrongArgumentsException();
            }

            List<Expenses> tempList = new List<Expenses>(mainList);
            List<Expenses> expensesWithTheHighestPrice = new List<Expenses>();

            for (int i = 0; (i < 3) && (tempList.Count > 0); i++)
            {
                Expenses highestPriceExpense = null;

                foreach (var expense in tempList)
                {
                    if (expense.CurrencyExpense.ToLower().Contains("dollar"))
                    {
                        highestPriceExpense = expense;
                        break;
                    }
                }

                if (highestPriceExpense == null)
                {
                    throw new NoFindingsException();
                }

                foreach (var checker in tempList)
                {
                    if (checker.CurrencyExpense.ToLower().Contains("dollar") && checker.ExpenseAmount > highestPriceExpense.ExpenseAmount)
                    {
                        highestPriceExpense = checker;
                    }
                }

                expensesWithTheHighestPrice.Add(highestPriceExpense);
                tempList.Remove(highestPriceExpense);
            }

            return expensesWithTheHighestPrice;
        }

        // Поділити добу на проміжки по 6 годин і визначити в які проміжки найбільше значних витрат, дрібних витрат, витрат всього.
        public static string AnalysisOfPurchaseStatistics(List<Expenses> mainList, string date)
        {
            try
            {
                double significantPurchaseThreshold = 5000;

                bool purchasesExist = mainList.Any(p => p.Date == date);
                if (!purchasesExist)
                {
                    throw new NoFindingsException();
                }

                var intervals = new List<(TimeSpan start, TimeSpan end)>
                {
                (TimeSpan.FromHours(0), TimeSpan.FromHours(6)),
                (TimeSpan.FromHours(6), TimeSpan.FromHours(12)),
                (TimeSpan.FromHours(12), TimeSpan.FromHours(18)),
                (TimeSpan.FromHours(18), TimeSpan.FromHours(24))
                };

                var intervalResults = intervals.Select(interval => new
                {
                    Interval = interval,
                    SignificantPurchases = mainList.Count(p => p.Date == date &&
                                                              p.Time >= interval.start &&
                                                              p.Time < interval.end &&
                                                              p.ExpenseAmount >= (p.CurrencyExpense == "Hryvnia" ? significantPurchaseThreshold : significantPurchaseThreshold / p.ExchangeRate)),

                    MinorPurchases = mainList.Count(p => p.Date == date &&
                                                        p.Time >= interval.start &&
                                                        p.Time < interval.end &&
                                                        p.ExpenseAmount < (p.CurrencyExpense == "Hryvnia" ? significantPurchaseThreshold : significantPurchaseThreshold / p.ExchangeRate)),

                    TotalPurchases = mainList.Count(p => p.Date == date &&
                                                        p.Time >= interval.start &&
                                                        p.Time < interval.end)
                }).ToList();

                if (!intervalResults.Any())
                {
                    throw new NoFindingsException();
                }

                string result = "";

                foreach (var intervalResult in intervalResults)
                {
                    string formattedStart = intervalResult.Interval.start.ToString(@"hh\:mm\:ss");
                    string formattedEnd = intervalResult.Interval.end == TimeSpan.FromHours(24) ? "00:00:00" : intervalResult.Interval.end.ToString(@"hh\:mm\:ss");

                    result += $"Від {formattedStart} до {formattedEnd}: " +
                              $"\nЗначні покупки: {intervalResult.SignificantPurchases} " +
                              $"\nДрібні покупки: {intervalResult.MinorPurchases} " +
                              $"\nВсього покупок: {intervalResult.TotalPurchases}\n";
                }

                return result;
            }
            catch (Exception ex)
            {
                return $"Виникла помилка під час аналізу статистики: {ex.Message}";
            }
        }
        //Визначити скільки разів на місяць здійснюється в середньому витрата «харчування» розміром менше 20 грн.
        public static string GetFoodExpenseStatisticsUnder20UAH(List<Expenses> mainList)
        {
            var tempList = mainList;
            double priceThreshold = 20;
            string productTypeCriteria = "food";
            List<int> tempInt = new List<int>();

            List<DateTime> tempDate = new List<DateTime>();

            foreach (var a in tempList)
            {
                tempDate.Add(DateTime.ParseExact(a.Date, "MM/dd/yyyy", null));
            }

            DateTime maxDate = tempDate.Max();
            DateTime minDate = tempDate.Min();


            var filteredExpenses = tempList
            .Where(expense =>
            expense.ExpenseAmount <= (expense.CurrencyExpense == "Hryvnia" ? priceThreshold : priceThreshold / expense.ExchangeRate) &&
            expense.ExpenseType.ToLower().Contains(productTypeCriteria.ToLower()))
            .ToList();

            TimeSpan timeDifference = maxDate - minDate;
            int monthsDifference = (int)(timeDifference.TotalDays / 30.44);

            return "Кількість задовільних запитів: " + filteredExpenses.Count.ToString() + ", \nКількість місяців: " + monthsDifference.ToString();
        }

        // Вивести тип і підтип 5 найдрібніших витрат
        public static List<Expenses> getTypesOfFiveSmallestExpenses(List<Expenses> mainList)
        {
            if (mainList == null)
            {
                throw new EmptyListException();
            }

            if (mainList.Count < 5)
            {
                throw new WrongArgumentsException();
            }

            List<Expenses> tempList = new List<Expenses>(mainList);
            List<Expenses> smallestExpenses = new List<Expenses>();

            for (int i = 0; i < 5 && tempList.Count > 0; i++)
            {
                Expenses smallestExpense = null;

                foreach (var expense in tempList)
                {
                    if (smallestExpense == null || CalculateExpenseInHryvnias(expense) < CalculateExpenseInHryvnias(smallestExpense))
                    {
                        smallestExpense = expense;
                    }
                }

                if (smallestExpense == null)
                {
                    throw new NoFindingsException();
                }

                smallestExpenses.Add(smallestExpense);
                tempList.Remove(smallestExpense);
            }

            return smallestExpenses;
        }
        public static double CalculateExpenseInHryvnias(Expenses expense)
        {
            if (expense.CurrencyExpense.ToLower().Contains("hryvnia"))
            {
                return expense.ExpenseAmount;
            }
            else
            {
                return expense.ExpenseAmount * expense.ExchangeRate;
            }
        }
        //Вивести витрати у дні коли змінювався курс валют.
        public static List<Expenses> GetPurchasesOnCurrencyChangeDays(List<Expenses> expensesList)
        {         
            List<Expenses> daysWithExchangeRateChange = new List<Expenses>();
            if (expensesList == null)
            {
                throw new EmptyListException();
            }
            Dictionary<string, double> lastExchangeRates = new Dictionary<string, double>();
            expensesList.Sort((x, y) => DateTime.ParseExact(x.Date, "MM/dd/yyyy", null).CompareTo(DateTime.ParseExact(y.Date, "MM/dd/yyyy", null)));
            for (int i = 1; i < expensesList.Count; i++)
            {
                string currency = expensesList[i].CurrencyExpense; 
                if (!lastExchangeRates.ContainsKey(currency))
                {
                    lastExchangeRates[currency] = expensesList[i - 1].ExchangeRate;
                }

                if (lastExchangeRates[currency] != expensesList[i].ExchangeRate)
                {
                    daysWithExchangeRateChange.Add(expensesList[i]);
                }

                
                lastExchangeRates[currency] = expensesList[i].ExchangeRate;
            }
            return daysWithExchangeRateChange;
        }
    }
}