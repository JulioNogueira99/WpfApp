using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Enums;
using WpfApp.Models;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class CadastroPessoa : UserControl
    {
        private StatusPedido _statusAnterior;
        private bool selecionouStatus = false;

        public CadastroPessoa()
        {
            InitializeComponent();

            if (DataContext is PessoaViewModel vm)
                vm.PedidoIncluirSolicitado += OnPedidoIncluirSolicitado;
        }

        private void OnPedidoIncluirSolicitado(Pessoa pessoa)
        {
            var pedidoControl = new CadastroPedido();
            pedidoControl.SetPessoa(pessoa);

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContent.Content = pedidoControl;
        }

        private void BtnLimparFiltroPedidos_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as PessoaViewModel;
            if (vm != null)
            {
                vm.FiltroStatusPedido = null;
            }
        }

        private void ComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is Pedido pedido)
            {
                _statusAnterior = pedido.Status;
                selecionouStatus = true;
            }
        }


        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is Pedido pedido)
            {
                _statusAnterior = pedido.Status;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is Pedido pedido && combo.SelectedItem is StatusPedido novoStatus)
            {
                if (selecionouStatus && novoStatus != _statusAnterior)
                {
                    if (DataContext is PessoaViewModel vm)
                    {
                        vm.AtualizarStatusPedido(pedido, novoStatus, _statusAnterior);
                        _statusAnterior = novoStatus;
                        selecionouStatus = false;
                    }
                }

            }
        }



    }
}
