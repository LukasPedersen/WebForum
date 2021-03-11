using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class Repository : IRepository
    {
        public static bool loggedIn;
        public static string currentLoggedInUsername;
        private List<Topic> allTopics = new List<Topic>();

        public static IConfigurationRoot configuration { get; private set; }
        static string connectionString;
        SqlConnection sqlCon = new SqlConnection(connectionString);

        public static void SetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Create new user. Needs a <paramref name="username"/>, <paramref name="password"/>, <paramref name="firstName"/>, <paramref name="lastName"/>, <paramref name="email"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        public void CreateUser(string username, string password, string firstName, string lastName, string email)
        {
            string sqlCreateUserCommand = "EXEC spCreateUser @username, @password, @f_name, @l_name, @email, @active = 0";
            SqlCommand sqlCmd = new SqlCommand(sqlCreateUserCommand, sqlCon);
            sqlCmd.Parameters.AddWithValue("@username", username);
            sqlCmd.Parameters.AddWithValue("@password", password);
            sqlCmd.Parameters.AddWithValue("@f_name", firstName);
            sqlCmd.Parameters.AddWithValue("@l_name", lastName);
            sqlCmd.Parameters.AddWithValue("@email", email);
            sqlCon.Open();
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            finally
            {
                sqlCon.Close();
            }
        }

        /// <summary>
        /// Check if user exist in database and return (true || false)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pasword"></param>
        /// <returns></returns>
        public bool UserLogin(string username, string pasword)
        {
            SqlCommand cmd = new SqlCommand("Select count(*) from Login where username= @userName AND passWord = @passWord", sqlCon);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@passWord", pasword);
            sqlCon.Open();
            try
            {
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    loggedIn = true;
                    currentLoggedInUsername = username;
                    return true;
                }
                else
                    return false;
            }
            finally
            {
                sqlCon.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Topic> GetAllTopics()
        {
            using (sqlCon)
            {
                using (var command = sqlCon.CreateCommand())
                {
                    command.CommandText = "SELECT topics_ID, chatRoom_ID, headder, text, forfatter FROM Topics";
                    try
                    {
                        sqlCon.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var indexOfColumn1 = reader.GetOrdinal("topics_ID");
                            var indexOfColumn2 = reader.GetOrdinal("chatRoom_ID");
                            var indexOfColumn3 = reader.GetOrdinal("headder");
                            var indexOfColumn4 = reader.GetOrdinal("text");
                            var indexOfColumn5 = reader.GetOrdinal("forfatter");

                            while (reader.Read())
                            {
                                int tempTopicID = Convert.ToInt32(reader.GetValue(indexOfColumn1));
                                string tempHeadder = reader.GetValue(indexOfColumn3).ToString();
                                string tempText = reader.GetValue(indexOfColumn4).ToString();
                                string tempForfatter = reader.GetValue(indexOfColumn5).ToString();

                                allTopics.Add(new Topic { topicID = tempTopicID, headder = tempHeadder, text = tempText, forfatter = tempForfatter });
                            }
                        }
                        return allTopics;
                    }
                    finally
                    {
                        sqlCon.Close();
                    }
                }
            }
        }

        public void CreateTopic(string headder, string text)
        {
            int UserID;
            SqlCommand cmd = new SqlCommand("Select users_ID from Login where username= @userName", sqlCon);
            cmd.Parameters.AddWithValue("@Username", currentLoggedInUsername);

            sqlCon.Open();
            try
            {
                UserID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                sqlCon.Close();
            }
            SqlCommand cmd1 = new SqlCommand("EXEC spCreateTopic @forfatterUser_ID, @headder, @text, @forfatter", sqlCon);
            cmd1.Parameters.AddWithValue("@forfatterUser_ID", UserID);
            cmd1.Parameters.AddWithValue("@headder", headder);
            cmd1.Parameters.AddWithValue("@text", text);
            cmd1.Parameters.AddWithValue("@forfatter", currentLoggedInUsername);
            sqlCon.Open();
            try
            {
                cmd1.ExecuteNonQuery();
            }
            finally
            {
                sqlCon.Close();
            }
        }
    }
    
}
