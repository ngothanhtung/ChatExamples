using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ChatExamples.Components;

namespace ChatExamples.Models
{
    public class MessageRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public IEnumerable<Message> GetAllMessages()
        {
            var messages = new List<Message>();
            using (var connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT Id, MessageContent, UserName, CreatedDate FROM [dbo].[Messages] ORDER BY CreatedDate", connection))
                {
                    if (connection.State == ConnectionState.Closed)
                    { 
                        connection.Open();
                    }

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        messages.Add(new Message()
                        {
                            Id =  new Guid(reader["Id"].ToString()),
                            MessageContent = reader["MessageContent"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        });
                    }

                    reader.Close();
                }

            }
            return messages;
        }

        public void SqlDependencyRegister()
        {
            using (var connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT Id, MessageContent, UserName, CreatedDate FROM [dbo].[Messages]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(SqlDependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    command.ExecuteReader();
                }

            }
        }

        private void SqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependencyHub.SendNotificationSqlDependency();
                
                SqlDependencyRegister();
            }
        }
    }
}