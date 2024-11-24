using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using MVCVDO.Models;
using System.Data;

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
    }
}