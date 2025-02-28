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

namespace Demka_test.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddAndEditAndDeleteRequestWindow.xaml
    /// </summary>
    public partial class AddAndEditAndDeleteRequestWindow : Window
    {
        private Entities dbContext;
        private Requests currentRequest;
        private string UserRole;

        public AddAndEditAndDeleteRequestWindow(int idRequest = 0, string userRole = "")
        {
            InitializeComponent();
            dbContext = new Entities();
            UserRole = userRole;

            SetElementVisibility();

            LoadComboBoxes();
            if (idRequest > 0)
            {
                LoadRequestData(idRequest);
            }
        }

        private void LoadComboBoxes()
        {
            ClimateTechTypeComboBox.ItemsSource = dbContext.climateTechTypes.ToList();
            ClimateTechTypeComboBox.DisplayMemberPath = "climateTechType";

            CustomerComboBox.ItemsSource = dbContext.Users.Where(u => u.idTypeRole == 4).ToList();
            CustomerComboBox.DisplayMemberPath = "fio";

            MasterComboBox.ItemsSource = dbContext.Users.Where(u => u.idTypeRole == 2).ToList();
            MasterComboBox.DisplayMemberPath = "fio";


            RequestStatusComboBox.ItemsSource = dbContext.requestStatus.ToList();
            RequestStatusComboBox.DisplayMemberPath = "requestStatus1";

        }

        private void LoadRequestData(int idRequest)
        {
            currentRequest = dbContext.Requests.Find(idRequest);
            if (currentRequest != null)
            {
                RequestNumberTextBox.Text = currentRequest.isRequest.ToString();
                StartDatePicker.SelectedDate = currentRequest.startDate;
                ClimateTechTypeComboBox.SelectedItem = dbContext.climateTechTypes.FirstOrDefault(t => t.idClimateTechType == currentRequest.idclimateTechType);
                ClimateTechModelTextBox.Text = currentRequest.climateTechModel.ToString();
                ProblemDescriptionTextBox.Text = currentRequest.problemDescryption.ToString();

                CompletionDatePicker.SelectedDate = currentRequest.completionDate;

                RepairPartsTextBox.Text = currentRequest.repairParts?.ToString();

                if (currentRequest.idUser.HasValue)
                {
                    CustomerComboBox.SelectedItem = dbContext.Users.FirstOrDefault(u => u.idUser == currentRequest.idUser);
                }

                if (currentRequest.idMaster.HasValue)
                {
                    MasterComboBox.SelectedItem = dbContext.Users
                        .FirstOrDefault(u => u.idUser == currentRequest.idMaster);
                }
                if (currentRequest.idRequestStatus.HasValue)
                {
                    RequestStatusComboBox.SelectedItem = dbContext.requestStatus
                        .FirstOrDefault(s => s.idRequestStatus == currentRequest.idRequestStatus);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentRequest == null)
                {
                    currentRequest = new Requests();
                    dbContext.Requests.Add(currentRequest);
                }

                currentRequest.startDate = StartDatePicker.SelectedDate;
                currentRequest.climateTechModel = ClimateTechModelTextBox.Text;
                currentRequest.problemDescryption = ProblemDescriptionTextBox.Text;

                // Добавьте сохранение новых полей
                currentRequest.completionDate = CompletionDatePicker.SelectedDate;
                currentRequest.repairParts = RepairPartsTextBox.Text;

                var selectedTechType = ClimateTechTypeComboBox.SelectedItem as climateTechTypes;
                currentRequest.idclimateTechType = selectedTechType?.idClimateTechType;

                var selectedCustomer = CustomerComboBox.SelectedItem as Users;
                currentRequest.idUser = selectedCustomer?.idUser;

                var selectedMaster = MasterComboBox.SelectedItem as Users;
                currentRequest.idMaster = selectedMaster?.idUser;

                var selectedStatus = RequestStatusComboBox.SelectedItem as requestStatus;
                currentRequest.idRequestStatus = selectedStatus?.idRequestStatus;

                dbContext.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.YesNo);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void SetElementVisibility()
        {
            if (UserRole != "Менеджер" && UserRole != "Специалист" && UserRole != "Оператор")
            {
                MasterComboBox.Visibility = Visibility.Collapsed;
                CustomerComboBox.Visibility = Visibility.Collapsed;
                RepairPartsTextBox.Visibility = Visibility.Collapsed;
                RequestStatusComboBox.Visibility = Visibility.Collapsed;
                CompletionDatePicker.Visibility = Visibility.Collapsed;

                MasterLabel.Visibility = Visibility.Collapsed; 
                CustomerLabel.Visibility = Visibility.Collapsed; 
                RepairPartsLabel.Visibility = Visibility.Collapsed;
                RequestStatusLabel.Visibility = Visibility.Collapsed;
                CompletionDatePickerLabel.Visibility = Visibility.Collapsed;
            }

        }
    }
}
