using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models
{
    public class Pessoa
    {
        [JsonProperty]
        public int Id { get; private set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }

        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var cpfNumeros = new string(cpf.Where(char.IsDigit).ToArray());
            return cpfNumeros.Length == 11;
        }

        internal void SetId(int id)
        {
            Id = id;
        }


    }

}
