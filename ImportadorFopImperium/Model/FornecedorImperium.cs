namespace ImportadorFopImperium.Model
{
    public class FornecedorImperium
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CPF_CGC { get; set; }
        public string Fantasia { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public long Codigo_Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public string Fax { get; set; }
        public string Obs { get; set; }
        public string Data_Cadastro { get; set; }
        public string Email { get; set; }
        public string Email_Pedido { get; set; }
        public string RG_IE { get; set; }
    }
}
