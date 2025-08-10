using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WpfApp.Models;
using System;
using System.Linq;

namespace WpfApp.Services
{
    public class PessoaService
    {
        private readonly string caminhoBase = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        private readonly string arquivo;
        private List<Pessoa> listaPessoas = new List<Pessoa>();
        private int _proximoId;

        public PessoaService()
        {
            arquivo = Path.Combine(caminhoBase, "Data", "pessoas.json");

            Directory.CreateDirectory(Path.GetDirectoryName(arquivo));
            Carregar();

            _proximoId = listaPessoas.Any() ? listaPessoas.Max(p => p.Id) + 1 : 1;
        }

        public List<Pessoa> ObterPessoas()
        {
            return listaPessoas;
        }

        public void Adicionar(Pessoa pessoa)
        {
            pessoa.SetId(_proximoId++);
            listaPessoas.Add(pessoa);
            Salvar();
        }

        public void Atualizar(Pessoa pessoa)
        {
            var index = listaPessoas.FindIndex(p => p.Id == pessoa.Id);
            if (index >= 0)
            {
                listaPessoas[index] = pessoa;
                Salvar();
            }
        }

        public void Remover(Pessoa pessoa)
        {
            listaPessoas.RemoveAll(p => p.Id == pessoa.Id);
            Salvar();
        }

        private void Carregar()
        {
            if (File.Exists(arquivo))
            {
                var json = File.ReadAllText(arquivo);
                listaPessoas = JsonConvert.DeserializeObject<List<Pessoa>>(json) ?? new List<Pessoa>();
            }
        }

        private void Salvar()
        {
            var json = JsonConvert.SerializeObject(listaPessoas, Formatting.Indented);
            File.WriteAllText(arquivo, json);
            Console.WriteLine($"Salvando arquivo em: {Path.GetFullPath(arquivo)}");
        }
    }
}
