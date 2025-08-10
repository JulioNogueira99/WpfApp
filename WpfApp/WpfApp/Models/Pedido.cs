using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Enums;

namespace WpfApp.Models
{
    public class ItemPedido
    {
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }

        public decimal ValorTotal => Produto.Valor * Quantidade;
    }

    public class Pedido
    {
        private static int _contadorId = 1;

        public int Id { get; set; }

        public Pessoa Pessoa { get; set; }
        public List<ItemPedido> Produtos { get; set; } = new List<ItemPedido>();
        public decimal ValorTotal => Produtos.Sum(p => p.ValorTotal);
        public DateTime DataVenda { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;

        public Pedido()
        {
            DataVenda = DateTime.Now;
        }

        public bool Validar()
        {
            return Pessoa != null
                   && Produtos != null
                   && Produtos.Count > 0;
        }

        public static void AjustarContador(int maxId)
        {
            _contadorId = maxId + 1;
        }

        public static int GerarNovoId()
        {
            return _contadorId++;
        }
    }


}
