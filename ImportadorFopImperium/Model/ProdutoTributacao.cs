namespace ImportadorFopImperium.Model
{
    public class ProdutoTributacao
    {
        public int Loja { get; set; }
        public string Origem { get; set; }
        public string Tipo_Produto { get; set; }
        public long Sit_Trib_Entrada { get; set; }
        public decimal ICMS_Entrada { get; set; }
        public decimal Reducao_Base { get; set; }
        public string Tab_ICMS_Entrada { get; set; }
        public long Sit_Trib_Saida { get; set; }
        public decimal ICMS_Saida { get; set; }
        public decimal Reducao_Base_Saida { get; set; }
        public string Tab_ICMS_Saida { get; set; }
        public string Cod_Trib { get; set; }
        public decimal IPI { get; set; }
        public decimal IVA { get; set; }
        public string Tipo_PIS_Cofins { get; set; }
        public string CST_PIS { get; set; }
        public string CST_PIS_Saida { get; set; }
        public string CSS_Apurada { get; set; }
        public decimal Carga_Tributaria_Federal { get; set; }
        public decimal Carga_Tributaria { get; set; }
        public string Chave_NCM { get; set; }
        public string CST_IPI_Saida { get; set; }
        public string CST_IPI_Entrada { get; set; }
        public string Tipo_IVA { get; set; }
        public string Calcula_IVA_Ajustado { get; set; }
        public string Natureza_Receita { get; set; }
        public decimal FECOEP { get; set; }
        public decimal PIS_Saida { get; set; }
        public decimal Cofins_Saida { get; set; }
        public decimal PIS_Entrada { get; set; }
        public decimal Cofins_Entrada { get; set; }
    }
}
