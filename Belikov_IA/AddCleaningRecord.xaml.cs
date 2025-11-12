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
    /// Логика взаимодействия для AddCleaningRecord.xaml
    /// </summary>
    public partial class AddCleaningRecord : Window
    {
        public bool IsSuccess { get; private set; } = false;

        public AddCleaningRecord()
        {
            InitializeComponent();
            dpCleaningDate.SelectedDate = DateTime.Now; // Устанавливаем текущую дату по умолчанию
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка введенных данных
                if (!int.TryParse(txtRoomId.Text, out int roomId) || roomId <= 0)
                {
                    MessageBox.Show("Введите корректный ID комнаты!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dpCleaningDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату уборки!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем выбранный статус
                string status = (cmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

                // Добавляем запись в базу данных
                using (var db = new Model1Entities())
                {
                    var newCleaning = new Cleaning_Rooms
                    {
                        cleaningTime = dpCleaningDate.SelectedDate.Value,
                        room_id = roomId,
                        cleaningStatus = status,
                        employee_id = null // Для менеджера оставляем null
                    };

                    db.Cleaning_Rooms.Add(newCleaning);
                    db.SaveChanges();

                    IsSuccess = true;
                    MessageBox.Show("Запись уборки успешно добавлена!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
