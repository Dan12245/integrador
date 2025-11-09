using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media; // VisualTreeHelper
using System.Globalization;
using System.Threading.Tasks;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Editors
{
    public partial class ConsumoEditorWindow : Window
    {
        private MainViewModel _vm;
        private bool _userClickedGridCell = false; // única definición

        public ConsumoEditorWindow(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
            Loaded += ConsumoEditorWindow_Loaded;
            ConsumoTextBox.Text = string.Empty;
            PreviewMouseLeftButtonDown += Window_PreviewMouseLeftButtonDown;
        }

        private async void ConsumoEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // asegurar que hay edificio seleccionado (se hace en Inicio, pero por robustez)
            if (_vm.Buildings.Count ==0)
            {
                await _vm.LoadBuildingsAsync();
            }
            if (_vm.SelectedBuilding == null && _vm.Buildings.Count >0)
            {
                _vm.SelectedBuilding = _vm.Buildings[0];
            }
            if (_vm.SelectedBuilding != null && _vm.Items.Count ==0)
            {
                await _vm.LoadConsumptionForSelectedBuildingAsync();
            }
            InputDate = _vm.Today.Date;
        }

        public MainViewModel.DailyEntry? SelectedEntry { get; set; }
        public DateTime? InputDate { get; set; }

        private async void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedBuilding == null) { MessageBox.Show("Seleccione un edificio en Inicio."); return; }
            if (InputDate is not DateTime selectedDate) { MessageBox.Show("Seleccione una fecha válida."); return; }
            if (selectedDate < _vm.StartOfYear || selectedDate > _vm.Today) { MessageBox.Show("Fecha fuera de rango."); return; }
            if (string.IsNullOrWhiteSpace(ConsumoTextBox.Text)) { MessageBox.Show("Ingrese un consumo numérico."); return; }
            var raw = ConsumoTextBox.Text.Replace(',', '.');
            if (!double.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double consumo) || consumo <0) { MessageBox.Show("Valor de consumo inválido."); return; }
            consumo = Math.Round(consumo,2, MidpointRounding.AwayFromZero);
            var ok = await _vm.SaveOrUpdateConsumptionAsync(selectedDate, consumo);
            if (!ok)
            {
                MessageBox.Show("No se pudo guardar en la base de datos. Verifique conexión y que existe el índice único (building_id, day).");
                return;
            }
            ConsumoTextBox.Text = string.Empty;
            _userClickedGridCell = false; UpdateDeleteButtonState();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry == null || _vm.SelectedBuilding == null) { return; }
            var ok = await _vm.DeleteConsumptionAsync(SelectedEntry.Day.Date);
            if (!ok) { MessageBox.Show("No se pudo eliminar en la base de datos."); return; }
            SelectedEntry = null;
            ConsumoTextBox.Text = string.Empty;
            _userClickedGridCell = false; UpdateDeleteButtonState();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { SelectedEntry = (sender as DataGrid)?.SelectedItem as MainViewModel.DailyEntry; UpdateDeleteButtonState(); }
        private void ConsumosGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { if (e.OriginalSource is FrameworkElement fe && fe.DataContext is MainViewModel.DailyEntry) _userClickedGridCell = true; }
        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var src = e.OriginalSource as DependencyObject; bool keep = false; while (src != null) { if (src == ConsumosGrid || src == DeleteButton) { keep = true; break; } src = VisualTreeHelper.GetParent(src); }
            if (!keep) { ConsumosGrid.UnselectAll(); SelectedEntry = null; _userClickedGridCell = false; UpdateDeleteButtonState(); }
        }
        private void UpdateDeleteButtonState() { if (DeleteButton != null) DeleteButton.IsEnabled = _userClickedGridCell && SelectedEntry != null; }
        // Validación de entrada decimal (máximo2 decimales, separador '.')
        private void ConsumoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = sender as TextBox; if (tb == null) return;
            var selectionStart = tb.SelectionStart; var selectionLength = tb.SelectionLength;
            var input = e.Text == "," ? "." : e.Text; // normalizar coma
            var newText = tb.Text.Remove(selectionStart, selectionLength).Insert(selectionStart, input);
            if (newText.StartsWith(".")) newText = "0" + newText;
            // sólo dígitos y un punto
            if (newText.Any(c => !(char.IsDigit(c) || c == '.'))) { e.Handled = true; return; }
            if (newText.Count(c => c == '.') >1) { e.Handled = true; return; }
            var parts = newText.Split('.');
            if (parts.Length ==2 && parts[1].Length >2) { e.Handled = true; return; }
            // si reemplazamos coma por punto, manejar manualmente
            if (input != e.Text) { tb.SelectedText = input; e.Handled = true; }
        }
        private void ConsumoTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text)) { e.CancelCommand(); return; }
            var text = (e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty).Replace(',', '.');
            if (!double.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var val) || val <0) { e.CancelCommand(); return; }
            var s = Math.Round(val,2, MidpointRounding.AwayFromZero).ToString("F2", CultureInfo.InvariantCulture);
            (sender as TextBox)!.Text = s; e.CancelCommand();
        }
    }
}
