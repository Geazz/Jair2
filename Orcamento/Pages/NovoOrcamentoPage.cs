using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OrcamentoObra.Models;
using OrcamentoObra.Services;

namespace OrcamentoObra.Pages;

public partial class NovoOrcamentoPage : Page
{
    private readonly ObservableCollection<ItemOrcamento> _itens = new();

    public NovoOrcamentoPage()
    {
        InitializeComponent();

        TxtNumero.Text = DataService.ProximoNumero();
        DpData.SelectedDate = DateTime.Today;
        DpValidade.SelectedDate = DateTime.Today.AddDays(30);
        CmbCliente.ItemsSource = DataService.Dados.Clientes;
        GridItens.ItemsSource = _itens;

        if (DataService.Dados.Clientes.Count == 0)
            Utilitarios.Aviso("Ainda não existe nenhum cliente. Adicione um cliente antes de fazer o orçamento.");
    }

    private void RbIva_Changed(object s, RoutedEventArgs e)
    {
        TxtPercIva.IsEnabled = RbComIva.IsChecked == true;
        if (!TxtPercIva.IsEnabled) TxtPercIva.Clear();
        AtualizarTotais();
    }

    private void AdicionarItem_Click(object s, RoutedEventArgs e)
    {
        if (!Utilitarios.Preenchido(
            (TxtQtd, "Quantidade"), (TxtPrecoUnit, "Preço unitário"), (TxtDescItem, "Descrição do item")))
            return;

        if (!int.TryParse(TxtQtd.Text.Trim(), out int quantidade) || quantidade <= 0)
        {
            Utilitarios.Aviso("A quantidade deve ser um número inteiro maior que zero.");
            TxtQtd.Focus();
            return;
        }

        if (!Utilitarios.TryDecimal(TxtPrecoUnit.Text, out decimal precoUnitario) || precoUnitario < 0)
        {
            Utilitarios.Aviso("Indique um preço unitário válido.");
            TxtPrecoUnit.Focus();
            return;
        }

        _itens.Add(new ItemOrcamento
        {
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario,
            Descricao = TxtDescItem.Text.Trim()
        });

        TxtQtd.Clear();
        TxtPrecoUnit.Clear();
        TxtDescItem.Clear();
        TxtQtd.Focus();
        AtualizarTotais();
    }

    private void RemoverItem_Click(object s, RoutedEventArgs e)
    {
        if (GridItens.SelectedItem is ItemOrcamento item)
        {
            _itens.Remove(item);
            AtualizarTotais();
        }
        else
            Utilitarios.Aviso("Selecione um item na tabela para o remover.");
    }

    private void AtualizarTotais()
    {
        decimal subtotal = _itens.Sum(i => i.PrecoTotal);
        string texto = $"Subtotal: {Utilitarios.Moeda(subtotal)}";

        if (RbComIva.IsChecked == true && Utilitarios.TryDecimal(TxtPercIva.Text, out decimal perc) && perc >= 0)
        {
            decimal iva = subtotal * perc / 100m;
            texto += $"     IVA ({perc}%): {Utilitarios.Moeda(iva)}     Total: {Utilitarios.Moeda(subtotal + iva)}";
        }

        LblTotais.Text = texto;
    }

    private void Continuar_Click(object s, RoutedEventArgs e)
    {
        if (!Utilitarios.Preenchido(
            (TxtNumero, "Número do orçamento"), (TxtLocalidade, "Localidade"), (TxtDescricao, "Descrição da obra")))
            return;

        if (DpData.SelectedDate is null)
        {
            Utilitarios.Aviso("O campo 'Data' é obrigatório.");
            return;
        }

        if (DpValidade.SelectedDate is null)
        {
            Utilitarios.Aviso("O campo 'Validade' é obrigatório.");
            return;
        }

        if (DpValidade.SelectedDate < DpData.SelectedDate)
        {
            Utilitarios.Aviso("A validade não pode ser anterior à data do orçamento.");
            return;
        }

        if (CmbCliente.SelectedItem is not Cliente cliente)
        {
            Utilitarios.Aviso("O campo 'Cliente' é obrigatório.");
            return;
        }

        if (RbComIva.IsChecked != true && RbSemIva.IsChecked != true)
        {
            Utilitarios.Aviso("Indique se o orçamento é com IVA ou sem IVA.");
            return;
        }

        bool comIva = RbComIva.IsChecked == true;
        decimal percentagem = 0;

        if (comIva)
        {
            if (!Utilitarios.TryDecimal(TxtPercIva.Text, out percentagem) || percentagem < 0 || percentagem > 100)
            {
                Utilitarios.Aviso("Indique uma percentagem de IVA válida (0 a 100).");
                TxtPercIva.Focus();
                return;
            }
        }

        if (_itens.Count == 0)
        {
            Utilitarios.Aviso("Adicione pelo menos um item ao orçamento.");
            return;
        }

        var orcamento = new Orcamento
        {
            Numero = TxtNumero.Text.Trim(),
            Data = DpData.SelectedDate.Value,
            ClienteId = cliente.Id,
            Validade = DpValidade.SelectedDate.Value,
            Localidade = TxtLocalidade.Text.Trim(),
            Descricao = TxtDescricao.Text.Trim(),
            ComIva = comIva,
            PercentagemIva = percentagem,
            Itens = _itens.ToList()
        };

        MainWindow.Ir(new PreviewOrcamentoPage(orcamento, true));
    }

    private void Voltar_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new HomePage());
}
