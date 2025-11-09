using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;


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

        // límites para el selector de fecha
        public DateTime StartOfYear => new DateTime(year,1,1);
        public DateTime Today => DateTime.Today.Year == year ? DateTime.Today : new DateTime(year,1,1);

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

        public MainViewModel()
        {
            daysInYear = DateTime.IsLeapYear(year) ?366 :365;
            totalConsumption = new double[daysInYear]; // inicia en0

            // Recalcular gráfico cuando cambie la colección o cualquier propiedad de un elemento
            Items.CollectionChanged += (s, e) => RebuildFromItems();

            SelectedPeriod = "week";
            UpdateValues();
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
            Array.Clear(totalConsumption,0, totalConsumption.Length);
            var startOfYear = new DateTime(year,1,1);
            foreach (var it in Items)
            {
                // limitar a año en curso y no permitir futuro
                if (it.Day.Year != year) continue;
                if (it.Day.Date > Today.Date) continue;

                int index = (int)(it.Day.Date - startOfYear).TotalDays;
                if (index >=0 && index < totalConsumption.Length)
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
            var startOfYear = new DateTime(year,1,1);
            var today = Today;
            var todayIndex = Math.Clamp((today - startOfYear).Days,0, daysInYear -1);
            switch (selectedPeriod)
            {
                case "week":
                    {
                        var end = todayIndex;
                        var start = Math.Max(0, end -6);
                        var count = end - start +1;

                        Values = new ISeries[]
                        {
                            new LineSeries<double> { Values = Slice(totalConsumption, start, count) }
                        };

                        var startDate = startOfYear.AddDays(start);
                        var labels = new string[count];
                        for (int i =0; i < count; i++)
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
                            chart.SaveImage(stream, SKEncodedImageFormat.Png,100);
                            ChartWeekData = stream.ToArray();
                        }
                        break;
                    }
                case "month":
                    {
                        var monthStart = new DateTime(year, today.Month,1);
                        var dim = DateTime.DaysInMonth(year, today.Month);
                        var start = (monthStart - startOfYear).Days;
                        var count = Math.Clamp(today.Day,1, dim);

                        Values = new ISeries[]
                        {
                            new LineSeries<double> { Values = Slice(totalConsumption, start, count) }
                        };

                        var labels = new string[count];
                        for (int i =0; i < count; i++)
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
                            chart.SaveImage(stream, SKEncodedImageFormat.Png,100);
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
                        for (int m =1; m <= today.Month; m++)
                            labels[m -1] = new DateTime(year, m,1).ToString("MMMM", Culture);
                        XAxes[0].Labels = labels;

                        var chart = new SKCartesianChart
                        {
                            Series = new ISeries[] { new LineSeries<double> { Values = monthAverages } },
                            XAxes = XAxes,
                            YAxes = YAxes
                        };
                        using (var stream = new MemoryStream())
                        {
                            chart.SaveImage(stream, SKEncodedImageFormat.Png,100);
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
            Array.Copy(source, start, result,0, count);
            return result;
        }

        private static double[] GetMonthlyAveragesToDate(int year, double[] daily, DateTime today)
        {
            int months = today.Month;
            var result = new double[months];
            var offset =0;

            for (int month =1; month <= months; month++)
            {
                var dim = DateTime.DaysInMonth(year, month);
                var take = month == today.Month ? Math.Clamp(today.Day,1, dim) : dim;

                take = Math.Min(take, Math.Max(0, daily.Length - offset));

                if (take <=0)
                {
                    result[month -1] =0;
                }
                else
                {
                    double sum =0;
                    for (int i =0; i < take; i++) sum += daily[offset + i];
                    result[month -1] = sum / take;
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