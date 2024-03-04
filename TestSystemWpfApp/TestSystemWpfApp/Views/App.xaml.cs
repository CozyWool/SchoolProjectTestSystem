using System.Windows;
using AutoMapper;
using TestSystemWpfApp.Configuration;
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
        var configuration = CreateMapperConfiguration();
        var mapper = configuration.CreateMapper();
        var view = new TestSystemView();
        var viewModel = new TestSystemViewModel(view, mapper);
        view.Show();
    }

    private static MapperConfiguration CreateMapperConfiguration() => new(cfg => cfg.AddProfile<AutoMapperProfile>());
}