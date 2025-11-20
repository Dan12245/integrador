using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly int year = DateTime.Today.Year;
        private readonly int daysInYear;

        // arreglo diario usado por la gráfica (llenado desde Items)
        private double[] totalConsumption;

        // colección editable por el usuario (DataGrid)
        public ObservableCollection<DailyEntry> Items { get; } = new();

        // edificios del usuario
        public class BuildingInfo
        {
            public int Id { get; set; }
            public string Alias { get; set; } = string.Empty;
            public override string ToString() => Alias;
        }

        public ObservableCollection<BuildingInfo> Buildings { get; } = new();

        [ObservableProperty]
        private BuildingInfo selectedBuilding;

        [ObservableProperty]
        private bool isBusy; // agregado

        // límites para el selector de fecha
        public DateTime StartOfYear => new DateTime(year, 1, 1);
        public DateTime Today => DateTime.Today.Year == year ? DateTime.Today : new DateTime(year, 1, 1);

        [ObservableProperty]
        private string selectedPeriod = string.Empty;

        [ObservableProperty]
        private IEnumerable<ISeries> values = Array.Empty<ISeries>();

        //Culture is used to format date labels according to the selected culture (based on PC's system language and region)
        [ObservableProperty]
        private CultureInfo culture = CultureInfo.CurrentCulture;
        [ObservableProperty]
        private byte[] _chartWeekData;
        [ObservableProperty]
        private byte[] _chartMonthData;
        [ObservableProperty]
        private byte[] _chartYearData;
        partial void OnCultureChanged(CultureInfo value) => UpdateValues();

        public Axis[] XAxes { get; set; } = new Axis[]
        {
            new Axis { TextSize =18 }
        };
        public Axis[] YAxes { get; set; } = new Axis[]
        {
            new Axis { TextSize =18 }
        };

        // evita cargas reentrantes
        private bool _loadingBuildings;

        public MainViewModel()
        {
            daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            totalConsumption = new double[daysInYear]; // inicia en0

            // Recalcular gráfico cuando cambie la colección o cualquier propiedad de un elemento
            Items.CollectionChanged += (s, e) => RebuildFromItems();

            SelectedPeriod = "week";
            UpdateValues();
        }

        // cargar edificios del usuario y seleccionar el de menor Id
        public async Task LoadBuildingsAsync()
        {
            if (_loadingBuildings) return; // evita reentrada
            _loadingBuildings = true;
            try
            {
                IsBusy = true;
                var cx = new conexion();
                Buildings.Clear();
                // tomar el user id global tras login
                int uid = GlobalData.userid;
                if (uid <= 0) return;
                var list = await cx.GetBuildingsForUser(uid);

                // filtrar duplicados por Id y por Alias normalizado
                var ids = new HashSet<int>();
                var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var (id, aliasRaw) in list)
                {
                    var alias = (aliasRaw ?? string.Empty).Trim();
                    if (!ids.Add(id)) continue; // ya tenemos ese Id
                    if (!aliases.Add(alias)) continue; // alias repetido

                    Buildings.Add(new BuildingInfo { Id = id, Alias = alias });
                }
                // seleccionar el de menor Id por defecto
                if (Buildings.Count > 0)
                {
                    BuildingInfo min = null;
                    foreach (var b in Buildings)
                        if (min == null || b.Id < min.Id) min = b;
                    SelectedBuilding = min;
                }
            }
            finally { IsBusy = false; _loadingBuildings = false; }
        }

        partial void OnSelectedBuildingChanged(BuildingInfo value)
        {
            _ = LoadConsumptionForSelectedBuildingAsync();
        }

        public async Task LoadConsumptionForSelectedBuildingAsync()
        {
            try
            {
                IsBusy = true;
                if (SelectedBuilding == null) return;
                var cx = new conexion();
                var rows = await cx.GetConsumptionForBuilding(SelectedBuilding.Id, year);
                Items.Clear();
                foreach (var tuple in rows)
                {
                    var day = tuple.Day;
                    var cons = tuple.Consumption;
                    Items.Add(new DailyEntry { Day = day.Date, Consumption = cons });
                }
                RebuildFromItems();
            }
            finally { IsBusy = false; }
        }

        public async Task<bool> SaveOrUpdateConsumptionAsync(DateTime day, double consumption)
        {
            if (SelectedBuilding == null) return false;
            try
            {
                IsBusy = true;
                var cx = new conexion();
                var ok = await cx.UpsertConsumption(SelectedBuilding.Id, day.Date, Math.Round(consumption, 2));
                if (!ok) return false;
                var existing = FindEntryByDay(day.Date);
                if (existing != null) existing.Consumption = Math.Round(consumption, 2); else Items.Add(new DailyEntry { Day = day.Date, Consumption = Math.Round(consumption, 2) });
                RebuildFromItems();
                return true;
            }
            finally { IsBusy = false; }
        }

        public async Task<bool> DeleteConsumptionAsync(DateTime day)
        {
            if (SelectedBuilding == null) return false;
            try
            {
                IsBusy = true;
                var cx = new conexion();
                var ok = await cx.DeleteConsumption(SelectedBuilding.Id, day.Date);
                if (!ok) return false;
                var existing = FindEntryByDay(day.Date);
                if (existing != null) Items.Remove(existing);
                RebuildFromItems();
                return true;
            }
            finally { IsBusy = false; }
        }

        private DailyEntry FindEntryByDay(DateTime day)
        {
            foreach (var it in Items)
                if (it.Day.Date == day.Date) return it;
            return null;
        }

        // clase que representa una fila en el DataGrid
        public partial class DailyEntry : ObservableObject
        {
            private DateTime _day;
            private double _consumption;

            public DateTime Day
            {
                get => _day;
                set => SetProperty(ref _day, value);
            }
            public double Consumption
            {
                get => _consumption;
                set => SetProperty(ref _consumption, value);
            }
        }

        // llamado para reconstruir el arreglo diario desde Items y refrescar la gráfica
        public void RebuildFromItems()
        {
            Array.Clear(totalConsumption, 0, totalConsumption.Length);
            var startOfYear = new DateTime(year, 1, 1);
            foreach (var it in Items)
            {
                // limitar a año en curso y no permitir futuro
                if (it.Day.Year != year) continue;
                if (it.Day.Date > Today.Date) continue;

                int index = (int)(it.Day.Date - startOfYear).TotalDays;
                if (index >= 0 && index < totalConsumption.Length)
                {
                    // Acumular múltiples consumos registrados para el mismo día
                    totalConsumption[index] += it.Consumption;
                }
            }
            UpdateValues();
        }

        partial void OnSelectedPeriodChanged(string value) => UpdateValues();

        private void UpdateValues()
        {
            var startOfYear = new DateTime(year, 1, 1);
            var today = Today;
            var todayIndex = Math.Clamp((today - startOfYear).Days, 0, daysInYear - 1);
            switch (selectedPeriod)
            {
                case "week":
                    {
                        var end = todayIndex;
                        var start = Math.Max(0, end - 6);
                        var count = end - start + 1;

                        Values = new ISeries[]
                        {
                            new LineSeries<double> { Values = Slice(totalConsumption, start, count) }
                        };

                        var startDate = startOfYear.AddDays(start);
                        var labels = new string[count];
                        for (int i = 0; i < count; i++)
                            labels[i] = startDate.AddDays(i).ToString("ddd", Culture);
                        XAxes[0].Labels = labels;

                        var chart = new SKCartesianChart
                        {
                            Series = new ISeries[] { new LineSeries<double> { Values = Slice(totalConsumption, start, count) } },
                            XAxes = XAxes,
                            YAxes = YAxes
                        };
                        using (var stream = new MemoryStream())
                        {
                            chart.SaveImage(stream, SKEncodedImageFormat.Png, 100);
                            ChartWeekData = stream.ToArray();
                        }
                        break;
                    }
                case "month":
                    {
                        var monthStart = new DateTime(year, today.Month, 1);
                        var dim = DateTime.DaysInMonth(year, today.Month);
                        var start = (monthStart - startOfYear).Days;
                        var count = Math.Clamp(today.Day, 1, dim);

                        Values = new ISeries[]
                        {
                            new LineSeries<double> { Values = Slice(totalConsumption, start, count) }
                        };

                        var labels = new string[count];
                        for (int i = 0; i < count; i++)
                            labels[i] = monthStart.AddDays(i).ToString("d MMM", Culture);
                        XAxes[0].Labels = labels;

                        var chart = new SKCartesianChart
                        {
                            Series = new ISeries[] { new LineSeries<double> { Values = Slice(totalConsumption, start, count) } },
                            XAxes = XAxes,
                            YAxes = YAxes
                        };
                        using (var stream = new MemoryStream())
                        {
                            chart.SaveImage(stream, SKEncodedImageFormat.Png, 100);
                            ChartMonthData = stream.ToArray();
                        }

                        break;
                    }
                case "year":
                    {
                        // Promedio por mes solo para meses pasados y el mes actual, limitado al día de hoy
                        var monthAverages = GetMonthlyAveragesToDate(year, totalConsumption, today);
                        Values = new ISeries[]
                        {
                        new LineSeries<double> { Values = monthAverages }
                        };

                        var labels = new string[today.Month];
                        for (int m = 1; m <= today.Month; m++)
                            labels[m - 1] = new DateTime(year, m, 1).ToString("MMMM", Culture);
                        XAxes[0].Labels = labels;

                        var chart = new SKCartesianChart
                        {
                            Series = new ISeries[] { new LineSeries<double> { Values = monthAverages } },
                            XAxes = XAxes,
                            YAxes = YAxes
                        };
                        using (var stream = new MemoryStream())
                        {
                            chart.SaveImage(stream, SKEncodedImageFormat.Png, 100);
                            ChartYearData = stream.ToArray();
                        }
                        break;
                    }
                default:
                    Values = Array.Empty<ISeries>();
                    XAxes[0].Labels = Array.Empty<string>();
                    break;
            }
        }

        private static double[] Slice(double[] source, int start, int count)
        {
            var result = new double[count];
            Array.Copy(source, start, result, 0, count);
            return result;
        }

        private static double[] GetMonthlyAveragesToDate(int year, double[] daily, DateTime today)
        {
            int months = today.Month;
            var result = new double[months];
            var offset = 0;

            for (int month = 1; month <= months; month++)
            {
                var dim = DateTime.DaysInMonth(year, month);
                var take = month == today.Month ? Math.Clamp(today.Day, 1, dim) : dim;

                take = Math.Min(take, Math.Max(0, daily.Length - offset));

                if (take <= 0)
                {
                    result[month - 1] = 0;
                }
                else
                {
                    double sum = 0;
                    for (int i = 0; i < take; i++) sum += daily[offset + i];
                    result[month - 1] = sum / take;
                }

                offset += dim;
                if (offset >= daily.Length) offset = daily.Length;
            }

            return result;
        }

        // Comandos que son activados al hacer click en cada uno de los botones(/Week/Month/Year)
        [RelayCommand] private void GoToPage2() => SelectedPeriod = "week";
        [RelayCommand] private void GoToPage3() => SelectedPeriod = "month";
        [RelayCommand] private void SeeAll() => SelectedPeriod = "year";
    }
}