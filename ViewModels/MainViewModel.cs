using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

namespace CRA.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Année des données (aligne les dates et le tableau)
        private readonly int year = DateTime.Today.Year;
        private readonly int daysInYear;
        private readonly double[] totalConsumption;

        [ObservableProperty]
        private string selectedPeriod = string.Empty;

        [ObservableProperty]
        private IEnumerable<ISeries> values = Array.Empty<ISeries>();

        // Culture utilisée pour formater les libellés
        [ObservableProperty]
        private CultureInfo culture = CultureInfo.CurrentCulture;
        partial void OnCultureChanged(CultureInfo value) => UpdateValues();

        public Axis[] XAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                Name = "Day",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Blue),
                TextSize = 20,
                LabelsRotation = 90,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 2 }
            }
        };

        public Axis[] YAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                Name = "Liters",
                LabelsPaint = new SolidColorPaint(SKColors.Green),
                TextSize = 20,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray),
            }
        };

        public MainViewModel()
        {
            daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            totalConsumption = GenerateRandomData(daysInYear, 170, 380);

            SelectedPeriod = "day";
            UpdateValues();
        }

        private static double[] GenerateRandomData(int count, int min, int max)
        {
            var rnd = new Random();
            var data = new double[count];
            for (int i = 0; i < count; i++) data[i] = rnd.Next(min, max);
            return data;
        }

        partial void OnSelectedPeriodChanged(string value) => UpdateValues();

        private void UpdateValues()
        {
            var startOfYear = new DateTime(year, 1, 1);
            var today = DateTime.Today.Year == year ? DateTime.Today : startOfYear;
            var todayIndex = Math.Clamp((today - startOfYear).Days, 0, daysInYear - 1);

            switch (selectedPeriod)
            {
                case "day":
                    {
                        Values = new ISeries[]
                        {
                        new LineSeries<double> { Values = new[] { totalConsumption[todayIndex] } }
                        };
                        XAxes[0].Labels = new[]
                        {
                        startOfYear.AddDays(todayIndex).ToString("dddd", Culture)
                    };
                        break;
                    }

                case "week":
                    {
                        // Les 7 derniers jours (ou moins si début d’année)
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
                        break;
                    }

                case "month":
                    {
                        var monthStart = new DateTime(year, today.Month, 1);
                        var dim = DateTime.DaysInMonth(year, today.Month);
                        var start = (monthStart - startOfYear).Days;

                        Values = new ISeries[]
                        {
                        new LineSeries<double> { Values = Slice(totalConsumption, start, dim) }
                        };

                        var labels = new string[dim];
                        for (int i = 0; i < dim; i++)
                            labels[i] = monthStart.AddDays(i).ToString("d MMM", Culture);
                        XAxes[0].Labels = labels;
                        break;
                    }

                case "year":
                    {
                        // 12 points : moyenne par mois
                        var monthAverages = GetMonthlyAverages(year, totalConsumption);
                        Values = new ISeries[]
                        {
                        new LineSeries<double> { Values = monthAverages }
                        };

                        var labels = new string[12];
                        for (int m = 1; m <= 12; m++)
                            labels[m - 1] = new DateTime(year, m, 1).ToString("MMMM", Culture);
                        XAxes[0].Labels = labels;
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

        private static double[] GetMonthlyAverages(int year, double[] daily)
        {
            var result = new double[12];
            var offset = 0;

            for (int month = 1; month <= 12; month++)
            {
                var dim = DateTime.DaysInMonth(year, month);
                var take = Math.Min(dim, Math.Max(0, daily.Length - offset));

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

        // Comandos que son activados al hacer click en cada uno de los botones(Day/Week/Month/Year)
        [RelayCommand] private void GoToPage1() => SelectedPeriod = "day";
        [RelayCommand] private void GoToPage2() => SelectedPeriod = "week";
        [RelayCommand] private void GoToPage3() => SelectedPeriod = "month";
        [RelayCommand] private void SeeAll() => SelectedPeriod = "year";
    }
}