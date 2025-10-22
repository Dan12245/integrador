using BCrypt.Net;
using CRA;
using Npgsql;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Windows.System;
using static C.R.A_Consumo_reducido_de_agua.Registro;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    //esta parte es importane pq aca hacemos el string de conexion no lo borren o nos quedamos sin base de  datos
    class conexion
    {
        //si quieren cambiar de base de datos aca ponen los parametros nuevos y se genera solo
        //pero si la cadena es diferente ps nomas acomodan la wea
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
        //Esta funcion es relleno, solo es para comprobar si hay conexion con la base de datos
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
        //Con esta funcion registramos a un usuario nuevo en la base de datos
        public bool Insertar(string nombre, string email, string password)
        {
            try
            {
                //hacemos nuestra conexion
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                //Aca es donde encriptamos la contraseña usando BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                //escribimos el comando
                string cadena = "INSERT INTO cra.users (name, email, password) VALUES (@Name, @correo, @contra)";
                //hacemos nuestro ejecutor y lo usamos
                using var ejecutor = new NpgsqlCommand(cadena, conex);
                //las variables que vamos a insertar a la base de datos
                ejecutor.Parameters.AddWithValue("@Name", nombre);
                ejecutor.Parameters.AddWithValue("@correo", email);
                ejecutor.Parameters.AddWithValue("@contra", passwordHash);
                ejecutor.ExecuteNonQuery();
                MessageBox.Show("Usuario registrado!");
                //como ultimo paso cerramos conexion
                conex.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + ex.ToString());
                return false;
            }
        }
        //esta funcion es para el login 
        public bool sesion(string email, string contraseña)
        {
            try
            {
                //hacemos la conexion
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                //hacemos el comando pero con variables para que no nos tumben la tabla
                string query = "SELECT name, password FROM cra.users WHERE email = @correo";
                //con el using nos ahorramos tener que cerrar la conexion al final del codigo
                using (var ejecutor = new NpgsqlCommand(query, conex))
                {
                    ejecutor.Parameters.AddWithValue("@correo", email);
                    //usamos el using para no tener que estar cerrando la conexion a cada 5 lineas
                    using (var reader = ejecutor.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Sacamos los datos 
                            string nombre = reader.GetString(0);
                            string passwordHash = reader.GetString(1);

                            // como la contraseña esta encriptada le metemos la funcion de BCrypt para que pueda entender la contra
                            if (BCrypt.Net.BCrypt.Verify(contraseña, passwordHash))
                            {
                                //si la contraseña es correcta entonces devolvemos true
                                GlobalData.UserName = nombre;
                                return true;
                            }
                            else
                            {
                                //y si es incorrecta entonces devolvemos un false
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("No existe un usuario con ese correo.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + ex.ToString());
                return false;
            }
        }
        //Funcion para agregar domicilio
        public bool agregar_domicilio(string email)
        {
            //hacemos nuestra conexion
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                // hacemos la conexion y hacemos una variable para guardar el id del usuario
                int userId = 0;
                //hacemos nuestro query para buscar el id y lo almacenamos en nuestra variable
                string query = "SELECT user_id FROM cra.users WHERE email=@correo";
                using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
                {
                    //ejecutamos el query y guardamos el id
                    command.Parameters.AddWithValue("@correo", email);
                    object result = command.ExecuteScalar();
                    userId = Convert.ToInt32(result);
                }

                // estos datos estan fijos por mientras, uan vez tengamos el boton para agregar datos
                //los cambio
                string alias = "mi casita toda chula";
                string descripcion = "tiene una puerta, un cuarto y no tiene baños";
                // hacemos el query para ingresar los datos del usuario
                query = "INSERT INTO cra.buildings (user_id, alias, description) VALUES (@user_id, @alias, @description)";
                using (NpgsqlCommand ejecutor = new NpgsqlCommand(query, conex))
                {
                    //aca nomas los estamos metiendo
                    ejecutor.Parameters.AddWithValue("@user_id", userId);
                    ejecutor.Parameters.AddWithValue("@alias", alias);
                    ejecutor.Parameters.AddWithValue("@description", descripcion);

                    ejecutor.ExecuteNonQuery();
                }
                conex.Close();
                // y le decimos al usuario que ya la registro
                MessageBox.Show("Domicilio registrado!");
                return true;
                conex.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
        //creo que el nombre explica bien lo que hace la funcion
        public void eliminar_domicilio()
        {
            try
            {
                //este string no importa mucho, solo es para tener algo que borrar, una ves tengamos
                //la opcion de eliminar lo quito
                string alias = "mi casita toda chula";
                // hacemos la conexion y la abrimos
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                //hacemos nuestro query para buscar alias del domicilio a eliminar
                string query = "DELETE FROM cra.buildings WHERE alias=@alias";
                using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
                {
                    command.Parameters.AddWithValue("@alias", alias);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Domicilio eliminado");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        //funcion para ver si el codigo de invitacion existe
        public bool Invitar_usuario()
        {
            try
            {
                //esta variable tambien se puede borrar sin tanto pedo, nomas ando esperando a que
                //suelten la version con los botones para lo demas
                string invitacion = "437208959";
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                string query = "SELECT * FROM cra.users WHERE inv_code=@inv_code";
                using (var ejecutor = new NpgsqlCommand(query, conex))
                {
                     ejecutor.Parameters.AddWithValue("@inv_code", invitacion);
                    //usamos el using para no tener que estar cerrando la conexion a cada 5 lineas
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
    }
}
