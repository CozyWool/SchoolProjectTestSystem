using System.Windows;
using AutoMapper;
using TestSystemViewer.Configuration;
using TestSystemViewer.ViewModels;

namespace TestSystemViewer.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var configuration = CreateMapperConfiguration();
        var mapper = configuration.CreateMapper();
        var viewModel = new TestSystemViewModel(mapper);
        var view = new TestSystemView(viewModel);
        viewModel.CloseAction = view.Close;
        view.Show();
    }

    private static MapperConfiguration CreateMapperConfiguration() => new(cfg => cfg.AddProfile<AutoMapperProfile>());
}