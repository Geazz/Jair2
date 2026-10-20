using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OrcamentoObra.Models;
using OrcamentoObra.Services;

namespace OrcamentoObra.Pages;

public partial class NovoClientePage : Page
{
    public NovoClientePage() => InitializeComponent();

    private void Guardar_Click(object s, RoutedEventArgs e)
    {
        if (!Utilitarios.Preenchido(
            (TxtNome, "Nome"), (TxtNif, "NIF"), (TxtMorada, "Morada"),
            (TxtTelemovel, "Telemóvel"), (TxtEmail, "Email"),
            (TxtBancario, "Dados bancários"), (TxtGasto, "Dinheiro gasto com obras")))
            return;

        string nif = TxtNif.Text.Trim();
        if (nif.Length != 9 || !nif.All(char.IsDigit))
        {
            Utilitarios.Aviso("O NIF deve ter 9 dígitos.");
            TxtNif.Focus();
            return;
        }

        if (!Utilitarios.TryDecimal(TxtGasto.Text, out decimal gasto) || gasto < 0)
        {
            Utilitarios.Aviso("Indique um valor válido para o dinheiro gasto com obras.");
            TxtGasto.Focus();
            return;
        }

        if (DataService.Dados.Clientes.Any(c => c.Nif == nif))
        {
            Utilitarios.Aviso("Já existe um cliente com este NIF.");
            TxtNif.Focus();
            return;
        }

        DataService.Dados.Clientes.Add(new Cliente
        {
            Nome = TxtNome.Text.Trim(),
            Nif = nif,
            Morada = TxtMorada.Text.Trim(),
            Telemovel = TxtTelemovel.Text.Trim(),
            Email = TxtEmail.Text.Trim(),
            DadosBancarios = TxtBancario.Text.Trim(),
            DinheiroGastoObras = gasto
        });
        DataService.Guardar();

        MessageBox.Show("Cliente guardado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        MainWindow.Ir(new HomePage());
    }

    private void Voltar_Click(object s, RoutedEventArgs e) => MainWindow.Ir(new HomePage());
}
