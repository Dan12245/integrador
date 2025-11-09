using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Editors
{
     public partial class ConsumoEditorWindow : Window
     {
         private MainViewModel _vm;
         public MainViewModel ViewModel => DataContext as MainViewModel;
         private bool _userClickedGridCell = false; // controla habilitación de Delete

         public ConsumoEditorWindow(MainViewModel vm)
         {
             InitializeComponent();
             _vm = vm;
             DataContext = vm;
             if (!_vm.Items.Any()) SelectedEntry = null; else SelectedEntry = _vm.Items.FirstOrDefault();
             InputDate = _vm.Today.Date;
             ConsumoTextBox.Text = string.Empty;
             UpdateDeleteButtonState();
             PreviewMouseLeftButtonDown += Window_PreviewMouseLeftButtonDown; // manejar click fuera del grid
         }

         public MainViewModel.DailyEntry? SelectedEntry { get; set; }
         public DateTime? InputDate { get; set; }

         private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
         {
             if (InputDate is not DateTime selectedDate) { MessageBox.Show("Seleccione una fecha válida."); return; }
             if (selectedDate < _vm.StartOfYear || selectedDate > _vm.Today) { MessageBox.Show("Fecha fuera de rango."); return; }
             if (string.IsNullOrWhiteSpace(ConsumoTextBox.Text)) { MessageBox.Show("Ingrese un consumo numérico."); return; }
             var raw = ConsumoTextBox.Text.Replace(',', '.');
             if (!double.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double consumo) || consumo <0)
             { MessageBox.Show("Valor de consumo inválido."); return; }
             consumo = Math.Round(consumo,2, MidpointRounding.AwayFromZero);

             var existente = _vm.Items.FirstOrDefault(x => x.Day.Date == selectedDate.Date);
             if (existente != null)
             {
                 existente.Consumption = consumo;
                 MessageBox.Show("Consumo actualizado para la fecha seleccionada.");
                 SelectedEntry = existente; // selección programática, no habilita delete
             }
             else
             {
                 var nuevo = new MainViewModel.DailyEntry { Day = selectedDate.Date, Consumption = consumo };
                 _vm.Items.Add(nuevo);
                 SelectedEntry = nuevo;
                 ConsumosGrid.SelectedItem = nuevo; // aún no habilita delete
             }

             ConsumoTextBox.Text = string.Empty;
             _vm.RebuildFromItems();
             _userClickedGridCell = false; // requiere click explícito
             UpdateDeleteButtonState();
         }

         private void Delete_Click(object sender, RoutedEventArgs e)
         {
         if (SelectedEntry != null)
         {
             var toRemove = SelectedEntry;
             _vm.Items.Remove(toRemove);
             SelectedEntry = null;
             ConsumoTextBox.Text = string.Empty;
             _vm.RebuildFromItems();
             }
             _userClickedGridCell = false;
             UpdateDeleteButtonState();
         }

         private void Close_Click(object sender, RoutedEventArgs e) => Close();

         private void ConsumoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
         {
             // permitir solo dígitos y '.' como separador decimal, máximo 2 decimales
             var text = ConsumoTextBox.Text;
             var selectionStart = ConsumoTextBox.SelectionStart;
             var selectionLength = ConsumoTextBox.SelectionLength;
             var input = e.Text == "," ? "." : e.Text; // normalizar coma a punto
             var newText = text.Remove(selectionStart, selectionLength).Insert(selectionStart, input);

             // si empieza con '.' anteponer0
             if (newText.StartsWith(".")) newText = "0" + newText;

             // validar caracteres
             foreach (var c in newText)
             {
                if (!(char.IsDigit(c) || c == '.')) { e.Handled = true; return; }
             }

             // solo un '.'
             if (newText.Count(c => c == '.') >1) { e.Handled = true; return; }

             var parts = newText.Split('.');
             if (parts.Length ==2 && parts[1].Length >2) { e.Handled = true; return; }

             e.Handled = false;
             if (input != e.Text) // si se reemplazó coma por punto, insertar manualmente
             {
                 ConsumoTextBox.SelectedText = input;
                 e.Handled = true; // ya manejado
             }
         }

         private void ConsumoTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
         {
         if (e.DataObject.GetDataPresent(DataFormats.Text))
         {
             var text = (e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty).Replace(',', '.');
             if (!double.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var val) || val <0)
             { 
                    e.CancelCommand(); return;
             }
             var s = Math.Round(val,2, MidpointRounding.AwayFromZero).ToString("F2", CultureInfo.InvariantCulture);
             ConsumoTextBox.Text = s;
             e.CancelCommand();
         }
            else e.CancelCommand();
         }

         private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             if (sender is DataGrid dg)
             {
                 SelectedEntry = dg.SelectedItem as MainViewModel.DailyEntry;
                 UpdateDeleteButtonState();
             }
         }

         private void ConsumosGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             if (e.OriginalSource is FrameworkElement fe && fe.DataContext is MainViewModel.DailyEntry)
             {
                _userClickedGridCell = true;
             }
         }

         private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             var source = e.OriginalSource as DependencyObject;
             bool allowKeepSelection = false;
             while (source != null)
             {
                 if (source == ConsumosGrid || source == DeleteButton)
                 { allowKeepSelection = true; break; }
                 source = VisualTreeHelper.GetParent(source);
             }
             if (!allowKeepSelection)
             {
                 ConsumosGrid.UnselectAll();
                 SelectedEntry = null;
                 _userClickedGridCell = false;
                 UpdateDeleteButtonState();
             }
         }
         private void UpdateDeleteButtonState()
         {
             if (DeleteButton != null)
             DeleteButton.IsEnabled = _userClickedGridCell && SelectedEntry != null;
         }
     }
}
