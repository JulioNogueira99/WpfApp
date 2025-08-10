using System.Windows;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class CadastroPessoaDetalhe : Window
    {
        public Pessoa Pessoa { get; private set; }

        public CadastroPessoaDetalhe(Pessoa pessoa)
        {
            InitializeComponent();
            Pessoa = pessoa;

            txtNome.Text = Pessoa.Nome;
            txtCPF.Text = Pessoa.CPF;
            txtEndereco.Text = Pessoa.Endereco;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Nome é obrigatório.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCPF.Text) || !Pessoa.ValidarCPF(txtCPF.Text))
            {
                MessageBox.Show("CPF é obrigatório e deve ser válido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCPF.Focus();
                return;
            }

            Pessoa.Nome = txtNome.Text.Trim();
            Pessoa.CPF = txtCPF.Text.Trim();
            Pessoa.Endereco = txtEndereco.Text.Trim();

            DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
