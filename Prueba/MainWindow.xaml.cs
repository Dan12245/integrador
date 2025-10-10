using Prueba.View.UserControls;
using System.Windows;

namespace Prueba
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MenuBar MenuBar;
        bool StateBtn = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnX_Click(object sender, RoutedEventArgs e)
        {
            Esterno.Text = "En chihuahua hay una muchacha chula que vende chile chilaca a 8.80";
            StateBtn = true;
            Quesadilla quesadilla = new Quesadilla(this, MenuBar);
            quesadilla.Show();
        }
    }
}