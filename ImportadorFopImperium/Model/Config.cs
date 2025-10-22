using ImportadorFopImperium.Enum;

namespace ImportadorFopImperium.Model
{
    public class Config
    {
        #region CONFIG INI
        public string Servidor_MySQL { get; set; }
        public string Usuario_MySQL { get; set; }
        public string Senha_MySQL { get; set; }
        public string Banco_MySQL { get; set; }
        public long Qtde_Importar { get; set; }
        public bool Remover_Digito_Verificador_Ean { get; set; }
        public TipoConexaoEnum Conexao_Origem { get; set; }
        public TipoConexaoEnum Conexao_Destino { get; set; }
        #endregion

        #region SQL Server
        public string Tipo_Conexao { get; set; }
        public string Servidor_SQLServer { get; set; }
        public string Usuario_SQLServer { get; set; }
        public string Senha_SQLServer { get; set; }
        public string Banco_SQLServer { get; set; }
        public string Linguagem_SQL_Server { get; set; }
        #endregion

        #region Postgre SQL
        public string Host_PostgreSQL { get; set; }
        public string Porta_PostgreSQL { get; set; }
        public string Usuario_PostgreSQL { get; set; }
        public string Senha_PostgreSQL { get; set; }
        public string Banco_PostgreSQL { get; set; }
        public string Schema_PostgreSQL { get; set; }
        #endregion
    }
}
