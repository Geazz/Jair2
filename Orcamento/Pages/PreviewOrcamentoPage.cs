using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OrcamentoObra.Models;
using OrcamentoObra.Services;

namespace OrcamentoObra.Pages;

public partial class PreviewOrcamentoPage : Page
{
    private readonly Orcamento _orcamento;
    private readonly Cliente? _cliente;

    /// novo = true  → orçamento acabado de preencher (pode ser salvo)
    /// novo = false → orçamento já guardado, aberto a partir da procura
    public PreviewOrcamentoPage(Orcamento orcamento, bool novo)
    {
        InitializeComponent();

        _orcamento = orcamento;
        _cliente = DataService.Dados.Clientes.FirstOrDefault(c => c.Id == orcamento.ClienteId);

        Conteudo.Content = PrintService.CriarDocumento(orcamento, _cliente);

        BtnSalvar.IsEnabled = novo;
        if (!novo) BtnSalvar.Content = "Já guardado";
    }

    private void Imprimir_Click(object s, RoutedEventArgs e) =>
        PrintService.Imprimir(_orcamento, _cliente);

    private void Salvar_Click(object s, RoutedEventArgs e)
    {
        DataService.Dados.Orcamentos.Add(_orcamento);
        DataService.Guardar();

        BtnSalvar.IsEnabled = false;
        BtnSalvar.Content = "Já guardado";
        MessageBox.Show("Orçamento guardado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Home_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new HomePage());
}
