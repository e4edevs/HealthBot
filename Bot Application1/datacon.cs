using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HealthBot
{
    public class datacon
    {
        public DataSet ds = new DataSet();
        //MySqlConnection con;
        MySqlDataAdapter lda, ada, adda, fda;
        public string nm;

        public datacon()
        {
            var con = new MySqlConnection("Server=localhost;Database=healthbot;Uid=root;Password=;Connection Timeout = 15");
            con.Open();
            con.Close();

            string sql = "select * from ailments";
            ada = new MySqlDataAdapter(sql, con);
            ada.Fill(ds, "ailments");

            sql = "select * from foods";
            fda = new MySqlDataAdapter(sql, con);
            fda.Fill(ds, "foods");

            sql = "select * from advice";
            adda = new MySqlDataAdapter(sql, con);
            adda.Fill(ds, "advice");

            sql = "select * from logs";
            lda = new MySqlDataAdapter(sql, con);
            lda.Fill(ds, "logs");

            nm = "Connection established";
        }

        public void savequery(string query, string intent, string reply)
        {
            MySqlCommandBuilder cb = new MySqlCommandBuilder(lda);
            DataRow dr = ds.Tables["logs"].NewRow();
            dr["chatQuery"] = query;
            dr["chatIntent"] = intent;
            dr["chatReply"] = reply;
            dr["timestamp"] = DateTime.Now;
            ds.Tables["logs"].Rows.Add(dr);
            lda.Update(ds, "logs");
        }
    }
}