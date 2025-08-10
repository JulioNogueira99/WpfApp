using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Enums;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class PessoaViewModel : INotifyPropertyChanged
    {
        private readonly PessoaService _pessoa = new PessoaService();
        private readonly PedidoService _pedido = new PedidoService();

        public event Action<Pessoa> PedidoIncluirSolicitado;
        public ObservableCollection<Pessoa> Pessoas { get; set; }
        public ObservableCollection<Pessoa> PessoasFiltradas { get; set; }
        public ObservableCollection<Pedido> Pedidos { get; set; }
        public ObservableCollection<Pedido> PedidosFiltrados { get; set; }
        public ObservableCollection<Pedido> PedidosPessoa { get; set; }
        public ObservableCollection<Pedido> PedidosPessoaFiltrados { get; set; }
        public ObservableCollection<StatusPedido> StatusAlteraveis { get; } = new ObservableCollection<StatusPedido>
        {
            StatusPedido.Pago,
            StatusPedido.Enviado,
            StatusPedido.Recebido
        };

        private Pessoa pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => pessoaSelecionada;
            set
            {
                pessoaSelecionada = value;
                OnPropertyChanged();
                CarregarPedidosPessoa();
                ((RelayCommand)SalvarCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ExcluirCommand).RaiseCanExecuteChanged();
                ((RelayCommand)EditarCommand).RaiseCanExecuteChanged();
            }
        }

        private StatusPedido? filtroStatusPedido;
        public StatusPedido? FiltroStatusPedido
        {
            get => filtroStatusPedido;
            set
            {
                filtroStatusPedido = value;
                OnPropertyChanged();
                AplicarFiltroPedidos();
            }
        }

        private string filtroNome = string.Empty;
        public string FiltroNome
        {
            get => filtroNome;
            set
            {
                filtroNome = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private string filtroCPF = string.Empty;
        public string FiltroCPF
        {
            get => filtroCPF;
            set
            {
                filtroCPF = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        public ICommand PesquisarCommand { get; }
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand IncluirPedidoCommand { get; }
        public ICommand MarcarPagoCommand { get; }
        public ICommand MarcarEnviadoCommand { get; }
        public ICommand MarcarRecebidoCommand { get; }

        public PessoaViewModel()
        {
            Pessoas = new ObservableCollection<Pessoa>(_pessoa.ObterPessoas());
            PessoasFiltradas = new ObservableCollection<Pessoa>(Pessoas);

            Pedidos = new ObservableCollection<Pedido>(_pedido.ObterPedidos());
            PedidosFiltrados = new ObservableCollection<Pedido>(Pedidos);

            IncluirCommand = new RelayCommand(Incluir);
            EditarCommand = new RelayCommand(Editar, PodeEditar);
            SalvarCommand = new RelayCommand(Salvar, PodeSalvar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExcluir);

            IncluirPedidoCommand = new RelayCommand(IncluirPedido);
        }

        private void AplicarFiltro()
        {
            var listaFiltrada = Pessoas.Where(p =>
                (string.IsNullOrWhiteSpace(FiltroNome) || p.Nome.IndexOf(FiltroNome, StringComparison.OrdinalIgnoreCase) >= 0)
                && (string.IsNullOrWhiteSpace(FiltroCPF) || p.CPF.Contains(FiltroCPF))
            ).ToList();

            Application.Current.Dispatcher.Invoke(() =>
            {
                PessoasFiltradas.Clear();
                foreach (var p in listaFiltrada)
                    PessoasFiltradas.Add(p);
            });
        }

        private void Incluir()
        {
            var novaPessoa = new Pessoa();

            var janela = new CadastroPessoaDetalhe(novaPessoa)
            {
                Owner = Application.Current.MainWindow
            };

            bool? resultado = janela.ShowDialog();
            if (resultado == true)
            {
                Pessoas.Add(novaPessoa);
                PessoasFiltradas.Add(novaPessoa);
                PessoaSelecionada = novaPessoa;
                _pessoa.Adicionar(novaPessoa);
            }
        }

        private void Editar()
        {
            if (PessoaSelecionada == null) return;

            var pessoaParaEditar = new Pessoa
            {
                Nome = PessoaSelecionada.Nome,
                CPF = PessoaSelecionada.CPF,
                Endereco = PessoaSelecionada.Endereco
            };

            var janela = new CadastroPessoaDetalhe(pessoaParaEditar)
            {
                Owner = Application.Current.MainWindow
            };

            bool? resultado = janela.ShowDialog();
            if (resultado == true)
            {
                PessoaSelecionada.Nome = pessoaParaEditar.Nome;
                PessoaSelecionada.CPF = pessoaParaEditar.CPF;
                PessoaSelecionada.Endereco = pessoaParaEditar.Endereco;

                _pessoa.Atualizar(PessoaSelecionada);

                OnPropertyChanged(nameof(Pessoas));
                AplicarFiltro();
            }
        }

        private bool PodeEditar()
        {
            return PessoaSelecionada != null && PessoaSelecionada.Id != 0;
        }

        private void Salvar()
        {
            if (PessoaSelecionada == null) return;

            if (PessoaSelecionada.Id == 0)
            {
                _pessoa.Adicionar(PessoaSelecionada);
            }
            else
            {
                _pessoa.Atualizar(PessoaSelecionada);
            }
        }

        private bool PodeSalvar()
        {
            return PessoaSelecionada != null
                && !string.IsNullOrWhiteSpace(PessoaSelecionada.Nome)
                && !string.IsNullOrWhiteSpace(PessoaSelecionada.CPF);
        }

        private void Excluir()
        {
            if (PessoaSelecionada == null) return;

            var resultado = MessageBox.Show($"Confirma a exclusão de {PessoaSelecionada.Nome}?", "Confirmar exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                _pessoa.Remover(PessoaSelecionada);
                Pessoas.Remove(PessoaSelecionada);
                PessoasFiltradas.Remove(PessoaSelecionada);
                PessoaSelecionada = null;
            }
        }

        private bool PodeExcluir()
        {
            return PessoaSelecionada != null && PessoaSelecionada.Id != 0;
        }

        private void IncluirPedido()
        {
            if (PessoaSelecionada == null)
            {
                MessageBox.Show("Selecione uma pessoa antes de incluir o pedido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pedidoService = new PedidoService();
            var produtoService = new ProdutoService();
            var pessoaService = new PessoaService();

            var vmPedido = new CadastroPedidoViewModel(pedidoService, produtoService, pessoaService);
            var pessoaNaLista = vmPedido.Pessoas.FirstOrDefault(p => p.Id == PessoaSelecionada.Id);

            vmPedido.PessoaSelecionada = pessoaNaLista;
            vmPedido.IsPedidoIniciadoViaPessoa = true;

            var telaPedido = new CadastroPedido
            {
                DataContext = vmPedido
            };

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = telaPedido;
        }

        private void CarregarPedidosPessoa()
        {
            if (PessoaSelecionada == null)
            {
                PedidosPessoa = new ObservableCollection<Pedido>();
                PedidosPessoaFiltrados = new ObservableCollection<Pedido>();
            }
            else
            {
                var pedidosDaPessoa = Pedidos.Where(p => p.Pessoa.Id == PessoaSelecionada.Id).ToList();
                PedidosPessoa = new ObservableCollection<Pedido>(pedidosDaPessoa);
                PedidosPessoaFiltrados = new ObservableCollection<Pedido>(pedidosDaPessoa);
            }

            OnPropertyChanged(nameof(PedidosPessoa));
            OnPropertyChanged(nameof(PedidosPessoaFiltrados));

            AplicarFiltroPedidos();
        }


        private void AplicarFiltroPedidos()
        {
            if (PedidosPessoa == null) return;

            var filtrados = PedidosPessoa.AsEnumerable();

            if (FiltroStatusPedido.HasValue)
            {
                filtrados = filtrados.Where(p => p.Status == FiltroStatusPedido.Value);
            }

            PedidosPessoaFiltrados.Clear();
            foreach (var p in filtrados)
                PedidosPessoaFiltrados.Add(p);
        }

        public void AtualizarStatusPedido(Pedido pedido, StatusPedido novoStatus, StatusPedido statusAntigo)
        {
            if (pedido == null || statusAntigo != StatusPedido.Pendente)
                return;

            pedido.Status = novoStatus;
            _pedido.Atualizar(pedido);

            AplicarFiltroPedidos();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string nomePropriedade = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }
    }
}
