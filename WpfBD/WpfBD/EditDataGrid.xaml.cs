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
    /// Логика взаимодействия для EditDataGrid.xaml
    /// </summary>
    public partial class EditDataGrid : Window
    {


        string text_;
        public string TextBoxText
        {
            get { return text_; }
            private set { text_ = value; }
        }
        public EditDataGrid(string text)
        {
            InitializeComponent();
            TextBoxText = text;
            TextEditBox.Text = TextBoxText;
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxText = TextEditBox.Text;
            Close();

        }
    }
}
