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
    /// Логика взаимодействия для StaticsWindow.xaml
    /// </summary>
    public partial class StaticsWindow : Window
    {
        private List<Requests> requests;

        public StaticsWindow()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var context = Helper.GetContext();

            // Количество выполненных заявок
            var completedRequestsCount = context.Requests.Count(r => r.idRequestStatus == 2);
            CompletedRequestsCount.Text = completedRequestsCount.ToString();

            // Среднее время выполнения заявки
            var completedRequests = context.Requests.Where(r => r.idRequestStatus == 2 && r.completionDate.HasValue && r.startDate.HasValue).ToList();

            double averageCompletionTime = completedRequests.Average(r => (r.completionDate.Value - r.startDate.Value).TotalHours);

            AverageCompletionTime.Text = averageCompletionTime.ToString();

            // Статистика по типам неисправностей
            var problemStatistics = context.Requests.GroupBy(r => r.problemDescryption).Select(g => new { Problem = g.Key, Count = g.Count() }).ToList();

            ProblemStatistics.ItemsSource = problemStatistics;
            ProblemStatistics.DisplayMemberPath = "Problem";
        }
    }
}