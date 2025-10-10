using Prueba.View.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Prueba
{
    /// <summary>
    /// Lógica de interacción para Quesadilla.xaml
    /// </summary>
    public partial class Quesadilla : Window
    {
        MainWindow MainWindow;
        MenuBar menuBar;
        public Quesadilla()
        {
            InitializeComponent();
        }
        public Quesadilla(MainWindow mainWindow, MenuBar menuBar)
        {
            InitializeComponent();
            this.MainWindow = mainWindow;
            this.menuBar = menuBar;
        }

        private void bntQuesadilla_Click(object sender, RoutedEventArgs e)
        {
            MenuBar menuBar = new MenuBar();
            menuBar.tbBuscar.Text = "hola";
        }
    }
}
