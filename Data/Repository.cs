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
        private List<Thread> allThreads = new List<Thread>();

        public static IConfigurationRoot configuration { get; private set; }
        static string connectionString;
        SqlConnection sqlCon = new SqlConnection(connectionString);

        public static void SetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
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
            if (username == "" || pasword == "")
            {
                return false;
            }
            else
            {
                SqlCommand cmd = new SqlCommand("Select * from Login where username = @userName AND passWord = @passWord", sqlCon);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@passWord", pasword);
                sqlCon.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
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
        }

        /// <summary>
        /// Returens list of topics
        /// </summary>
        /// <returns></returns>
        public List<Topic> GetAllTopics()
        {

            sqlCon.ConnectionString = connectionString;
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

        public List<Thread> GetAllThreads()
        {
            using (sqlCon)
            {
                using (var command = sqlCon.CreateCommand())
                {
                    command.CommandText = "SELECT thread_ID, users_ID, topics_ID, headder, content, forfatter FROM Threads";
                    try
                    {
                        sqlCon.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var indexOfColumn1 = reader.GetOrdinal("users_ID");
                            var indexOfColumn2 = reader.GetOrdinal("topics_ID");
                            var indexOfColumn3 = reader.GetOrdinal("headder");
                            var indexOfColumn4 = reader.GetOrdinal("content");
                            var indexOfColumn5 = reader.GetOrdinal("forfatter");

                            while (reader.Read())
                            {
                                int tempUsersID = Convert.ToInt32(reader.GetValue(indexOfColumn1));
                                int tempTopicsID = Convert.ToInt32(reader.GetValue(indexOfColumn2));
                                string tempHeadder = reader.GetValue(indexOfColumn3).ToString();
                                string tempContent = reader.GetValue(indexOfColumn4).ToString();
                                string tempForfatter = reader.GetValue(indexOfColumn5).ToString();

                                allThreads.Add(new Thread { userID = tempUsersID, topicsID = tempTopicsID, headder = tempHeadder, content = tempContent, threadForfatter = tempForfatter });
                            }
                        }
                        return allThreads;
                    }
                    finally
                    {
                        sqlCon.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a topic and inserts it into database
        /// </summary>
        /// <param name="headder"></param>
        /// <param name="text"></param>
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
        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="forfatter"></param>
        public void DeleteTopic(int topicID, string forfatter)
        {
            if (forfatter == currentLoggedInUsername || currentLoggedInUsername == "Admin")
            {
                SqlCommand cmd1 = new SqlCommand("DELETE FROM Threads WHERE topics_ID = @topics_ID", sqlCon);
                cmd1.Parameters.AddWithValue("@topics_ID", topicID);
                sqlCon.Open();
                try
                {
                    cmd1.ExecuteNonQuery();
                }
                finally
                {
                    sqlCon.Close();
                }
                string sqlCreateUserCommand = "EXEC spDeleteTopic @topics_ID, @forfatter";
                SqlCommand sqlCmd = new SqlCommand(sqlCreateUserCommand, sqlCon);
                sqlCmd.Parameters.AddWithValue("@topics_ID", topicID);
                sqlCmd.Parameters.AddWithValue("@forfatter", forfatter);

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
            else
            {
                //Write massage
            }
        }

        public void CreateThread(int topicID, string headder, string contents)
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
            SqlCommand cmd1 = new SqlCommand("EXEC spCreateThread @users_ID, @topics_ID, @headder, @content, @forfatter", sqlCon);
            cmd1.Parameters.AddWithValue("@users_ID", UserID);
            cmd1.Parameters.AddWithValue("@topics_ID", topicID);
            cmd1.Parameters.AddWithValue("@headder", headder);
            cmd1.Parameters.AddWithValue("@content", contents);
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
