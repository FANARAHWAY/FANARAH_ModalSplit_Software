using Avalonia.Controls;
using Avalonia.Media.Imaging;
using AvaloniaModel.Misc;
using AvaloniaModel.Models;
using AvaloniaModel.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AvaloniaModel.ViewModels
{
    public class ComboboxItem<T>
    {
        public T Value { get; set; }
        public string Text { get; set; }
    }

    public partial class MainWindowViewModel : ViewModelBase
    {
        const int NORMALMAXTIME = 1000;
        const int WALKMAXTIME = 600;
        const int MAXRECORDS = 120;
        const float BUSCONSTANT = 2.025f;

        [ObservableProperty]
        private MessageBoxService _messageBox = new MessageBoxService();

        [ObservableProperty]
        Bitmap? selectedImage;

        [ObservableProperty]
        int walkMaxTime = NORMALMAXTIME;

        [ObservableProperty]
        List<Aggregated>? resultList;

        [ObservableProperty]
        ComboboxItem<TravelMode> selectedMode;

        [ObservableProperty]
        bool isCostEnabled = false;

        [ObservableProperty]
        bool isTimeEnabled = false;

        [ObservableProperty]
        bool isFrictEnabled = false;

        [ObservableProperty]
        bool isCarEnabled = false;

        [ObservableProperty]
        bool isWalkEnabled = false;

        [ObservableProperty]
        bool isRushEnabled = false;

        [ObservableProperty]
        bool isAgeEnabled = false;

        /// <summary>
        /// Travel mode change event
        /// </summary>
        /// <param name="value">Travel mode after change</param>
        partial void OnSelectedModeChanged(ComboboxItem<TravelMode> value)
        {
            WalkMaxTime = NORMALMAXTIME;
            parameters.BusConstant = 0;

            IsCostEnabled = false;
            IsTimeEnabled = false;
            IsFrictEnabled = false;
            IsCarEnabled = false;
            IsWalkEnabled = false;
            IsRushEnabled = false;
            IsAgeEnabled = false;

            switch (value.Value)
            {
                case TravelMode.Walk:
                    IsCostEnabled = true;
                    IsTimeEnabled = true;
                    IsCarEnabled = true;
                    IsAgeEnabled = true;
                    WalkMaxTime = WALKMAXTIME;
                    break;

                case TravelMode.Drive_park:
                    IsCostEnabled = true;
                    IsTimeEnabled = true;
                    IsFrictEnabled = true;
                    IsWalkEnabled = true;
                    IsCarEnabled = true;
                    IsRushEnabled = true;
                    IsAgeEnabled = true;
                    break;

                case TravelMode.Taxi:
                    IsCostEnabled = true;
                    IsRushEnabled = true;
                    IsTimeEnabled = true;
                    break;

                case TravelMode.Bus:
                    IsCostEnabled = true;
                    IsRushEnabled = true;
                    IsTimeEnabled = true;
                    parameters.BusConstant = BUSCONSTANT;
                    break;

                default:
                    break;
            }
        }

        public ComboboxItem<AgeRange> selectedAge { get; set; }

        List<Aggregated> expList = new();

        [ObservableProperty]
        ModelParams parameters;

        [ObservableProperty]
        bool showPropability;

        [ObservableProperty]
        string? csvPath;
        public List<ComboboxItem<AgeRange>> AllAges => Enum.GetValues<AgeRange>().Select(x => new ComboboxItem<AgeRange>
        {
            Value = x,
            Text = x.GetDescription()
        }).ToList();

        public List<ComboboxItem<TravelMode>> AllModes => Enum.GetValues<TravelMode>().Select(x => new ComboboxItem<TravelMode>
        {
            Value = x,
            Text = x.GetDescription()
        }).ToList();

        /// <summary>
        /// Display suitable trabel mode image 
        /// </summary>
        /// <param name="imgPath">suitable trabel mode image path in Assets folder</param>
        public void changeImg(string imgPath)
        {
            var assetUri = new Uri("avares://AvaloniaModel" + imgPath);
            SelectedImage = new Bitmap(Avalonia.Platform.AssetLoader.Open(assetUri));
        }

        ModeSplitModel<Aggregated> DAmodel = new ModeSplitModel<Aggregated>();
        ModeSplitModel<ModelParams> DACoeff = new ModeSplitModel<ModelParams>();

        public ComboBoxItem SelectedRush { get; set; }

        /// <summary>
        /// Constructor, displays coefficient in grid
        /// </summary>
        public MainWindowViewModel()
        {
            SelectedRush = new();
            Parameters = new();
            ShowPropability = false;
            List<ModelParams> coefficients = DACoeff.readCSV(Directory.GetCurrentDirectory() + "\\Coefficient.csv");

            ResultList = coefficients.Select(c => new Aggregated(c)).ToList();
        }

        /// <summary>
        /// Enter button event, Add travel mode to exp list
        /// </summary>
        public void AddData()
        {
            Parameters.Rush = Convert.ToInt16(SelectedRush.Tag);
            Parameters.Mode = SelectedMode.Value;
            Parameters.Age = (int)selectedAge.Value;
            var aggrData = new Aggregated(Parameters);
            expList.Add(aggrData);
            Parameters = new();
            parameters.Rush = 0;
        }

        /// <summary>
        /// Calculates Aggregate list
        /// </summary>
        public void DoAggregate()
        {
            ShowPropability = true;
            List<ModelParams> coefficients = DACoeff.readCSV(Directory.GetCurrentDirectory() + "\\Coefficient.csv");

            ResultList = expList.Zip(coefficients, (first, second) => new Aggregated
            {
                Age = second.Age,
                Car = second.Car,
                CaseNumber = first.CaseNumber,
                Cost = second.Cost,
                Frict = second.Frict,
                Mode = first.Mode,
                Rush = second.Rush,
                TravelTime = second.TravelTime,
                WalkPlus = second.WalkPlus,
                BusConstant = second.BusConstant
            }).ToList();

            DAmodel.ExpAggrFunList(ref resultList);
            DAmodel.PropFunList(ref resultList);

            DAmodel.saveCSV(ResultList, "AggrResult");

            TravelMode? mostPropability = ResultList.MaxBy(p => p.Prop)?.Mode;

            switch (mostPropability)
            {
                case TravelMode.Walk:
                    changeImg("/Assets/walk.png");
                    break;

                case TravelMode.Bus:
                    changeImg("/Assets/Bus.jpg");
                    break;

                case TravelMode.Taxi:
                    changeImg("/Assets/Driven.jpg");
                    break;

                case TravelMode.Drive_park:
                    changeImg("/Assets/Drive.jpg");
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Clear current travel mode data and all previous travel modes data
        /// </summary>
        public void ResetData()
        {
            expList.Clear();
            Parameters = new();
            Parameters.Rush = 0;
        }

        /// <summary>
        /// Calculates DisAggregate list
        /// </summary>
        public async void DoDisAggregate()
        {
            List<ModelParams> coefficients = DACoeff.readCSV(Directory.GetCurrentDirectory() + "\\Coefficient.csv");

            expList = DAmodel.readCSV(CsvPath);

            if (expList.Count > MAXRECORDS)
            {
                await _messageBox.ShowAsync(string.Format("Maximum is {0} records", MAXRECORDS), "Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Stop);
                return;
            }
            DAmodel.ExpAggrFunList(ref expList);

            DAmodel.PropFunList(ref expList);

            DAmodel.saveCSV(expList, "DisAggrResult");
            Process.Start(new ProcessStartInfo()
            {
                FileName = Directory.GetCurrentDirectory() + "\\DisAggrResult.csv",
                UseShellExecute = true
            });
        }

        public void openUserGuide()
        {
            //var x = new Uri("avares://AvaloniaModel/Fanarah ModeSplit Software User Guide.pdf");
            var filePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Fanarah ModeSplit Software User Guide.pdf");

            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
    }
}
