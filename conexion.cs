using C.R.A_Consumo_reducido_de_agua;
using CRA;
using Npgsql;
using System.Windows;
using System.Xml.Linq;
using Windows.System;
using static C.R.A_Consumo_reducido_de_agua.Registro;

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
        //Esta wea esta separada en regiones para que sea mas facil de encontrar una funcion en caso de
        //que se necesite cambiar o algo
        //hablando de aca van las funciones que tienen que ver con el usuario principal de la app
        //digamos el arrendador (o arrendatario, no me acuerdo cual es cual y me da hueva buscar)
        #region Usuario principal
        //Con esta funcion registramos a un usuario nuevo en la base de datos
        public bool registrar_usuario(string nombre, string email, string password)
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
        //esta funcion es para iniciar sesion 
        public bool iniciar_sesion(string email, string contraseña)
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
        //funcion para eliminar a un usuario
        public void Eliminar_usuario(string email)
        {
            //En teoria ya deberia de borrar usuarios, y por ende deberian jalar todas las funciones
            //pq dependian del email, entonces cuando quieran usar una funcion de las que cree solamente
            //agregan esto para que saque el email y puedan usarla sin pedos pq la mayoria depende del
            //email y si acaso un dato extra que ocupen como el consumo o esas weas, pero eliminar un
            //usuario no ocupa tanto. Bno la line que deben agregar es esta:
            //conexion con = new conexion(); esto es para poder usar las funciones, aca no hay pedo con copiar y pegar
            //string user_email = GlobalData.email; esta linea es la importante ya que jala el email para poder usarlo
            //con.Eliminar_usuario(user_email); y ya mandamos llamar a la funcion que hace la chamba de eliminar al usuario
            try
            {

                conex.ConnectionString = cadena_conexion;
                conex.Open();
                //aca nomas hacemos un DELETE ya que postgress puede borrar todo con una configuracion
                //que ya esta activada para borrar todo lo relacionado con la foreign key, en este caso
                //el id del usuario, asi que ya no hay que meter mas que este query
                string query = "DELETE FROM cra.users WHERE email = @email;";
                using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("usuario eliminado");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }
        #endregion
        //aca ponemos funciones relacionadas a los domicilios
        #region domicilio
        //Funcion para agregar domicilio
        public bool agregar_domicilio(string email)
        {
            //hacemos nuestra conexion
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                int userId = id_usuario(email);
                // estos datos estan fijos por mientras, uan vez tengamos el boton para agregar datos
                //los cambio
                string alias = "mi casita toda chula";
                string descripcion = "tiene un colchon que me robé de la calle, un cuarto y no tiene baños";
                // hacemos el query para ingresar los datos del usuario
                string query = "INSERT INTO cra.buildings (user_id, alias, description) VALUES (@user_id, @alias, @description)";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
        //creo que el nombre explica bien lo que hace la funcion
        public void eliminar_domicilio(string alias)
        {
            try
            {
                //este string no importa mucho, solo es para tener algo que borrar, una ves tengamos
                //la opcion de eliminar lo quito
                alias = "mi casita toda chula";
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

        //con esto cambiamos los datos del domicilio que lit nomas es la descripcion y el alias
        //El que quiera cambiar eso nomas es pq le van a checar el fono yo creo
        public void editar_domicilio(string email)
        {
            conex.ConnectionString = cadena_conexion;
            conex.Open();
            int userId = id_usuario(email);
            //jalamos el id del usuario para poder buscar el edificio correcto
            //hacemos nuestro query para buscar el id y lo almacenamos en nuestra variable
            //estas variables se van a pasar por text box pero como no existen todavia se quedan en variables
            string alias = "casa fea";
            string descripcion = "tiene una puerta, un cuarto y tiene 3 baños";
            string query = "UPDATE cra.buildings SET (alias=@alias, description=@description) WHERE user_id=@user_id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
            {
                //ejecutamos el query y guardamos el id
                command.Parameters.AddWithValue("@alias", alias);
                command.Parameters.AddWithValue("@description", descripcion);
                object result = command.ExecuteScalar();
                userId = Convert.ToInt32(result);
            }

        }
        #endregion
        //aca son funciones para sacar los ID de usuario, consumo, casa e invitado
        //borran estas weas y todo truena pq la mayoria de las funciones sacan datos de aca
        #region id
        public int id_usuario(string email)
        {
            try
            {
                string query = "SELECT user_id FROM cra.users WHERE email=@correo";
                using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
                {
                    command.Parameters.AddWithValue("@correo", email);
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
                return -1;
            }

        }
        public int id_edificio(string email)
        {
            int userId = id_usuario(email);
            int building_id = 0;
            //ya con el id del usuario sacamos el del edificio para poder asignarlo en la tabla
            //de consumo y no se meta con otro usuario
            string query = "SELECT building_id FROM cra.buildings WHERE user_id=@user_id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
            {
                //ejecutamos el query y guardamos el id
                command.Parameters.AddWithValue("@user_id", userId);
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
        public int consumo_id(string email)
        {
            int building_id = id_edificio(email);
            int consumo_id = 0;
            string query = "SELECT id FROM cra.consumption_per_day WHERE building_id=@building_id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, conex))
            {
                //ejecutamos el query y guardamos el id
                command.Parameters.AddWithValue("@building_id", building_id);
                object result = command.ExecuteScalar();
                consumo_id = Convert.ToInt32(result);
                return Convert.ToInt32(result);
            }
        }
        #endregion
        //aca van las funciones para los invitados
        #region invitados
        public bool existe_invitacion(string email)
        {
            string invitacion = "437208959";
            conex.ConnectionString = cadena_conexion;
            conex.Open();
            int user_id = id_usuario(email);
            string query = "SELECT COUNT(*) FROM cra.users WHERE inv_code=@inv_code";
            using (var ejecutor = new NpgsqlCommand(query, conex))
            {
                ejecutor.Parameters.AddWithValue("@inv_code", invitacion);
                int existe = Convert.ToInt32(ejecutor.ExecuteScalar());
                if (existe == 0)
                {
                    MessageBox.Show("Código de invitación no válido.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        //no se si esta funcion va aca o neh, asi que por mientras se queda aca
        public bool Invitar_usuario(string email, string name, bool y_o_n)
        {
            try
            {
                //esta variable tambien se puede borrar sin tanto pedo, nomas ando esperando a que
                //suelten la version con los botones para lo demas
                string invitacion = "437208959";
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                int user_id = id_usuario(email);
                if (existe_invitacion(email))
                {
                    string query = "INSERT INTO cra.invited_users (user_id,name,read_only) VALUES (@user_id,@name,@read_only)";
                    using (var ejecutor = new NpgsqlCommand(query, conex))
                    {
                        ejecutor.Parameters.AddWithValue("@user_id", user_id);
                        ejecutor.Parameters.AddWithValue("@name", name);
                        ejecutor.Parameters.AddWithValue("@read_only", y_o_n);
                        ejecutor.ExecuteNonQuery();

                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }
        public void editar_invitado(string email, string name, bool read_only)
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                int user_id = id_usuario(email);
                if (existe_invitacion(email))
                {
                    string query = "UPDATE cra.invited_users SET user_id=@user_id and @read_only=read_only WHERE building_id=@building_id AND day=@day";
                    using (var ejecutor = new NpgsqlCommand(query, conex))
                    {
                        ejecutor.Parameters.AddWithValue("@user_id", user_id);
                        ejecutor.Parameters.AddWithValue("@read_only", read_only);
                        ejecutor.ExecuteNonQuery();
                        MessageBox.Show("Consumo cambiado");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }
        public void eliminar_invitado(string email, string name)
        {
            conex.ConnectionString = cadena_conexion;
            conex.Open();
            try
            {
                int user_id = id_usuario(email);
                if (existe_invitacion(email))
                {
                    string query = "DELETE FROM cra.invited_users WHERE user_id = @user_id and name=@name;";
                    using (var ejecutor = new NpgsqlCommand(query, conex))
                    {
                        ejecutor.Parameters.AddWithValue("@user_id", user_id);
                        ejecutor.Parameters.AddWithValue("@name", name);
                        ejecutor.ExecuteNonQuery();
                    }
                    MessageBox.Show("invitado eliminado");
                }
                else
                {
                    MessageBox.Show("invitado no encontrado");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }

        }
        #endregion
        //y las funciones relacionadas con el consumo
        #region consumo
        public bool agregar_consumo(string email)
        {
            try
            {
                int consumo = 20;
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                int building_id = id_edificio(email);
                //esta wea solo es para meter la fecha
                DateTime fecha = DateTime.Now;
                //y ya con todo eso lo metemos a la tabla de consumo
                string query = "INSERT INTO cra.consumption_per_day (building_id,day,consumption)VALUES(@building_id,@day,@consumption)";
                using (var ejecutor = new NpgsqlCommand(query, conex))
                {
                    ejecutor.Parameters.AddWithValue("@building_id", building_id);
                    ejecutor.Parameters.AddWithValue("@day", fecha);
                    ejecutor.Parameters.AddWithValue("@consumption", consumo);
                    ejecutor.ExecuteNonQuery();
                    MessageBox.Show("Consumo registrado");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
                return false;
            }
        }
        public bool eliminar_consumo()
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                //esta wea despues veo como la cambio para que el usuario no batalle a la hora de eliminar
                //el consumo pq la neta no tengo una idea de como hacerlo de momento
                DateTime fecha = DateTime.Now;
                string query = "DELETE FROM cra.consumption_per_day WHERE day=@day";
                using (var ejecutor = new NpgsqlCommand(query, conex))
                {
                    ejecutor.Parameters.AddWithValue("@day", fecha);
                    ejecutor.ExecuteNonQuery();
                    MessageBox.Show("Consumo eliminado");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
                return false;
            }
        }
        //cambiar un consumo
        public void cambiar_consumo(string email)
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                int building_id = id_edificio(email);
                string query = "UPDATE cra.consumption_per_day SET consumption=@consumption WHERE building_id=@building_id AND day=@day";
                DateTime fecha = DateTime.Now;
                int litros = 4;
                using (var ejecutor = new NpgsqlCommand(query, conex))
                {
                    ejecutor.Parameters.AddWithValue("@consumption", litros);
                    ejecutor.Parameters.AddWithValue("@building_id", building_id);
                    ejecutor.Parameters.AddWithValue("@day", fecha.Date);
                    ejecutor.ExecuteNonQuery();
                    MessageBox.Show("Consumo cambiado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }
        public double[] consumo(string email)
        {   //esta wea es para obtener la cantidad de dias del año
            int year = DateTime.Now.Year;
            //con esto checamos si el año es bisiesto o neh
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            //un arreglo con la cantidad de dias
            double[] consumo = new double[daysInYear];
            conex.ConnectionString = cadena_conexion;
            conex.Open();
            int building_id = id_edificio(email);
            int i = 0;
            string query = "SELECT day,consumption FROM cra.consumption_per_day WHERE @building_id=building_id";
            using (var ejecutor = new NpgsqlCommand(query, conex))
            {
                ejecutor.Parameters.AddWithValue("@building_id", building_id);

                using (var reader = ejecutor.ExecuteReader())
                {
                    //con este ciclo llenamos el arreglo
                    while (reader.Read() && i < consumo.Length)
                    {
                        DateTime dia = reader.GetDateTime(reader.GetOrdinal("day"));
                        double valor = reader.GetDouble(reader.GetOrdinal("consumption"));

                        int index = dia.DayOfYear - 1; // día 1 → índice 0
                        if (index >= 0 && index < consumo.Length)
                            consumo[index] = valor;
                    }
                }
                return consumo;
            }
            #endregion
        }
    }
}