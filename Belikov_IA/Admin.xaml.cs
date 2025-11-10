using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Belikov_IA
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        
       
        public Admin()
        {
            InitializeComponent();
            LoadUsersData();

        }
        public class UserDisplay
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Role { get; set; }
            public string Username { get; set; }
            public bool IsLocked { get; set; }
            public bool ISFirst { get; set; }
        }
        //Загрузка данных
        private void LoadUsersData()
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    // Получаем всех пользователей из базы данных
                    var users = db.Users.ToList();

                    // Создаем список для отображения
                    var userList = new List<UserDisplay>();

                    foreach (var user in users)
                    {
                        userList.Add(new UserDisplay
                        {
                            // Предполагая, что в вашей таблице Users есть такие поля
                            LastName = user.SecondName ?? "", // Если поле может быть null
                            FirstName = user.FirstName ?? "",
                            Role = user.role ?? "User",
                            Username = user.UserName ?? user.UserName ?? "", // зависит от структуры вашей БД
                            IsLocked = user.IsBlocked ?? false, // предполагая булево поле
                            ISFirst =  user.IsFirst ?? true
                        });
                    }

                    // Устанавливаем источник данных для DataGrid
                    UsersDataGrid.ItemsSource = userList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Кнопочка
        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    var updatedUsers = UsersDataGrid.ItemsSource as List<UserDisplay>;

                    if (updatedUsers != null)
                    {
                        foreach (var displayUser in updatedUsers)
                        {
                            var dbUser = db.Users.FirstOrDefault(u =>
                                u.UserName == displayUser.Username ||
                                u.UserName == displayUser.Username);

                            if (dbUser != null)
                            {
                                dbUser.SecondName = displayUser.LastName;
                                dbUser.FirstName = displayUser.FirstName;
                                dbUser.role = displayUser.Role;
                                dbUser.IsBlocked = displayUser.IsLocked;
                                dbUser.IsFirst = displayUser.ISFirst;
                            }
                        }

                        db.SaveChanges();
                        MessageBox.Show("Изменения успешно сохранены!", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);


                        LoadUsersData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Ещё кнопочка
        private void UnlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null)
            {
                var selectedUser = UsersDataGrid.SelectedItem as UserDisplay;
                selectedUser.IsLocked = false;
                UsersDataGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для разблокировки", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
       //И ещё кнопочка для перехода в AddUser
        private void Adduser_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно добавления пользователя
            AddUser addUserWindow = new AddUser();
            addUserWindow.Owner = this; // Устанавливаем владельца
            addUserWindow.ShowDialog(); // Открываем как модальное окно

            // Обновляем список после закрытия окна добавления
            if (addUserWindow.DialogResult == true)
            {
                LoadUsersData();
            }
        }
        //Обновление данных путем загрузки
        public void RefreshData()
        {
            LoadUsersData();
        }
        //Функция элементов таблицы
        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (UsersDataGrid.SelectedItem != null)
            {

                var selectedUser = UsersDataGrid.SelectedItem as UserDisplay;
                Console.WriteLine($"Выбран пользователь: {selectedUser.Username}, Первый вход: {selectedUser.ISFirst}");


            }


        }


        // РЕШИЛ ДОБАВИТЬ УДАЛЕНИЕ:
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null)
            {
                var selectedUser = UsersDataGrid.SelectedItem as UserDisplay;

                // Подтверждение удаления
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить пользователя {selectedUser.Username}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new Model1Entities())
                        {
                            var userToDelete = db.Users.FirstOrDefault(u => u.UserName == selectedUser.Username);

                            if (userToDelete != null)
                            {
                                db.Users.Remove(userToDelete);
                                db.SaveChanges();

                                MessageBox.Show($"Пользователь {selectedUser.Username} успешно удален!",
                                              "Успех",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Information);

                                // Обновляем список пользователей
                                LoadUsersData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления!",
                              "Информация",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
        }



    }
}
