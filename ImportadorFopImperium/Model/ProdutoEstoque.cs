namespace ImportadorFopImperium.Model
{
    public class ProdutoEstoque
    {
        public int Loja { get; set; }
        public decimal Estoque_Atual { get; set; }
        public decimal  Estoque_Minimo { get; set; }
        public int Cobertura_Estoque { get; set; }
    }
}
