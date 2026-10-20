using System.Windows;
using System.Windows.Controls;
using OrcamentoObra.Pages;

namespace OrcamentoObra;

public partial class MainWindow : Window
{
    public static MainWindow Instancia { get; private set; } = null!;

    public MainWindow()
    {
        InitializeComponent();
        Instancia = this;
        Principal.Navigate(new HomePage());
    }

    public static void Ir(Page pagina) => Instancia.Principal.Navigate(pagina);
}
