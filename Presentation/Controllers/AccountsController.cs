using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Presentation.Controllers
{
    public class AccountsController
    {
        public AccountsController() { }


     /*   public IActionResult Login(string username, string password)
        {
            //username = admin
            //password =  ' or 1=1;--
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "";
            connection.Open();

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText= "Select Count(*) from AspNetUsers where " +
                "Email = @username and Password = @password";

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            //Select Count(*) from AspNetUsers where Email = 'admin' and Password = '' or 1=1;--'

            int rows = cmd.ExecuteNonQuery();

            connection.Close();

            if (rows > 0)
            {
                //Authenticate the user as username


            }
            else
            {
                //redirect to action
            }


        }*/
    }
}
