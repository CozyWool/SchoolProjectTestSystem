using TestSystemViewer.ViewModels;

namespace TestSystemViewer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class TestSystemView
{
    public TestSystemView(TestSystemViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}