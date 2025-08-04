using ImportadorFopImperium.Enum;
using ImportadorFopImperium.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImportadorFopImperium
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        Importacao ImportacaoImperium = new Importacao();
        ContadorImportacao contadorImportacao = new ContadorImportacao();

        List<ProdutoImperium> lstProdutoComErro = new List<ProdutoImperium>();

        List<string> lstIdInserido = new List<string>();
        List<ProdutoImperium> lstProdutosDuplicados = new List<ProdutoImperium>();

        BackgroundWorker mBackGroundWorker = null;

        SqlConnection connectionSqlServer = new SqlConnection();
        MySqlConnection connecctionMysql = new MySqlConnection();

        OperacaoImportador operacaoImportador = new OperacaoImportador();

        string strConexaoSqlServer = string.Empty;
        string strConexaoMySql = string.Empty;

        int qtdeImportar = 100;

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            Logar("Teste de log");
            string strServer = "localhost";
            string strDataBase = "sc2010";

            string strServerMysql = "localhost";
            string strUserMysql = "root";
            string strPasswordMysql = "root";
            string strDataBaseMysql = "db_imperium";

            strConexaoSqlServer = $"Server={strServer};DataBase={strDataBase};Trusted_Connection=True;";
            strConexaoMySql = $"server={strServerMysql};user id={strUserMysql};password={strPasswordMysql};database={strDataBaseMysql};";

            mBackGroundWorker = new BackgroundWorker();
            mBackGroundWorker.DoWork += mBackGroundWorker_DoWork;
            mBackGroundWorker.ProgressChanged += MBackGroundWorker_ProgressChanged;
            mBackGroundWorker.RunWorkerCompleted += MBackGroundWorker_RunWorkerCompleted;
            mBackGroundWorker.RunWorkerAsync();
        }
        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnCarregar_Click(object sender, EventArgs e)
        {
            if (!mBackGroundWorker.IsBusy)
            {
                operacaoImportador = OperacaoImportador.Carregar;
                mBackGroundWorker.RunWorkerAsync();
            }
        }
        private void btnImportar_Click(object sender, EventArgs e)
        {
            operacaoImportador = OperacaoImportador.Importar;
            mBackGroundWorker.RunWorkerAsync();
        }
        private void mBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (operacaoImportador == OperacaoImportador.Carregar)
                CarregarInformacoes();
            else if (operacaoImportador == OperacaoImportador.Importar)
                ImportarInformacoes();
        }
        private void MBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void MBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (operacaoImportador == OperacaoImportador.Carregar)
            {
                InformaContadorRegistrosCarregados();
                HabiltaCheckBox();

                btnImportar.Enabled = true; //TODO: FAZER UMA VERIFICACAO PARA HABILITAR O BOTAO

                MessageBox.Show("Carregamento Concluído !");
            }
            else if (operacaoImportador == OperacaoImportador.Importar)
            {
                InformaContadorRegistrosImportados();

                MessageBox.Show("Importação Concluída !");
            }
        }

        private void chkProdutos_CheckedChanged(object sender, EventArgs e)
        {
            chkFamilias.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Familia.Rows.Count > 0;
            chkItensFornecedor.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Itens_Fornecedor.Rows.Count > 0;
        }

        #region CARREGAR

        private void CarregarInformacoes()
        {
            try
            {
                ImportacaoImperium.Dt_Tributacao = CarregarTributacao();
                ImportacaoImperium.Dt_Pis = CarregarPis();
                ImportacaoImperium.Dt_Cofins = CarregarCofins();
                ImportacaoImperium.Dt_Produto = CarregarProdutos();
                ImportacaoImperium.Dt_Entidades = CarregarEntidades();
                ImportacaoImperium.Lista_Clientes = FiltraEntidadeCliente();
                ImportacaoImperium.Lista_Fornecedores = FiltraEntidadeFornecedor();
                ImportacaoImperium.Dt_Fone_Entidade = CarregarFoneEntidade();
                ImportacaoImperium.Dt_Email_Entidade = CarregarEmailEntidade();
                ImportacaoImperium.Dt_Familia = CarregarFamilia();
                ImportacaoImperium.Dt_Itens_Fornecedor = CarregarItensFornecedor();
                ImportacaoImperium.Dt_Grupo = CarregarGrupo();
                ImportacaoImperium.Dt_SubGrupo = CarregarSubGrupo();
                ImportacaoImperium.Dt_SubGrupo1 = CarregarSubGrupo1();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable CarregarTributacao()
        {
            try
            {
                string comando = @"SELECT * 
                                    FROM CadProduto.AliquotaICMS 
                                    WHERE id IN 
                                    (SELECT aliqICMS 
                                        FROM CadProduto.Produto
                                        GROUP BY aliqICMS
                                    );";

                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable CarregarPis()
        {
            try
            {
                string comando = @"SELECT * 
                                    FROM CadProduto.AliquotaPIS;";

                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable CarregarCofins()
        {
            try
            {
                string comando = @"SELECT * 
                                    FROM CadProduto.AliquotaCofins;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable CarregarProdutos() 
        {
            try
            {
                string comando = @"SELECT * FROM CadProduto.Produto;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar produtos");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarEntidades()
        {
            try
            {
                string comando = @"SELECT e.id, e.razaoSocial, e.nomeFantasia, ISNULL(ce.logradouro, 'SEM ENDERECO') AS endereco, ce.numero, ce.bairro, UPPER(mun.nomeMunicipio) AS cidade, mun.cdMunicipio, uf.siglaUF AS uf, ce.cep,
                    e.cnpj, e.inscricaoEstadual, '00' AS credito, 0 AS limite, ISNULL(e.dtNascimento, '1990-01-01') AS dt_nasc, 0 AS usado, 'IMPORTADO' AS obs, 1 AS empresa_convenio, 'TT' AS tipo, 1 AS tipofidelidade, 2 as condicaoPagamento, '' AS fone, '' AS email, e.pessoaJuridica, e.bloqueado, e.isFuncionario, e.isContador, e.isMotorista, e.isCliente, e.isFornecedor, e.isTransportadora
                    FROM Cadastro.Entidade e
                    LEFT JOIN Cadastro.Endereco ce ON e.id = ce.fkEntidade
                    LEFT JOIN Cadastro.Municipio mun ON ce.fkMunicipio = mun.cdMunicipio
                    LEFT JOIN Cadastro.UF uf ON mun.fkUF = uf.cdUF
                    ORDER BY e.id;";

                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                FecharConexaoSqlServer();
                Logar("Erro ao carregar entidades");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarFoneEntidade()
        {
            try
            {
                string comando = @"SELECT * FROM Cadastro.Fone;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar informações de telefone das entidades");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarEmailEntidade()
        {
            try
            {
                string comando = @"SELECT * FROM Cadastro.Email;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar informações de email das entidades");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarFamilia()
        {
            try
            {
                string comando = @"SELECT * FROM CadProduto.Familia;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar informações da familia dos produtos");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarItensFornecedor()
        {
            string comando = @"SELECT * FROM CadProduto.Referencia;";
            return RecuperaDataTableSQLServer(comando);
        }
        private DataTable CarregarGrupo()
        {
            string comando = @"SELECT * FROM CadProduto.SuperDepto;";
            return RecuperaDataTableSQLServer(comando);
        }
        private DataTable CarregarSubGrupo()
        {
            string comando = @"SELECT * FROM CadProduto.Categoria;";
            return RecuperaDataTableSQLServer(comando);
        }
        private DataTable CarregarSubGrupo1()
        {
            string comando = @"SELECT * FROM CadProduto.SubCategoria;";
            return RecuperaDataTableSQLServer(comando);
        }
        private List<ClienteImperium> FiltraEntidadeCliente()
        {
            var lstCliente = new List<ClienteImperium>();

            try
            {
                if (ImportacaoImperium.Dt_Entidades.Rows.Count > 0)
                {
                    foreach (DataRow r in ImportacaoImperium.Dt_Entidades.Select($"isCliente = 1"))
                        lstCliente.Add(RetornaClienteImperiumPorDataRow(r));
                }

                return lstCliente;
            }
            catch (Exception ex)
            {
                Logar("Erro ao filtrar entidade do tipo cliente");
                Logar(ex.Message);
                return null;
            }
        }
        private List<FornecedorImperium> FiltraEntidadeFornecedor()
        {
            var lstFornecedores = new List<FornecedorImperium>();

            try
            {
                if (ImportacaoImperium.Dt_Entidades.Rows.Count > 0)
                {
                    foreach (DataRow r in ImportacaoImperium.Dt_Entidades.Select("isFornecedor = 1"))
                        lstFornecedores.Add(RetornaFornecedorImperiumPorDataRow(r));
                }

                return lstFornecedores;
            }
            catch (Exception ex)
            {
                Logar("Erro ao filtrar entidade do tipo fornecedor");
                Logar(ex.Message);
                return null;
            }
        }

        #endregion

        #region IMPORTAR

        private void ImportarInformacoes()
        {
            ExecutaAumentoPacoteMySQL();

            if (chkProdutos.Checked)
            {
                ImportarTributacoes();
                ImportarProdutos();

                if (chkFamilias.Checked)
                {
                    ImportarFamilia();
                }

                if (true) //TODO: ARVORE MERCADOLOGICO
                {
                    ImportarGrupo();
                    ImportarSubGrupo();
                    ImportarSubGrupo1();
                }
            }

            if (chkClientes.Checked)
            {
                ImportarClientes();
            }

            if (chkFornecedores.Checked)
            {
                ImportarFornecedores();
            }
        }
        private void ImportarTributacoes()
        {
            ImportacaoImperium.Lista_Tributacoes = new List<Tributacao>();

            foreach (DataRow t in ImportacaoImperium.Dt_Tributacao.Rows)
            {
                Tributacao tributacao = new Tributacao();
                tributacao.Aliquota_ICMS = ConverterDecimal(t["taxa"].ToString());
                tributacao.Descricao = t["descricaoAliquota"].ToString();
                tributacao.Id_FOP = ConverterInt32(t["id"].ToString());

                if (tributacao.Aliquota_ICMS == 0M)
                {
                    tributacao.Sit_Trib = RetornaSitTribPorDescritivo(tributacao.Descricao);
                    tributacao.Valor = RetornaCampoValorTabelaTributacao(tributacao.Descricao);
                    tributacao.CodPDV = RetornaCodPDVPorDescritivo(tributacao.Descricao);
                }
                else
                {
                    tributacao.Sit_Trib = RetornaSitTribPorAliquota(tributacao.Aliquota_ICMS);
                    tributacao.Valor = RetornaCampoValorTabelaTributacao(tributacao.Aliquota_ICMS);
                    tributacao.CodPDV = "99"; //RetornaCodPDVPorAliquota(tributacao.Aliquota_ICMS); //TODO: FAZER PARÂMETRO
                }

                ImportacaoImperium.Lista_Tributacoes.Add(tributacao);
            }

            if (ImportacaoImperium.Lista_Tributacoes.Count > 0)
                ExecutaComandoTributacao();
        }
        private void ImportarProdutos()
        {
            var lstProduto = new List<ProdutoImperium>();

            foreach (DataRow r in ImportacaoImperium.Dt_Produto.Rows)
            {
                ProdutoImperium produto = RetornaProdutoImperiumPorDataRow(r);

                if (produto.Id > 0)
                {
                    if (lstIdInserido.Contains(produto.Ean1))
                        lstProdutosDuplicados.Add(produto);
                    else
                    {
                        lstProduto.Add(produto);
                        lstIdInserido.Add(produto.Ean1);
                    }
                }
                else
                    lstProdutoComErro.Add(produto);
            }

            ExecutaComandoProduto(lstProduto);

            if (chkItensFornecedor.Checked)
            {
                ImportarItensFornecedor(lstProduto);
            }

            LimpaEanProdutoNaTabelaProdutoEan();
        }
        private void ImportarClientes()
        {
            ExecutaComandoCliente();
        }
        private void ImportarFornecedores()
        {
            ExecutaComandoForncedor();
        }
        private void ImportarFamilia()
        {
            Dictionary<long, string> familias = new Dictionary<long, string>();

            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Familia.Rows)
                    familias.Add(ConverterInt64(r["id"].ToString()), r["nomeFamilia"].ToString());

                if (familias.Count > 0)
                {
                    ExecutaComandoFamilia(familias);
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao importar familias dos produtos");
                Logar(ex.Message);
            }
        }
        private void ImportarItensFornecedor(List<ProdutoImperium> lstProduto)
        {
            List<ProdutoFornecedor> produtosFornecedor = new List<ProdutoFornecedor>();

            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Itens_Fornecedor.Rows)
                {
                    var produto = lstProduto.FirstOrDefault(p => p.Ean1 == r["fkProduto"].ToString());

                    if (produto != null)
                    {
                        produtosFornecedor.Add(new ProdutoFornecedor()
                        {
                            Id = produto.Id,
                            Id_Fornecedor = ConverterInt64(r["fkFornecedor"].ToString()),
                            Cod_Referencia = ConverterInt64(r["sref"].ToString()).ToString(),
                            Embalagem = ConverterDecimal(r["tamEmb"].ToString())
                        });
                    }
                }

                ExecutaComandoItensFornecedor(produtosFornecedor);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ImportarGrupo()
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Grupo.Rows)
                    grupos.Add(new Grupo()
                    {
                        Id = ConverterInt64(r["id"].ToString()),
                        Descricao = r["nomeDepto"].ToString()
                    });

                if (grupos.Count > 0)
                    ExecutaComandoGrupo(grupos);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ImportarSubGrupo()
        {
            List<SubGrupo> subgrupos = new List<SubGrupo>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_SubGrupo.Rows)
                {
                    subgrupos.Add(new SubGrupo()
                    {
                        Id = ConverterInt64(r["id"].ToString()),
                        Id_Grupo = ConverterInt64(r["fkDepto"].ToString()),
                        Descricao = r["nomeCategoria"].ToString()
                    });
                }

                if (subgrupos.Count > 0)
                    ExecutaComandoSubGrupo(subgrupos);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ImportarSubGrupo1()
        {
            List<SubGrupo1> subgrupos1 = new List<SubGrupo1>();

            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_SubGrupo1.Rows)
                    subgrupos1.Add(new SubGrupo1()
                    {
                        Id = ConverterInt64(r["id"].ToString()),
                        Id_Grupo = RetornaFkDeptoCategoria(ConverterInt64(r["fkCategoria"].ToString())),
                        Id_SubGrupo = ConverterInt64(r["fkCategoria"].ToString()),
                        Descricao = r["nomeCategoria"].ToString(),
                        
                    });

                if (subgrupos1.Count > 0)
                    ExecutaComandoSubGrupo1(subgrupos1);
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region SQL SERVER

        private void AbrirConexaoSqlServer()
        {
            try
            {
                if (connectionSqlServer.State != ConnectionState.Open)
                {
                    connectionSqlServer = new SqlConnection(strConexaoSqlServer);
                    connectionSqlServer.Open();
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao abrir conexão SQL Server");
                Logar(ex.Message);
            }
        }
        private void FecharConexaoSqlServer()
        {
            try
            {
                if (connectionSqlServer.State != ConnectionState.Closed)
                    connectionSqlServer.Close();
            }
            catch (Exception ex)
            {
                Logar("Erro ao fechar conexão SQL Server");
                Logar(ex.Message);
            }
        }

        #endregion

        #region MYSQL

        private void AbrirConexaoMysql()
        {
            try
            {
                if (connecctionMysql.State != ConnectionState.Open)
                {
                    connecctionMysql = new MySqlConnection(strConexaoMySql);
                    connecctionMysql.Open();
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao abrir conexão MySQL");
                Logar(ex.Message);
            }
        }
        private void FecharConexaoMysql()
        {
            try
            {
                if (connecctionMysql.State != ConnectionState.Closed)
                    connecctionMysql.Close();
            }
            catch (Exception ex)
            {
                Logar("Erro ao fechar conexão MySQL");
                Logar(ex.Message);
            }
        }
        private MySqlParameter CriaParametroMySQL(string parametro, MySqlDbType tipo, object valor)
        {
            MySqlParameter mySqlParameter = new MySqlParameter();
            mySqlParameter.ParameterName = parametro;
            mySqlParameter.MySqlDbType = tipo;
            mySqlParameter.Value = valor;

            return mySqlParameter;
        }

        #endregion

        #region TRIBUTAÇÃO

        private void ExecutaComandoTributacao()
        {
            try
            {
                AdicionaColunaAuxiliarTributacao();

                string comandoTruncar = @"TRUNCATE cadtributacao;";

                AbrirConexaoMysql();
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO cadtributacao (descricao, sittrib, valor, codPDV, aliquotaICMS, reducao, idfop) VALUES ";
                StringBuilder stringBuilder = new StringBuilder(comando);

                foreach (Tributacao t in ImportacaoImperium.Lista_Tributacoes)
                {
                    stringBuilder.Append($"('{t.Descricao}', '{t.Sit_Trib}', '{t.Valor}', '{t.CodPDV}', {AjustaStringDecimal(t.Aliquota_ICMS.ToString())}, {AjustaStringDecimal(t.Reducao.ToString())}, {t.Id_FOP});");

                    command = new MySqlCommand(stringBuilder.ToString(), connecctionMysql);
                    command.ExecuteNonQuery();
                    t.Id_Imperium = command.LastInsertedId;

                    stringBuilder.Clear();
                    stringBuilder.Append(comando);
                }
            }
            catch (Exception ex)
            {
                var xx = ex.Message;
            }

        }
        private void AdicionaColunaAuxiliarTributacao()
        {
            try
            {
                string comando = @"ALTER TABLE cadtributacao ADD COLUMN `idfop` INTEGER(10) AFTER `reducao`;";
                AbrirConexaoMysql();
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            finally
            {
                FecharConexaoMysql();
            }
        }
        private void RemoveColunaAuxiliarTributacao()
        {
            try
            {
                string comando = @"ALTER TABLE cadtributacao DROP COLUMN `idfop`;";
                AbrirConexaoMysql();
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            finally
            {
                FecharConexaoMysql();
            }
        }
        private string RetornaCodPDVPorAliquota(decimal aliquota)
        {
            switch (aliquota)
            {
                case 18.00M:
                    return "00";
                case 25.00M:
                    return "07";
                case 12.00M:
                    return "08";
                case 7.00M:
                    return "09";
                default:
                    return "";
            }
        }
        private string RetornaCodPDVPorDescritivo(string descritivo)
        {
            if (descritivo.Contains("ISENTO"))
                return "05";
            else if (descritivo.Contains("SUBSTITUICAO"))
                return "04";
            else if (descritivo.Contains("NAO TRIBUTADO"))
                return "05";
            else if (descritivo.Contains("SERVICO"))
                return "20";
            else
                return "";
        }
        private string RetornaCampoValorTabelaTributacao(decimal aliquota)
        {
            string strAliquota = aliquota.ToString().Replace(",", "");

            if (aliquota < 10)
                return strAliquota.PadLeft(4, '0');
            else
                return strAliquota;
        }
        private string RetornaCampoValorTabelaTributacao(string descritivo)
        {
            if (descritivo.Contains("ISENTO"))
                return "II";
            else if (descritivo.Contains("SUBSTITUICAO"))
                return "FF";
            else if (descritivo.Contains("NAO TRIBUTADO"))
                return "N1";
            else if (descritivo.Contains("SERVICO"))
                return "IS";
            else
                return "";
        }
        private string RetornaSitTribPorAliquota(decimal aliquota)
        {
            switch (aliquota)
            {
                case 18.00M:
                    return "01";
                case 25.00M:
                    return "02";
                case 12.00M:
                    return "03";
                case 7.00M:
                    return "04";
                default:
                    return "";
            }
        }
        private string RetornaSitTribPorDescritivo(string descritivo)
        {
            if (descritivo.Contains("ISENTO"))
                return "II";
            else if (descritivo.Contains("SUBSTITUICAO"))
                return "FF";
            else if (descritivo.Contains("NAO TRIBUTADO"))
                return "N1";
            else if (descritivo.Contains("SERVICO"))
                return "IS";
            else
                return "";
        }

        #endregion

        #region PRODUTO
        private void ExecutaComandoProduto(List<ProdutoImperium> lstProduto)
        {
            try
            {
                ExecutaAumentoPacoteMySQL();
                AbrirConexaoMysql();

                #region TRUNCATE TABELAS PRODUTO

                string comandoTruncar = "TRUNCATE produto; TRUNCATE produto_preco; TRUNCATE produto_estoque; TRUNCATE produto_tributacao; TRUNCATE produto_ean;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                #endregion

                #region COMANDOS

                string comandoProduto = @"INSERT INTO produto(Descricao, DescrRed, EmbEntra, EmbSaida, UnidEntra, UnidSaida, Obs, Validade, idGrupo, idSubGrupo, idSubGrupo1,idSituacao, DtCadastro, PesoVariavel, Etiqueta, Ean, Ean1, ClassFiscal, cest, Vasilhame, Tipo, idTabelaNutricao, idFamilia) 
                  VALUES ";

                string comandoPreco = @"INSERT INTO produto_preco(
                                        IDPRODUTO, ID_LOJA, CUSTO, CUSTO_MEDIO, VENDA1, VENDA2, DTINICIOPROMO, DTFINALPROMO, MARGEM, IDFAMILIA, VENDA1_ANTERIOR
                                        ) 
                                        VALUES ";

                string comandoEstoque = @"INSERT INTO produto_estoque(
                                            idProduto, ID_LOJA, estoque_atual, estoque_minimo, coberturaEstoque
                                            ) 
                                            VALUES ";

                string comandoTributacao = @"INSERT INTO produto_tributacao (idproduto, id_loja, origemprod, tipoprod, sittribcompra, icmscompra, redbase, tabicmsprodentrada, sittrib, icms, redbasevenda, tabicmsprod, codtrib, ipi, iva, tipopiscofins, cst_pis, cst_pis_saida, ccs_apurada, cargaTributariaFederal, cargaTributaria, chaveNCM, cst_ipi_saida, cst_ipi_entrada, tipoiva, calculaIvaAjustado, nat_receita, fecoep, pis, cofins, pisentrada, cofinsentrada) VALUES ";

                string comandoProdutoEan = @"INSERT INTO produto_ean (idProduto, CodigoEan, qtde_emb, valor_venda) VALUES ";

                #endregion

                StringBuilder strBuilderProduto = new StringBuilder(comandoProduto);
                StringBuilder strBuilderPreco = new StringBuilder(comandoPreco);
                StringBuilder strBuilderEstoque = new StringBuilder(comandoEstoque);
                StringBuilder strBuilderTributacao = new StringBuilder(comandoTributacao);
                StringBuilder strBuilderProdutoEan = new StringBuilder(comandoProdutoEan);

                int cont = 0;

                foreach (ProdutoImperium p in lstProduto)
                {
                    cont++;
                    contadorImportacao.Cont_Produtos++;
                    p.Id = contadorImportacao.Cont_Produtos;

                    if (CarregaProdutoEan(p) > 0)
                    {
                        foreach (ProdutoEan pEan in p.Lst_Ean)
                            strBuilderProdutoEan.Append(RetornaLinhaInserirProdutoEan(pEan));

                        InsertBanco(strBuilderProdutoEan, command, 1, 1);

                        strBuilderProdutoEan.Clear();
                        strBuilderProdutoEan.Append(comandoProdutoEan);
                    }

                    strBuilderProduto.AppendLine(RetornaLinhaInserirProduto(p));
                    strBuilderPreco.AppendLine(RetornaLinhaInserirPreco(p));
                    strBuilderEstoque.AppendLine(RetornaLinhaInserirEstoque(p));
                    strBuilderTributacao.AppendLine(RetornaLinhaInserirTributacao(p));

                    if (cont == qtdeImportar)
                    {
                        try
                        {
                            InsertBanco(strBuilderProduto, command);
                            InsertBanco(strBuilderPreco, command);
                            InsertBanco(strBuilderEstoque, command);
                            InsertBanco(strBuilderTributacao, command);

                            strBuilderProduto.Clear();
                            strBuilderPreco.Clear();
                            strBuilderEstoque.Clear();
                            strBuilderTributacao.Clear();

                            strBuilderProduto.Append(comandoProduto);
                            strBuilderPreco.Append(comandoPreco);
                            strBuilderEstoque.Append(comandoEstoque);
                            strBuilderTributacao.Append(comandoTributacao);

                            cont = 0;
                        }
                        catch (Exception ex)
                        {
                            var _ = ex.Message;
                            var __ = strBuilderProduto.ToString();
                        }
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderProduto, command);
                    InsertBanco(strBuilderPreco, command);
                    InsertBanco(strBuilderEstoque, command);
                    InsertBanco(strBuilderTributacao, command);

                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao montar comando de inserção produto");
                Logar(ex.Message);
            }
            finally
            {
                try
                {
                    RemoveColunaAuxiliarTributacao();
                }
                catch { }

                FecharConexaoMysql();
            }
        }
        private string RetornaLinhaInserirProduto(ProdutoImperium produto)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"(");

            //stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"'{produto.Descricao.Replace("'", " ")}',");
            stringBuilder.Append($"'{produto.Descricao_Reduzida.Replace("'", " ")}',");
            stringBuilder.Append($"{produto.Embalagem_Entrada.ToString().Replace(",", ".")},");
            stringBuilder.Append($"{produto.Embalagem_Saida.ToString().Replace(",", ".")},");
            stringBuilder.Append($"'{produto.Unidade_Entrada}',");
            stringBuilder.Append($"'{produto.Unidade_Saida}',");
            stringBuilder.Append($"'{produto.Obs}',");
            stringBuilder.Append($"{produto.Validade},");
            stringBuilder.Append($"{produto.Id_Grupo},");
            stringBuilder.Append($"{produto.Id_SubGrupo},");
            stringBuilder.Append($"{produto.Id_SubGrupo1},");
            stringBuilder.Append($"{produto.Id_Situacao},");
            stringBuilder.Append($"'{produto.Data_Cadastro:yyyy-MM-dd}',");
            stringBuilder.Append($"{produto.Peso_Variavel},");
            stringBuilder.Append($"{produto.Etiqueta},");
            stringBuilder.Append($"'{produto.Ean}',");
            stringBuilder.Append($"'{produto.Ean1}',");
            stringBuilder.Append($"'{produto.ClassFiscal}',");
            stringBuilder.Append($"'{produto.Cest}',");
            stringBuilder.Append($"{produto.Vasilhame},");
            stringBuilder.Append($"'{produto.Tipo}',");
            stringBuilder.Append($"{produto.Id_TabelaNutricional},");
            stringBuilder.Append($"{produto.Id_Familia}");

            stringBuilder.Append($"),");

            return stringBuilder.ToString();
        }
        private string RetornaLinhaInserirPreco(ProdutoImperium produto)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"{produto.Preco.LOJA},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.CUSTO.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.CUSTO_MEDIO.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA1.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA2.ToString())},");
            stringBuilder.Append($"{produto.Preco.DTINICIOPROMOCAO},");
            stringBuilder.Append($"{produto.Preco.DTFINALPROMOCAO},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.MARGEM.ToString())},");
            stringBuilder.Append($"{produto.Preco.IDFAMILIA},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA1_ANTERIOR.ToString())}");
            stringBuilder.Append($"),");

            return stringBuilder.ToString();
        }
        private string RetornaLinhaInserirEstoque(ProdutoImperium produto)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"{produto.Estoque.Loja},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Estoque.Estoque_Atual.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Estoque.Estoque_Minimo.ToString())},");
            stringBuilder.Append($"{produto.Estoque.Cobertura_Estoque}");
            stringBuilder.Append($"),");
            
            return stringBuilder.ToString();
        }
        private string RetornaLinhaInserirTributacao(ProdutoImperium produto)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"{produto.Tributacao.Loja},");
            stringBuilder.Append($"'{produto.Tributacao.Origem}',");
            stringBuilder.Append($"'{produto.Tributacao.Tipo_Produto}',");
            stringBuilder.Append($"{produto.Tributacao.Sit_Trib_Entrada},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.ICMS_Entrada.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Reducao_Base.ToString())},");
            stringBuilder.Append($"'{produto.Tributacao.Tab_ICMS_Entrada}',");
            stringBuilder.Append($"{produto.Tributacao.Sit_Trib_Saida},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.ICMS_Saida.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Reducao_Base_Saida.ToString())},");
            stringBuilder.Append($"'{produto.Tributacao.Tab_ICMS_Saida}',");
            stringBuilder.Append($"'{produto.Tributacao.Cod_Trib}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.IPI.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.IVA.ToString())},");
            stringBuilder.Append($"'{produto.Tributacao.Tipo_PIS_Cofins}',");
            stringBuilder.Append($"'{produto.Tributacao.CST_PIS}',");
            stringBuilder.Append($"'{produto.Tributacao.CST_PIS_Saida}',");
            stringBuilder.Append($"'{produto.Tributacao.CSS_Apurada}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Carga_Tributaria_Federal.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Carga_Tributaria.ToString())},");
            stringBuilder.Append($"'{produto.Tributacao.Chave_NCM}',");
            stringBuilder.Append($"'{produto.Tributacao.CST_IPI_Saida}',");
            stringBuilder.Append($"'{produto.Tributacao.CST_IPI_Entrada}',");
            stringBuilder.Append($"'{produto.Tributacao.Tipo_IVA}',");
            stringBuilder.Append($"'{produto.Tributacao.Calcula_IVA_Ajustado}',");
            stringBuilder.Append($"'{produto.Tributacao.Natureza_Receita}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.FECOEP.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.PIS_Saida.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Cofins_Saida.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.PIS_Entrada.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Tributacao.Cofins_Entrada.ToString())}");

            stringBuilder.Append("),");
            return stringBuilder.ToString();
        }
        private string RetornaLinhaInserirProdutoEan(ProdutoEan produto)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id_Produto},");
            stringBuilder.Append($"'{produto.Codigo_Ean}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Qtde_Emb.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Valor_Venda.ToString())}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        private ProdutoImperium RetornaProdutoImperiumPorDataRow(DataRow r)
        {
            int loja = ConverterInt32(r["fkCliente"].ToString());
            string descricaoReduzida = r["nomeImpressao"].ToString().Length > 24 ? r["nomeImpressao"].ToString().Substring(0, 24) : r["nomeImpressao"].ToString();
            string cest = r["cest"].ToString().Length > 7 ? r["cest"].ToString().Substring(0, 7) : r["cest"].ToString();

            ProdutoImperium produto = new ProdutoImperium();
            produto.Id = ConverterInt64(r["id"].ToString());
            produto.Descricao = r["nomeProduto"].ToString();
            produto.Descricao_Reduzida = string.IsNullOrEmpty(descricaoReduzida) ? produto.Descricao.Length > 24 ? produto.Descricao.Substring(0, 24) :  produto.Descricao : descricaoReduzida;
            produto.Unidade_Entrada = r["unidade"].ToString();
            produto.Unidade_Saida = r["unidade"].ToString();
            produto.Embalagem_Entrada = ConverterDecimal(r["tamCaixa"].ToString());
            produto.Embalagem_Saida = ConverterDecimal(r["tamEmbVenda"].ToString());
            produto.Obs = "IMPORTADO";
            produto.Validade = 0; //TODO: VERIFICAR CAMPO
            produto.Id_Grupo = ConverterInt32(r["fkCategoria"].ToString());
            produto.Id_SubGrupo = ConverterInt32(r["fkSubCategoria"].ToString());
            produto.Id_SubGrupo1 = 0;
            produto.Id_Situacao = r["ativo"].ToString() == "0" ? 2 : 1;
            produto.Peso_Variavel = r["balanca"].ToString() == "1" ? 1 : 0;
            produto.Etiqueta = 1;
            produto.Ean = ConverterInt64(r["id"].ToString()).ToString();
            produto.Ean1 = r["id"].ToString();
            produto.ClassFiscal = r["classFiscal"].ToString();
            produto.Cest = cest;
            produto.Vasilhame = ConverterInt32(r["isvasilhame"].ToString());
            produto.Tipo = r["balanca"].ToString() == "1" ? "P" : "U";
            produto.Id_TabelaNutricional = 0; //TODO: VERIFICAR CAMPO
            produto.Id_Familia = ConverterInt64(r["fkFamilia"].ToString());

            produto.Preco = new ProdutoPreco();
            int tamanhoCaixa = ConverterInt32(r["tamCaixa"].ToString()) == 0 ? 1 : ConverterInt32(r["tamCaixa"].ToString());
            DateTime inicioPromo = string.IsNullOrEmpty(r["dtInicioPromo"].ToString()) ? ConverterDateTime("2021-01-01") : ConverterDateTime(r["dtInicioPromo"].ToString());
            DateTime finalPromo = string.IsNullOrEmpty(r["dtFimPromo"].ToString()) ? ConverterDateTime("2021-01-01") : ConverterDateTime(r["dtFimPromo"].ToString());
            produto.Preco.LOJA = loja;
            produto.Preco.CUSTO = Math.Round(ConverterDecimal(r["custoCaixa"].ToString()) / tamanhoCaixa, 4);
            produto.Preco.CUSTO_MEDIO = ConverterDecimal(r["custoMedio"].ToString());
            produto.Preco.VENDA1 = ConverterDecimal(r["precoVenda"].ToString());
            produto.Preco.VENDA2 = 0;
            produto.Preco.DTINICIOPROMOCAO = inicioPromo.ToString("yyyy-MM-dd");
            produto.Preco.DTINICIOPROMOCAO = finalPromo.ToString("yyyy-MM-dd");
            produto.Preco.MARGEM = ConverterDecimal(r["margemDesejada"].ToString());
            produto.Preco.VENDA1_ANTERIOR = ConverterDecimal(r["precoAnterior"].ToString());
            produto.Preco.IDFAMILIA = ConverterInt32(r["fkFamilia"].ToString());

            produto.Estoque = new ProdutoEstoque();
            produto.Estoque.Loja = loja;
            produto.Estoque.Estoque_Atual = ConverterDecimal(r["estoqueAtual"].ToString());
            produto.Estoque.Estoque_Minimo = ConverterDecimal(r["estoqueMin"].ToString());
            produto.Estoque.Cobertura_Estoque = 0;

            produto.Tributacao = new ProdutoTributacao();
            produto.Tributacao.Loja = loja;
            produto.Tributacao.Origem = r["origem"].ToString() != "0" ? r["origem"].ToString() : "0 - NACIONAL";
            produto.Tributacao.Tipo_Produto = "00 - MERCADORIA PARA REVENDA";
            AjustaTributacaoProduto(produto);

            return produto;
        }
        private void AjustaTributacaoProduto(ProdutoImperium produto)
        {
            produto.Tributacao.Sit_Trib_Entrada = produto.Tributacao.Sit_Trib_Saida = RetornaSitTrib(produto.Ean1);
            produto.Tributacao.ICMS_Entrada = produto.Tributacao.ICMS_Saida = RetornaAliquotaICMS(produto.Ean1);
            produto.Tributacao.Reducao_Base = produto.Tributacao.Reducao_Base_Saida = RetornaAliquotaReducaoICMS(produto.Ean1);
            produto.Tributacao.Tab_ICMS_Entrada = produto.Tributacao.Tab_ICMS_Saida = RetornaTabICMS(produto.Ean1);
            produto.Tributacao.Cod_Trib = RetornaCodTrib(produto.Ean1);
            produto.Tributacao.IPI = 0;
            produto.Tributacao.IVA = 0;
            produto.Tributacao.Tipo_PIS_Cofins = RetornaTipoPisCofins(produto.Ean1);
            produto.Tributacao.CST_PIS = RetornaCstPisEntrada(produto.Ean1);
            produto.Tributacao.CST_PIS_Saida = RetornaCstPisSaida(produto.Ean1);
            produto.Tributacao.CSS_Apurada = "02 - Contribuição não-cumulativa apurada a alíquotas diferenciadas";
            produto.Tributacao.Carga_Tributaria_Federal = 0;
            produto.Tributacao.Carga_Tributaria = 0;
            produto.Tributacao.Chave_NCM = "M2L5P8";
            produto.Tributacao.CST_IPI_Saida = produto.Tributacao.CST_IPI_Entrada = "";
            produto.Tributacao.Tipo_IVA = "P";
            produto.Tributacao.Calcula_IVA_Ajustado = "N";
            produto.Tributacao.Natureza_Receita = RetornaValorCampoDtProduto(produto.Ean1, "naturezaReceitaPisCofins");
            produto.Tributacao.FECOEP = 0;
            produto.Tributacao.PIS_Entrada = produto.Tributacao.PIS_Saida = RetornaAliquotaPis(produto.Ean1);
            produto.Tributacao.Cofins_Entrada = produto.Tributacao.Cofins_Saida = RetornaAliquotaCofins(produto.Ean1);
        }
        private long RetornaSitTrib(string idFOP)
        {
            try
            {
                long idAliqICMS_FOP = ConverterInt64(RetornaValorCampoDtProduto(idFOP, "aliqICMS"));

                if (ImportacaoImperium.Lista_Tributacoes.Count > 0 && idAliqICMS_FOP > 0)
                    return ImportacaoImperium.Lista_Tributacoes.FirstOrDefault(t => t.Id_FOP == idAliqICMS_FOP).Id_Imperium;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return 0;
            }
        }
        private decimal RetornaAliquotaICMS(string idFOP)
        {
            try
            {
                long idAliqICMS_FOP = ConverterInt64(RetornaValorCampoDtProduto(idFOP, "aliqICMS"));

                if (ImportacaoImperium.Lista_Tributacoes.Count > 0 && idAliqICMS_FOP > 0)
                    return ImportacaoImperium.Lista_Tributacoes.FirstOrDefault(t => t.Id_FOP == idAliqICMS_FOP).Aliquota_ICMS;
                else
                    return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private decimal RetornaAliquotaReducaoICMS(string idFOP)
        {
            try
            {
                long idAliqICMS_FOP = ConverterInt64(RetornaValorCampoDtProduto(idFOP, "aliqICMS"));

                if (ImportacaoImperium.Lista_Tributacoes.Count > 0 && idAliqICMS_FOP > 0)
                    return ImportacaoImperium.Lista_Tributacoes.FirstOrDefault(t => t.Id_FOP == idAliqICMS_FOP).Reducao;
                else
                    return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string RetornaTabICMS(string idFOP)
        {
            try
            {
                string cstFOP = RetornaValorCampoDtProduto(idFOP, "tribICMS");

                switch (cstFOP)
                {
                    case "00":
                        return "00 - TRIBUTADA INTEGRALMENTE";
                    case "20":
                        return "20 - COM REDUÇÃO DE BASE DE CALCULO";
                    case "60":
                        return "60 - ICMS COBRADO ANT. POR SUBST. TRIB.";
                    case "90":
                        return "90 - OUTRAS";
                    case "40":
                    default:
                        return "40 - ISENTA";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string RetornaCodTrib(string idFOP)
        {
            try
            {
                long idAliqICMS_FOP = ConverterInt64(RetornaValorCampoDtProduto(idFOP, "aliqICMS"));

                if (ImportacaoImperium.Lista_Tributacoes.Count > 0 && idAliqICMS_FOP > 0)
                    return ImportacaoImperium.Lista_Tributacoes.FirstOrDefault(t => t.Id_FOP == idAliqICMS_FOP).CodPDV;
                else
                    return "99";
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string RetornaTipoPisCofins(string idFOP)
        {
            try
            {
                string valor = RetornaValorCampoDtProduto(idFOP, "tribPIS");

                switch (valor)
                {
                    case "01":
                    case "02":
                        return "T";
                    case "03":
                    case "04":
                        return "S";
                    case "07":
                    case "08":
                        return "I";
                    case "09":
                    case "10":
                        return "M";
                    case "05":
                    case "06":
                    default:
                        return "I";
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return "I";
            }
        }
        private string RetornaCstPisEntrada(string idFOP)
        {
            try
            {
                string valor = RetornaValorCampoDtProduto(idFOP, "tribPIS");

                switch (valor)
                {
                    case "01":
                    case "02":
                        return "50 - Operação com Direito a Crédito - Vinculada Exclusivamente a Receita Tributada no Mercado Interno ";
                    case "03":
                    case "04":
                        return "75 - Operação de Aquisição por Substituição Tributária";
                    case "07":
                    case "08":
                        return "74 - Operação de Aquisição sem Incidência da Contribuição";
                    case "09":
                    case "10":
                        return "70 - Operação de Aquisição sem Direito a Crédito";
                    case "05":
                    case "06":
                    default:
                        return "73 - Operação de Aquisição a Alíquota Zero";
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return "I";
            }
        }
        private string RetornaCstPisSaida(string idFOP)
        {
            try
            {
                string valor = RetornaValorCampoDtProduto(idFOP, "tribPIS");

                switch (valor)
                {
                    case "01":
                    case "02":
                        return "01 - Operação Tributável com Alíquota Básica";
                    case "03":
                    case "04":
                        return "05 - Operação Tributável por Substituição Tributária";
                    case "07":
                    case "08":
                        return "08 - Operação sem Incidência da Contribuição";
                    case "09":
                    case "10":
                        return "04 - Operação Tributável Monofásica - Revenda a Alíquota Zero";
                    case "05":
                    case "06":
                    default:
                        return "06 - Operação Tributável a Alíquota Zero";
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return "06 - Operação Tributável a Alíquota Zero";
            }
        }
        private decimal RetornaAliquotaPis(string idFOP)
        {
            try
            {
                string valor = RetornaValorCampoDtProduto(idFOP, "tribPIS");

                switch (valor)
                {
                    case "01":
                    case "02":
                        return 1.65M;
                    default:
                        return 0M;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return 0M;
            }
        }
        private decimal RetornaAliquotaCofins(string idFOP)
        {
            try
            {
                string valor = RetornaValorCampoDtProduto(idFOP, "tribPIS");

                switch (valor)
                {
                    case "01":
                    case "02":
                        return 7.60M;
                    default:
                        return 0M;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return 0M;
            }
        }
        #endregion

        #region FAMILIA

        private void ExecutaComandoFamilia(Dictionary<long, string> dicionarioFamilia)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE familia;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = "INSERT INTO familia (idFamilia, nome) VALUES ";
                StringBuilder stringBuilder = new StringBuilder(comando);

                int cont = 0;

                foreach (var id in dicionarioFamilia.Keys)
                {
                    contadorImportacao.Cont_Familias++;
                    cont++;
                    dicionarioFamilia.TryGetValue(id, out string nomeFamilia);
                    stringBuilder.Append($"({id}, '{nomeFamilia}'),");

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(stringBuilder, command, 1, 1);
                        stringBuilder.Clear();
                        stringBuilder.Append(comando);
                        cont = 0; 
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(stringBuilder, command, 1, 1);
                    stringBuilder.Clear();
                    stringBuilder.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            finally
            {
                FecharConexaoMysql();
            }
        }

        #endregion

        #region ÁRVORE MERCADOLÓGICA
        private void ExecutaComandoGrupo(List<Grupo> grupos)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE grupo;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO grupo (IDGRUPO, NOME) VALUES ";
                StringBuilder strGrupo = new StringBuilder(comando);
                int cont = 0;

                foreach (Grupo g in grupos)
                {
                    strGrupo.AppendLine(RetornaLinhaInserirGrupo(g));
                    cont++;

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(strGrupo, command);
                        strGrupo.Clear();
                        strGrupo.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strGrupo, command);
                    strGrupo.Clear();
                    strGrupo.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao executar comando de inserção de grupos");
                Logar(ex.Message);
            }
        }
        private string RetornaLinhaInserirGrupo(Grupo grupo)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{grupo.Id},");
            stringBuilder.Append($"'{grupo.Descricao}'");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        private void ExecutaComandoSubGrupo(List<SubGrupo> subgrupos)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE subgrupo;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO subgrupo (IdSubGrupo, IdGrupo, Nome) VALUES ";
                StringBuilder strGrupo = new StringBuilder(comando);
                int cont = 0;

                foreach (SubGrupo g in subgrupos)
                {
                    cont++;
                    strGrupo.AppendLine(RetornaLinhaInserirSubGrupo(g));

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(strGrupo, command);
                        strGrupo.Clear();
                        strGrupo.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strGrupo, command);
                    strGrupo.Clear();
                    strGrupo.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao executar comando de inserção de subgrupos");
                Logar(ex.Message);
            
            }
            finally
            {
                FecharConexaoMysql();
            }
        }
        private string RetornaLinhaInserirSubGrupo(SubGrupo subgrupo)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{subgrupo.Id},");
            stringBuilder.Append($"{subgrupo.Id_Grupo},");
            stringBuilder.Append($"'{subgrupo.Descricao}'");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        private void ExecutaComandoSubGrupo1(List<SubGrupo1> subgrupos1)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE subgrupo1;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO subgrupo1 (idSubGrupo1, idGrupo, idSubGrupo, Nome) VALUES ";
                StringBuilder strSubGrupo = new StringBuilder(comando);
                int cont = 0;

                foreach (SubGrupo1 s in subgrupos1)
                {
                    strSubGrupo.AppendLine(RetornaLinhaInserirSubGrupo1(s));
                    cont++;

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(strSubGrupo, command);
                        strSubGrupo.Clear();
                        strSubGrupo.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strSubGrupo, command);
                    strSubGrupo.Clear();
                    strSubGrupo.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string RetornaLinhaInserirSubGrupo1(SubGrupo1 subgrupo1)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{subgrupo1.Id},");
            stringBuilder.Append($"{subgrupo1.Id_Grupo},");
            stringBuilder.Append($"{subgrupo1.Id_SubGrupo},");
            stringBuilder.Append($"'{subgrupo1.Descricao}'");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }

        #endregion

        #region ITENS FORNECEDOR

        private void ExecutaComandoItensFornecedor(List<ProdutoFornecedor> lstProdutoFornecedor)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE itensfornecedor;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO itensfornecedor(
                                        idFornecedor, idProduto, idProduto_grade, Referencia, Embalagem
                                        ) VALUES ";

                StringBuilder strBuilderItensFornecedor = new StringBuilder(comando);
                int cont = 0;

                foreach (ProdutoFornecedor p in lstProdutoFornecedor)
                {
                    strBuilderItensFornecedor.AppendLine(RetornaLinhaIserirProdutoFornecedor(p));
                    contadorImportacao.Cont_ItensFornecedor++;
                    cont++;

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(strBuilderItensFornecedor, command);
                        strBuilderItensFornecedor.Clear();
                        strBuilderItensFornecedor.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderItensFornecedor, command);
                    strBuilderItensFornecedor.Clear();
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao executar comando de inserção de itens fornecedor");
                Logar(ex.Message);
            }
        }
        private string RetornaLinhaIserirProdutoFornecedor(ProdutoFornecedor produto)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id_Fornecedor},");
            stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"0,");
            stringBuilder.Append($"'{produto.Cod_Referencia}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Embalagem.ToString())}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        #endregion

        #region PRODUTO EAN

        private int CarregaProdutoEan(ProdutoImperium produto)
        {
            int cont = 0;
            var eans = RetornaValorCampoDtProduto(produto.Ean1, "listaEans");

            if (!string.IsNullOrEmpty(eans))
            {
                produto.Lst_Ean = new List<ProdutoEan>();

                foreach (var item in eans.Split(';'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        produto.Lst_Ean.Add(new ProdutoEan()
                        {
                            Id_Produto = produto.Id,
                            Codigo_Ean = item,
                            Qtde_Emb = 1,
                            Valor_Venda = 0
                        });

                        cont++;
                    }
                }
            }

            return cont;
        }
        private void LimpaEanProdutoNaTabelaProdutoEan()
        {
            try
            {
                AbrirConexaoMysql();
                string comando = @"DELETE FROM produto_ean WHERE CodigoEan IN (SELECT ean FROM produto);";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            finally
            {
                FecharConexaoMysql();
            }
        }

        #endregion

        #region CLIENTE

        private string RetornaLinhaInserirCliente(ClienteImperium cliente)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");

            builder.Append($"{cliente.Id},");
            builder.Append($"'{cliente.Nome}',");
            builder.Append($"'{cliente.Fantasia}',");
            builder.Append($"'{cliente.Endereco}',");
            builder.Append($"'{cliente.Numero}',");
            builder.Append($"'{cliente.Bairro}',");
            builder.Append($"'{cliente.Cidade}',");
            builder.Append($"{cliente.Codigo_Municipio},");
            builder.Append($"'{cliente.UF}',");
            builder.Append($"'{cliente.CEP}',");
            builder.Append($"'{cliente.CPF}',");
            builder.Append($"'{cliente.RG}',");
            builder.Append($"'{cliente.Credito}',");
            builder.Append($"{cliente.Limite.ToString().Replace(",", ".")},");
            builder.Append($"'{cliente.Data_Nascimento}',");
            builder.Append($"{cliente.Usado.ToString().Replace(",", ".")},");
            builder.Append($"'{cliente.Obs}',");
            builder.Append($"{cliente.Empresa_Convenio},");
            builder.Append($"{cliente.Loja},");
            builder.Append($"'{cliente.Tipo}',");
            builder.Append($"{cliente.Tipo_Fidelidade},");
            builder.Append($"{cliente.Condicao_Pagamento},");
            builder.Append($"'{cliente.Fone}',");
            builder.Append($"'{cliente.Celular}',");
            builder.Append($"'{cliente.Email}'");

            builder.Append("),");

            return builder.ToString();
        }
        private void ExecutaComandoCliente()
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = "TRUNCATE cliente;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                StringBuilder strBuilderComando = new StringBuilder();
                string comando = @"INSERT INTO cliente(
                                    idcliente, nome, fantasia, endereco, numero, bairro, cidade, codmunicipio, uf,
                                    cep, cpf, rg, credito, limite, dt_nasc, usado, obs, empresa_convenio, loja, tipo, tipofidelidade, condicaoPagamento,
                                    fone, celular, email
                                    ) VALUES ";

                strBuilderComando.Append(comando);
                int cont = 0;

                foreach (ClienteImperium c in ImportacaoImperium.Lista_Clientes)
                {
                    strBuilderComando.AppendLine(RetornaLinhaInserirCliente(c));
                    cont++;
                    contadorImportacao.Cont_Clientes++;

                    if (cont == qtdeImportar)
                    {
                        try
                        {
                            InsertBanco(strBuilderComando, command);

                            cont = 0;
                            strBuilderComando.Clear();
                            strBuilderComando.Append(comando);
                        }
                        catch (Exception)
                        {
                            Logar("Erro ao inserir clientes");
                            throw;
                        }
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderComando, command);

                    cont = 0;
                    strBuilderComando.Clear();
                    strBuilderComando.Append(comando);
                }
            }
            catch (Exception ex)
            {
                Logar("Erro Geral ao inserir clientes");
                Logar(ex.Message);
            }
            finally
            {
                FecharConexaoMysql();
            }
        }
        private ClienteImperium RetornaClienteImperiumPorDataRow(DataRow r)
        {
            ClienteImperium cliente = new ClienteImperium();
            cliente.Id = ConverterInt32(r["id"].ToString());
            cliente.Nome = TiraAspasSimplesString(r["razaoSocial"].ToString());
            cliente.Fantasia = TiraAspasSimplesString(r["nomeFantasia"].ToString());
            cliente.Endereco = TiraAspasSimplesString(r["endereco"].ToString());
            cliente.Numero = TiraAspasSimplesString(r["numero"].ToString());
            cliente.Bairro = TiraAspasSimplesString(r["bairro"].ToString());
            cliente.Cidade = TiraAspasSimplesString(r["cidade"].ToString());
            cliente.Codigo_Municipio = ConverterInt64(r["cdMunicipio"].ToString());
            cliente.UF = r["uf"].ToString();
            cliente.CEP = r["cep"].ToString().Replace("-", "");
            cliente.CPF = r["cnpj"].ToString();
            cliente.RG = r["inscricaoEstadual"].ToString();
            cliente.Credito = r["credito"].ToString();
            cliente.Limite = ConverterDecimal(r["limite"].ToString());
            cliente.Data_Nascimento = ConverterDateTime(r["dt_nasc"].ToString()).ToString("yyyy-MM-dd");
            cliente.Usado = ConverterDecimal(r["usado"].ToString());
            cliente.Obs = r["obs"].ToString();
            cliente.Empresa_Convenio = ConverterInt32(r["empresa_convenio"].ToString());
            cliente.Tipo = r["tipo"].ToString();
            cliente.Tipo_Fidelidade = ConverterInt32(r["tipofidelidade"].ToString());
            cliente.Condicao_Pagamento = ConverterInt32(r["condicaoPagamento"].ToString());

            AdicionarFoneCliente(cliente);
            AdicionarEmailCliente(cliente);

            return cliente;
        }
        private void AdicionarFoneCliente(ClienteImperium cliente)
        {
            try
            {
                if (ImportacaoImperium.Dt_Fone_Entidade != null)
                {
                    var fones = RetornaFoneEntidade(cliente.Id);

                    if (fones.Count > 0 || fones != null)
                    {
                        foreach (string tipo in fones.Keys)
                        {
                            fones.TryGetValue(tipo, out string valor);

                            if (tipo == "Principal")
                                cliente.Fone = valor;
                            else if (tipo == "Celular")
                                cliente.Celular = valor;
                            else
                                cliente.Fone2 = string.IsNullOrEmpty(cliente.Fone2) ? valor : (cliente.Fone2 += $";{valor}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao vincular telefone ao cliente");
                Logar(ex.Message);
            }
        }
        private void AdicionarEmailCliente(ClienteImperium cliente)
        {
            try
            {
                if (ImportacaoImperium.Dt_Email_Entidade != null)
                {
                    var emails = RetornaEmailEntidade(cliente.Id);

                    if (emails.Count > 0 || emails != null)
                    {
                        foreach (string tipo in emails.Keys)
                        {
                            emails.TryGetValue(tipo, out string valor);

                            if (tipo == "Principal")
                                cliente.Email = valor;
                            else
                                cliente.Obs = string.IsNullOrEmpty(cliente.Obs) ? valor : (cliente.Obs += $";{valor}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao adicionar email ao cliente");
                Logar(ex.Message);
            }
        }


        #endregion

        #region FORNECEDOR

        private FornecedorImperium RetornaFornecedorImperiumPorDataRow(DataRow r)
        {
            FornecedorImperium fornecedor = new FornecedorImperium();
            fornecedor.Id = ConverterInt64(r["id"].ToString());
            fornecedor.Nome = TiraAspasSimplesString(r["razaoSocial"].ToString());
            fornecedor.CPF_CGC = LimpaStringDocumento(r["cnpj"].ToString());
            fornecedor.RG_IE = LimpaStringDocumento(r["inscricaoEstadual"].ToString());
            fornecedor.Fantasia = TiraAspasSimplesString(r["nomeFantasia"].ToString());
            fornecedor.Endereco = TiraAspasSimplesString(r["endereco"].ToString());
            fornecedor.Numero = r["numero"].ToString();
            fornecedor.Bairro = TiraAspasSimplesString(r["bairro"].ToString());
            fornecedor.Cidade = TiraAspasSimplesString(r["cidade"].ToString());
            fornecedor.Codigo_Municipio = ConverterInt64(r["cdMunicipio"].ToString());
            fornecedor.UF = r["uf"].ToString();
            fornecedor.CEP = r["cep"].ToString().Replace("-", "");
            AdicionarFoneFornecedor(fornecedor);
            AdicionarEmailFornecedor(fornecedor);

            return fornecedor;
        }
        private void ExecutaComandoForncedor()
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = "TRUNCATE fornecedor;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                int cont = 0;
                
                string comando = @"INSERT INTO fornecedor(
                                    idfornecedor, NOME, CPF_CGC, RG_IE, FANTASIA, ENDERECO, NUMERO, BAIRRO, CIDADE, CODMUNICIPIO ,UF, CEP, TELEFONE, FAX, OBS, DTCADASTRO, EMAIL, EMAILPEDIDO 
                                    ) VALUES ";

                StringBuilder strBuilderComando = new StringBuilder(comando);

                foreach (FornecedorImperium f in ImportacaoImperium.Lista_Fornecedores)
                {
                    strBuilderComando.AppendLine(RetornaLinhaInserirFornecedor(f));
                    cont++;
                    contadorImportacao.Cont_Fornecedores++;

                    if (cont == qtdeImportar)
                    {
                        InsertBanco(strBuilderComando, command);

                        strBuilderComando.Clear();
                        strBuilderComando.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderComando, command);

                    strBuilderComando.Clear();
                    strBuilderComando.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao executar comando de inserção de fornecedor");
                Logar(ex.Message);
            }
            finally
            {
                FecharConexaoMysql();
            }
        }
        private string RetornaLinhaInserirFornecedor(FornecedorImperium fornecedor)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("(");

            strBuilder.Append($"{fornecedor.Id},");
            strBuilder.Append($"'{fornecedor.Nome}',");
            strBuilder.Append($"'{fornecedor.CPF_CGC}',");
            strBuilder.Append($"'{fornecedor.RG_IE}',");
            strBuilder.Append($"'{fornecedor.Fantasia}',");
            strBuilder.Append($"'{fornecedor.Endereco}',");
            strBuilder.Append($"'{fornecedor.Numero}',");
            strBuilder.Append($"'{fornecedor.Bairro}',");
            strBuilder.Append($"'{fornecedor.Cidade}',");
            strBuilder.Append($"{fornecedor.Codigo_Municipio},");
            strBuilder.Append($"'{fornecedor.UF}',");
            strBuilder.Append($"'{fornecedor.CEP}',");
            strBuilder.Append($"'{fornecedor.Telefone}',");
            strBuilder.Append($"'{fornecedor.Fax}',");
            strBuilder.Append($"'{fornecedor.Obs}',");
            strBuilder.Append($"CURDATE(),");
            strBuilder.Append($"'{fornecedor.Email}',");
            strBuilder.Append($"'{fornecedor.Email_Pedido}'");

            strBuilder.Append("),");

            return strBuilder.ToString();
        }
        private void AdicionarFoneFornecedor(FornecedorImperium fornecedor)
        {
            try
            {
                if (ImportacaoImperium.Dt_Fone_Entidade != null)
                {
                    var fones = RetornaFoneEntidade(fornecedor.Id);

                    foreach (string tipo in fones.Keys)
                    {
                        fones.TryGetValue(tipo, out string valor);

                        if (tipo == "Principal")
                            fornecedor.Telefone = valor;
                        else if (tipo == "Celular")
                            fornecedor.Fax = valor;
                        else
                            fornecedor.Obs = string.IsNullOrEmpty(fornecedor.Obs) ? valor : fornecedor.Obs += $";{valor}";
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void AdicionarEmailFornecedor(FornecedorImperium fornecedor)
        {
            try
            {
                if (ImportacaoImperium.Dt_Email_Entidade != null)
                {
                    var emails = RetornaEmailEntidade(fornecedor.Id);

                    foreach (string tipo in emails.Keys)
                    {
                        emails.TryGetValue(tipo, out string valor);

                        if (tipo == "Principal")
                            fornecedor.Email = valor;
                        else if (tipo == "Compras")
                            fornecedor.Email_Pedido = valor;
                        else
                            fornecedor.Obs = string.IsNullOrEmpty(fornecedor.Obs) ? valor : fornecedor.Obs += $";{valor}";
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region MÉTODOS EM GERAL
        private void ExecutaAumentoPacoteMySQL()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(strConexaoMySql))
                {
                    conn.Open();
                    string comando = @"SET GLOBAL max_allowed_packet = 1073741824;";
                    MySqlCommand command = new MySqlCommand(comando, conn);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao aumentar pacote de dados");
                Logar(ex.Message);
            }
        }
        private DataTable RecuperaDataTableSQLServer(string comando)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strConexaoSqlServer))
                {
                    DataTable dt = new DataTable();
                    SqlCommand command = new SqlCommand(comando, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void InsertBanco(StringBuilder strBuilder, MySqlCommand command, int posicaoRemover = 3, int qtdeRemover = 1)
        {
            string strComandoProduto = $"{strBuilder.ToString().Remove(strBuilder.ToString().Length - posicaoRemover, qtdeRemover)};";
            command = new MySqlCommand(strComandoProduto, connecctionMysql);
            command.ExecuteNonQuery();
        }
        private Dictionary<string, string> RetornaFoneEntidade(long idEntidade)
        {
            try
            {
                if (ImportacaoImperium.Dt_Fone_Entidade.Rows.Count > 0)
                {
                    var dicionarioFones = new Dictionary<string, string>();

                    if (dicionarioFones.Count > 0 || dicionarioFones != null)
                    {
                        foreach (DataRow r in ImportacaoImperium.Dt_Fone_Entidade.Select($"fkEntidade = {idEntidade}"))
                            dicionarioFones.Add(r["tipo"].ToString(), r["numero"].ToString());
                    }

                    return dicionarioFones;
                }

                return new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Logar("Erro ao retornar fone da entidade");
                Logar(ex.Message);
                return new Dictionary<string, string>();
            }
        }
        private Dictionary<string, string> RetornaEmailEntidade(long idEntidade)
        {
            try
            {
                if (ImportacaoImperium.Dt_Email_Entidade.Rows.Count > 0)
                {
                    var dicionarioEmails = new Dictionary<string, string>();

                    foreach (DataRow r in ImportacaoImperium.Dt_Email_Entidade.Select($"fkEntidade = {idEntidade}"))
                        dicionarioEmails.Add(r["tipo"].ToString(), r["numero"].ToString());
                }

                return new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Logar("Erro ao retornar email da entidade");
                Logar(ex.Message);
                return new Dictionary<string, string>(); ;
            }
        }
        private string RetornaValorCampoDtProduto(string idFOP, string campoFOP)
        {
            if (ImportacaoImperium.Dt_Produto.Rows.Count > 0)
            {
                foreach(DataRow r in ImportacaoImperium.Dt_Produto.Select($"id = '{idFOP}'"))
                    return r[campoFOP].ToString();
            }

            return "";
        }
        private long RetornaFkDeptoCategoria(long idCategoria)
        {
            if (ImportacaoImperium.Dt_SubGrupo.Rows.Count > 0)
            {
                foreach (DataRow r in ImportacaoImperium.Dt_SubGrupo.Select($"id = {idCategoria}"))
                    return ConverterInt64(r["fkDepto"].ToString());
            }

            return 0;
        }
        private void HabiltaCheckBox()
        {
            if (ImportacaoImperium != null)
            {
                chkProdutos.Enabled = ImportacaoImperium.Dt_Produto.Rows.Count > 0;
                chkClientes.Enabled = ImportacaoImperium.Lista_Clientes.Count > 0;
                chkFornecedores.Enabled = ImportacaoImperium.Lista_Fornecedores.Count > 0;
            }
        }
        private void InformaContadorRegistrosCarregados()
        {
            lblProdutos.Text = ImportacaoImperium.Dt_Produto != null ? ImportacaoImperium.Dt_Produto.Rows.Count.ToString() : "0";
            lblClientes.Text = ImportacaoImperium.Lista_Clientes != null ? ImportacaoImperium.Lista_Clientes.Count.ToString() : "0";
            lblFornecedores.Text = ImportacaoImperium.Lista_Fornecedores != null ? ImportacaoImperium.Lista_Fornecedores.Count.ToString() : "0";
            lblFamilias.Text = ImportacaoImperium.Dt_Familia != null ? ImportacaoImperium.Dt_Familia.Rows.Count.ToString() : "0";
            lblItensFornecedor.Text = ImportacaoImperium.Dt_Itens_Fornecedor != null ? ImportacaoImperium.Dt_Itens_Fornecedor.Rows.Count.ToString() : "0";
        }
        private void InformaContadorRegistrosImportados()
        {
            lblContProdutos.Text = contadorImportacao.Cont_Produtos.ToString();
            lblContClientes.Text = contadorImportacao.Cont_Clientes.ToString();
            lblContFornecedores.Text = contadorImportacao.Cont_Fornecedores.ToString();
            lblContFamilias.Text = contadorImportacao.Cont_Familias.ToString();
            lblContItensFornecedor.Text = contadorImportacao.Cont_ItensFornecedor.ToString();
        }
        private void Logar(string mensagem)
        {
            try
            {
                string nomeArquivo = $"LOG_{DateTime.Now:yyyyMMdd}.log";
                string caminhoArquivo = $"{Directory.GetCurrentDirectory()}\\{nomeArquivo}";

                if (!File.Exists(caminhoArquivo))
                    File.Create(caminhoArquivo);

                using (StreamWriter sw = File.AppendText(caminhoArquivo))
                {
                    string hora = DateTime.Now.ToString("HH:mm:ss");
                    sw.WriteLine($"{hora}-> {mensagem}");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao criar arquivo de log");
                Logar(ex.Message);
            }
        }

        #endregion

        #region MÉTODOS DE TRATAMENTO/CONVERSÃO DE DADOS

        private int ConverterInt32(string valor)
        {
            try
            {
                int.TryParse(valor, out int result);
                return result;
            }
            catch { return 0; }
        }
        private long ConverterInt64(string valor)
        {
            try
            {
                Int64.TryParse(valor, out long result);
                return result;
            }
            catch { return 0; }
        }
        private decimal ConverterDecimal(string valor, bool trocarPontoPorVirgula = true)
        {
            try
            {
                valor = trocarPontoPorVirgula ? valor.Replace(".", ",") : valor;
                decimal.TryParse(valor, out decimal result);
                return result;
            }
            catch { return 0M; }
        }
        private DateTime ConverterDateTime(string valor)
        {
            try
            {
                DateTime.TryParse(valor, out DateTime result);
                return result;
            }
            catch { return DateTime.MinValue; }
        }
        private string TiraAspasSimplesString(string valor, string caracterReplace = "", bool trim = true)
        {
            var retorno = valor.Replace("'", caracterReplace);
            return trim ? retorno.Trim() : retorno;
        }
        private string LimpaStringDocumento(string valor, bool trim = true)
        {
            var retorno = valor.Replace("/", "").Replace(".", "").Replace("-", "");
            return trim ? retorno.Trim() :  retorno;
        }
        private string AjustaStringDecimal(string valor)
        {
            return valor.Replace(",", ".");
        }
        #endregion

    }
}
