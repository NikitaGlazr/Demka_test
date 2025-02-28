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
using System.Data.Entity;


namespace Demka_test.Pages
{
    /// <summary>
    /// Логика взаимодействия для specialistWindowOrder.xaml
    /// </summary>
    public partial class specialistWindowOrder : Window
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }
        private List<Requests> _allRequests;

        public specialistWindowOrder(string userName, string userRole)
        {
            InitializeComponent();
            UserName = userName;
            UserRole = userRole;
            LoadData();
        }
        private void LoadData()
        {
            if (UserRole == "Менеджер" || UserRole == "Специалист" || UserRole == "Оператор")
            {
                btnAddRequest.Visibility = Visibility.Visible;
                btnDeleteRequest.Visibility = Visibility.Visible;
                btnEditRequest.Visibility = Visibility.Visible;
            }
            else
            {
                btnDeleteRequest.Visibility = Visibility.Hidden;
                btnEditRequest.Visibility = Visibility.Hidden;
            }

            var context = Helper.GetContext();

            var requests = context.Requests.Include(r => r.climateTechTypes).Include(r => r.requestStatus).Include(r => r.Users).ToList();

            _allRequests = requests;
            OrderListView.ItemsSource = requests;
        }



        private void tbSerach_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                if (textBox == null)
                {
                    return;
                }
                else
                {
                    string searchText = textBox.Text.ToLower();
                    var filteredList = _allRequests.Where(r =>
                        r.isRequest.ToString().Contains(searchText) ||
                        r.climateTechModel.ToLower().Contains(searchText) ||
                        r.problemDescryption.ToLower().Contains(searchText) ||
                        r.startDate.ToString().Contains(searchText) ||
                        (r.idRequestStatus == 1 && "В процессе ремонта".ToLower().Contains(searchText)) ||
                        (r.idRequestStatus == 2 && "Готова к выдаче".ToLower().Contains(searchText)) ||
                        (r.idRequestStatus == 3 && "Новая заявка".ToLower().Contains(searchText))).ToList();

                    OrderListView.ItemsSource = filteredList;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void cmdSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmdSort.SelectedItem == null)
            {
                return;
            }
            else
            {
                var selectSort = (cmdSort.SelectedItem as ComboBoxItem)?.Content.ToString();
                var filteredRequests = FilterRequests();

                if (selectSort == "Сортировать по возрастанию") 
                {
                    filteredRequests = filteredRequests.OrderBy(r => r.isRequest).ToList();
                } else if (selectSort == "Сортировать по убыванию") 
                {
                    filteredRequests = filteredRequests.OrderByDescending(r => r.isRequest).ToList();
                }

                OrderListView.ItemsSource = filteredRequests;
            }
        }

        private void cmdFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmdFilter.SelectedItem == null)
            {
                return;
            }
            else 
            {
                var filteredRequests = FilterRequests();
                OrderListView.ItemsSource = filteredRequests;
            }

        }

        private List<Requests> FilterRequests()
        {
            var selectedFilter = (cmdFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            var filteredRequests = _allRequests;

            if (selectedFilter == "Кондиционер")
            {
                filteredRequests = filteredRequests.Where(r => r.climateTechTypes.climateTechType == "Кондиционер").ToList();
            }
            else if (selectedFilter == "Увлажнитель воздуха")
            {
                filteredRequests = filteredRequests.Where(r => r.climateTechTypes.climateTechType == "Увлажнитель воздуха").ToList();
            }
            else if (selectedFilter == "Сушилка для рук")
            {
                filteredRequests = filteredRequests.Where(r => r.climateTechTypes.climateTechType == "Сушилка для рук").ToList();
            }
            else if (selectedFilter == "В процессе ремонта")
            {
                filteredRequests = filteredRequests.Where(r => r.requestStatus.requestStatus1 == "В процессе ремонта").ToList();
            }
            else if (selectedFilter == "Готова к выдаче")
            {
                filteredRequests = filteredRequests.Where(r => r.requestStatus.requestStatus1 == "Готова к выдаче").ToList();
            }
            else if (selectedFilter == "Новая заявка")
            {
                filteredRequests = filteredRequests.Where(r => r.requestStatus.requestStatus1 == "Новая заявка").ToList();
            }

            return filteredRequests;
        }

        //Добавить переход для обычного пользоваеля и спрятать некоторые элементы.
        private void btnAddRequest_Click(object sender, RoutedEventArgs e)
        {
            if (OrderListView.SelectedItem is Requests selectedRequest)
            {
                AddAndEditAndDeleteRequestWindow editWindow = new AddAndEditAndDeleteRequestWindow(selectedRequest.isRequest);
                editWindow.Show();
                LoadData();
            }

        }

        private void btnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (OrderListView.SelectedItems is Requests selectedRequest)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверенны, что хотите удалить: №{selectedRequest.isRequest}?", "Подтвердить удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) 
                    { 
                    
                        var dbContext = Helper.GetContext();

                        var requestToDelete = dbContext.Requests.FirstOrDefault(r => r.isRequest == selectedRequest.isRequest);

                        if (requestToDelete != null) 
                        {
                            dbContext.Requests.Remove(requestToDelete);
                            dbContext.SaveChanges();
                            LoadData();
                            MessageBox.Show("Заявка успешна удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                }
                else 
                {
                    MessageBox.Show("Пожалуйста, выберите заявку для удаления", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Произошла ошибка при удалении заявки: {ex.Message}","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Добавить переход для обычного пользоваеля и спрятать некоторые элементы.
        private void btnEditRequest_Click(object sender, RoutedEventArgs e)
        {
            if (OrderListView.SelectedItem is Requests selectedRequest)
            {
                AddAndEditAndDeleteRequestWindow editWindow = new AddAndEditAndDeleteRequestWindow(selectedRequest.isRequest, UserRole);
                editWindow.Show();
                LoadData();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        { 
            MainWindow mainWindow = new MainWindow(); // Создание нового экземпляра MainWindow
            mainWindow.Show(); // Показать окно MainWindow
            this.Close();
        }

    
        private void OrderListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (OrderListView.SelectedItem is Requests selectedRequest)
            {
                AddAndEditAndDeleteRequestWindow editWindow = new AddAndEditAndDeleteRequestWindow(selectedRequest.isRequest, UserRole);
                editWindow.Show();
                LoadData();
            }
        }

        private void OrderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        
        }

        private void btnStaticWindow_Click(object sender, RoutedEventArgs e)
        {
            StaticsWindow staticsWindow = new StaticsWindow(); // Создание нового экземпляра MainWindow
            staticsWindow.Show(); // Показать окно MainWindow
            this.Close();
        }
    }
}
