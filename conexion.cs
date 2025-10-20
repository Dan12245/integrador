using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static C.R.A_Consumo_reducido_de_agua.Registro;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    class conexion
    {
        NpgsqlConnection conex = new NpgsqlConnection();
        static string server = "aws-1-us-east-2.pooler.supabase.com";
        static string puerto = "6543";
        static string bd = "postgres";
        static string usuario = "postgres.eyroumbxtugceuwckkyg";
        static string password = "elpiggadekarim";
        /*
         * Datos para acceder a la base de datos
         "User Id=postgres.eyroumbxtugceuwckkyg;Password=[YOUR-PASSWORD];Server=aws-1-us-east-2.pooler.supabase.com;Port=6543;Database=postgres"
        */

        //cadena de conexion
        string cadena_conexion = "User Id=" + usuario + ";" + "Password=" + password + ";" + "Server=" + server + ";" + "Port=" + puerto + ";" + "Database=" + bd;
        public NpgsqlConnection establecer_conexion()
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                MessageBox.Show("conectado con exito");
                conex.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + e.ToString());
            }
            return conex;
        }
        public void Insertar(string nombre, string email, string password)
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                string cadena = "INSERT INTO cra.users (name, email, password) VALUES (@Name, @correo, @contra)";
                using var ejecutor = new NpgsqlCommand(cadena, conex);
                ejecutor.Parameters.AddWithValue("@Name", nombre);
                ejecutor.Parameters.AddWithValue("@correo", email);
                ejecutor.Parameters.AddWithValue("@contra", password);
                ejecutor.ExecuteNonQuery();
                MessageBox.Show("Usuario registrado!");
                conex.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + ex.ToString());
            }
        }
        public bool sesion(string email, string contraseña)
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();

                string query = "SELECT name FROM cra.users WHERE email = @correo AND password = @contra";

                using (var ejecutor = new NpgsqlCommand(query, conex))
                {

                    ejecutor.Parameters.AddWithValue("@correo", email);
                    ejecutor.Parameters.AddWithValue("@contra", contraseña);

                    object count = ejecutor.ExecuteScalar();

                    conex.Close();

                    if (count != null)
                    {
                        // ✅ Usuario encontrado, guarda el nombre globalmente
                        GlobalData.UserName = count.ToString();
                        return true;
                    }
                    else
                    {
                        // ❌ Usuario no encontrado
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + ex.ToString());
                return false;
            }
        }
    }
}
