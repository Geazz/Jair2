namespace OrcamentoObra.Models;

public class ItemOrcamento
{
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string Descricao { get; set; } = "";
    public decimal PrecoTotal => Quantidade * PrecoUnitario;
}

public class Orcamento
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Numero { get; set; } = "";
    public DateTime Data { get; set; } = DateTime.Today;
    public string ClienteId { get; set; } = "";
    public DateTime Validade { get; set; } = DateTime.Today.AddDays(30);
    public string Localidade { get; set; } = "";
    public string Descricao { get; set; } = "";
    public bool ComIva { get; set; }
    public decimal PercentagemIva { get; set; }
    public List<ItemOrcamento> Itens { get; set; } = new();

    public decimal Subtotal => Itens.Sum(i => i.PrecoTotal);
    public decimal ValorIva => ComIva ? Subtotal * PercentagemIva / 100m : 0;
    public decimal Total => Subtotal + ValorIva;
}
