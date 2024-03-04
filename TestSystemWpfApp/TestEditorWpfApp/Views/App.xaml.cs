using System.Windows;
using AutoMapper;
using TestEditorWpfApp.Configuration;
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

        var configuration = CreateMapperConfiguration();
        var mapper = configuration.CreateMapper();
        var view = new TestEditorView();
        var viewModel = new TestEditorViewModel(view, mapper);
        view.Show();
    }

    private static MapperConfiguration CreateMapperConfiguration() => new(cfg => cfg.AddProfile<AutoMapperProfile>());
}