
namespace ImportadorFopImperium.Model
{
    public class ProdutoPreco
    {
        public int LOJA { get; set; }
        public decimal CUSTO { get; set; }
        public decimal CUSTO_MEDIO { get; set; }
        public decimal VENDA1 { get; set; }
        public decimal VENDA2 { get; set; }
        public decimal PRPROMOCAO { get; set; }
        public string DTINICIOPROMOCAO { get; set; } = "2021-01-01";
        public string DTFINALPROMOCAO { get; set; } = "2021-01-01";
        public decimal MARGEM { get; set; }
        public decimal VENDA1_ANTERIOR { get; set; }
        public int IDFAMILIA { get; set; }
    }
}
