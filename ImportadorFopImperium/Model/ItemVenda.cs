using System;

namespace ImportadorFopImperium.Model
{
    public class ItemVenda
    {
        public long Cupom { get; set; }
        public long Id_Produto { get; set; }
        public long CodigoEan { get; set; }
        public decimal Valor { get; set; }
        public decimal Qtde { get; set; }
        public int ECF { get; set; }
        public string Modelo { get; set; }
        public decimal Desconto { get; set; }
        public int Loja { get; set; }
        public string Datamov { get; set; }
        public decimal Custo_Produto { get; set; }
        public string Hora_Cupom { get; set; }
        public long Id_Vendedor { get; set; }
        public string Situacao { get; set; }
        public decimal Valor_Unitario { get; set; }
    }
}
