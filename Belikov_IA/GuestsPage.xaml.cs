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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Belikov_IA
{
    /// <summary>
    /// Логика взаимодействия для GuestsPage.xaml
    /// </summary>
    public partial class GuestsPage : Page
    {
        public GuestsPage()
        {
            InitializeComponent();
            LoadGuestsData();
        }

        // Класс для отображения данных гостей
        public class GuestDisplay
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string document_number { get; set; }
        }

        // Метод загрузки данных из таблицы Guests
        private void LoadGuestsData()
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    // Получаем всех гостей из базы данных
                    var guests = db.Guests.ToList();

                    // Создаем список для отображения
                    var guestList = new List<GuestDisplay>();

                    foreach (var guest in guests)
                    {
                        guestList.Add(new GuestDisplay
                        {
                            first_name = guest.FirstName ?? "",
                            last_name = guest.SecondName ?? "",
                            email = guest.email ?? "",
                            phone = guest.phone ?? "",
                            document_number = guest.documentNumber ?? ""
                        });
                    }

                    // Устанавливаем источник данных для DataGrid
                    GuestsDataGrid.ItemsSource = guestList;
                    GuestsCount.Text = guestList.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
}
