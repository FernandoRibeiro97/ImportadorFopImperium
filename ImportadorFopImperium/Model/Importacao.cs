using System.Collections.Generic;
using System.Data;

namespace ImportadorFopImperium.Model
{
    public class Importacao
    {
        public DataTable Dt_Tributacao { get; set; }
        public DataTable Dt_Pis { get; set; }
        public DataTable Dt_Cofins { get; set; }
        public DataTable Dt_Produto { get; set; }
        public DataTable Dt_Entidades { get; set; }
        public DataTable Dt_Fone_Entidade { get; set; }
        public DataTable Dt_Email_Entidade { get; set; }
        public DataTable Dt_Familia { get; set; }
        public DataTable Dt_Itens_Fornecedor { get; set; }
        public DataTable Dt_Grupo { get; set; }
        public DataTable Dt_SubGrupo { get; set; }
        public DataTable Dt_SubGrupo1 { get; set; }
        public DataTable Dt_Contas_Pagar { get; set; }
        public List<ClienteImperium> Lista_Clientes { get; set; }
        public List<FornecedorImperium> Lista_Fornecedores { get; set; }
        public List<Tributacao> Lista_Tributacoes { get; set; }
    }
}
