using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using OrcamentoObra.Models;

namespace OrcamentoObra.Services;

public static class PrintService
{
    public const double Largura = 720;
    private static readonly double[] Colunas = { 1, 4, 2, 2 };

    /// Cria o documento do orçamento (banner + dados + itens + totais).
    public static FrameworkElement CriarDocumento(Orcamento o, Cliente? c)
    {
        var doc = new StackPanel { Width = Largura, Background = Brushes.White };

        doc.Children.Add(new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/Assets/banner.png")),
            Width = Largura,
            Stretch = Stretch.Uniform
        });

        var corpo = new StackPanel { Margin = new Thickness(30, 10, 30, 25) };
        doc.Children.Add(corpo);

        corpo.Children.Add(Texto($"ORÇAMENTO Nº {o.Numero}", 20, true, TextAlignment.Center));
        corpo.Children.Add(Texto($"Data: {o.Data:dd/MM/yyyy}     Validade: {o.Validade:dd/MM/yyyy}     Localidade: {o.Localidade}", 11, false, TextAlignment.Center));

        corpo.Children.Add(Titulo("Dados do cliente"));
        if (c == null)
            corpo.Children.Add(Texto("Cliente não encontrado.", 12));
        else
        {
            corpo.Children.Add(Texto($"Nome: {c.Nome}", 12));
            corpo.Children.Add(Texto($"NIF: {c.Nif}", 12));
            corpo.Children.Add(Texto($"Morada: {c.Morada}", 12));
            corpo.Children.Add(Texto($"Telemóvel: {c.Telemovel}   Email: {c.Email}", 12));
            corpo.Children.Add(Texto($"Dados bancários: {c.DadosBancarios}", 12));
        }

        corpo.Children.Add(Titulo("Descrição da obra"));
        corpo.Children.Add(Texto(o.Descricao, 12));

        corpo.Children.Add(Titulo("Itens"));
        corpo.Children.Add(Linha(true, "Qtd.", "Descrição", "Preço unit.", "Preço total"));
        foreach (var i in o.Itens)
            corpo.Children.Add(Linha(false, i.Quantidade.ToString(), i.Descricao,
                Utilitarios.Moeda(i.PrecoUnitario), Utilitarios.Moeda(i.PrecoTotal)));

        corpo.Children.Add(Linha(false, "", "", "Subtotal", Utilitarios.Moeda(o.Subtotal)));
        if (o.ComIva)
            corpo.Children.Add(Linha(false, "", "", $"IVA ({o.PercentagemIva}%)", Utilitarios.Moeda(o.ValorIva)));
        else
            corpo.Children.Add(Texto("Documento sem IVA.", 10));
        corpo.Children.Add(Linha(true, "", "", "TOTAL", Utilitarios.Moeda(o.Total)));

        return doc;
    }

    /// Abre o diálogo de impressão (escolher "Microsoft Print to PDF" para gerar PDF).
    public static void Imprimir(Orcamento o, Cliente? c)
    {
        var pd = new PrintDialog();
        if (pd.ShowDialog() != true) return;

        double pw = pd.PrintableAreaWidth;
        double ph = pd.PrintableAreaHeight;

        // Viewbox reduz o documento para caber numa página, se necessário
        var vb = new Viewbox
        {
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
            Width = pw,
            Height = ph,
            Child = CriarDocumento(o, c)
        };
        vb.Measure(new Size(pw, ph));
        vb.Arrange(new Rect(0, 0, pw, ph));

        pd.PrintVisual(vb, $"Orçamento {o.Numero}");
    }

    private static TextBlock Texto(string texto, double tamanho, bool negrito = false,
        TextAlignment alinhamento = TextAlignment.Left) => new()
    {
        Text = texto,
        FontSize = tamanho,
        FontWeight = negrito ? FontWeights.Bold : FontWeights.Normal,
        TextAlignment = alinhamento,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(0, 2, 0, 2)
    };

    private static TextBlock Titulo(string texto) => new()
    {
        Text = texto,
        FontSize = 14,
        FontWeight = FontWeights.Bold,
        Foreground = Brushes.DarkBlue,
        Margin = new Thickness(0, 14, 0, 4)
    };

    private static Grid Linha(bool cabecalho, params string[] valores)
    {
        var g = new Grid { Margin = new Thickness(0, 1, 0, 1) };
        foreach (var w in Colunas)
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w, GridUnitType.Star) });

        for (int i = 0; i < valores.Length; i++)
        {
            var tb = new TextBlock
            {
                Text = valores[i],
                FontWeight = cabecalho ? FontWeights.Bold : FontWeights.Normal,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(4, 3, 4, 3),
                Background = cabecalho ? Brushes.LightGray : Brushes.Transparent,
                TextAlignment = i >= 2 ? TextAlignment.Right : TextAlignment.Left
            };
            Grid.SetColumn(tb, i);
            g.Children.Add(tb);
        }
        return g;
    }
}
