using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mathematics.App.ViewModels;

namespace Mathematics.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainWindowViewModel _viewModel = new();
	
	public MainWindow()
	{
		InitializeComponent();
		
		DataContext = _viewModel;
	}

	private void PreviewButton_OnClick(object sender, RoutedEventArgs e)
	{
		Dispatcher.Invoke(_viewModel.CreateNoise);
	}
}