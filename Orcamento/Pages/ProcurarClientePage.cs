using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OrcamentoObra.Models;
using OrcamentoObra.Services;

namespace OrcamentoObra.Pages;

public partial class ProcurarClientePage : Page
{
    public ProcurarClientePage()
    {
        InitializeComponent();
        Atualizar();
    }

    private void Atualizar()
    {
        string termo = TxtPesquisa.Text.Trim();

        ListaClientes.ItemsSource = DataService.Dados.Clientes
            .Where(c => string.IsNullOrEmpty(termo)
                     || c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)
                     || c.Nif.Contains(termo))
            .OrderBy(c => c.Nome)
            .ToList();
    }

    private void Pesquisa_TextChanged(object s, TextChangedEventArgs e) => Atualizar();

    private void ListaClientes_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        if (ListaClientes.SelectedItem is not Cliente c)
        {
            TxtDetalhes.Text = "Selecione um cliente na lista.";
            return;
        }

        int numOrcamentos = DataService.Dados.Orcamentos.Count(o => o.ClienteId == c.Id);

        TxtDetalhes.Text =
            $"Nome: {c.Nome}\n" +
            $"NIF: {c.Nif}\n" +
            $"Morada: {c.Morada}\n" +
            $"Telemóvel: {c.Telemovel}\n" +
            $"Email: {c.Email}\n" +
            $"Dados bancários: {c.DadosBancarios}\n" +
            $"Dinheiro gasto com obras: {Utilitarios.Moeda(c.DinheiroGastoObras)}\n" +
            $"Orçamentos feitos: {numOrcamentos}";
    }

    private void Voltar_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new HomePage());
}
