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
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Usuario;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Invitados.xaml
    /// </summary>
    public partial class Invitados : UserControl
    {
        public class Invitado
        {
            public int Id { get; set; } //este es solo para la base de datos (o eso me dijo dani)
            public string Nombre { get; set; } //nombre para que el usuario identifique a los invitados
            public string Descripcion { get; set; } //y la descripcion, meramente de relleno(¿
        }

        public ObservableCollection<Invitado> Usuario_Invitado { get; set; }
        private const string FilePath = "Invitados.json";
        public bool Editor = false;
        public Invitados()
        {
            InitializeComponent();
            Usuario_Invitado = CargarDatos();
            Grid_Invitados.ItemsSource = Usuario_Invitado;
            ActualizarImagen();
        }

        private ObservableCollection<Invitado> CargarDatos()
        {
            try

            {
                //detecta si es que el archivo existe
                if (File.Exists(FilePath))
                {
                    //si es asi, lee el archivo y deserializa el json en una coleccion de Invitados
                    string json = File.ReadAllText(FilePath);
                    var lista = JsonSerializer.Deserialize<ObservableCollection<Invitado>>(json);
                    return lista ?? new ObservableCollection<Invitado>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
            return new ObservableCollection<Invitado>();
        }

        private void ActualizarImagen()
        {
            //carga la ruta dependiendo de si estas en modo editor o no
            string ruta = Editor ? "/Images/IconoGuardado.png" : "/Images/Editar usuario.png";
            //y actualiza la imagen del boton
            BotonImagen.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }
        private void EliminarInvitado()
        {
            //detecta si es que tienes seleccionado algun elemento del data grid
            if (Grid_Invitados.SelectedItem != null)
            {
                //si es asi, declara una variable domicilio que sera igual al domicilio seleccionado
                var invitado = (Invitado)Grid_Invitados.SelectedItem;
                //y elimina ese domicilio de la coleccion de usuario_invitado
                Usuario_Invitado.Remove(invitado);
            }
        }
        private void GuardarDatos()
        {
            try
            {
                //esto creo el json, como? no se, preguntale a visual xd
                string json = JsonSerializer.Serialize(Usuario_Invitado, new JsonSerializerOptions { WriteIndented = true });
                //y esto lo guarda en un archivo llamado usuario_invitado.json
                File.WriteAllText(FilePath, json);
                MessageBox.Show("Datos guardados correctamente.");
            }
            catch (Exception ex)
            {
                //errorsito por si aca, no vaya a ser el viablo :anguished:
                MessageBox.Show($"Error al guardar datos: {ex.Message}");
            }
        }

        private void Boton_Agregar_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoAgregarPersona();

            // ShowDialog() devuelve true si presionaron Aceptar
            if (dialogo.ShowDialog() == true)
            {
                // Obtener nuevo ID
                int nuevoId = Usuario_Invitado.Count > 0 ? Usuario_Invitado.Max(p => p.Id) + 1 : 1;

                // Crear nueva persona con los datos del diálogo

                ///primero que nada, declaramos una variable llamada NuevoDomicilio como un nuevo domicilio (omg)
                var NuevoInvitado = new Invitado
                {
                    //hacemos que el Id sea igual a la cantidad de usuario_invitado + 1 (asi no se repiten los Ids)
                    Id = Usuario_Invitado.Count + 1,
                    //declaramos el nombre como un nuevo domicilio
                    Nombre = dialogo.Nombre,
                    //y la descripcion como un campo vacio para que se pueda editar a gusto del usuario
                    Descripcion = dialogo.Descripcion
                };
            //finalmente, agregamos el nuevo domicilio a la coleccion de usuario_invitado
            Usuario_Invitado.Add(NuevoInvitado);

            MessageBox.Show("Persona agregada exitosamente!", "Éxito",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            GuardarDatos();
        }

        private void Boton_Editar_Click(object sender, RoutedEventArgs e)
        {
            Editor = !Editor;
            Grid_Invitados.IsReadOnly = !Editor;

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

        private void Boton_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarInvitado();
            GuardarDatos();
        }
    }
}
