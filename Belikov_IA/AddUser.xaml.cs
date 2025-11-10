using System;
using System.Windows;

namespace Belikov_IA
{
    public partial class AddUser : Window
    {
        

        public AddUser()
        {
            InitializeComponent();
            
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddUserToDatabase();

                MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddUserToDatabase()
        {
            using (var db = new Model1Entities())
            {
                // Создаем нового пользователя
                var newUser = new Users
                {
                    // Обязательные поля
                    FirstName = txtFirstName.Text.Trim(),
                    SecondName = txtLastName.Text.Trim(),
                    UserName = txtUserName.Text.Trim(),
                    Passwor = txtPassword.Text.Trim(),
                    role = (cmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString(),

                    // Необязательные поля с проверкой на null
                    IsFirst= true,

                   
                };

              

                // Добавляем и сохраняем
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtUserName.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
         
        }
        






    }
}
