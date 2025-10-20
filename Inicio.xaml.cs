using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static C.R.A_Consumo_reducido_de_agua.MainWindow;
using static C.R.A_Consumo_reducido_de_agua.Registro;

namespace C.R.A_Consumo_reducido_de_agua
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : UserControl
    {
        string ruta = "UserData.json";
        public Inicio()
        {
            
            InitializeComponent();
            UserData.Load();
            if (GlobalData.UserName == null)
            {
                GlobalData.UserName = "Usuario";
            }

            if  (UserData.Uso == false){
                texto_modo.Text = "Modo Empresarial";
            }

            else
            {
                texto_modo.Text = "Modo Doméstico";
            }

            Texto_Bienvenida.Text = $"Hola, {GlobalData.UserName}!";
            Texto_Porcentaje.Text = $"¡Tu consumo de agua ha sido del {new Random().Next(10, 101)}% este mes!";

            // Mostrar primer consejo
            texto_consejo.Text = consejos[indiceActual];

            // Configurar el temporizador
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private List<string> consejos = new List<string>
        {
        "Dúchate rápido: Intenta reducir el tiempo en la regadera a 5 minutos o menos.",
         "Cierra la llave cuando no la uses: Al enjabonarte las manos, etc evita dejar correr el agua.",
         "Usa un vaso para cepillarte los dientes: En vez de usar la llave, llena un vaso y con eso se enjuaga",
         "Lava los trastes con método: Enjabona todo primero con la llave cerrada y luego enjuágalos de una.",
         "Carga la lavadora al máximo recomendado: No uses la lavadora con poca ropa.",
         "Revisa fugas en baños y llaves: Una fuga puede desperdiciar demasiada agua al mes sin que se note.",
         "Riega de noche o temprano: Así evitas la evaporación por el sol y las plantas no se secan.",
         "Reutiliza agua cuando se pueda: El agua de lavar frutas o verduras puede servir para regar plantas.",
        };

        private int indiceActual = 0;
        private DispatcherTimer timer;

        private void Timer_Tick(object sender, EventArgs e)
        {
            SiguienteConsejo();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            SiguienteConsejo();
            ReiniciarTimer();
        }

        private void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            indiceActual--;
            if (indiceActual < 0)
                indiceActual = consejos.Count - 1;

            texto_consejo.Text = consejos[indiceActual];
            ReiniciarTimer();
        }

        private void SiguienteConsejo()
        {
            indiceActual++;
            if (indiceActual >= consejos.Count)
                indiceActual = 0;

            texto_consejo.Text = consejos[indiceActual];
        }

        private void ReiniciarTimer()
        {
            timer.Stop();
            timer.Start();
        }

        private void btnSalto_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)this.Resources["SaltoStoryboard"];
            sb.Begin();
        }

        public void CambiarEscena(UserControl nuevoControl)
        {
            Login_Window.Children.Clear();
            Login_Window.Children.Add(nuevoControl);
        }
        //spam de sofi: “
       // Hola papus :D
      //  Gerardwayfan71_"

        private void Boton_Usuario_Invitados(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Usuario());
        }

        private void Boton_Reporte(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Reporte());
        }

        private void Boton_ir_a_Configuracion(object sender, RoutedEventArgs e)
        {
            CambiarEscena(new Configuracion());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
