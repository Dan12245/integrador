using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;
using Npgsql;
using NpgsqlTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Windows.System;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;
using static QuestPDF.Helpers.Colors;
using static SkiaSharp.HarfBuzz.SKShaper;

//ignoren este comentario solo es para llegar a las 600 lineas
//_. . ..._ . ._. __. ___ _. _. ._ __. .. ..._ . _.__ ___ .._ .._ .__. 

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    //esta parte es importane pq aca hacemos el string de conexion no lo borren o nos quedamos sin base de datos
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
        public string cadenaconexion()
        {
            string cadena_conexion = "User Id=" + usuario + ";" + "Password=" + password + ";" + "Server=" + server + ";" + "Port=" + puerto + ";" + "Database=" + bd;
            return cadena_conexion;
        }
        //cadena de conexion
        string cadena_conexion = "User Id=" + usuario + ";" + "Password=" + password + ";" + "Server=" + server + ";" + "Port=" + puerto + ";" + "Database=" + bd;
        //Esta funcion es relleno, solo es para comprobar si hay conexion con la base de datos
        public NpgsqlConnection establecer_conexion()
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                // Removed MessageBox de prueba
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
        public async Task <bool> registrar_usuario(string nombre, string email, string password)
        {
            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                   await con.OpenAsync();

                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    string cadena = "INSERT INTO cra.users (name, email, password) VALUES (@Name, @correo, @contra) RETURNING user_id;";

                   await using (var ejecutor = new NpgsqlCommand(cadena, con))
                    {
                        ejecutor.Parameters.AddWithValue("@Name", nombre);
                        ejecutor.Parameters.AddWithValue("@correo", email);
                        ejecutor.Parameters.AddWithValue("@contra", passwordHash);

                        object result = await ejecutor.ExecuteScalarAsync();
                        if (result != null)
                        {
                            GlobalData.userid = Convert.ToInt32(result);
                            MessageBox.Show("Usuario registrado!");
                            return true;
                        }
                        else
                        {                            
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex); return false;
            }
        }
        //esta funcion es para iniciar sesion 
        public async Task<bool> iniciar_sesion(string email, string contraseña)
        {
            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                   await con.OpenAsync();

                    string query = "SELECT user_id, name, password FROM cra.users WHERE email = @correo";
                    await using (var ejecutor = new NpgsqlCommand(query, con))
                    {
                        ejecutor.Parameters.AddWithValue("@correo", email);

                        await using (var reader = await ejecutor.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string nombre = reader.GetString(1);
                                string passwordHash = reader.GetString(2);

                                if (BCrypt.Net.BCrypt.Verify(contraseña, passwordHash))
                                {
                                    GlobalData.userid = userId;
                                    GlobalData.UserName = nombre;
                                    GlobalData.email = email;
                                    return true;
                                }
                                else
                                {
                                    MessageBox.Show("Contraseña incorrecta.");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:"+ex);
                return false;
            }
        }

        //funcion para eliminar a un usuario
        public async void Eliminar_usuario(string email)
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
               await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    await con.OpenAsync();
                    //aca nomas hacemos un DELETE ya que postgress puede borrar todo con una configuracion
                    //que ya esta activada para borrar todo lo relacionado con la foreign key, en este caso
                    //el id del usuario, asi que ya no hay que meter mas que este query
                    string query = "DELETE FROM cra.users WHERE email = @email;";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("usuario eliminado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }
        #endregion
        //aca ponemos funciones relacionadas a los domicilios
        #region domicilio
        //Funcion para agregar domicilio
        public async Task<int> agregar_domicilio(string alias, string descripcion, int userId)
        {
            MessageBox.Show("si entró padrino");
            try
            {
                await using var conexion = new NpgsqlConnection(cadenaconexion());
                await conexion.OpenAsync();

                string query = @"INSERT INTO cra.buildings (alias, description, user_id) 
                        VALUES (@alias, @description, @user_id) 
                        RETURNING building_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@alias", alias);
                command.Parameters.AddWithValue("@description", descripcion ?? "");
                command.Parameters.AddWithValue("@user_id", userId);

                // Obtener el ID generado
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }            
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        //creo que el nombre explica bien lo que hace la funcion
        public async void eliminar_domicilio(string alias, int user_id)
        {
            try
            {             
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    //hacemos nuestro query para buscar alias del domicilio a eliminar               
                    await con.OpenAsync();
                    string query = "DELETE FROM cra.buildings WHERE alias=@alias and user_id=@user_id";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@alias", alias);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }

        //con esto cambiamos los datos del domicilio que lit nomas es la descripcion y el alias
        //El que quiera cambiar eso nomas es pq le van a checar el fono yo creo
        public async Task<bool> editar_domicilio(string alias,string descripcion, int buildingId)
        {
          
            try
            {
               
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    //hacemos nuestro query para buscar alias del domicilio a eliminar               
                    await con.OpenAsync();
                    //jalamos el id del usuario para poder buscar el edificio correcto
                    //hacemos nuestro query para buscar el id y lo almacenamos en nuestra variable
                    //estas variables se van a pasar por text box pero como no existen todavia se quedan en variables
                    string query = "UPDATE cra.buildings SET alias=@alias, description=@description WHERE building_id=@buildingId";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        //ejecutamos el query y guardamos el id
                        command.Parameters.AddWithValue("@alias", alias);
                        command.Parameters.AddWithValue("@description", descripcion);
                        command.Parameters.AddWithValue("@buildingId", buildingId);
                        object result = await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return false;
            }

        }
        #endregion
        //aca son funciones para sacar los ID de usuario, consumo, casa e invitado
        //borran estas weas y todo truena pq la mayoria de las funciones sacan datos de aca
        #region id
        public async Task<int> id_usuario(string email)
        {
            NpgsqlConnection.ClearAllPools();
            try
            {
                await using var cone = new NpgsqlConnection(cadena_conexion);
                await cone.OpenAsync();                
                string query = "SELECT user_id FROM cra.users WHERE email = @correo";
                await using var command = new NpgsqlCommand(query, cone);
                command.Parameters.AddWithValue("@correo", email);

                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        public async Task<int> id_edificio(int userId, string alias)
        {
            try
            {                
                await using var con = new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();

                string query = "SELECT building_id FROM cra.buildings WHERE user_id = @user_id and alias = @alias";
                await using var command = new NpgsqlCommand(query, con);
                command.Parameters.AddWithValue("@user_id", userId);
                command.Parameters.AddWithValue("@alias", alias.Trim());
                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        public async Task<int> id_consumo(int building_id)
        {
            try
            {
                await using var con = new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();

                string query = "SELECT id FROM cra.consumption_per_day WHERE building_id = @building_id";
                await using var command = new NpgsqlCommand(query, con);
                command.Parameters.AddWithValue("@building_id", building_id);

                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }
        public async Task<int> id_invitado(int userid, string name)
        {
            NpgsqlConnection.ClearAllPools();
            try
            {
                await using var cone = new NpgsqlConnection(cadena_conexion);
                await cone.OpenAsync();
                string query = "SELECT inv_user_id FROM cra.invited_users WHERE user_id = @user_id and name=@name";
                await using var command = new NpgsqlCommand(query, cone);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@name", name);
                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        #endregion
        //aca van las funciones para los invitados
        #region invitados
        public async Task<bool> existe_invitacion(int user_id, string invitacion)
        {
            try
            {
                await using var con = new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();

                string query = "SELECT COUNT(*) FROM cra.users WHERE inv_code = @inv_code AND user_id = @user_id";

                await using var ejecutor = new NpgsqlCommand(query, con);
                ejecutor.Parameters.AddWithValue("@inv_code", invitacion);
                ejecutor.Parameters.AddWithValue("@user_id", user_id);

                int existe = Convert.ToInt32(await ejecutor.ExecuteScalarAsync()); 

                if (existe == 0)
                {
                    MessageBox.Show("Código de invitación no válido.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar código: {ex.Message}");
                return false;
            }
        }
        //no se si esta funcion va aca o neh, asi que por mientras se queda aca
        public async Task<int> agregar_invitado(string nombre, bool readOnly)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(cadena_conexion);
                await conexion.OpenAsync();

                string query = @"INSERT INTO cra.invited_users (user_id, name, read_only) 
                                VALUES (@user_id, @name, @read_only) 
                                RETURNING inv_user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@user_id", Login.userid);
                command.Parameters.AddWithValue("@name", nombre);
                command.Parameters.AddWithValue("@read_only", readOnly);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar invitado: {ex.Message}");
                return -1;
            }
        }
        public async Task<bool> editar_invitado(int invUserId, string nuevoNombre, bool readOnly)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(cadena_conexion);
                await conexion.OpenAsync();

                string query = @"UPDATE cra.invited_users 
                                SET name = @name, read_only = @read_only 
                                WHERE inv_user_id = @inv_user_id AND user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@name", nuevoNombre);
                command.Parameters.AddWithValue("@read_only", readOnly);
                command.Parameters.AddWithValue("@inv_user_id", invUserId);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                int filasAfectadas = await command.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar invitado: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> eliminar_invitado(int invUserId)
        {
            try
            {
                await using var conexion = new NpgsqlConnection(cadena_conexion);
                await conexion.OpenAsync();

                string query = "DELETE FROM cra.invited_users WHERE inv_user_id = @inv_user_id AND user_id = @user_id";

                await using var command = new NpgsqlCommand(query, conexion);
                command.Parameters.AddWithValue("@inv_user_id", invUserId);
                command.Parameters.AddWithValue("@user_id", Login.userid);

                int filasAfectadas = await command.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar invitado: {ex.Message}");
                return false;
            }
        }
        #endregion
        #region Reportes
        public async void reportes(int user_id, string type, string description)
        {
            try
            {
                await using var con= new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();
                int id = Login.userid;
                string com = "INSERT INTO cra.bugs (user_id, type, description) VALUES (@user_id, @type, @description)";
                await using (var ejecutor = new NpgsqlCommand(com, con))
                {
                    ejecutor.Parameters.AddWithValue("@user_id", id);                    
                    ejecutor.Parameters.AddWithValue("@type", type);
                    ejecutor.Parameters.AddWithValue("@description", description);
                    await ejecutor.ExecuteNonQueryAsync();
                }
                MessageBox.Show("Error enviado");
            }
            catch (Exception ex) {
                MessageBox.Show("error, "+ex.ToString());
            }
        }
        #endregion
        //y las funciones relacionadas con el consumo

        #region metodos edicion de consumo
        public async Task<List<(int Id, string Alias)>> GetBuildingsForUser(int userId)
         {
             var result = new List<(int, string)>();
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 string sql = "SELECT building_id, COALESCE(alias,'Edificio '||building_id) FROM cra.buildings WHERE user_id=@uid ORDER BY building_id";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@uid", userId);
                 await using var reader = await cmd.ExecuteReaderAsync();
                 while (await reader.ReadAsync())
                 {
                     int bid = reader.GetInt32(0);
                     string alias = reader.GetString(1);
                     result.Add((bid, alias));
                 }
             }
             catch (Exception ex) { Console.WriteLine("GetBuildingsForUser error:" + ex); }
             return result;
         }

         public async Task<List<(DateTime Day, double Consumption)>> GetConsumptionForBuilding(int buildingId, int year)
         {
             var result = new List<(DateTime, double)>();
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 DateTime start = new DateTime(year,1,1);
                 DateTime end = new DateTime(year,12,31);
                 string sql = "SELECT day, consumption FROM cra.consumption_per_day WHERE building_id=@bid AND day BETWEEN @d1 AND @d2 ORDER BY day";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@bid", buildingId);
                 cmd.Parameters.Add("@d1", NpgsqlDbType.Date).Value = start.Date;
                 cmd.Parameters.Add("@d2", NpgsqlDbType.Date).Value = end.Date;
                 await using var reader = await cmd.ExecuteReaderAsync();
                 while (await reader.ReadAsync())
                 {
                     var day = reader.GetDateTime(0);
                     // consumption es float4 (real) -> leer como Single y convertir a double
                     float consF = reader.GetFloat(1);
                     result.Add((day, (double)consF));
                 }
             }
             catch (Exception ex) { Console.WriteLine("GetConsumptionForBuilding error:" + ex); }
             return result;
         }
         public async Task<bool> UpsertConsumption(int buildingId, DateTime day, double consumption)
         {
             try
             {
                 // asegurar índice único (userId + buildingId) antes de insertar
                 await EnsureConsumptionUniqueIndexAsync();
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 var dateVal = day.Date;
                 var realVal = (float)Math.Round(consumption,2, MidpointRounding.AwayFromZero);
                 string sql = "INSERT INTO cra.consumption_per_day(building_id, day, consumption) VALUES(@b,@d,@c) ON CONFLICT (building_id, day) DO UPDATE SET consumption=EXCLUDED.consumption";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@b", buildingId);
                 cmd.Parameters.Add("@d", NpgsqlDbType.Date).Value = dateVal;
                 cmd.Parameters.Add("@c", NpgsqlDbType.Real).Value = realVal;
                 await cmd.ExecuteNonQueryAsync();
                 return true;
             }
             catch (Exception ex)
             {
                 Console.WriteLine("UpsertConsumption error:" + ex);
                 return false;
             }
         }
         public async Task<bool> DeleteConsumption(int buildingId, DateTime day)
         {
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 string sql = "DELETE FROM cra.consumption_per_day WHERE building_id=@b AND day=@d";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@b", buildingId);
                 cmd.Parameters.Add("@d", NpgsqlDbType.Date).Value = day.Date;
                 int affected = await cmd.ExecuteNonQueryAsync();
             return affected >0;
             }
                 catch (Exception ex) { Console.WriteLine("DeleteConsumption error:" + ex); return false; }
         }
         #endregion
         // Asegura el índice único requerido para ON CONFLICT (buildingID + UserID)

         public async Task EnsureConsumptionUniqueIndexAsync()
         {
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 const string sql = "CREATE UNIQUE INDEX IF NOT EXISTS ux_consumption_building_day ON cra.consumption_per_day (building_id, day)";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 await cmd.ExecuteNonQueryAsync();
             }
             catch (Exception ex)
             {
                 Console.WriteLine("EnsureConsumptionUniqueIndexAsync error:" + ex);
             }
         }
    }
}
