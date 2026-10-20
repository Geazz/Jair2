using System.IO;
using System.Text.Json;
using OrcamentoObra.Models;

namespace OrcamentoObra.Services;

public class DadosApp
{
    public List<Cliente> Clientes { get; set; } = new();
    public List<Orcamento> Orcamentos { get; set; } = new();
}

public static class DataService
{
    // dados.json fica na pasta do executável
    public static readonly string Caminho = Path.Combine(AppContext.BaseDirectory, "dados.json");
    private static readonly JsonSerializerOptions Opcoes = new() { WriteIndented = true };

    public static DadosApp Dados { get; private set; } = Carregar();

    private static DadosApp Carregar()
    {
        if (!File.Exists(Caminho)) return new DadosApp();
        try
        {
            return JsonSerializer.Deserialize<DadosApp>(File.ReadAllText(Caminho)) ?? new DadosApp();
        }
        catch
        {
            // Ficheiro corrompido: guarda cópia de segurança e começa limpo
            File.Move(Caminho, Caminho + ".erro", true);
            return new DadosApp();
        }
    }

    public static void Guardar() =>
        File.WriteAllText(Caminho, JsonSerializer.Serialize(Dados, Opcoes));

    public static string ProximoNumero()
    {
        string ano = DateTime.Today.Year.ToString();
        int n = Dados.Orcamentos.Count + 1;
        while (Dados.Orcamentos.Any(o => o.Numero == $"ORC-{ano}-{n:0000}")) n++;
        return $"ORC-{ano}-{n:0000}";
    }
}
