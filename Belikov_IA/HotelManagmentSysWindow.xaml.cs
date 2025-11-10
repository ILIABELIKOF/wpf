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

namespace Belikov_IA
{
    /// <summary>
    /// Логика взаимодействия для HotelManagmentSysWindow.xaml
    /// </summary>
    public partial class HotelManagmentSysWindow : Window
    {
        public HotelManagmentSysWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CleaningManagementFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.Close();
        }

        private void ToAdmin(object sender, RoutedEventArgs e)
        {
            Admin Admin = new Admin();
            Admin.Owner = this; 
            Admin.Show();
            this.Hide();
        }
    }
}
