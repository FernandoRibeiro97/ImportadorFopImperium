namespace ImportadorFopImperium.Model
{
    public class ItemEntrada
    {
        public long Id_NF { get; set; }
        public int Loja { get; set; }
        public long Id_Produto { get; set; }
        public string Cod_Produto { get; set; }
        public decimal Valor_Total { get; set; }
        public decimal Qtde { get; set; }
        public decimal Valor_Unitario { get; set; }
        public decimal Margem { get; set; }
        public decimal Valor_Tributacao { get; set; }
        public decimal Valor_Ipi { get; set; }
        public decimal Valor_Desconto { get; set; }
        public decimal Perc_Desconto { get; set; }
        public decimal Valor_Acrescimo { get; set; }
        public decimal Perc_Acrescimo { get; set; }
        public decimal Valor_Frete { get; set; }
        public decimal Perc_Frete { get; set; }
        public decimal Preco_Vista { get; set; }
        public decimal Preco_Sugestao { get; set; }
        public decimal Margem_Iva { get; set; }
        public decimal Reducao_Icms { get; set; }
        public decimal Reducao_Icms_ST { get; set; }
        public string Modalidade { get; set; }
        public string CFOP { get; set; }
        public decimal Perc_FCP { get; set; }
        public decimal Valor_Base_FCP { get; set; }
        public decimal Valor_FCP { get; set; }
        public decimal Perc_FCP_ST { get; set; }
        public decimal Valor_Base_FCP_ST { get; set; }
        public decimal Valor_FCP_ST { get; set; }
    }
}
