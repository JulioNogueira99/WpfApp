using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp;
using WpfApp.Enums;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.ViewModels;
using WpfApp.Views;

public class CadastroPedidoViewModel : INotifyPropertyChanged
{
    private readonly PedidoService _pedidoService;
    private readonly ProdutoService _produtoService;
    private readonly PessoaService _pessoaService;

    public ObservableCollection<Pessoa> Pessoas { get; }
    public ObservableCollection<Produto> Produtos { get; }
    public ObservableCollection<ItemPedido> ItensPedido { get; }
    public ObservableCollection<FormaPagamento> FormasPagamento { get; }

    private bool _pessoaBloqueada;
    public bool PessoaBloqueada
    {
        get => _pessoaBloqueada;
        private set => SetProperty(ref _pessoaBloqueada, value);
    }

    public bool PodeSelecionarPessoa => !PessoaBloqueada;

    private Pessoa _pessoaSelecionada;
    public Pessoa PessoaSelecionada
    {
        get => _pessoaSelecionada;
        set
        {
            if (SetProperty(ref _pessoaSelecionada, value))
            {
                PessoaBloqueada = _pessoaSelecionada != null;
            }
        }
    }

    private Produto _produtoSelecionado;
    public Produto ProdutoSelecionado
    {
        get => _produtoSelecionado;
        set
        {
            if (SetProperty(ref _produtoSelecionado, value))
            {
                AdicionarProdutoCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private int _quantidade = 1;
    public int Quantidade
    {
        get => _quantidade;
        set
        {
            if (SetProperty(ref _quantidade, value))
            {
                AdicionarProdutoCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private FormaPagamento _formaPagamentoSelecionada;
    public FormaPagamento FormaPagamentoSelecionada
    {
        get => _formaPagamentoSelecionada;
        set => SetProperty(ref _formaPagamentoSelecionada, value);
    }

    public decimal ValorTotal => ItensPedido.Sum(i => i.Quantidade * i.Produto.Valor);

    private bool _pedidoFinalizado;
    public bool PedidoFinalizado
    {
        get => _pedidoFinalizado;
        set
        {
            if (SetProperty(ref _pedidoFinalizado, value))
            {
                AdicionarProdutoCommand.RaiseCanExecuteChanged();
                FinalizarPedidoCommand.RaiseCanExecuteChanged();
                RemoverProdutoCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsPedidoIniciadoViaPessoa { get; set; } = false;

    public event Action PedidoCancelado;

    private ItemPedido _itemSelecionado;
    public ItemPedido ItemSelecionado
    {
        get => _itemSelecionado;
        set
        {
            if (SetProperty(ref _itemSelecionado, value))
            {
                RemoverProdutoCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand AdicionarProdutoCommand { get; }
    public RelayCommand RemoverProdutoCommand { get; }
    public RelayCommand FinalizarPedidoCommand { get; }
    public RelayCommand CancelarPedidoCommand { get; }

    public CadastroPedidoViewModel(PedidoService pedidoService, ProdutoService produtoService, PessoaService pessoaService)
    {
        _pedidoService = pedidoService;
        _produtoService = produtoService;
        _pessoaService = pessoaService;

        Pessoas = new ObservableCollection<Pessoa>(_pessoaService.ObterPessoas());
        Produtos = new ObservableCollection<Produto>(_produtoService.ObterProdutos());
        ItensPedido = new ObservableCollection<ItemPedido>();
        ItensPedido.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(ValorTotal));
            FinalizarPedidoCommand.RaiseCanExecuteChanged();
            AdicionarProdutoCommand.RaiseCanExecuteChanged();
        };

        FormasPagamento = new ObservableCollection<FormaPagamento>(
            Enum.GetValues(typeof(FormaPagamento)).Cast<FormaPagamento>());

        FormaPagamentoSelecionada = FormasPagamento.FirstOrDefault();

        AdicionarProdutoCommand = new RelayCommand(AdicionarProduto, PodeAdicionarProduto);
        RemoverProdutoCommand = new RelayCommand(RemoverProduto, PodeRemoverProduto);
        FinalizarPedidoCommand = new RelayCommand(FinalizarPedido, PodeFinalizarPedido);
        CancelarPedidoCommand = new RelayCommand(CancelarPedido);
    }

    private void AdicionarProduto()
    {
        var existente = ItensPedido.FirstOrDefault(i => i.Produto.Id == ProdutoSelecionado.Id);
        if (existente != null)
        {
            existente.Quantidade += Quantidade;
        }
        else
        {
            ItensPedido.Add(new ItemPedido { Produto = ProdutoSelecionado, Quantidade = Quantidade });
        }

        ProdutoSelecionado = null;
        Quantidade = 1;
    }

    private bool PodeAdicionarProduto()
    {
        return ProdutoSelecionado != null && Quantidade > 0 && !PedidoFinalizado;
    }

    private void RemoverProduto()
    {
        if (ItemSelecionado != null)
        {
            ItensPedido.Remove(ItemSelecionado);
        }
    }

    private bool PodeRemoverProduto()
    {
        return ItemSelecionado != null && !PedidoFinalizado;
    }

    private void FinalizarPedido()
    {
        var pedido = new Pedido
        {
            Pessoa = PessoaSelecionada,
            Produtos = ItensPedido.ToList(),
            FormaPagamento = FormaPagamentoSelecionada,
            Status = StatusPedido.Pendente,
            DataVenda = DateTime.Now
        };

        _pedidoService.Adicionar(pedido);

        PedidoFinalizado = true;

        System.Windows.MessageBox.Show("Pedido finalizado com sucesso!", "Sucesso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

        ResetarPedido();

        var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            mainWindow.MainContent.Content = new CadastroPessoa();
        }
    }


    private void CancelarPedido()
    {
        ResetarPedido();

        if (IsPedidoIniciadoViaPessoa)
        {
            PedidoCancelado?.Invoke();

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = new CadastroPessoa();
            }
        }
    }


    private void ResetarPedido()
    {
        PessoaSelecionada = null;
        ItensPedido.Clear();
        ProdutoSelecionado = null;
        Quantidade = 1;
        FormaPagamentoSelecionada = FormasPagamento.FirstOrDefault();
        PedidoFinalizado = false;

        ((RelayCommand)FinalizarPedidoCommand).RaiseCanExecuteChanged();
        ((RelayCommand)AdicionarProdutoCommand).RaiseCanExecuteChanged();
        ((RelayCommand)CancelarPedidoCommand).RaiseCanExecuteChanged();
    }

    private bool PodeFinalizarPedido()
    {
        return PessoaSelecionada != null && ItensPedido.Any() && !PedidoFinalizado;
    }



    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propName);
        return true;
    }
    #endregion
}

