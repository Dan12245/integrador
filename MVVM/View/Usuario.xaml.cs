using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para Usuario.xaml
    /// </summary>
    public partial class Usuario : UserControl
    {
        private TextBox selectedTextBox = null;
        private int domiciliosActivos = 1; // Solo el principal está visible inicialmente  
        private bool estaEditando = false;
        public Usuario()
        {
            InitializeComponent();
            Texto_Nombre.Text = $"{GlobalData.UserName}";
            Barra_meta.Value = new Random().Next(10, 101);

            // Inicializar estado de los TextBox
            InicializarDomicilios();
            ActualizarBotones();
        }

        private void InicializarDomicilios()
        {
            // Hacer que todos los domicilios sean de solo lectura inicialmente
            Domicilio_Principal.IsReadOnly = true;
            Domicilio_1.IsReadOnly = true;
            Domicilio_2.IsReadOnly = true;
            Domilicio_3.IsReadOnly = true;

            // Ocultar los domicilios secundarios
            Domicilio_1.Visibility = Visibility.Collapsed;
            Domicilio_2.Visibility = Visibility.Collapsed;
            Domilicio_3.Visibility = Visibility.Collapsed;

            // Agregar eventos para selección de TextBox
            Domicilio_Principal.GotFocus += TextBox_GotFocus;
            Domicilio_1.GotFocus += TextBox_GotFocus;
            Domicilio_2.GotFocus += TextBox_GotFocus;
            Domilicio_3.GotFocus += TextBox_GotFocus;

            // Agregar eventos para Enter y LostFocus
            Domicilio_Principal.KeyDown += TextBox_KeyDown;
            Domicilio_1.KeyDown += TextBox_KeyDown;
            Domicilio_2.KeyDown += TextBox_KeyDown;
            Domilicio_3.KeyDown += TextBox_KeyDown;

            Domicilio_Principal.LostFocus += TextBox_LostFocus;
            Domicilio_1.LostFocus += TextBox_LostFocus;
            Domicilio_2.LostFocus += TextBox_LostFocus;
            Domilicio_3.LostFocus += TextBox_LostFocus;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            selectedTextBox = sender as TextBox;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb != null && !tb.IsReadOnly)
                {
                    tb.IsReadOnly = true;
                    // Mover el foco a otro elemento para que se dispare LostFocus
                    Boton_Menu.Focus();
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //TextBox tb = sender as TextBox;
            //if (tb != null && !tb.IsReadOnly)
            //{
            //    tb.IsReadOnly = true;
           // }
        }

        private void Button_Agregar(object sender, RoutedEventArgs e)
        {
            if (domiciliosActivos >= 4)
            {
                MessageBox.Show("Has alcanzado el máximo de domicilios permitidos (3 adicionales).",
                    "Límite alcanzado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string user_email = GlobalData.email;
            var con = new conexion();
            int id = GlobalData.userid;
            switch (domiciliosActivos)
            {
                case 1:
                    Domicilio_1.Visibility = Visibility.Visible;
                    domiciliosActivos++;
                     con = new conexion();
                    //con.agregar_domicilio(user_email,"Domicilio 1",id);
                    con.agregar_domicilio_async(user_email, Domicilio_1.Text, id);
                    break;
                case 2:
                    Domicilio_2.Visibility = Visibility.Visible;
                    domiciliosActivos++;
                     con = new conexion();
                    con.agregar_domicilio_async(user_email, Domicilio_2.Text, id);
                    break;
                case 3:
                    Domilicio_3.Visibility = Visibility.Visible;
                    domiciliosActivos++;
                    con = new conexion();
                    con.agregar_domicilio_async(user_email, Domilicio_3.Text, id);
                    break;
            }

            ActualizarBotones();
        }

        private void Button_Quitar(object sender, RoutedEventArgs e)
        {
           
            if (domiciliosActivos <= 1)
            {
                MessageBox.Show("No puedes eliminar el domicilio principal.",
                    "Acción no permitida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string user_email = GlobalData.email;
            var con = new conexion();
            int id = GlobalData.userid;
            // Eliminar el último domicilio visible
            switch (domiciliosActivos)
            {
                case 4:
                    Domilicio_3.Visibility = Visibility.Collapsed;
                    Domilicio_3.Text = "Domicilio 3"; // Resetear texto
                    domiciliosActivos--;
                    con.eliminar_domicilio(Domilicio_3.Text, id);
                    break;
                case 3:
                    Domicilio_2.Visibility = Visibility.Collapsed;
                    Domicilio_2.Text = "Domicilio 2";
                    domiciliosActivos--;
                    con.eliminar_domicilio(Domicilio_2.Text, id) ;
                    break;
                case 2:
                    Domicilio_1.Visibility = Visibility.Collapsed;
                    Domicilio_1.Text = "Domicilio 1";
                    domiciliosActivos--;
                     con.eliminar_domicilio(Domicilio_1.Text, id);
                   

                    break;
            }

            ActualizarBotones();
        }

        private async void Button_Editar(object sender, RoutedEventArgs e)
        {
            //bien, la explicacion de como jala (a medias) esta cosa es que habia una parte del codigo
            //que hacia que el IsReadOnly se leyera como false aunque ya fuera considerado un true esto hacia
            //que el codigo se confundiera y lo tomara como que no hacia nadota y ps tronaba, tmbn por eso
            //ahora esta en un if para separar los casos y que no se confundan de neuvo
            if (selectedTextBox == null)
            {
                MessageBox.Show("Por favor, selecciona primero un domicilio para editar.",
                    "Selección requerida", MessageBoxButton.OK, MessageBoxImage.Information);              
                return;
            }

            // Verificar que el TextBox seleccionado sea visible
            if (selectedTextBox.Visibility != Visibility.Visible)
            {
                MessageBox.Show("El domicilio seleccionado no está visible.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //a partir de aca no jala
            // Habilitar edición
            if (selectedTextBox.IsReadOnly)
            {
                // 1️⃣ Primer click → habilitar edición
                selectedTextBox.IsReadOnly = false;
                selectedTextBox.Focus();
                selectedTextBox.SelectAll();
                MessageBox.Show("Modo edición activado");
            }
            else
            {
                // 2️⃣ Segundo click → guardar cambios
                MessageBox.Show("Guardando cambios...");

                selectedTextBox.IsReadOnly = true;

                conexion con = new conexion();
                string mail = GlobalData.email;
                int iduser = GlobalData.userid;

                try
                {
                    int idedificio = await con.id_edificio(iduser);
                    MessageBox.Show($"Resultado idedificio = {idedificio}");

                    if (await con.editar_domicilio(selectedTextBox.Text, idedificio))
                    {
                        MessageBox.Show("Domicilio editado correctamente.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error dentro del botón: {ex.Message}");
                }
            }
        }

        private void ActualizarBotones()
        {
            // Deshabilitar botón Agregar si ya hay 4 domicilios
            if (domiciliosActivos >= 4)
            {
                Button agregarBtn = FindName("Boton_Agregar") as Button;
                if (agregarBtn == null)
                {
                    agregarBtn = LogicalTreeHelper.FindLogicalNode(this, "Boton_Agregar") as Button;
                }
                // Buscar el botón manualmente si no tiene nombre
                var buttons = FindVisualChildren<Button>(this);
                foreach (var btn in buttons)
                {
                    if (btn.Content?.ToString() == "Agregar")
                    {
                        btn.IsEnabled = false;
                        btn.Opacity = 0.5;
                        break;
                    }
                }
            }
            else
            {
                var buttons = FindVisualChildren<Button>(this);
                foreach (var btn in buttons)
                {
                    if (btn.Content?.ToString() == "Agregar")
                    {
                        btn.IsEnabled = true;
                        btn.Opacity = 1.0;
                        break;
                    }
                }
            }

            // Deshabilitar botón Quitar si solo queda el principal
            if (domiciliosActivos <= 1)
            {
                var buttons = FindVisualChildren<Button>(this);
                foreach (var btn in buttons)
                {
                    if (btn.Content?.ToString() == "Quitar")
                    {
                        btn.IsEnabled = false;
                        btn.Opacity = 0.5;
                        break;
                    }
                }
            }
            else
            {
                var buttons = FindVisualChildren<Button>(this);
                foreach (var btn in buttons)
                {
                    if (btn.Content?.ToString() == "Quitar")
                    {
                        btn.IsEnabled = true;
                        btn.Opacity = 1.0;
                        break;
                    }
                }
            }
        }

        // Método auxiliar para encontrar elementos visuales
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Configuracion());
        }

        private void Boton_ir_a_Invitados(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Invitados());
        }

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Reporte());
        }

        private void Boton_ir_a_Inicio(object sender, RoutedEventArgs e)
        {
            CloseAllCallouts();
            CambiarEscena(new Inicio());
        }

        #region ReportCallouts
        private readonly List<Popup> _calloutPopups = new();

        private void Report_Help_Click(object sender, RoutedEventArgs e)
        {
            if (_calloutPopups.Count > 0)
            {
                CloseAllCallouts();
                return;
            }

            ShowCalloutFor(Boton_Configuracion,
                "Presiona aquí para ir a la ventana principal.\nTambien puedes acceder a la ventana presionando '1'."
                );
            ShowCalloutFor(Boton_Menu,
                "Presiona aquí para ir a la ventana de configuración.\nTambien puedes acceder a la ventana presionando '2'."
                );
            ShowCalloutFor(Boton_Reportar,
                "Presiona aquí para ir a la ventana de reporte de errores.\nTambien puedes acceder a la ventana presionando '4'."
                );
        }

        private void ShowCalloutFor(FrameworkElement target, string message)
        {
            var callout = new CalloutControl
            {
                Text = message,
                IsHitTestVisible = false
            };

            var popup = new Popup
            {
                Child = callout,
                PlacementTarget = target,
                Placement = PlacementMode.Right,
                HorizontalOffset = 10,
                VerticalOffset = 0,
                StaysOpen = true,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade
            };

            EventHandler layoutHandler = (_, __) => popup.HorizontalOffset += 0;
            target.LayoutUpdated += layoutHandler;

            popup.Closed += (_, __) =>
            {
                target.LayoutUpdated -= layoutHandler;
                _calloutPopups.Remove(popup);
                popup.Child = null;
            };

            _calloutPopups.Add(popup);
            popup.IsOpen = true;
        }

        private void CloseAllCallouts()
        {
            foreach (var p in _calloutPopups.ToArray())
            {
                p.IsOpen = false;
            }
            _calloutPopups.Clear();
        }
        #endregion
    }
}
