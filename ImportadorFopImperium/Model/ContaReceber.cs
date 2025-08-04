using System;

namespace ImportadorFopImperium.Model
{
    public class ContaReceber
    {
        public long Id_Cliente { get; set; }
        public string Numero_Venda { get; set; }
        public decimal Valor_Vista { get; set; }
        public DateTime Data_Venda { get; set; }
        public string Situacao { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public int Loja { get; set; }
        public int ECF { get; set; }
        public string Tipo_Cobranca { get; set; }
        public string Obs { get; set; }
        public long Id_Pc1 { get; set; }
        public long Id_Pc2 { get; set; }
    }
}
