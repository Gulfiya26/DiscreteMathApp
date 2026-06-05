
using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using TipaDiplom.Services;

namespace TipaDiplom.Views
{
    public partial class AdminStudentsView : UserControl
    {
        private readonly AdminService _adminService;

        public AdminStudentsView()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                var students = _adminService.GetStudentStats();
                StudentsGrid.ItemsSource = students;
                CountText.Text = $"Всего студентов: {students.Count}";
            }
            catch (Exception ex)
            {
                CountText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}