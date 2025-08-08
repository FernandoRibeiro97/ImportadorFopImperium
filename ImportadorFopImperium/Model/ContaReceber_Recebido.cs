using System;

namespace ImportadorFopImperium.Model
{
    public class ContaReceber_Recebido
    {
        public long Id_Debito { get; set; }
        public DateTime Dt_Pagto { get; set; }
        public decimal Valor_Recebido { get; set; }
        public string Baixa_Manual { get; set; }
        public int Id_Operador { get; set; }
        public int Loja_Recebimento { get; set; }
        public decimal Valor_Desconto { get; set; }
        public decimal Valor_Juros { get; set; }
        public string Usuario { get; set; }
        public long Id_Pc1 { get; set; }
        public long Id_Pc2 { get; set; }
    }
}
