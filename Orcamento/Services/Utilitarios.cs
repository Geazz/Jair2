using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace OrcamentoObra.Services;

public static class Utilitarios
{
    private static readonly CultureInfo PT = new("pt-PT");

    /// Verifica se todos os campos estão preenchidos. Mostra aviso no primeiro vazio.
    public static bool Preenchido(params (TextBox Campo, string Nome)[] campos)
    {
        foreach (var (campo, nome) in campos)
        {
            if (string.IsNullOrWhiteSpace(campo.Text))
            {
                Aviso($"O campo '{nome}' é obrigatório.");
                campo.Focus();
                return false;
            }
        }
        return true;
    }

    public static bool TryDecimal(string texto, out decimal valor) =>
        decimal.TryParse(texto, NumberStyles.Number, PT, out valor) ||
        decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);

    public static string Moeda(decimal valor) => valor.ToString("C2", PT);

    public static void Aviso(string mensagem) =>
        MessageBox.Show(mensagem, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
}
