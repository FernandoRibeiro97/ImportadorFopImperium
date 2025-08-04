using System;
using System.Collections.Generic;

namespace ImportadorFopImperium.Model
{
    public class NotaEntrada
    {
        public string Numero { get; set; }
        public decimal Valor_Total { get; set; }
        public decimal Valor_Base_Icms { get; set; }
        public decimal Valor_Icms { get; set; }
        public decimal Valor_Outras { get; set; }
        public decimal Valor_IPI { get; set; }
        public decimal Valor_Base_Icms_ST { get; set; }
        public decimal Valor_Icms_ST { get; set; }
        public long Id_Fornecedor { get; set; }
        public DateTime Data_Emissao { get; set; }
        public DateTime Data_Entrada { get; set; }
        public string Obs { get; set; }
        public int Serie { get; set; }
        public string Especie { get; set; }
        public string Modelo { get; set; }
        public int Loja { get; set; }
        public string Situacao { get; set; }
        public string Chave_Eletronica { get; set; }
        public string Protocolo { get; set; }
        public string Usuario { get; set; }
        public List<ItemEntrada> Itens { get; set; }
    }
}
