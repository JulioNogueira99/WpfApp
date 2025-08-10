using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class ProdutoService
{
    private readonly string arquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Data", "produtos.json");
    private List<Produto> listaProdutos = new List<Produto>();
    private int contadorId;

    public ProdutoService()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(arquivo));
        if (!File.Exists(arquivo))
            File.WriteAllText(arquivo, "[]");

        Carregar();
    }

    private void Carregar()
    {
        try
        {
            if (File.Exists(arquivo))
            {
                var json = File.ReadAllText(arquivo);
                listaProdutos = JsonConvert.DeserializeObject<List<Produto>>(json) ?? new List<Produto>();

                contadorId = listaProdutos.Any() ? listaProdutos.Max(p => p.Id) + 1 : 1;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro ao carregar produtos: {ex.Message}");
            listaProdutos = new List<Produto>();
            contadorId = 1;
        }
    }

    public List<Produto> ObterProdutos()
    {
        Carregar();
        return listaProdutos;
    }

    public void Adicionar(Produto produto)
    {
        if (produto.Id == 0)
        {
            int novoId = listaProdutos.Any() ? listaProdutos.Max(p => p.Id) + 1 : 1;
            produto.SetId(novoId);
        }

        listaProdutos.Add(produto);
        Salvar();
    }


    public void Atualizar(Produto produto)
    {
        var index = listaProdutos.FindIndex(p => p.Id == produto.Id);
        if (index >= 0)
        {
            listaProdutos[index] = produto;
            Salvar();
        }
    }

    public void Remover(Produto produto)
    {
        listaProdutos.RemoveAll(p => p.Id == produto.Id);
        Salvar();
    }

    private void Salvar()
    {
        var json = JsonConvert.SerializeObject(listaProdutos, Formatting.Indented);
        File.WriteAllText(arquivo, json);
    }
}
