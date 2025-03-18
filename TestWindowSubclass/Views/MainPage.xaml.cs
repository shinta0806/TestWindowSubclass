using Microsoft.UI.Xaml.Controls;

using TestWindowSubclass.ViewModels;

namespace TestWindowSubclass.Views;

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
