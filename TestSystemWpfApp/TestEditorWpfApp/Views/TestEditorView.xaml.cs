using System.ComponentModel;
using TestEditorWpfApp.ViewModels;

namespace TestEditorWpfApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class TestEditorView
{
    public TestEditorView()
    {
        InitializeComponent();
    }

    private void TestEditorView_OnClosing(object sender, CancelEventArgs e)
    {
        if (DataContext is not TestEditorViewModel viewModel) return;

        if (!viewModel.SaveChanges())
        {
            e.Cancel = true;
        }
    }
}