using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using MVCVDO.Models;
using System.Data;
using System.Drawing;
using System.Web.Mvc;
using System.Text;

namespace MVCVDO.Repository
{
    public class EventRepository
    {
        private SqlConnection conn;

        private void Connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["getconnect"].ToString();
            conn = new SqlConnection(constr);
        }

        //// USER REGISTRATION
        public bool Register(Users obj)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("NewRegister", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email",obj.email);
            cmd.Parameters.AddWithValue("@Pass", obj.password);
            cmd.Parameters.AddWithValue("@CurrentPass", obj.currentPassword);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            if (i>=1){

                return true;

            }
            else
            {
                return false;
            }
        }

        //// USER LOGIN
        public DataTable Login(Users obj)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("RegisterLogin", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", obj.email);
            cmd.Parameters.AddWithValue("@Pass", obj.password);
            
            conn.Open();
            DataTable dt = new DataTable();
            SqlDataReader sda = cmd.ExecuteReader();
            dt.Load(sda);
            conn.Close();
            return dt;
        }

        //// GET ALL EMAIL
        public List<Users> GetAllEmail()
        {
            Connection();

            List<Users> EmailList = new List<Users>();

            SqlCommand cmd = new SqlCommand("GetAllEmails", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            conn.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();

            foreach (DataRow dr in dt.Rows)
            {
                EmailList.Add(
                    new Users
                    {
                        id = Convert.ToInt32(dr["id"]),
                        email=Convert.ToString(dr["email"])

                    }
                    );
            }

            return EmailList;
        }


        //// Save Details
        public bool SaveDetails(string email,string password,string usertype)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("UpdateGeneratePassword", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@CurrentPassword", password);

            cmd.Parameters.AddWithValue("@Usertype", usertype);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            if (i >= 1)
            {

                return true;

            }
            else
            {
                return false;
            }
        }

        /// Add Department
        public bool AddDepartment(Department obj)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("AddDepartment", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DepartmentName", obj.DepartmentName);
            cmd.Parameters.AddWithValue("@Status", obj.Status);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            if (i >= 1)
            {

                return true;

            }
            else
            {
                return false;
            }
        }

        // reset password

        public bool ResetPassword(int id, Users user)
        {
            Connection();  // Ensure the connection to the database is working

            SqlCommand cmd = new SqlCommand("ResetPassword", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Password", user.password);  // Assuming user.password is the new password
            cmd.Parameters.AddWithValue("@CurrentPassword", user.password);  // Assuming user.currentPassword is the old password


            try
            {
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;  // Check if the update was successful
            }
            catch (Exception ex)
            {
                // Handle exceptions and log if necessary
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public Users GetUserById(int id)
        {
            Connection();  // Ensure the connection to the database is working

            SqlCommand cmd = new SqlCommand("GetUserById", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);  // Pass the user ID to the stored procedure

            Users user = null;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user = new Users
                        {
                            id = Convert.ToInt32(reader["id"]),
                            email = reader["email"].ToString(),
                            password = reader["password"].ToString(),
                            currentPassword = reader["currentPassword"].ToString(),
                            usertype = reader["usertype"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }

            return user;  // Return the retrieved user
        }

        // Get Application Status
        public DataTable GetApplicationStatus(int id)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("ApplicationStatus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@R_Id", id);

            conn.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            conn.Close();
            return dt;
        }

        // get all deparmtent from the table
        public DataTable SelectDepartment(Department obj)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("GetAllDepartment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            conn.Close();
            return dt;
        }

        // to create dropdown
        public SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            if (table != null && table.Rows.Count > 0)
            {
                var list = table.AsEnumerable().Select(row => new
                {
                    Value = row[valueField],
                    Text = row[textField]
                }).ToList();

                return new SelectList(list, "Value", "Text");
            }
            else
            {
                return new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        // new application
        public bool NewApplication(int id, Application obj)
        {
            Connection();
            DateTime time = DateTime.Today;
            string format = "yyyy-MM-dd";
            string format1 = "yyyy-MM-dd 23:59:59";

            StringBuilder sb = new StringBuilder();
            if (obj.Dance) {
                sb.Append("Dance" + ", ");
            }
            if (obj.Song) { 
                sb.Append("Song" + ", ");
            }
            if (obj.Drawing) { 
                sb.Append("Drawing" + ", ");
            }

            char[] charsToTrim = { ',', ' ' };
            obj.Events = sb.ToString().Trim().TrimEnd(charsToTrim);

            SqlCommand com = new SqlCommand("AddNewApplication", conn);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@R_Id", id);
            com.Parameters.AddWithValue("@ApplicantName", obj.ApplicantName);
            com.Parameters.AddWithValue("@Gender", obj.Gender);
            com.Parameters.AddWithValue("@Department", obj.Department);
            com.Parameters.AddWithValue("@Events", obj.Events);
            com.Parameters.AddWithValue("@Mobile", obj.Mobile);
            com.Parameters.AddWithValue("@Address", obj.Address);
            com.Parameters.AddWithValue("@Comments", obj.Comments);
            com.Parameters.AddWithValue("@ApplicationDate", time.ToString(format));
            com.Parameters.AddWithValue("@ApplicationUpdateDate", time.ToString(format1));

            conn.Open();
            int result = com.ExecuteNonQuery();
            conn.Close();

            return result > 0;
        }
    }
}