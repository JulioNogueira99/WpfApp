using System.Windows;
using WpfApp.Services;
using WpfApp.Views;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnPessoas_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CadastroPessoa();
        }

        private void BtnProdutos_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CadastroProduto();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            var pedidoService = new PedidoService();
            var produtoService = new ProdutoService();
            var pessoaService = new PessoaService();

            var vmPedido = new CadastroPedidoViewModel(pedidoService, produtoService, pessoaService);

            var telaPedido = new CadastroPedido
            {
                DataContext = vmPedido
            };

            MainContent.Content = telaPedido;
        }

    }
}
