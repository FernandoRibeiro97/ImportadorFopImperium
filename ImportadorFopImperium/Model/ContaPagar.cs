using System;

namespace ImportadorFopImperium.Model
{
    public class ContaPagar
    {
        public long Id_Fornecedor { get; set; }
        public string Numero_Doc { get; set; }
        public DateTime Data_Entrada { get; set; }
        public DateTime Data_Emissao { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public decimal Valor_Doc { get; set; }
        public long Id_Pc1 { get; set; }
        public string Obs { get; set; }
        public DateTime Data_Pagto { get; set; }
        public string Tipo { get; set; }
        public string Numero_Cheque { get; set; }
        public long Id_Banco { get; set; }
        public long Id_Pedido { get; set; }
        public string Situacao { get; set; }
        public long Id_Cheque { get; set; }
        public long Id_NF_Entrada { get; set; }
        public string Parcelado { get; set; }
        public int Loja { get; set; }
        public string Previsao { get; set; }
        public long Id_Pc2 { get; set; }
        public long Id_Tipo_Cobranca { get; set; }
        public decimal Valor_Desconto { get; set; }
        public string Historico { get; set; }
        public decimal Valor_Cheque { get; set; }
        public decimal Valor_Complemento { get; set; }
        public string Duplicata { get; set; }
        public decimal Valor_Acrescimo { get; set; }
        public decimal Valor_Custas_Cartorio { get; set; }
        public decimal Valor_Total_Pagar { get; set; }
        public long Id_Sub_Categoria { get; set; }
        public long Id_NF_Entrada_GNRE { get; set; }
        public string Tipo_Documento { get; set; }
        public long Id_Tranferencia { get; set; }
    }
}
