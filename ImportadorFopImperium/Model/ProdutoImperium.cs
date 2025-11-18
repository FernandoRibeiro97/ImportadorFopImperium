using System;
using System.Collections.Generic;

namespace ImportadorFopImperium.Model
{
    public class ProdutoImperium
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Descricao_Reduzida { get; set; }
        public decimal Embalagem_Entrada { get; set; }
        public decimal Embalagem_Saida { get; set; }
        public string Unidade_Entrada { get; set; }
        public string Unidade_Saida { get; set; }
        public string Obs { get; set; }
        public int Validade { get; set; }
        public int Id_Grupo { get; set; }
        public int Id_SubGrupo { get; set; }
        public int Id_SubGrupo1 { get; set; }
        public int Id_Situacao { get; set; }
        public DateTime Data_Cadastro { get; set; } = DateTime.Now;
        public int Peso_Variavel { get; set; }
        public int Etiqueta { get; set; }
        public string Ean { get; set; }
        public string Ean1 { get; set; }
        public string ClassFiscal { get; set; }
        public string Cest { get; set; }
        public int Vasilhame { get; set; }
        public string Tipo { get; set; }
        public int Id_TabelaNutricional { get; set; }
        public long Id_Familia { get; set; }
        public string Cotacao { get; set; }
        public ProdutoPreco Preco { get; set; }
        public ProdutoEstoque Estoque { get; set; }
        public ProdutoTributacao Tributacao { get; set; }
        public List<ProdutoEan> Lst_Ean { get; set; }
        public string Inf_Adicional { get; set; }
        public string Setor_Balanca { get; set; }
        public string Setor_Balanca_Nome { get; set; }
    }
}
