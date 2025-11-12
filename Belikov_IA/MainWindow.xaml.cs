using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Belikov_IA
{
    
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
       
        

        private async void LoginButton_Click(object sender, RoutedEventArgs e) { 
        
        
            string username = txtUsername.Text.Trim();
            string passwod = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwod))
            {
                MessageBox.Show("Введите логин  и пароль!");
                return;
            }

            using (var context = new Model1Entities())
            {


                var user = await context.Users
                    .Where(u => u.UserName == username)
                    .FirstOrDefaultAsync();
                if (user == null) {

                    MessageBox.Show("Неверный логин или пароль", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsBlocked.HasValue && user.IsBlocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь к админу ", "Acces is locked", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                

                if (user.lastLogin.HasValue && (DateTime.Now - user.lastLogin.Value).TotalDays > 30 && user.role != "Admin") 
                {
                    user.IsBlocked = true;
                    await context.SaveChangesAsync();
                    MessageBox.Show(" БАН", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }

                if (user.Passwor == passwod)
                {

                    user.lastLogin = DateTime.Now;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы авторизованы ", "Welcome!", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                    if (user.IsFirst==true)
                    {
                        ChangePasw changePasw = new ChangePasw(user.id,  user.Passwor);
                        changePasw.Owner = this;
                        changePasw.ShowDialog();
                        this.Hide();
                    }
                    else
                    {
                        if (user.role == "администратор")
                        {
                            Admin Admin = new Admin();
                            Admin.Show();
                        }
                        else if (user.role == "сотрудник")
                        {
                            // Передаем ID и роль "менеджер" (даже если в БД указано иначе)
                            CleaningSheduleWindow cleaning = new CleaningSheduleWindow(user.id, "сотрудник");
                            cleaning.Owner = this;
                            cleaning.Show();
                            
                        }
                        else if (user.role == "менеджер") {

                            HotelManagmentSysWindow Manager = new HotelManagmentSysWindow(user.id);
                            Manager.Owner = this;
                            Manager.Show();
                            

                        }
                        
                    }
                }
                else
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts == 3)
                    {
                        user.IsBlocked = true;
                        MessageBox.Show("Вы заблокированы, обратитесь к админу ", "Acces is locked", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        int AttemptLeft = 3 - (user.FailedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный логин и пароль. Осталось попыток:{AttemptLeft}.", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    await context.SaveChangesAsync();                
                }
                


                


            
            }
            
            
         }



    }
}
