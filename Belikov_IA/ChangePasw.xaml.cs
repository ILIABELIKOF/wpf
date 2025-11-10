using System;
using System.Linq;
using System.Windows;

namespace Belikov_IA
{
    public partial class ChangePasw : Window
    {
        private readonly int _userId;
        private string currentUsername; // текущий пользователь
        private string currentPassword; // текущий пароль
        

        // Конструктор по умолчанию для XAML, если нужно открыть через конфиг
        public ChangePasw()
        {
            InitializeComponent();
        }
        
        //Основной конструктор
        public ChangePasw(int userid, string OldPassword)
        {
            InitializeComponent();
            _userId = userid;
            

            try
            {
                using (var db = new Model1Entities())
                {
                    var user = db.Users.FirstOrDefault(u => u.id == _userId);
                    if (user != null)
                    {
                        currentPassword = user.Passwor; // Сохраняем текущий пароль
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}");
            }

            MessageBox.Show("Пожалуйста введите старый пароль и новый пароль для подтверждения.");



        }

 //Метод для создания валидации, проверки корректности  и зполнения  информации
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            string OPassword = txtOldPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtAcceptNewPassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(OPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }
            //Проверка старый пароль
            if (OPassword != currentPassword)
            {
               
                MessageBox.Show("Неверный старый пароль!");
                return;
            }
            
            // Проверка что новый пароль прошел валидацию
            if (newPassword.Length < 6 || confirmPassword.Length<6)
            {
                MessageBox.Show("Новый пароль не соответствует требованиям безопасности!");
                MessageBox.Show("Пароль слишком короткий (минимум 6 символов)");
                return;
            }
           
            // Проверка совпадения нового пароля и подтверждения
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Новые пароли не совпадают!");
                return;
            }
            MessageBox.Show("Пароль соответствует требованиям безопасности");
           

            SaveNewPassword(newPassword);
            ClearPasswordFields();
            this.Hide();
        }

//Метод для процесса сохранения нового пароля
        private void SaveNewPassword(string newPassword)
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    // Находим пользователя в базе данных
                    var user = db.Users.FirstOrDefault(u => u.id==_userId);

                    if (user != null)
                    {
                        // Сохраняем новый пароль (без хэширования)
                        user.Passwor = newPassword;
                        user.IsFirst = false;
                        db.SaveChanges();
                        

                        // Логирование успешного изменения
                        MessageBox.Show($"Пароль для пользователя {currentUsername} был автоматически обновлен");
                    


                    // Проверяем роль пользователя и открываем окно Admin если роль "администратор"
                    if (user.role != null && user.role.ToLower() == "администратор")
                    {
                        // Закрываем текущее окно смены пароля
                        

                        // Открываем окно администратора
                        Admin adminWindow = new Admin();
                        adminWindow.Show();
                            this.Hide();
                    }
                    else
                    {
                        
                        MessageBox.Show("Пароль успешно изменен!");
                            RoomPage roomPage = new RoomPage();
                            roomPage.ShowsNavigationUI = true;
                            this.Hide();
                    }
                    }
                    
                    else
                    {
                        MessageBox.Show("Ошибка: пользователь не найден в базе данных");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при автоматическом сохранении пароля: {ex.Message}");
            }
        }


        //Просто очистка полей
        private void ClearPasswordFields()
        {
            try
            {
                // Очищаем все поля паролей
                txtOldPassword.Password = "";
                txtNewPassword.Password = "";
                txtAcceptNewPassword.Password = "";

                // Сбрасываем фокус на первое поле
                txtOldPassword.Focus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при очистке полей: {ex.Message}");
            }
        }
    }
}