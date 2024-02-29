using System.Windows;
using TestSystemWpfApp.ViewModels;
using Application = System.Windows.Application;

namespace TestSystemWpfApp.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var view = new TestSystemView();
        var viewModel = new TestSystemViewModel(view);
        view.Show();
    }
}