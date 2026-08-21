using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaModel.UserControls;
using AvaloniaModel.ViewModels;

namespace AvaloniaModel.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

	public async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
	{
		var vm = this.DataContext as MainWindowViewModel;
		MyFileDialog fd = new MyFileDialog();
		await fd.OpenFileButton_Clicked(this);
		vm.CsvPath = fd.selectedPath;
	}
}