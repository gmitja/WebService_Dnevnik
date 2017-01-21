using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Webservice
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {

        string conString = @"Data Source=.\SQLEXPRESS;Initial Catalog=test;Integrated Security=True";
        public string   Welcom(string name)
        {
            return string.Format("Hi {0}", name);
        }

        public string VrniVseArtikle()
        {    
            string vrni = "";
            SqlDataReader sdr;
            using(SqlConnection conn=new SqlConnection(conString))
            {
                conn.Open();
                SqlCommand com = new SqlCommand("SELECT * FROM ARTIKLI",conn);
                sdr = com.ExecuteReader();
                while (sdr.Read())
                {
                    vrni +=sdr["ID"].ToString() + "," + sdr["EAN"].ToString() + ","+sdr["NAZIV"].ToString()+sdr["KOLICINA"].ToString()+";";
                }
                sdr.Close();
                conn.Close();
            }

            return vrni;
        }


        public string VstaviRokUporabe(string ean,string datum_uporabe)
        {
            SqlDataReader sdr, sdr1;
            string koda_ean = "";
            //string[] polje = p.Split('-');
            koda_ean = ean;
            string datum_roka = datum_uporabe;
            using (SqlConnection connection = new SqlConnection(conString))
            {
                int index = 0, id_artikla = 0;
                connection.Open();
                SqlCommand com = new SqlCommand("SELECT MAX(Id) FROM Dnevnik", connection);
                sdr = com.ExecuteReader();
                while (sdr.Read())
                {
                    index = int.Parse(sdr[0].ToString());
                }
                index++;
                sdr.Close();
                SqlCommand com1 = new SqlCommand("SELECT ID FROM ARTIKLI WHERE EAN='" + koda_ean + "'", connection);
                sdr1 = com1.ExecuteReader();
                while (sdr1.Read())
                {
                    id_artikla = int.Parse(sdr1[0].ToString());
                }

                DateTime dt = Convert.ToDateTime(datum_roka);
                sdr1.Close();
                DateTime zdaj = DateTime.Now;
                connection.Close();
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Dnevnik (Id, Datum_vnosa, Datum_roka, Id_artikla) VALUES (@id, @datum_vnosa, @datum_roka,@id_a)", connection);
                cmd.Parameters.AddWithValue("@id", index);
                cmd.Parameters.AddWithValue("@datum_vnosa", zdaj);
                cmd.Parameters.AddWithValue("@datum_roka", dt);
                cmd.Parameters.AddWithValue("@id_a", id_artikla);

                cmd.ExecuteNonQuery();

                connection.Close();

            }
            return "odlicno;dolicno";
        }

        public string VrniDnevnik()
        {
            string vrni = "";
            SqlDataReader sdr;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                conn.Open();
                SqlCommand com = new SqlCommand("SELECT A.EAN,A.NAZIV, A.KOLICINA,d.Datum_vnosa, d.Datum_roka FROM ARTIKLI A INNER JOIN Dnevnik d ON  A.ID=d.Id_artikla", conn);
                sdr = com.ExecuteReader();
                while (sdr.Read())
                {
                    DateTime dt =(DateTime.Parse(sdr["Datum_vnosa"].ToString()));
                    string datum_vnosa = dt.Date.ToString("d");

                    DateTime dt1 = (DateTime.Parse(sdr["Datum_roka"].ToString()));
                    string datum_roka = dt1.Date.ToString("d");
                    vrni += sdr["EAN"].ToString()+">"+sdr["NAZIV"].ToString() + ">" + sdr["KOLICINA"].ToString() + ">" +datum_vnosa+">"+ datum_roka  + ";";
                }
                sdr.Close();
                conn.Close();
            }

            return vrni;
        }
    
    }
}
