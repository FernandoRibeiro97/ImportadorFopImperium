using Org.BouncyCastle.Utilities.IO.Pem;
using System;

namespace ImportadorFopImperium.Model
{
    public class ClienteImperium
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Fantasia { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public long Codigo_Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Credito { get; set; }
        public decimal Limite { get; set; }
        public string Data_Nascimento { get; set; }
        public decimal Usado { get; set; }
        public string Obs { get; set; }
        public int Empresa_Convenio { get; set; }
        public int Loja { get; set; }
        public string Tipo { get; set; }
        public int Tipo_Fidelidade { get; set; }
        public int Condicao_Pagamento { get; set; }
        public string Fone { get; set; }
        public string Fone2 { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
    }
}
