using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_task
{
    class DataBaseManager
    {
        public static string connectString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = test_task_bd.mdb";
        OleDbConnection myConnection;
        public DataBaseManager()
        {
            myConnection = new OleDbConnection(connectString);
        }
        public int Authorization(string login, string pass)
        {
            OpenConnection();
            string query = $@"SELECT [Код пользователя]
                            FROM [Логин/пароль]
                            WHERE Логин = '{login}' AND Пароль = '{pass}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            
            OleDbDataReader reader = command.ExecuteReader();
            bool availability = reader.Read();
            reader.Close();
            int userId = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection();
            if (availability)
                return userId;
            else
                return 0;
        }
        public bool Register(int userid, string login, string pass, string name, string surname, int idPosition, int idChief)
        {
            OpenConnection();
            string query = $@"INSERT INTO [Логин/пароль] ([Код пользователя], Логин, Пароль)
                            VALUES ('{userid}', '{login}', '{pass}')";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            if (command.ExecuteNonQuery() == 1)
            {
                DateTime dateTimeToday = DateTime.Today;

                query = $@"INSERT INTO [Данные пользователей] ([Код пользователя], [Имя], [Фамилия], [Код должности], [Код доступа], [Код начальника], [Дата приема на работу])
                       VALUES ('{userid}', '{name}', '{surname}', {idPosition}, 2, {idChief}, '{dateTimeToday}')";
                command = new OleDbCommand(query, myConnection);
                if (command.ExecuteNonQuery() == 1)
                {
                    CloseConnection();
                    return true;
                }
                else
                {
                    CloseConnection();
                    return false;
                }
            }
            else
            {
                CloseConnection();
                return false;
            }
        }
        public bool CheckLogin(string login)
        {
            OpenConnection();
            string query = $@"SELECT [Код пользователя]
                            FROM [Логин/пароль]
                            WHERE Логин = '{login}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            bool availability = reader.Read();
            reader.Close();
            CloseConnection();
            if (availability)
                return false;
            else
                return true;
        }
        public void GetPosition(ListBox listBox)
        {
            OpenConnection();
            string query = $@"SELECT Должность
                            FROM Должности";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listBox.Items.Add(reader[0].ToString() + " ");
            }
            reader.Close();
            CloseConnection();
        }
        public void GetNamePotentialChief(ListBox listBox)
        {
            OpenConnection();
            string query = $@"SELECT Имя, Фамилия
                            FROM [Данные пользователей]
                            WHERE([Код должности] <> 1 AND [Код пользователя] <> 1)";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listBox.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
            }
            reader.Close();
            CloseConnection();
        }
        public int GetBaseSalary()
        {
            OpenConnection();
            string query = $@"SELECT [Базовая ставка]
                              FROM Ставка";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            int salary = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection();
            return salary;
        }
        public int GetUserIdForName(string name, string surname)
        {
            OpenConnection();
            string query = $@"SELECT [Код пользователя]
                            FROM [Данные пользователей]
                            WHERE Имя = '{name}' AND Фамилия = '{surname}'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            int userid = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection();
            return userid;
        }

        public DataSet GetUserInformarion(int userId)
        {
            OpenConnection();
            string query = $@"SELECT Имя, Фамилия
                              FROM[Данные пользователей]
                              WHERE([Код пользователя] = {userId})";
            return CreateDataSet(query);
        }
        public void InformationAllUser(ListBox listBox, List<int> userIdinListBox)
        {
            OpenConnection();
            string query = $@"SELECT Имя, Фамилия, [Код пользователя]
                            FROM[Данные пользователей]
                            WHERE [Код пользователя] <> 1";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            
            while(reader.Read())
            {
                listBox.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                userIdinListBox.Add(Convert.ToInt32(reader[2]));
            }
            reader.Close();
            CloseConnection();
        }
        public int GetLastUserId()
        {
            OpenConnection();
            string query = $@"SELECT SUM([Код пользователя]) AS Expr1
                              FROM [Данные пользователей]";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            int userid = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection();
            return userid;
        }

        public DataSet GetUserIdForChiefId(int ChiefId)
        {
            OpenConnection();
            string query = $@"SELECT [Код пользователя]
                              FROM [Данные пользователей]
                              WHERE ([Код начальника] = {ChiefId})";
            return CreateDataSet(query);
        }

        public DataSet GetCheckAvailableTables(int userId)
        {
            OpenConnection();
            string query = $@"SELECT [Уровни доступа].[Видеть подчиненных], [Уровни доступа].[Видеть личные данные других], [Уровни доступа].[Видеть всех пользователей], [Уровни доступа].[Редактировать данные пользователей]
                              FROM ([Уровни доступа] INNER JOIN [Данные пользователей] ON [Уровни доступа].[Код доступа] = [Данные пользователей].[Код доступа])
                              WHERE ([Данные пользователей].[Код пользователя] = {userId})";
            return CreateDataSet(query);
        }

            public DataSet InformationForUserId(int userId)
        {
            OpenConnection();
            string query = $@"SELECT [Данные пользователей].Имя, [Данные пользователей].Фамилия, Должности.Должность, [Данные пользователей].[Код начальника] AS Начальник, [Данные пользователей].[Дата приема на работу]
                              FROM ([Данные пользователей] INNER JOIN Должности ON [Данные пользователей].[Код должности] = Должности.[Код должности])
                              WHERE ([Данные пользователей].[Код пользователя] = {userId})";
            return CreateDataSet(query);   
        }
        public int GetPositionForUserId(int userId)
        {
            OpenConnection();
            string query = $@"SELECT [Код должности]
                              FROM [Данные пользователей]
                              WHERE ([Код пользователя] = {userId})";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            int position = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection();
            return position;
        }
        public void UpdateUserInformation(int userId, string name, string surName)
        {
            OpenConnection();
            string query = $@"UPDATE [Данные пользователей]
                              SET Имя = '{name}', Фамилия = '{surName}'
                              WHERE ([Данные пользователей].[Код пользователя] = {userId})";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteScalar();
            CloseConnection();
        }

        public void DeleteUser(int userId)
        {
            OpenConnection();
            string query = $@"DELETE FROM [Данные пользователей]
                              WHERE ([Код пользователя] = {userId})";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteScalar();
            query = $@"DELETE FROM [Логин/пароль]
                       WHERE ([Код пользователя] = {userId})";
            command = new OleDbCommand(query, myConnection);
            command.ExecuteScalar();
            query = $@"UPDATE [Данные пользователей]
                       SET [Код начальника] = 
                       WHERE ([Код начальника] = {userId})";
            command = new OleDbCommand(query, myConnection);
            CloseConnection();
        }
        private DataSet CreateDataSet(string query)
        {
            OleDbCommand command = new OleDbCommand(query, myConnection);
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.SelectCommand = command;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            CloseConnection();
            return dataSet;
        }
        public void OpenConnection()
        {
            myConnection.Open();
        }
        public void CloseConnection()
        {
            myConnection.Close();
        }
    }
}
