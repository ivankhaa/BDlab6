using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace WpfBD
{
    public class Log : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value + "\n-------------------------------------------------\n" + _text;
              /*  if (_text.Length > 370)
                    _text = _text.Substring(0, 370) + "...";*/

                OnPropertyChanged("Text");
            }
        }
        public void TextClear()
        {
            _text = "";
            OnPropertyChanged("Text");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ComboBoxList
    {
        public int ID { get; set; }
        public string TEXT { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ComboBoxList> comboBoxList { get; set; }
        DateTime Date { get; set; }
        Dictionary<string, Dictionary<string, List<object>>> data = null;
        List<Dictionary<string, object>> actions = null;
        DatabaseAccess dbAccess = null;
        string TabItemName { get; set; }
        Log _log;
        private void Log_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            infoBox.Text = _log.Text;
        }

        public MainWindow()
        {
            InitializeComponent();
            _log = new Log();
            _log.PropertyChanged += Log_PropertyChanged;
            var connectionWindow = new ConnectionWindow();
            if (connectionWindow.ShowDialog() == true)
            {
                dbAccess = connectionWindow.dbAccess;
                dbAccess._log = _log;
            }
            else
            {
                Close();
                return;
            }
            actions = new List<Dictionary<string, object>>();
            data = dbAccess.GetAllTables();
        }
        private void GetAllTables()
        {
            data = dbAccess.GetAllTables();
            LoadComboBoxes();
        }
        private void LoadComboBoxes()
        {
            var grid = this.FindName($"{TabItemName}Grid") as Grid;
            if (grid != null)
                foreach (var child in grid.Children)
                {
                    comboBoxList = new List<ComboBoxList>();
                    if (child is ComboBox comboBox)
                    {
                        for (int i = 0; i < data[comboBox.Tag.ToString()][comboBox.Name].Count; i++)
                            comboBoxList.Add(new ComboBoxList() { ID = (int)data[comboBox.Tag.ToString()][comboBox.Name][i], TEXT = rowText(comboBox.Tag.ToString(), i) });
                        comboBox.ItemsSource = comboBoxList;
                        comboBox.SelectedIndex = 0;
                    }
                }

            string rowText(string tag, int i)
            {
                string str = "";
                foreach (var key in data[tag].Keys.Skip(1))
                {
                    str += data[tag][key][i] + " ";
                }
                str = str.Substring(0, str.Length - 1);
                return str;
            }
        }
        private void AddActions(string name, object obj) 
        {
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add(name, obj);
            actions.Add(tmp);
        }
        public void BindDataToGrid(DataGrid grid, Dictionary<string, List<object>> data)
        {
            grid.Columns.Clear();
            grid.Items.Clear();
            TableDBdel.Items.Clear();

            // додаємо колонки до DataGrid
            foreach (string columnName in data.Keys)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = columnName;
                column.Binding = new Binding($"[{columnName}]");
                grid.Columns.Add(column);
            }

            // додаємо стовпець з кнопками до DataGrid
            DataGridTemplateColumn actionsColumn = new DataGridTemplateColumn();
            actionsColumn.Header = "Actions";
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            DataTemplate buttonTemplate = new DataTemplate();
            buttonTemplate.VisualTree = buttonFactory;
            actionsColumn.CellTemplate = buttonTemplate;

            // додаємо дані до DataGrid
            for (int i = 0; i < data.Values.First().Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (string columnName in data.Keys)
                {
                    var dataObject = data[columnName][i];
                    if (dataObject is DateTime)
                    {
                        var DTdata = (DateTime)data[columnName][i];
                        data[columnName][i] = DTdata.ToString("yyyy.MM.dd");
                    }

                    row[columnName] = data[columnName][i];
                }

                grid.Items.Add(row);
                // Додаємо кнопку до рядка
                TableDBdel.Items.Add(actionsColumn);
            }
        }
        public Dictionary<string, List<object>> ExtractDataFromGrid(DataGrid grid)
        {
            Dictionary<string, List<object>> data = new Dictionary<string, List<object>>();

            // витягаємо назви стовпців
            List<string> columnNames = new List<string>();
            foreach (DataGridColumn column in grid.Columns)
            {
                if (column is DataGridTextColumn textColumn)
                {
                    columnNames.Add(textColumn.Header.ToString());
                    data[textColumn.Header.ToString()] = new List<object>();
                }
            }

            // витягаємо дані з кожного рядка
            foreach (Dictionary<string, object> row in grid.Items)
            {
                foreach (string columnName in columnNames)
                {
                    data[columnName].Add(row[columnName]);
                }
            }

            return data;
        }
        private void TabControlInner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tb = null;
            // Отримуємо вибраний TabItem
            if (e.OriginalSource is TabControl tabControl)
            {
                if (tabControl.SelectedItem is TabItem tabItem)
                {
                    if (tabItem.Name != "")
                    {
                        if (TabItemName != null)
                            tb = (TabItem)FindName(TabItemName);
                        if (tb != null)
                        {
                            tb.IsSelected = false;
                        }

                        TabItemName = tabItem.Name;
                        BindDataToGrid(TableDB, data[TabItemName]);
                        LoadComboBoxes();
                    }
                }
            }
        }
        private void TabControlOuter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (sender is TabControl tabControl)
            {

                foreach (TabItem tabItem1 in tabControl.Items)
                {
                    tabItem1.IsEnabled = true;
                }

                if (tabControl.SelectedItem is TabItem tabItem)
                {

                    tabItem.IsEnabled = false;
                }
            }
        }
        private void RowDelete(Dictionary<string, List<object>> dataTable, int indexRow)
        {
            bool isFirstIteration = true;
            int id = 0;
            string additionalTag = "";
            foreach (var columnName in dataTable.Keys)
            {
                if (columnName.Length > 0 && dataTable.ContainsKey(columnName))
                {
                    var columnData = dataTable[columnName];
                    if (isFirstIteration)
                    {
                        if (columnData[indexRow].ToString() == "")
                            return;
                        id = (int)columnData[indexRow];
                        if (dbAccess.IsView(TabItemName))
                            additionalTag = "tab";
                        if (!dbAccess.Delete(TabItemName + additionalTag, columnName, id))
                            return;
                        isFirstIteration = false;
                        if ((int)columnData.Max() == id)
                        {
                            int _id = 0;
                            if (indexRow >= columnData.Count)
                                _id = (int)columnData[indexRow - 1] + 1;
                            else
                                _id = columnData.Count;
                            dbAccess.SetAUTO_INCREMENT(TabItemName + additionalTag, _id);
                        }

                    }
                    columnData.RemoveAt(indexRow);
                }
            }

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> del = new Dictionary<string, object>();
            int index = TableDBdel.SelectedIndex;
            MessageBoxResult result = MessageBox.Show("Ви дійсно бажаєте видалити цей рядок?", "Попередження", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                del.Add(TabItemName, index);
                del.Add("data", data[TabItemName]);
                AddActions("DELETE", del);
                if (TableDB.Items.Count > index)
                {
                    TableDB.Items.RemoveAt(index);
                    TableDBdel.Items.RemoveAt(index);
                }
               // RowDelete(data[TabItemName], index);
               
                //GetAllTables();
                data[TabItemName] = ExtractDataFromGrid(TableDB);
                BindDataToGrid(TableDB, data[TabItemName]);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string additionalTag = "";
            var grid = this.FindName($"{TabItemName}Grid") as Grid;

            var row = new Dictionary<string, object>();
            var TabRow = new Dictionary<string, Dictionary<string, object>>();
            foreach (var child in grid.Children)
            {
                if (child is TextBox textBox)
                {
                    if (textBox.Text == "") 
                    {
                        _log.Text = $"Column {textBox.Name} NOT NULL";
                        return;
                    }
                        
                    row.Add(textBox.Name, textBox.Text);
                    textBox.Text = "";
                }
                else if (child is ContentControl contentControl)
                {
                    if (contentControl.Content is UserControl)
                    {
                        var clientComboBox = grid.FindResource("ClientComboBox") as UserControl;
                        var comboBox = clientComboBox?.Content as ComboBox;
                        if (comboBox != null)
                        {
                            row.Add(comboBox.Name, comboBox.SelectedValue);
                        }
                    }
                }
                else if (child is ComboBox comboBox)
                    row.Add(comboBox.Name, comboBox.SelectedValue);
                else if (child is DatePicker datePicker)
                    row.Add(datePicker.Name, datePicker.SelectedDate.Value.ToString("yyyy.MM.dd"));
            }
            /*            if (dbAccess.IsView(TabItemName))
                            additionalTag = "tab";
                        dbAccess.Insert(TabItemName + additionalTag, row);
                        GetAllTables();*/
            int maxid = 1;
            if (data.ContainsKey(TabItemName))
            {
                object value = data[TabItemName].Values.ElementAt(0);

                if (value is List<object> list && list.Count > 0)
                {
                     maxid = list.Max(item => item is int id ? id : 0);
                    maxid += 1;
                }

            }
            Dictionary<string, object> newDictionary = new Dictionary<string, object>();

            // Вставка новой записи в начало словаря
            newDictionary.Add(data[TabItemName].ElementAt(0).Key, maxid);

            // Копирование оставшихся элементов из исходного словаря в новый словарь
            foreach (var pair in row)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }

            row = newDictionary;
            foreach (var kvp in row)
            {
                data[TabItemName][kvp.Key].Add(kvp.Value);
            }
            BindDataToGrid(TableDB, data[TabItemName]);
            TabRow.Add(TabItemName + additionalTag, row);

            AddActions("INSERT", TabRow);
            
        }
        private void Upload_Click(object sender, RoutedEventArgs e) 
        {
            foreach (var item in actions)
                if (item.ElementAt(0).Key == "INSERT")
                {
                    var inTab = (Dictionary<string, Dictionary<string, object>>)item.Values.First();
                    dbAccess.Insert(inTab.First().Key, inTab.First().Value);
                }
                else if (item.ElementAt(0).Key == "DELETE")
                {
                    var dlTab = (Dictionary<string, object>)item.Values.First();
                    RowDelete((Dictionary<string, List<object>>)dlTab["data"], (int)dlTab.First().Value);
                }
                else if (item.ElementAt(0).Key == "UPDATE")
                {
                    var upTab = (Dictionary<string, Dictionary<string, Dictionary<string, object>>>)item.Values.First();
                    dbAccess.Update(upTab.First().Value.First().Key, upTab.First().Key, upTab.First().Value.First().Value);
                }
            GetAllTables();
            BindDataToGrid(TableDB,data[TabItemName]);
            actions = new List<Dictionary<string, object>>();
        }
        private void ComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            var textBox = (TextBox)e.OriginalSource;
            var items = comboBox.Items.OfType<ComboBoxList>().Select(x => x.TEXT).ToList();

            if (!items.Any(item => item.StartsWith(comboBox.Text, StringComparison.CurrentCultureIgnoreCase)))
            {
                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                textBox.CaretIndex = textBox.Text.Length;
                comboBox.IsDropDownOpen = true;
            }
            if (textBox.Text == "")
                comboBox.SelectedIndex = 0;
            textBox.ScrollToHorizontalOffset(0);
        }
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn column = e.Column as DataGridTextColumn;
                if (column != null)
                {
                    // Встановлюємо форматування дати для стовпця
                    column.Binding.StringFormat = "yyyy.MM.dd";
                }
            }
        }
        private void TableDB_CellPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            // Отримання комірки, що містить курсор миші
            DataGridCell cell = GetCell(e);

            if (cell != null && cell.IsEditing)
            {
                var cellValue = cell.Content as TextBox;
                var columnName = cell.Column.Header.ToString();

                var rowIndex = TableDB.Items.IndexOf(cell.DataContext);
                var row = (Dictionary<string, object>)TableDB.Items[rowIndex];
                var originalValue = row[columnName].ToString();
                // Встановлення IsEditing відповідної комірки в false, якщо isView встановлено в true

                EditDataGrid window = new EditDataGrid(originalValue);
                window.Owner = Application.Current.MainWindow; // Установите владельца окна
                window.ShowDialog();
                var editedValue = window.TextBoxText;
            

                if (originalValue != editedValue)
                {
                    Dictionary<string, Dictionary<string, object>> tab = new Dictionary<string, Dictionary<string, object>>();
                    Dictionary<string, Dictionary<string, Dictionary<string, object>>> upd = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                    tab.Add(TabItemName, row);
                    upd.Add(columnName, tab);
                    // Значення було відредаговано
                    row[columnName] = editedValue;
                    AddActions("UPDATE", upd);
                    data[TabItemName] = ExtractDataFromGrid(TableDB);
                    //dbAccess.Update(TabItemName, columnName, row);
                    //GetAllTables();
                    BindDataToGrid(TableDB, data[TabItemName]);
                }

            }

            DataGridCell GetCell(MouseButtonEventArgs e)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // Перехід вгору по дереву елементів, поки не буде знайдено DataGridCell
                while ((dep != null) && !(dep is DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                return (DataGridCell)dep;
            }
        }
        private void TableDB_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
                e.Cancel = true;
                var columnName = e.Column.Header.ToString();
                var rowIndex = e.Row.GetIndex();
                var editedTextBox = (TextBox)e.EditingElement;
                var editedValue = editedTextBox.Text;

                var row = (Dictionary<string, object>)TableDB.Items[rowIndex];
                var originalValue = row[columnName].ToString();

                if (originalValue != editedValue)
                {
                    // Значення було відредаговано
                    row[columnName] = editedValue;
                    data[TabItemName] = ExtractDataFromGrid(TableDB);
                    dbAccess.Update(TabItemName, columnName, row);
                    GetAllTables();
                }

                Keyboard.ClearFocus();
                TableDB.SelectedItem = null;
        }
        private void ScrollViewerRight_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                ScrollBarLeft.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }

}
