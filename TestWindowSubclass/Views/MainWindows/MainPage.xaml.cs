using Microsoft.UI.Xaml.Controls;

using TestWindowSubclass.ViewModels.MainWindows;

namespace TestWindowSubclass.Views.MainWindows;

public sealed partial class MainPage : Page
{
	public MainViewModel ViewModel
	{
		get;
	}

	public MainPage()
	{
		ViewModel = new();
		InitializeComponent();
	}
}
