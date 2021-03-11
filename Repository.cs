using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Repository : IRepository
    {
        public static bool loggedIn;
        public static string currentLoggedInUsername;
        static string connectionString = @"Data Source=DESKTOP-6JLNEAU\SQLEXPRESS;Initial Catalog=Forum;Integrated Security=True";
        static SqlConnection sqlCon = new SqlConnection(connectionString);

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
    }
}
