namespace ImportadorFopImperium.Model
{
    public class TabelaNutricional
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public string Unidade_Porcao { get; set; }
        public int Parte_Inteira { get; set; }
        public string Parte_Decimal { get; set; }
        public string Medida_Utilizada { get; set; }
        public decimal Valor_Energetico { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Gorduras_Saturadas { get; set; }
        public decimal Fibra { get; set; }
        public decimal Ferro { get; set; }
        public decimal Gorduras_Trans { get; set; }
        public decimal Carboidratos { get; set; }
        public decimal Gorduras_Totais { get; set; }
        public decimal Colesterol { get; set; }
        public decimal Calcio { get; set; }
        public decimal Sodio { get; set; }
        public decimal Acucares_Totais { get; set; }
        public decimal Acucares_Adicionados { get; set; }
        public decimal Lactose { get; set; }
        public decimal Galactose { get; set; }
        public bool Alto_Acucar { get; set; }
        public bool Alto_Gordura { get; set; }
        public bool Alto_Sodio { get; set; }
    }
}
