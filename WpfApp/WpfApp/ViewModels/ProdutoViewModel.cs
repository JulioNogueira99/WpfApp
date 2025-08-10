using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class ProdutoViewModel : INotifyPropertyChanged
    {
        private readonly ProdutoService _produtoService = new ProdutoService();
        private bool _emProcessamento = false;

        public ObservableCollection<Produto> Produtos { get; set; }
        public ObservableCollection<Produto> ProdutosFiltrados { get; set; }

        private Produto produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => produtoSelecionado;
            set
            {
                if (_emProcessamento || produtoSelecionado == value)
                    return;

                _emProcessamento = true;

                produtoSelecionado = value;
                OnPropertyChanged();
                AtualizarBotoes();            

                _emProcessamento = false;             
            }
        }

        private bool estaEditando = false;
        public bool EstaEditando
        {
            get => estaEditando;
            set
            {
                estaEditando = value;
                OnPropertyChanged();
                AtualizarBotoes();
            }
        }

        private string nomeFiltro = string.Empty;
        public string NomeFiltro
        {
            get => nomeFiltro;
            set
            {
                if (_emProcessamento) return;
                nomeFiltro = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private string codigoFiltro = string.Empty;
        public string CodigoFiltro
        {
            get => codigoFiltro;
            set
            {
                if (_emProcessamento) return;
                codigoFiltro = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private string valorMinFiltro;
        public string ValorMinFiltro
        {
            get => valorMinFiltro;
            set
            {
                if (_emProcessamento) return;
                valorMinFiltro = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private string valorMaxFiltro;
        public string ValorMaxFiltro
        {
            get => valorMaxFiltro;
            set
            {
                if (_emProcessamento) return;
                valorMaxFiltro = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }

        private bool podeEditar;
        public bool PodeEditar
        {
            get => podeEditar;
            set { podeEditar = value; OnPropertyChanged(); }
        }

        private bool podeSalvar;
        public bool PodeSalvar
        {
            get => podeSalvar;
            set { podeSalvar = value; OnPropertyChanged(); }
        }

        private bool podeExcluir;
        public bool PodeExcluir
        {
            get => podeExcluir;
            set { podeExcluir = value; OnPropertyChanged(); }
        }

        public ProdutoViewModel()
        {
            var produtos = _produtoService.ObterProdutos();

            Produtos = new ObservableCollection<Produto>(produtos);
            ProdutosFiltrados = new ObservableCollection<Produto>(produtos);

            IncluirCommand = new RelayCommand(Incluir, () => !EstaEditando);
            EditarCommand = new RelayCommand(Editar, () => PodeEditar && !EstaEditando);
            SalvarCommand = new RelayCommand(Salvar, () => PodeSalvar && EstaEditando);
            ExcluirCommand = new RelayCommand(Excluir, () => PodeExcluir && !EstaEditando);

            AtualizarBotoes();
        }

        private void AplicarFiltro()
        {
            if (_emProcessamento) return;

            try
            {
                _emProcessamento = true;

                decimal.TryParse(ValorMinFiltro, out decimal valorMin);
                decimal.TryParse(ValorMaxFiltro, out decimal valorMax);

                var listaFiltrada = Produtos.Where(p =>
                    (string.IsNullOrWhiteSpace(NomeFiltro) || (p.Nome?.IndexOf(NomeFiltro, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                    (string.IsNullOrWhiteSpace(CodigoFiltro) || (p.Codigo?.IndexOf(CodigoFiltro, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                    (string.IsNullOrWhiteSpace(ValorMinFiltro) || p.Valor >= valorMin) &&
                    (string.IsNullOrWhiteSpace(ValorMaxFiltro) || p.Valor <= valorMax)
                ).ToList();

                ProdutosFiltrados.Clear();
                foreach (var p in listaFiltrada)
                    ProdutosFiltrados.Add(p);
            }
            finally
            {
                _emProcessamento = false;
            }
        }

        private void Incluir()
        {
            var novoProduto = new Produto
            {
                Nome = "Novo Produto",
                Codigo = "000",
                Valor = 0
            };

            Produtos.Add(novoProduto);
            ProdutosFiltrados.Add(novoProduto);

            produtoSelecionado = novoProduto;
            OnPropertyChanged(nameof(ProdutoSelecionado));

            EstaEditando = true;

            produtoSelecionado.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Produto.Validar))
                {
                    AtualizarBotoes();
                }
            };

            AtualizarBotoes();
        }

        private void Editar()
        {
            if (ProdutoSelecionado == null) return;
            EstaEditando = true;
        }

        private void Salvar()
        {
            if (ProdutoSelecionado == null) return;

            var erros = ProdutoSelecionado.Validacoes().ToList();
            if (erros.Any())
            {
                MessageBox.Show(string.Join("\n", erros), "Erros de Validação",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (ProdutoSelecionado.Id == 0)
                {
                    _produtoService.Adicionar(ProdutoSelecionado);
                    var novoId = _produtoService.ObterProdutos().Max(p => p.Id);
                    ProdutoSelecionado.SetId(novoId);
                }
                else
                {
                    _produtoService.Atualizar(ProdutoSelecionado);
                }

                EstaEditando = false;
                AplicarFiltro();
                MessageBox.Show("Produto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                var produtosAtualizados = _produtoService.ObterProdutos();
                Produtos = new ObservableCollection<Produto>(produtosAtualizados);
                ProdutosFiltrados = new ObservableCollection<Produto>(produtosAtualizados);

                OnPropertyChanged(nameof(Produtos));
                OnPropertyChanged(nameof(ProdutosFiltrados));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Excluir()
        {
            if (ProdutoSelecionado == null) return;

            var resultado = MessageBox.Show($"Confirma a exclusão do produto '{ProdutoSelecionado.Nome}'?", "Confirmar exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                _produtoService.Remover(ProdutoSelecionado);
                Produtos.Remove(ProdutoSelecionado);
                ProdutosFiltrados.Remove(ProdutoSelecionado);
                produtoSelecionado = null;
                OnPropertyChanged(nameof(ProdutoSelecionado));
            }
            EstaEditando = false;
            AtualizarBotoes();
            AplicarFiltro();
        }

        private void AtualizarBotoes()
        {
            PodeEditar = ProdutoSelecionado != null && ProdutoSelecionado.Id != 0 && !EstaEditando;
            PodeSalvar = ProdutoSelecionado != null && ProdutoSelecionado.Validar && EstaEditando;
            PodeExcluir = ProdutoSelecionado != null && ProdutoSelecionado.Id != 0 && !EstaEditando;

            ((RelayCommand)IncluirCommand).RaiseCanExecuteChanged();
            ((RelayCommand)EditarCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SalvarCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ExcluirCommand).RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}