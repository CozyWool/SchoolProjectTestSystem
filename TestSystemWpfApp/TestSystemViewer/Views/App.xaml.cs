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
        var view = new TestSystemView();
        var viewModel = new TestSystemViewModel(view, mapper);
        view.Show();
    }

    private static MapperConfiguration CreateMapperConfiguration() => new(cfg => cfg.AddProfile<AutoMapperProfile>());
}