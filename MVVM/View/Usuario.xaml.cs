using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using MiApp;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Invitados;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Usuario.xaml
    /// </summary>
    public partial class Usuario : UserControl
    {
        conexion con = new conexion();
        /// <summary>
        /// creo que es la primera vez que voy a hacer notas asi queeee....
        /// 
        /// Este es el registro para hacer que el data grid agarre domicilios mediante los botones
        /// de agregar, eliminar y editar :p
        /// 
        /// como primer paso, agregamos una public class Domicilios con los atributos que queremos
        /// agregar al data grid :p
        /// </summary>
        public class Domicilio
        {
            public int Id { get; set; } //este es solo para la base de datos (o eso me dijo dani)
            public string Nombre { get; set; } //nombre para que el usuario identifique su domicilio
            public string Descripcion { get; set; } //y la descripcion, meramente de relleno(¿
        }

        // Colección que alimenta el DataGrid.
        private ObservableCollection<Domicilio> Domicilios { get; set; } = new();

        //ahora, crearemos una funcion para que agregue domicilios (buscar con CTRL + F "private void AgregarDomicilio()"

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb != null && !tb.IsReadOnly)
                {
                    tb.IsReadOnly = true;
                    // En lugar de Boton_Menu (no existe en este UserControl) movemos el foco al DataGrid
                    miDataGrid.Focus();
                }
            }
        }

        private const string FilePath = "Domicilios.json";
        public bool Editor = false;
        public Usuario()
        {
            InitializeComponent();

            Domicilios = CargarDatos();
            miDataGrid.ItemsSource = Domicilios;

            if (Registro.GlobalData.UserName == null)
                Registro.GlobalData.UserName = "Usuario";

            InicializarTimer();
            ActualizarTextBox();

            // Corregido: referencia explícita a Registro.GlobalData
            Texto_Nombre.Text = $"{Registro.GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);
            Datos_Usuario.Text = $"Invitados: " + new Random().Next(0, 101);

            ActualizarImagen();
        }


        //funcion para actualizar la imagen del icono de editar/guardar
        private void ActualizarImagen()
        {
            //carga la ruta dependiendo de si estas en modo editor o no
            string ruta = Editor ? "/Images/IconoGuardado.png" : "/Images/Editar.png";
            //y actualiza la imagen del boton
            BotonImagen.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }


        private DispatcherTimer timer;
        private void InicializarTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromHours(1); // Actualiza cada hora
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        //la funcion para cargar los datos del json, nada especial, solo lee el archivo y lo deserializa en la coleccion de domicilios
        private ObservableCollection<Domicilio> CargarDatos()
        {
            try

            {
                //detecta si es que el archivo existe
                if (File.Exists(FilePath))
                {
                    //si es asi, lee el archivo y deserializa el json en una coleccion de domicilios
                    string json = File.ReadAllText(FilePath);
                    var lista = JsonSerializer.Deserialize<ObservableCollection<Domicilio>>(json);
                    return lista ?? new ObservableCollection<Domicilio>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
            return new ObservableCollection<Domicilio>();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ActualizarTextBox();
        }

        /// <summary>
        /// Esta funcion se utilizara a la hora de usar el boton de agregar domicilio
        /// asi queeeeeeee, toca ver como funciona :p
        /// </summary>

        private void EliminarDomicilio()
        {
            //detecta si es que tienes seleccionado algun elemento del data grid
            if (miDataGrid.SelectedItem != null)
            {
                //si es asi, declara una variable domicilio que sera igual al domicilio seleccionado
                var domicilio = (Domicilio)miDataGrid.SelectedItem;
                //y elimina ese domicilio de la coleccion de domicilios
                Domicilios.Remove(domicilio);
            }
        }

        private void ActualizarTextBox()
        {
            //cosa que calcula los dias restantes del mes, no se, esta chistoso xd
            DateTime hoy = DateTime.Now;
            int ultimoDia = DateTime.DaysInMonth(hoy.Year, hoy.Month);
            int diasRestantes = ultimoDia - hoy.Day + 1;

            Dias_restantes.Text = $"{diasRestantes} días restantes";
        }

        /// <summary>
        /// la funcion guardar datos es para basicamente, guardar datos (duh)
        /// aqui te va toda la pinche mierda que es esto
        /// </summary>
        private void GuardarDatos()
        {
            try
            {
                //esto creo el json, como? no se, preguntale a visual xd
                string json = JsonSerializer.Serialize(Domicilios, new JsonSerializerOptions { WriteIndented = true });
                //y esto lo guarda en un archivo llamado domicilios.json
                File.WriteAllText(FilePath, json);
                MessageBox.Show("Datos guardados correctamente.");
            }
            catch (Exception ex)
            {
                //errorsito por si aca, no vaya a ser el viablo :anguished:
                MessageBox.Show($"Error al guardar datos: {ex.Message}");
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Boton_Agregar2_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new AgregarDomicilio();

            // ShowDialog() devuelve true si presionaron Aceptar
            if (dialogo.ShowDialog() == true)
            {
                // Obtener nuevo ID
                int nuevoId = Domicilios.Count > 0 ? Domicilios.Max(p => p.Id) + 1 : 1;

                // Crear nueva persona con los datos del diálogo

                ///primero que nada, declaramos una variable llamada NuevoDomicilio como un nuevo domicilio (omg)
                var NuevoDomicilio = new Domicilio
                {
                    //hacemos que el Id sea igual a la cantidad de Domicilios + 1 (asi no se repiten los Ids)
                    Id = Domicilios.Count + 1,
                    //declaramos el nombre como un nuevo domicilio
                    Nombre = dialogo.Nombre,
                    //y la descripcion como un campo vacio para que se pueda editar a gusto del usuario
                    Descripcion = dialogo.Descripcion
                };
                //finalmente, agregamos el nuevo domicilio a la coleccion de Domicilios
                Domicilios.Add(NuevoDomicilio);

                MessageBox.Show("Domicilio agregado exitosamente!", "Éxito",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
            }
            GuardarDatos();
        }

        private void Boton_Editar2_Click(object sender, RoutedEventArgs e)
        {
            Editor = !Editor;
            miDataGrid.IsReadOnly = !Editor;

            if (!Editor)
            {
                
                GuardarDatos();
            }

            if (Editor)
            {
                MessageBox.Show("Modo edición activado");
            }

            ActualizarImagen(); 
        }


        private void Boton_Eliminar2_Click(object sender, RoutedEventArgs e)
        {
            EliminarDomicilio();
            GuardarDatos();
        }
    }
}
