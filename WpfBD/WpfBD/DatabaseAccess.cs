using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace WpfBD
{
    class DatabaseAccess
    {  
        private readonly string connectionString;
        public Log _log;
        private MySqlConnection connection = null;
        public DatabaseAccess(string Server, string User, string Database, string Password, Log log)
        {
            string connectionString = $"Server={Server};Database={Database};User={User};Password={Password};";
            _log = log;
            using (MySqlConnection connect = new MySqlConnection(connectionString))
            {
                this.connectionString = connectionString;
                connection = connect;
                
            }
            connection.Open();
        }
        public Dictionary<string, List<object>> GetTable(string tableName)
        {
            Dictionary<string, List<object>> rows = new Dictionary<string, List<object>>();
                //connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM {tableName}", connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.GetValue(i);

                            if (!rows.ContainsKey(columnName))
                            {
                                rows[columnName] = new List<object>();
                            }

                            rows[columnName].Add(value);
                        }
                    }
                }
                else
                {
                    // add empty keys for each column name
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        if (!rows.ContainsKey(columnName))
                        {
                            rows[columnName] = new List<object>();
                        }
                        rows[columnName].Add("");
                    }
                }
            reader.Close();

            return rows;
        }
        public Dictionary<string, Dictionary<string, List<object>>> GetAllTables()
        {
            Dictionary<string, Dictionary<string, List<object>>> tables = new Dictionary<string, Dictionary<string, List<object>>>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                 connection.Open();
                MySqlCommand command = new MySqlCommand($"SHOW TABLES", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string tableName = reader.GetValue(0).ToString();
                    tables[tableName] = GetTable(tableName);
                }
            }
            return tables;
        }
        public bool Insert(string tableName, Dictionary<string, object> row)
        {
            string columnNames = string.Join(",", row.Keys);
            string values = string.Join(",", row.Values.Select(value => value.ToString() == "" ? "NULL" : $"'{value.ToString()}'"));

            string query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({values})";
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    command.ExecuteNonQuery();
                    _log.Text = $"Запис доданий до таблиці {tableName}";
                }
                catch (MySqlException ex)
                {
                    _log.Text = ex.Message.ToString();
                    return false;
                }
            return true;
        }
        public void Update(string tableName, string columnName, Dictionary<string, object> data)
        {
            try
            {
                string idName = data.Keys.ElementAt(0);
                int id = (int)data.Values.ElementAt(0);

                object columnData = data[columnName];

                string sql = $"UPDATE {tableName} SET {columnName}='{columnData}' WHERE {idName} = {id}";
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                _log.Text = $"Відредагована комірка: Рядок з {idName} = {id}; Стовпчик {columnName}; Таблиця {tableName}";
            }
            catch (MySqlException ex)
            {
                _log.Text = ex.ToString();
            }
        }
        public bool Delete(string tableName, string idName, int id)
        {
            try
            {

                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;

                    command.CommandText = $"DELETE FROM {tableName} WHERE {idName} = @id";
                    command.Parameters.AddWithValue("@id", id);
                   
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        _log.Text = $"No rows were deleted from the {tableName} table with {idName} = {id}.";
                        return false;
                    }
                
            }
            catch (MySqlException ex)
            {
                _log.Text = $"An error occurred while deleting from the {tableName} table: {ex.Message}";
                return false;
            }
            _log.Text = $"Запис з ID = {id} видалено";
            return true;
        }
        public void SetAUTO_INCREMENT(string tableName, int n) 
        {
            string query = $"ALTER TABLE {tableName} AUTO_INCREMENT = {n};";


                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    command.ExecuteNonQuery();
                    _log.Text = $"Встановлено AUTO_INCREMENT = {n} у таблиці {tableName}";
                }
                catch (MySqlException ex)
                {
                    _log.Text = ex.ToString();
               }
            
        }

        public bool IsView(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = '{tableName}'";


                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    Int64 count = (Int64)command.ExecuteScalar();
                    return count > 0;
                }
            
        }
        public bool IsReadOnly(string tableName)
        {
            string query = $"SELECT ReadOnly FROM tablesinfo WHERE TableName = '{tableName}'";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result);
                    }
                    else
                    {
                        return false;
                    }
                }
            
        }
        public void Pay(string tableName, string idName, int id, string PayMethod)
        {
            try
            {

                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;

                    command.CommandText = $"DELETE FROM {tableName} WHERE {idName} = @id";
                    command.Parameters.AddWithValue("@id", id);
                    _log.Text = $"Запис з ID = {id} видалено";
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        _log.Text = $"No rows were PAY from the {tableName} table with {idName} = {id}.";
                    }
                    string sql = $"UPDATE PaymentsHistory SET PaymentMethod='{PayMethod}' WHERE {idName} ={id} AND PaymentDate BETWEEN DATE_ADD(NOW(), INTERVAL -5 SECOND) AND DATE_ADD(NOW(), INTERVAL 5 SECOND);";
                    command = new MySqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                
            }
            catch (MySqlException ex)
            {
                _log.Text = $"An error occurred while PAY from the {tableName} table: {ex.Message}";
            }

        }
    }
}
