using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Enums;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class CadastroPedido : UserControl
    {
        public CadastroPedido()
        {
            InitializeComponent();
        }

        public void SetPessoa(Pessoa pessoa)
        {
            if (pessoa == null) return;

            if (DataContext is CadastroPedidoViewModel vm)
            {
                vm.PessoaSelecionada = pessoa;
            }
        }
    }
}
