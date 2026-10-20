using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OrcamentoObra.Models;
using OrcamentoObra.Services;

namespace OrcamentoObra.Pages;

/// Linha apresentada na tabela (evita lógica de formatação no XAML).
public class LinhaOrcamento
{
    public Orcamento Orcamento { get; }
    public string Numero => Orcamento.Numero;
    public string Data => Orcamento.Data.ToString("dd/MM/yyyy");
    public string Cliente { get; }
    public string Validade => Orcamento.Validade.ToString("dd/MM/yyyy");
    public string Total => Utilitarios.Moeda(Orcamento.Total);

    public LinhaOrcamento(Orcamento o, string cliente)
    {
        Orcamento = o;
        Cliente = cliente;
    }
}

public partial class ProcurarOrcamentoPage : Page
{
    public ProcurarOrcamentoPage()
    {
        InitializeComponent();
        Atualizar();
    }

    private void Atualizar()
    {
        string termo = TxtPesquisa.Text.Trim();

        GridOrcamentos.ItemsSource = DataService.Dados.Orcamentos
            .OrderByDescending(o => o.Data)
            .Select(o => new LinhaOrcamento(o, NomeCliente(o.ClienteId)))
            .Where(l => string.IsNullOrEmpty(termo)
                     || l.Numero.Contains(termo, StringComparison.OrdinalIgnoreCase)
                     || l.Cliente.Contains(termo, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static string NomeCliente(string clienteId) =>
        DataService.Dados.Clientes.FirstOrDefault(c => c.Id == clienteId)?.Nome ?? "(cliente removido)";

    private void Pesquisa_TextChanged(object s, TextChangedEventArgs e) => Atualizar();

    private void Abrir()
    {
        if (GridOrcamentos.SelectedItem is LinhaOrcamento linha)
            MainWindow.Ir(new PreviewOrcamentoPage(linha.Orcamento, false));
        else
            Utilitarios.Aviso("Selecione um orçamento na lista.");
    }

    private void Abrir_Click(object s, RoutedEventArgs e) => Abrir();

    private void GridOrcamentos_MouseDoubleClick(object s, MouseButtonEventArgs e) => Abrir();

    private void Voltar_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new HomePage());
}
