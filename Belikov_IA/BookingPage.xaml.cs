using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Belikov_IA
{
    /// <summary>
    /// Логика взаимодействия для BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        public BookingPage()
        {
            InitializeComponent();
            LoadBookingData();
        }

        // Класс для отображения данных из трех таблиц
        public class BookingDisplay
        {
            public int reservation_id { get; set; }
            public string guest_first_name { get; set; }
            public string guest_last_name { get; set; }
            public int room_id { get; set; }
            public DateTime check_in_date { get; set; }
            public DateTime check_out_date { get; set; }
            public string stay_price { get; set; }
            public string status { get; set; }
        }

        // Метод загрузки данных из трех таблиц
        private void LoadBookingData()
        {
            try
            {
                using (var db = new Model1Entities())
                {
                    // ОБЪЕДИНЯЕМ ТРИ ТАБЛИЦЫ через LINQ Join
                    var bookingsData = from reservation in db.Reservations
                                       join guest in db.Guests on reservation.guest_id equals guest.id
                                       join payday in db.PayDay on reservation.id equals payday.reservation_id
                                       select new BookingDisplay
                                       {
                                           
                                           guest_first_name = guest.FirstName ?? "",
                                           guest_last_name = guest.SecondName ?? "",
                                           room_id = reservation.room_id ?? 0,
                                           check_in_date = reservation.checkIn ?? DateTime.Now,
                                           check_out_date = reservation.checkOut ?? DateTime.Now,
                                           stay_price = payday.price ?? "",
                                           status = reservation.status ?? ""
                                       };

                    var bookingList = bookingsData.ToList();

                    // Устанавливаем источник данных для DataGrid
                    BookingDataGrid.ItemsSource = bookingList;
                    BookingCount.Text = bookingList.Count.ToString();
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
