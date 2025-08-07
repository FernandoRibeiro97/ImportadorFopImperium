namespace ImportadorFopImperium.Model
{
    public class Config
    {
        #region CONFIG INI
        public string Servidor_MySQL { get; set; }
        public string Usuario_MySQL { get; set; }
        public string Senha_MySQL { get; set; }
        public string Banco_MySQL { get; set; }
        public string Tipo_Conexao { get; set; }
        public string Servidor_SQLServer { get; set; }
        public string Usuario_SQLServer { get; set; }
        public string Senha_SQLServer { get; set; }
        public string Banco_SQLServer { get; set; }
        public long Qtde_Importar { get; set; }
        #endregion

        public string Linguagem_SQL_Server { get; set; }
    }
}
