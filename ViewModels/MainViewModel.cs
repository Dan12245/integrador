using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace CRA.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly double[] TotalConsumption = GenerateRandomData(365, 170, 380);

        [ObservableProperty]
        private string selectedPeriod = string.Empty;

        [ObservableProperty]
        private IEnumerable<ISeries> values = Array.Empty<ISeries>();

        public MainViewModel()
        {
            SelectedPeriod = "day"; // inicial por defecto
            UpdateValues();
        }

        private static double[] GenerateRandomData(int count, int min, int max)
        {
            var rnd = new Random();
            var data = new double[count];
            for (int i = 0; i < count; i++) data[i] = rnd.Next(min, max + 1);
            return data;
        }

        partial void OnSelectedPeriodChanged(string value) => UpdateValues();

        private void UpdateValues()
        {
            switch (selectedPeriod)
            {
                case "day":
                    Values = new ISeries[] { new LineSeries<double> { Values = new double[] { TotalConsumption[0] } } };
                    break;
                case "week":
                    Values = new ISeries[] { new LineSeries<double> { Values = new ArraySegment<double>(TotalConsumption, 0, 7) } };
                    break;
                case "month":
                    Values = new ISeries[] { new LineSeries<double> { Values = new ArraySegment<double>(TotalConsumption, 0, 30) } };
                    break;
                case "year":
                    Values = new ISeries[] { new LineSeries<double> { Values = TotalConsumption } };
                    break;
                default:
                    Values = Array.Empty<ISeries>();
                    break;
            }
        }

        // Comandos que son activados al hacer click en cada uno de los botones(Day/Week/Month/Year)
        [RelayCommand]
        private void GoToPage1() => SelectedPeriod = "day";     

        [RelayCommand]
        private void GoToPage2() => SelectedPeriod = "week";   

        [RelayCommand]
        private void GoToPage3() => SelectedPeriod = "month";   

        [RelayCommand]
        private void SeeAll() => SelectedPeriod = "year";       
    }
}
