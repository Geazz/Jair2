using System.Windows;
using System.Windows.Controls;
using OrcamentoObra;

namespace OrcamentoObra.Pages;

public partial class HomePage : Page
{
    public HomePage() => InitializeComponent();

    private void NovoOrcamento_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new NovoOrcamentoPage());
    private void ProcurarOrcamento_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new ProcurarOrcamentoPage());
    private void ProcurarCliente_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new ProcurarClientePage());
    private void NovoCliente_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new NovoClientePage());
}
