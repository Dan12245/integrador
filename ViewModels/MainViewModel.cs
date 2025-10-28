using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace C.R.A_Consumo_reducido_de_agua.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // ========== CONFIGURACIÓN DE RESOLUCIÓN ==========
        // Cambia SCALE_FACTOR para aumentar o disminuir la resolución
        // 2.0 = Buena | 3.0 = Alta | 4.0 = Muy Alta | 5.0 = Ultra | 6.0 = Extrema
        private const double SCALE_FACTOR = 5.0; // ← CAMBIA ESTE VALOR
        private const int BASE_WIDTH = 430;
        private const int BASE_HEIGHT = 224;

        //Simulated data for one year
        private readonly int year = DateTime.Today.Year;
        private readonly int daysInYear;
        private readonly double[] totalConsumption;

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
            new Axis
            {
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
                {
                    IsAntialias = true
                }
            }
        };

        public Axis[] YAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
                {
                    IsAntialias = true
                }
            }
        };

        public MainViewModel()
        {
            daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            totalConsumption = GenerateRandomData(daysInYear, 170, 380);

            SelectedPeriod = "week";
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
                            CreateHighQualitySeries(new[] { totalConsumption[todayIndex] })
                        };
                        XAxes[0].Labels = new[]
                        {
                            startOfYear.AddDays(todayIndex).ToString("dddd", Culture)
                        };
                        break;
                    }

                case "week":
                    {
                        var end = todayIndex;
                        var start = Math.Max(0, end - 6);
                        var count = end - start + 1;
                        var data = Slice(totalConsumption, start, count);

                        Values = new ISeries[]
                        {
                            CreateHighQualitySeries(data)
                        };

                        var startDate = startOfYear.AddDays(start);
                        var labels = new string[count];
                        for (int i = 0; i < count; i++)
                            labels[i] = startDate.AddDays(i).ToString("ddd", Culture);
                        XAxes[0].Labels = labels;

                        // Crear imagen con ALTA CALIDAD
                        CreateHighQualityChartImage(data, labels, imageData => ChartWeekData = imageData);
                        break;
                    }

                case "month":
                    {
                        var monthStart = new DateTime(year, today.Month, 1);
                        var dim = DateTime.DaysInMonth(year, today.Month);
                        var start = (monthStart - startOfYear).Days;
                        var data = Slice(totalConsumption, start, dim);

                        Values = new ISeries[]
                        {
                            CreateHighQualitySeries(data)
                        };

                        var labels = new string[dim];
                        for (int i = 0; i < dim; i++)
                            labels[i] = monthStart.AddDays(i).ToString("d MMM", Culture);
                        XAxes[0].Labels = labels;

                        // Crear imagen con ALTA CALIDAD
                        CreateHighQualityChartImage(data, labels, imageData => ChartMonthData = imageData);
                        break;
                    }

                case "year":
                    {
                        var monthAverages = GetMonthlyAverages(year, totalConsumption);
                        Values = new ISeries[]
                        {
                            CreateHighQualitySeries(monthAverages)
                        };

                        var labels = new string[12];
                        for (int m = 1; m <= 12; m++)
                            labels[m - 1] = new DateTime(year, m, 1).ToString("MMMM", Culture);
                        XAxes[0].Labels = labels;

                        // Crear imagen con ALTA CALIDAD
                        CreateHighQualityChartImage(monthAverages, labels, imageData => ChartYearData = imageData);
                        break;
                    }

                default:
                    Values = Array.Empty<ISeries>();
                    XAxes[0].Labels = Array.Empty<string>();
                    break;
            }
        }

        /// <summary>
        /// Crea una serie con configuración de alta calidad
        /// </summary>
        private LineSeries<double> CreateHighQualitySeries(double[] data)
        {
            return new LineSeries<double>
            {
                Values = data,
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 3)
                {
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round
                },
                GeometrySize = 8,
                GeometryStroke = new SolidColorPaint(SKColors.DeepSkyBlue, 2)
                {
                    IsAntialias = true
                },
                GeometryFill = new SolidColorPaint(SKColors.White)
                {
                    IsAntialias = true
                },
                LineSmoothness = 0
            };
        }

        /// <summary>
        /// Crea una imagen de ultra alta calidad del chart
        /// </summary>
        private void CreateHighQualityChartImage(double[] data, string[] labels, Action<byte[]> setData)
        {
            // Calcular tamaño según el factor de escala
            int highResWidth = (int)(BASE_WIDTH * SCALE_FACTOR);
            int highResHeight = (int)(BASE_HEIGHT * SCALE_FACTOR);

            // Ajustar tamaños de elementos según la escala
            float lineThickness = (float)(3 * SCALE_FACTOR / 2);
            float geometrySize = (float)(8 * SCALE_FACTOR / 2);
            float geometryStrokeThickness = (float)(2 * SCALE_FACTOR / 2);
            float textSize = (float)(12 * SCALE_FACTOR / 2);

            var series = new LineSeries<double>
            {
                Values = data,
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, lineThickness)
                {
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round
                },
                GeometrySize = geometrySize,
                GeometryStroke = new SolidColorPaint(SKColors.DeepSkyBlue, geometryStrokeThickness)
                {
                    IsAntialias = true
                },
                GeometryFill = new SolidColorPaint(SKColors.White)
                {
                    IsAntialias = true
                },
                LineSmoothness = 0
            };

            var xAxis = new Axis
            {
                Labels = labels,
                TextSize = textSize,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
                {
                    IsAntialias = true
                }
            };

            var yAxis = new Axis
            {
                TextSize = textSize,
                LabelsPaint = new SolidColorPaint(SKColors.Black)
                {
                    IsAntialias = true
                }
            };

            var chart = new SKCartesianChart
            {
                Width = highResWidth,   // Resolución escalada
                Height = highResHeight, // Resolución escalada
                Series = new ISeries[] { series },
                XAxes = new[] { xAxis },
                YAxes = new[] { yAxis }
            };

            using (var stream = new MemoryStream())
            {
                // Usar calidad 100 para PNG sin pérdida
                chart.SaveImage(stream, SKEncodedImageFormat.Png, 100);
                setData(stream.ToArray());
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