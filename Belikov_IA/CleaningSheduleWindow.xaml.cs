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
    

    public partial class CleaningSheduleWindow : Window
    {
        private int _currentUserId;
        private string _currentUserRole;
        private bool _isManagerOrAdmin;

        // Список вариантов статуса (как в AddUser для ролей)
        public List<string> StatusOptions { get; set; } = new List<string>
        {
            "InProcess",
            "Complete"
        };

        public CleaningSheduleWindow(int userId, string userRole)
        {
            InitializeComponent();

            // Устанавливаем DataContext для доступа к StatusOptions из XAML
            this.DataContext = this;

            _currentUserId = userId;
            _currentUserRole = userRole?.ToLower() ?? "";
            _isManagerOrAdmin = (_currentUserRole == "менеджер" || _currentUserRole == "администратор");

            // Полноэкранный режим для лучшего отображения
            this.WindowState = WindowState.Maximized;

            InitializeUI();
            LoadCleaningData();
        }

        private void InitializeUI()
        {
            if (_isManagerOrAdmin)
            {
                this.Title = "Общий график уборки (Менеджер)";
                Cleaning.IsReadOnly = false; // Разрешаем редактирование
            }
            else
            {
                this.Title = "Мой график уборки";
                Cleaning.IsReadOnly = true;
                DisableEmployeeButtons();
            }
        }

        // Отключаем кнопки для сотрудника
        private void DisableEmployeeButtons()
        {
            if (btnAdd != null)
            {
                btnAdd.IsEnabled = false;
                btnAdd.Content = "ДОБАВИТЬ (нет прав)";
            }

            if (btnDelete != null)
            {
                btnDelete.IsEnabled = false;
                btnDelete.Content = "УДАЛИТЬ (нет прав)";
            }

            if (btnSaveAll != null)
            {
                btnSaveAll.IsEnabled = false;
                btnSaveAll.Content = "СОХРАНИТЬ ВСЁ (нет прав)";
            }
        }

        // Класс для отображения данных
        public class CleaningDisplay
        {
            public int cleaning_id { get; set; }
            public DateTime cleaningdate { get; set; }
            public int room_id { get; set; }
            public string status { get; set; } // "InProcess" или "Complete"
            public int? user_id { get; set; }
        }

        // Метод загрузки данных
        private void LoadCleaningData()
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    IQueryable<Cleaning_Rooms> query;

                    if (_isManagerOrAdmin)
                    {
                        query = db.Cleaning_Rooms;
                    }
                    else
                    {
                        query = db.Cleaning_Rooms.Where(c => c.employee_id == _currentUserId);
                    }

                    var cleaningData = query.ToList();
                    var cleaningList = new List<CleaningDisplay>();

                    foreach (var item in cleaningData)
                    {
                        // Преобразуем статусы из БД в наши значения
                        string status = "InProcess";
                        if (item.cleaningStatus == "Complete" || item.cleaningStatus == "Выполнено")
                            status = "Complete";
                        else if (item.cleaningStatus == "InProcess" || item.cleaningStatus == "В процессе")
                            status = "InProcess";

                        cleaningList.Add(new CleaningDisplay
                        {
                            cleaning_id = item.id,
                            cleaningdate = item.cleaningTime ?? DateTime.Now,
                            room_id = item.room_id ?? 0,
                            status = status,
                            user_id = item.employee_id
                        });
                    }

                    Cleaning.ItemsSource = cleaningList;
                    UpdateRecordsCount(cleaningList.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
            }
        }

        // КНОПКА ДОБАВЛЕНИЯ (с автосохранением)
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!_isManagerOrAdmin)
            {
                MessageBox.Show("Недостаточно прав для добавления записей!", "Ошибка прав");
                return;
            }

            // Открываем окно добавления записи
            AddCleaningRecord addWindow = new AddCleaningRecord();
            addWindow.Owner = this;
            addWindow.ShowDialog();

            // Если запись успешно добавлена, обновляем данные
            if (addWindow.IsSuccess)
            {
                LoadCleaningData();
            }
        }

        // КНОПКА УДАЛЕНИЯ (с автосохранением)
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!_isManagerOrAdmin)
            {
                MessageBox.Show("Недостаточно прав для удаления записей!", "Ошибка прав");
                return;
            }

            if (Cleaning.SelectedItem != null)
            {
                var selectedCleaning = Cleaning.SelectedItem as CleaningDisplay;

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить запись уборки (ID: {selectedCleaning.cleaning_id})?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new Model1Entities())
                        {
                            var cleaningToDelete = db.Cleaning_Rooms.FirstOrDefault(c => c.id == selectedCleaning.cleaning_id);

                            if (cleaningToDelete != null)
                            {
                                db.Cleaning_Rooms.Remove(cleaningToDelete);
                                db.SaveChanges(); // АВТОСОХРАНЕНИЕ

                                MessageBox.Show("Запись уборки успешно удалена!", "Успех");
                                LoadCleaningData(); // Обновляем данные
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления!", "Информация");
            }
        }

        // КНОПКА СОХРАНЕНИЯ ВСЕХ ИЗМЕНЕНИЙ
        private void BtnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (!_isManagerOrAdmin)
            {
                MessageBox.Show("Недостаточно прав для сохранения изменений!", "Ошибка прав");
                return;
            }

            SaveAllChanges();
        }

        // МЕТОД ДЛЯ СОХРАНЕНИЯ ВСЕХ ИЗМЕНЕНИЙ
        private void SaveAllChanges()
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    var updatedCleanings = Cleaning.ItemsSource as List<CleaningDisplay>;

                    if (updatedCleanings != null)
                    {
                        foreach (var displayCleaning in updatedCleanings)
                        {
                            var dbCleaning = db.Cleaning_Rooms.FirstOrDefault(c => c.id == displayCleaning.cleaning_id);

                            if (dbCleaning != null)
                            {
                                // Обновляем данные
                                dbCleaning.cleaningTime = displayCleaning.cleaningdate;
                                dbCleaning.room_id = displayCleaning.room_id;
                                dbCleaning.cleaningStatus = displayCleaning.status;
                            }
                        }

                        db.SaveChanges();
                        MessageBox.Show("Все изменения успешно сохранены!", "Успех");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка");
            }
        }

        // Автосохранение при изменении данных в DataGrid
        private void Cleaning_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_isManagerOrAdmin && e.EditAction == DataGridEditAction.Commit)
            {
                // Автоматически сохраняем при завершении редактирования ячейки
                SaveAllChanges();
            }
        }

        private void UpdateRecordsCount(int count)
        {
            if (_isManagerOrAdmin)
            {
                RecordsCount.Text = $"{count} (Все записи)";
            }
            else
            {
                RecordsCount.Text = $"{count} (Мои записи)";
            }
        }

        private void Cleaning_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика выбора строки
        }

        private void BtnCancel_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
