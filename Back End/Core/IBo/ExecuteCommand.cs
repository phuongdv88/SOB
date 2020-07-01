using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MI.Bo
{
    public interface IExecuters
    {
        void ExecuteCommand(string connStr, Action<SqlConnection> task);
        T ExecuteCommand<T>(string connStr, Func<SqlConnection, T> task);
    }
    public class Executers : IExecuters
    {
        public void ExecuteCommand(string connStr, Action<SqlConnection> task)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                task(conn);
                conn.Close();
            }
        }

        public T ExecuteCommand<T>(string connStr, Func<SqlConnection, T> task)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                return task(conn);
            }
        }
    }


}
