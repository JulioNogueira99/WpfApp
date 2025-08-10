using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Produto : INotifyPropertyChanged
{
    private string _nome;
    private string _codigo;
    private decimal _valor;

    public int Id { get; set; }

    public string Nome
    {
        get => _nome;
        set
        {
            if (_nome == value) return;
            _nome = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Validar));
        }
    }

    public string Codigo
    {
        get => _codigo;
        set
        {
            if (_codigo == value) return;
            _codigo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Validar));
        }
    }

    public decimal Valor
    {
        get => _valor;
        set
        {
            if (_valor == value) return;
            _valor = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Validar));
        }
    }

    public bool Validar =>
        !string.IsNullOrWhiteSpace(Nome) &&
        !string.IsNullOrWhiteSpace(Codigo) &&
        Valor >= 0;

    public IEnumerable<string> Validacoes()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            yield return "Nome é obrigatório";

        if (string.IsNullOrWhiteSpace(Codigo))
            yield return "Código é obrigatório";

        if (Valor < 0)
            yield return "Valor não pode ser negativo";
    }

    public void SetId(int id) => Id = id;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}