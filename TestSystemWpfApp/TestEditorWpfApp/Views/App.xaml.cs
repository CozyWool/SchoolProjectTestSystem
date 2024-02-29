using System.Windows;
using TestEditorWpfApp.ViewModels;
using Application = System.Windows.Application;

namespace TestEditorWpfApp.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var view = new TestEditorView();
        var viewModel = new TestEditorViewModel(view);
        view.Show();
    }
}