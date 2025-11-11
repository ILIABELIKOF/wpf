using System.Windows;
using System.Windows.Input;

namespace Belikov_IA
{
    public partial class ManagerLoginPage : Window
    {
        private const string CorrectPassword = "ManagerPassword!";

        public ManagerLoginPage()
        {
            InitializeComponent();
            ManagerPasswordBox.Focus();
        }
        //Кнопочка для запуска валидации
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            CheckPassword();
        }
        //Закрыть нафик
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
       
        //Элементарная валидация с переправой в окно менеджера
        private void CheckPassword()
        {
            string enteredPassword = ManagerPasswordBox.Password;

            if (enteredPassword == CorrectPassword)
            {
                // Пароль верный - открываем систему управления
                HotelManagmentSysWindow hotelSystem = new HotelManagmentSysWindow();
                hotelSystem.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный пароль владельца!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                ManagerPasswordBox.Password = "";
                ManagerPasswordBox.Focus();
            }
        }
    }
}