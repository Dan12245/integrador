using System.Windows;
using System.Windows.Controls;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.Controls
{
    public partial class CalloutControl : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(CalloutControl), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public CalloutControl()
        {
            InitializeComponent();
        }
    }
}