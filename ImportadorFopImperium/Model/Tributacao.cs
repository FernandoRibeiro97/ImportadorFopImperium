namespace ImportadorFopImperium.Model
{
    public class Tributacao
    {
        public long Id_Imperium { get; set; }
        public long Id_FOP { get; set; }
        public string Descricao { get; set; }
        public string Sit_Trib { get; set; }
        public string Valor { get; set; }
        public string CodPDV { get; set; }
        public decimal Aliquota_ICMS { get; set; }
        public decimal Reducao { get; set; }
        public string CST { get; set; }
    }
}
