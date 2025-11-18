using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IOPath = System.IO.Path;
using System.ComponentModel;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.Services;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel;
using System.Collections.Generic;
using System.Windows.Input;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.View
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class UserData


        {
            private static string folder = IOPath.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "C.R.A"
            );

            private static string filePath = IOPath.Combine(folder, "UserData.json");

            // Datos básicos
            public static string UserName { get; set; } = "";
            public static string ProfileImagePath { get; set; } = "";

            // Configuraciones
            public static string Nota { get; private set; } =
             "ATENCION! en caso de modificarse este archivo, su cuenta sera automaticamente baneada de la aplicacion, proceda con precaucion";

            public static bool Grafica { get; set; } = false;
            public static bool Frecuencia { get; set; } = false;
            public static bool Uso { get; set; } = false;
            public static string Rendimiento { get; set; } = "";
            public static bool Notificaciones { get; set; } = true;
            public static bool Invitados { get; set; } = false;
            public static string Idioma { get; set; } = "";
            public static int Tamaño { get; set; } =0;

            public static int Meta { get; set; } =0;

            public static int Domicilios { get; set; } =0;
            public static string Domicilio { get; set; } = "";
            public static bool Propio { get; set; } = false;



            public static void Save()
            {
                Directory.CreateDirectory(folder);

                var json = JsonSerializer.Serialize(new
                {
                    //si agregas una nueva variable, asegurate de agregarla aqui tambien
                    Nota,
                    UserName,
                    ProfileImagePath,
                    Domicilios,
                    Grafica,
                    Meta,
                    Frecuencia,
                    Uso,
                    Rendimiento,
                    Notificaciones,
                    Invitados,
                    Idioma,
                    Tamaño,
                    Domicilio,
                    Propio
                }, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(filePath, json);
            }

            public static void Load()
            {
                if (!File.Exists(filePath))
                    return; // Mantener valores por defecto si no existe archivo

                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                UserName = data.GetProperty("UserName").GetString() ?? "";
                ProfileImagePath = data.GetProperty("ProfileImagePath").GetString() ?? "";

                Meta = data.GetProperty("Meta").GetInt32();

                Grafica = data.GetProperty("Grafica").GetBoolean();
                Frecuencia = data.GetProperty("Frecuencia").GetBoolean();
                Uso = data.GetProperty("Uso").GetBoolean();
                Rendimiento = data.GetProperty("Rendimiento").GetString() ?? "";
                Notificaciones = data.GetProperty("Notificaciones").GetBoolean();
                Invitados = data.GetProperty("Invitados").GetBoolean();
                Idioma = data.GetProperty("Idioma").GetString() ?? "";
                Tamaño = data.GetProperty("Tamaño").GetInt32();
                Domicilio = data.GetProperty("Domicilio").GetString() ?? "";
                Propio = data.GetProperty("Propio").GetBoolean();
            }
        }

        private INotifyPropertyChanged? _navigationInpc;
        private bool _isStartupAnimating = true; // fuerza ocultar menú durante animación

        // Track keybindings we create so we can add/remove them based on current view
        private readonly List<KeyBinding> _menuKeyBindings = new();

        public MainWindow()
        {
            InitializeComponent();
            // DataContext is set by DI in App.xaml.cs
            UserData.Save();

            Loaded += MainWindow_Loaded;
            DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AttachNavigationListener();
            UpdateMenuVisibilityForCurrentView();

            // Register key bindings now that the window is loaded (DataContext should be available)
            RegisterMenuKeyBindings();

            // Ensure bindings reflect the current view (login/register should block shortcuts)
            ApplyMenuKeyBindingsState();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DetachNavigationListener();
            AttachNavigationListener();
            UpdateMenuVisibilityForCurrentView();

            // Re-register key bindings when DataContext changes
            RegisterMenuKeyBindings();

            // Ensure bindings reflect the current view after re-register
            ApplyMenuKeyBindingsState();
        }

        private void AttachNavigationListener()
        {
            if (DataContext is Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel.MainViewModel shell && shell.Navigation is INotifyPropertyChanged inpc)
            {
                _navigationInpc = inpc;
                _navigationInpc.PropertyChanged += Navigation_PropertyChanged;
            }
        }

        private void DetachNavigationListener()
        {
            if (_navigationInpc != null)
            {
                _navigationInpc.PropertyChanged -= Navigation_PropertyChanged;
                _navigationInpc = null;
            }
        }

        private void Navigation_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(INavigationService.CurrentView))
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateMenuVisibilityForCurrentView();
                    // Update key bindings when navigation target changes (block on Login/Registro)
                    ApplyMenuKeyBindingsState();
                });
            }
        }

        private object? GetCurrentViewModel()
        {
            if (DataContext is Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel.MainViewModel shell)
            {
                return shell.Navigation?.CurrentView;
            }
            return null;
        }

        private void UpdateMenuVisibilityForCurrentView()
        {
            var vm = GetCurrentViewModel();
            // ocultar siempre durante la animación; luego según la vista
            bool hide = _isStartupAnimating || vm is LoginViewModel || vm is RegistroViewModel;

            if (FindName("LeftMenu") is FrameworkElement menu)
            {
                menu.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
                menu.IsEnabled = !hide;
                menu.IsHitTestVisible = !hide;
            }

            if (FindName("MenuColumn") is ColumnDefinition col)
            {
                col.Width = hide ? new GridLength(0) : GridLength.Auto;
            }
        }

        /// <summary>
        /// Register (or re-register) window-level Ctrl+1..Ctrl+4 keybindings to navigate menu views.
        /// KeyBindings are created and tracked here, but only added to InputBindings when the
        /// current view is not Login or Registro. This lets us block shortcuts while on those views.
        /// Ctrl+1 => Inicio
        /// Ctrl+2 => Configuracion
        /// Ctrl+3 => Usuario
        /// Ctrl+4 => Reporte
        /// </summary>
        private void RegisterMenuKeyBindings()
        {
            // Remove previous bindings we added from InputBindings and clear our list
            foreach (var kb in _menuKeyBindings.ToArray())
            {
                if (InputBindings.Contains(kb)) InputBindings.Remove(kb);
            }
            _menuKeyBindings.Clear();

            if (DataContext is Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel.MainViewModel shell)
            {
                // Helper to create a binding and track it (but do NOT necessarily add to InputBindings yet)
                void CreateAndTrack(ICommand? cmd, Key key)
                {
                    if (cmd == null) return;
                    var gesture = new KeyGesture(key, ModifierKeys.Control);
                    var binding = new KeyBinding(cmd, gesture);
                    _menuKeyBindings.Add(binding);
                }

                CreateAndTrack(shell.NavigateToInicioCommand, Key.D1);
                CreateAndTrack(shell.NavigateToConfiguracionCommand, Key.D2);
                CreateAndTrack(shell.NavigateToUsuarioCommand, Key.D3);
                CreateAndTrack(shell.NavigateToReporteCommand, Key.D4);
            }
        }

        /// <summary>
        /// Apply (add/remove) the tracked key bindings to the window input bindings based on current view.
        /// If current view is LoginViewModel or RegistroViewModel we remove the bindings to block shortcuts.
        /// Otherwise we ensure the bindings are present so shortcuts work.
        /// </summary>
        private void ApplyMenuKeyBindingsState()
        {
            var vm = GetCurrentViewModel();
            bool shouldBlock = vm is LoginViewModel || vm is RegistroViewModel;

            if (shouldBlock)
            {
                // Ensure tracked bindings are not active
                foreach (var kb in _menuKeyBindings)
                {
                    if (InputBindings.Contains(kb))
                        InputBindings.Remove(kb);
                }
            }
            else
            {
                // Ensure tracked bindings are active
                foreach (var kb in _menuKeyBindings)
                {
                    if (!InputBindings.Contains(kb))
                        InputBindings.Add(kb);
                }
            }
        }

        public void CambiarFondoGradiente(Color colorInicio, Color colorFin, double offset =0.5, string direccion = "Horizontal")
        {
            LinearGradientBrush gradiente = new LinearGradientBrush();

            // Configurar la dirección del gradiente
            if (direccion.ToLower() == "horizontal")
            {
                // Horizontal: izquierda a derecha
                gradiente.StartPoint = new Point(0,0.5);
                gradiente.EndPoint = new Point(1,0.5);
            }
            else
            {
                // Vertical: arriba a abajo (por defecto)
                gradiente.StartPoint = new Point(0.5,0);
                gradiente.EndPoint = new Point(0.5,1);
            }

            gradiente.GradientStops.Add(new GradientStop(colorInicio, offset));
            gradiente.GradientStops.Add(new GradientStop(colorFin,1));

            RootContainer.Background = gradiente;
        }
        public bool escorreovalido(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }

        // Este evento se ejecuta cuando termina la animación
        private void LogoAnimacion_Completed(object sender, EventArgs e)
        {
            // navigate to Inicio
            if (DataContext is Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.ViewModel.MainViewModel shell)
            {
               //shell.NavigateToInicioCommand.Execute(null);
                shell.NavigateToLoginCommand.Execute(null);
            }
            // hide startup overlay if present
            if (FindName("StartupView") is FrameworkElement startup)
            {
                startup.Visibility = Visibility.Collapsed;
            }
            // fin de animación: habilitar lógica normal de visibilidad
            _isStartupAnimating = false;
            UpdateMenuVisibilityForCurrentView();
        }
    }
}
