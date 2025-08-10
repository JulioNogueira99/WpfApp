using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class PedidoService
    {
        private readonly string arquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Data", "pedidos.json");
        private List<Pedido> listaPedidos = new List<Pedido>();

        public PedidoService()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(arquivo));
            if (!File.Exists(arquivo))
                File.WriteAllText(arquivo, "[]");

            Carregar();
        }

        public List<Pedido> ObterPedidos()
        {
            return listaPedidos;
        }

        public void Adicionar(Pedido pedido)
        {
            if (pedido.Id == 0)
            {
                pedido.Id = Pedido.GerarNovoId();
            }
            listaPedidos.Add(pedido);
            Salvar();
        }


        public void Atualizar(Pedido pedido)
        {
            var index = listaPedidos.FindIndex(p => p.Id == pedido.Id);
            if (index >= 0)
            {
                listaPedidos[index] = pedido;
                Salvar();
            }
        }

        public void Remover(Pedido pedido)
        {
            listaPedidos.RemoveAll(p => p.Id == pedido.Id);
            Salvar();
        }

        private void Carregar()
        {
            if (File.Exists(arquivo))
            {
                var json = File.ReadAllText(arquivo);
                listaPedidos = JsonConvert.DeserializeObject<List<Pedido>>(json) ?? new List<Pedido>();

                if (listaPedidos.Any())
                {
                    int maxId = listaPedidos.Max(p => p.Id);
                    Pedido.AjustarContador(maxId);
                }
            }
        }


        private void Salvar()
        {
            var json = JsonConvert.SerializeObject(listaPedidos, Formatting.Indented);
            File.WriteAllText(arquivo, json);
        }
    }
}
