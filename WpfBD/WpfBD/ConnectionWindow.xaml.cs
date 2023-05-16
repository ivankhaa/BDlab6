using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfBD
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public string Server { get; private set; }
        public string User { get; private set; }
        public string Database { get; private set; }
        public string Password { get; private set; }
    public ConnectionWindow()
        {
            InitializeComponent();
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Server = tbServer.Text;
            User = tbUser.Text;
            Database = tbDatabase.Text;
            Password = tbPassword.Password;
           
            try
            {
                DatabaseAccess dbAccess = new DatabaseAccess($"Server={tbServer.Text};Database={tbDatabase.Text};User={tbUser.Text};Password={tbPassword.Password};",null);
                DialogResult = true; // закриваємо вікно підключення та повертаємо true
            }
            catch (Exception ex)
            {
                lblError.Content = ex.Message;
                lblError.Visibility = Visibility.Visible;
            }

          
        }
    }
}

