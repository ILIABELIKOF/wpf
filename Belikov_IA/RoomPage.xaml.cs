using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Belikov_IA
{
    /// <summary>
    /// Логика взаимодействия для RoomPage.xaml
    /// </summary>
    /// 
    public partial class RoomPage : Page
    {
        public RoomPage()
        {
            InitializeComponent();
            LoadRoomsData();    
        }

        // Класс для отображения данных о комнатах
        public class RoomDisplay
        {
            //public int room_id { get; set; } // Добавляем ID для связи
            public string floor { get; set; }
            public string room { get; set; }
            public string room_category { get; set; }
            
            public string room_status { get; set; }
        }


        // Метод загрузки данных из базы
        private void LoadRoomsData( )
        {
            try
            {
                using (var db = new Model1Entities())
                {   
                    // Получаем все комнаты из базы данных
                    var rooms = db.Rooms.ToList(); 
                    
                    // Создаем список для отображения
                    var roomList = new List<RoomDisplay>();

                    foreach (var room in rooms)
                    {
                        roomList.Add(new RoomDisplay
                        {
                            floor = room.floor ?? "",
                            room = room.room_number ?? "",
                            room_category = room.CategoryRoom ?? "",
                            room_status = room.StatusRoom ?? ""

                        });
                    }

                    // Устанавливаем источник данных для DataGrid
                    RoomsDataGrid.ItemsSource = roomList;
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
