using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Windows;

namespace Belikov_IA
{
    
    /// <summary>
    /// Логика взаимодействия для HotelManagmentSysWindow.xaml
    /// </summary>
    public partial class HotelManagmentSysWindow : Window
    {
        private int _currentUserId;
        // Конструктор по умолчанию
        public HotelManagmentSysWindow(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;

            GuestsFrame.Navigate(new GuestsPage());
            BookingsFrame.Navigate(new BookingPage());
            PhonesFrame.Navigate(new RoomPage());
        }
        
       
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
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
        private void toCleaningManage_click(object sender, RoutedEventArgs e)
        {
            // Получаем данные текущего пользователя из БД



            // Передаем ID и роль "менеджер" (даже если в БД указано иначе)
            CleaningSheduleWindow cleaning = new CleaningSheduleWindow(_currentUserId, "менеджер");
            cleaning.Owner = this;
            cleaning.Show();
            this.Hide();
        }
                    
                  
        
    }
}
