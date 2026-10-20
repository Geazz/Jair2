namespace OrcamentoObra.Models;

public class Cliente
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nome { get; set; } = "";
    public string Nif { get; set; } = "";
    public string Morada { get; set; } = "";
    public string Telemovel { get; set; } = "";
    public string Email { get; set; } = "";
    public string DadosBancarios { get; set; } = "";
    public decimal DinheiroGastoObras { get; set; }

    public override string ToString() => $"{Nome}  (NIF: {Nif})";
}
