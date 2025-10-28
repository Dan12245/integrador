using System.Windows;
using System.Windows.Controls;

namespace C.R.A_Consumo_reducido_de_agua.Controls
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