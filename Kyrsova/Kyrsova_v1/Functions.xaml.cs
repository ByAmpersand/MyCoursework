﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for Functions.xaml
    /// </summary>
    public partial class Functions : Window
    {
        List<Expenses> tempList = new List<Expenses>();

        public Functions(List<Expenses> mainList)
        {
            InitializeComponent();
            tempList = mainList;
            additionalListView.ItemsSource = null;
            additionalListView.ItemsSource = tempList;
            
        }
    }
}
