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
using System.Net.Configuration;
using System.Net.Security;
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

        TimeSpan tempoInicio = new TimeSpan();
        TimeSpan tempoFim = new TimeSpan();

        Config mConfig = new Config();

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            if (!VerificaArquivoINI())
                CriaArquivoIni();
            else
                CarregarConfiguracoes();

            CriarDiretorioLogs();

            if (mConfig.Tipo_Conexao == "2")
                strConexaoSqlServer = $"Server={mConfig.Servidor_SQLServer};Database={mConfig.Banco_SQLServer};User Id={mConfig.Usuario_SQLServer};Password={mConfig.Senha_SQLServer}; ";
            else
                strConexaoSqlServer = $"Server={mConfig.Servidor_SQLServer};DataBase={mConfig.Banco_SQLServer};Trusted_Connection=True;";

            strConexaoMySql = $"server={mConfig.Servidor_MySQL};user id={mConfig.Usuario_MySQL};password={mConfig.Senha_MySQL};database={mConfig.Banco_MySQL};";

            mBackGroundWorker = new BackgroundWorker();
            mBackGroundWorker.WorkerSupportsCancellation = true;
            mBackGroundWorker.DoWork += mBackGroundWorker_DoWork;
            mBackGroundWorker.ProgressChanged += MBackGroundWorker_ProgressChanged;
            mBackGroundWorker.RunWorkerCompleted += MBackGroundWorker_RunWorkerCompleted;
            mBackGroundWorker.RunWorkerAsync();
        }
        private void btnFechar_Click(object sender, EventArgs e)
        {
            if (mBackGroundWorker.IsBusy)
            {
                Logar("OPERACAO CANCELADA PELO USUARIO...");
                mBackGroundWorker.CancelAsync();
            }

            Close();
        }
        private void btnCarregar_Click(object sender, EventArgs e)
        {
            if (!mBackGroundWorker.IsBusy)
            {
                operacaoImportador = OperacaoImportador.Carregar;
                ControleTela(false);
                mBackGroundWorker.RunWorkerAsync();
            }
        }
        private void btnImportar_Click(object sender, EventArgs e)
        {
            if (!mBackGroundWorker.IsBusy)
            {
                operacaoImportador = OperacaoImportador.Importar;
                ControleTela(false);
                mBackGroundWorker.RunWorkerAsync();
            }
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
                HabilitaCheckBox();

                btnImportar.Enabled = true; //TODO: FAZER UMA VERIFICACAO PARA HABILITAR O BOTAO

                MessageBox.Show("Carregamento Concluído !");
                ControleTela(true);
            }
            else if (operacaoImportador == OperacaoImportador.Importar)
            {
                InformaContadorRegistrosImportados();

                tempoFim =  new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                lblTempoValorImportacao.Text = (tempoFim - tempoInicio).ToString();

                Logar($"IMPORTACAO CONCLUIDA...");
                Logar($"TEMPO DECORRIDO - {lblTempoValorImportacao.Text}");
                MessageBox.Show("Importação Concluída !");
                ControleTela(true);
            }
        }
        private void chkProdutos_CheckedChanged(object sender, EventArgs e)
        {
            chkFamilias.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Familia.Rows.Count > 0;
            chkItensFornecedor.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Itens_Fornecedor.Rows.Count > 0;
            chkGrupo.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Grupo.Rows.Count > 0;
            chkNFEntrada.Enabled = chkProdutos.Checked && ImportacaoImperium.Dt_Nota_Entrada.Rows.Count > 0;
        }
        private void chkFornecedores_CheckedChanged(object sender, EventArgs e)
        {
            chkContasPagar.Enabled = chkFornecedores.Checked;
        }
        private void chkClientes_CheckedChanged(object sender, EventArgs e)
        {
            chkContasReceber.Enabled = chkClientes.Checked;
        }
        private void chkGrupo_CheckedChanged(object sender, EventArgs e)
        {
            chkSubGrupo.Checked = chkSubGrupo1.Checked = chkGrupo.Checked;
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
                ImportacaoImperium.Dt_Contas_Pagar = CarregarContasPagar();
                ImportacaoImperium.Dt_Contas_Receber = CarregarContasReceber();
                ImportacaoImperium.Dt_Nota_Entrada = CarregarNotaEntrada();
                ImportacaoImperium.Dt_Nota_Entrada_Itens = CarregarNotaEntradaItens();
                ImportacaoImperium.Dt_Vendas = CarregarVendas();
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
        private DataTable CarregarContasPagar()
        {
            try
            {
                string comando = @"SELECT * FROM Financeiro.ContasPagar;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar contas a pagar");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarContasReceber()
        {
            try
            {
                string comando = @"SELECT * FROM Financeiro.ContasReceber;";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {
                Logar("Erro ao carregar informações de contas a receber");
                Logar(ex.Message);
                return null;
            }
        }
        private DataTable CarregarNotaEntrada()
        {
            try
            {
                string de = ConverterDateTime(dataNotaDE.Text).ToString("yyyy-MM-dd");
                string ate = ConverterDateTime(dataNotaATE.Text).ToString("yyyy-MM-dd");

                string comando = $@"SELECT *
                    FROM NF.NFe n
                    JOIN NF.TipoNFe t ON n.fkTipoNF = t.id
                    WHERE n.natOp = 'COMPRAS'
                    AND n.dhSaiEnt BETWEEN '{de}' AND '{ate}';";
                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable CarregarNotaEntradaItens()
        {
            try
            {
                string de = ConverterDateTime(dataNotaDE.Text).ToString("yyyy-MM-dd");
                string ate = ConverterDateTime(dataNotaATE.Text).ToString("yyyy-MM-dd");

                string comando = $@"SELECT i.* 
                                    FROM nf.NFeItens i 
                                    JOIN nf.NFe n ON i.nroNF = n.nroNF AND i.serie = n.serie AND i.emitCNPJ = n.emitCNPJ
                                    WHERE n.natOp = 'COMPRAS'
                                    AND n.dhSaiEnt BETWEEN '{de}' AND '{ate}';";

                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private DataTable CarregarVendas()
        {
            try
            {
                string de = ConverterDateTime(dataVendaDE.Text).ToString("yyyy-MM-dd");
                string ate = ConverterDateTime(dataVendaATE.Text).ToString("yyyy-MM-dd");

                string comando = $@"SELECT c.id AS codigo_fop, i.fkProduto AS codigoEan, i.vlTotal AS valor, qtdade AS quantidade, c.fkPDV AS ecf, i.vlDesconto AS descontoItem, fkLoja AS loja,c.dtInicio AS datamov, i.vlCustoMedioUnit AS custoProduto, iif(i.cancelado = 1, 'C', 'A') AS situacao, i.vlUnit AS valor_unitario
                    FROM Comercial.Venda c 
                    JOIN Comercial.ItemVendido i ON c.id = i.fkVenda
                    WHERE c.dtInicio BETWEEN '{de}' AND '{ate}';";

                return RecuperaDataTableSQLServer(comando);
            }
            catch (Exception ex)
            {

                throw;
            }
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
            tempoInicio = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            ExecutaAumentoPacoteMySQL();

            if (chkProdutos.Checked)
            {
                Logar("IMPORTANDO TRIBUTACOES...");
                ImportarTributacoes();

                Logar("IMPORTANDO PRODUTOS...");
                ImportarProdutos();

                if (chkFamilias.Checked)
                {
                    Logar("IMPORTANDO FAMILIAS...");
                    ImportarFamilia();
                }

                if (chkGrupo.Checked)
                {
                    Logar("IMPORTANDO GRUPOS...");
                    ImportarGrupo();

                    Logar("IMPORTANDO SUBGRUPOS...");
                    ImportarSubGrupo();

                    Logar("IMPORTANDO SUBGRUPOS1...");
                    ImportarSubGrupo1();

                    Logar("IMPORTANDO CORRIGINDO ARVORE MERCADOLOGICA...");
                    AdicionarGrupoAClassificar();
                    CorrigirArvoreMercadologica();
                }

                if (chkNFEntrada.Checked)
                {
                    Logar("IMPORTANDO NOTAS DE ENTRADA...");
                    ImportarNotaEntrada();
                }

                if (chkVenda.Checked)
                {
                    Logar("IMPORTANDO ITENS VENDA");
                    ImportarItemVenda();
                }
            }

            if (chkClientes.Checked)
            {
                Logar("IMPORTANDO CLIENTES...");
                ImportarClientes();

                Logar("IMPORTANDO CONTAS RECEBER...");
                ImportarContasReceber();
            }

            if (chkFornecedores.Checked)
            {
                Logar("IMPORTANDO FORNECEDORES...");
                ImportarFornecedores();

                if (chkContasPagar.Checked)
                {
                    Logar("IMPORTANDO CONTAS PAGAR...");
                    ImportarContasPagar();
                }
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
        private void ImportarContasPagar()
        {
            List<ContaPagar> pagar = new List<ContaPagar>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Contas_Pagar.Rows)
                    pagar.Add(RetornaContasPagarPorDataRow(r));

                if (pagar.Count > 0)
                    ExecutaComandoContasPagar(pagar);
            }
            catch (Exception ex)
            {
                Logar("Erro ao importar informações contas a pagar");
                Logar(ex.Message);
            }
        }
        private void ImportarContasReceber()
        {
            List<ContaReceber> receber = new List<ContaReceber>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Contas_Receber.Rows)
                    receber.Add(RetornaContaReceberPorDataRow(r));

                if (receber.Count > 0)
                    ExecutaComandoContasReceber(receber);
            }
            catch (Exception ex)
            {
                Logar("Erro ao importar informações de contas a receber");
                Logar(ex.Message);
            }
        }
        private void ImportarNotaEntrada()
        {
            List<NotaEntrada> notas = new List<NotaEntrada>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Nota_Entrada.Rows)
                    notas.Add(RetornaNotaEntradaPorDataRow(r));

                if (notas.Count > 0)
                {
                    foreach (NotaEntrada nota in notas)
                        nota.Itens = RetornaItensNotaEntrada(nota.Numero_FOP, nota.Serie_Fop, nota.Cnpj_Emitente_Fop);

                    ExecutaComandoNotaEntrada(notas);
                }
                    
            }
            catch (Exception ex)
            {
                Logar("Erro ao importar notas de entrada");
                Logar(ex.Message);
            }
        }
        private void ImportarItemVenda() 
        {
            List<ItemVenda> lstItemVenda = new List<ItemVenda>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Vendas.Rows)
                    lstItemVenda.Add(RetornaItemVendaPorDataRow(r));

                if (lstItemVenda.Count > 0)
                    ExecutaComandoItemVenda(lstItemVenda);
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
                {
                    connecctionMysql.Close();
                    connecctionMysql.Dispose();
                }
                    
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
            finally { FecharConexaoMysql(); }

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
                    if (string.IsNullOrEmpty(p.Descricao))
                    {
                        Logar($"Produto sem descrição - {p.Ean1}");
                        Logar("Passando para o próximo produto");
                        continue;
                    }

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

                    if (cont == mConfig.Qtde_Importar)
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

                CorrigirProdutoSemArvoreMercadologica();
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
        private ProdutoImperium RetornaProdutoImperiumPorDataRow(DataRow r)
        {
            int loja = ConverterInt32(r["fkCliente"].ToString());
            string descricaoReduzida = r["nomeImpressao"].ToString().Length > 24 ? r["nomeImpressao"].ToString().Substring(0, 24) : r["nomeImpressao"].ToString();
            string cest = r["cest"].ToString().Length > 7 ? r["cest"].ToString().Substring(0, 7) : r["cest"].ToString();

            ProdutoImperium produto = new ProdutoImperium();
            produto.Id = ConverterInt64(r["id"].ToString());
            produto.Descricao = r["nomeProduto"].ToString().Trim();
            produto.Descricao_Reduzida = string.IsNullOrEmpty(descricaoReduzida) ? produto.Descricao.Length > 24 ? produto.Descricao.Substring(0, 24) : produto.Descricao : descricaoReduzida;
            produto.Descricao_Reduzida = produto.Descricao_Reduzida.Trim();
            produto.Unidade_Entrada = r["unidade"].ToString();
            produto.Unidade_Saida = r["unidade"].ToString();
            produto.Embalagem_Entrada = ConverterDecimal(r["tamCaixa"].ToString());
            produto.Embalagem_Saida = ConverterDecimal(r["tamEmbVenda"].ToString());
            produto.Obs = "IMPORTADO";
            produto.Validade = 0; //TODO: VERIFICAR CAMPO
            produto.Id_Grupo = RetornaIdDeptoPorCategoria(ConverterInt32(r["fkCategoria"].ToString()));
            produto.Id_SubGrupo = ConverterInt32(r["fkCategoria"].ToString());
            produto.Id_SubGrupo1 = ConverterInt32(r["fkSubCategoria"].ToString());
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
            DateTime inicioPromo = new DateTime(2021, 1, 1);
            DateTime finalPromo = new DateTime(2021, 1, 1);
            decimal precoVenda = r["precoVenda"].ToString().Length > 14 ? 0 : ConverterDecimal(r["precoVenda"].ToString());
            decimal precoAnterior = r["precoAnterior"].ToString().Length > 14 ? 0 : ConverterDecimal(r["precoAnterior"].ToString());
            produto.Preco.LOJA = loja;
            produto.Preco.CUSTO = Math.Round(ConverterDecimal(r["custoCaixa"].ToString()) / tamanhoCaixa, 3);
            produto.Preco.CUSTO_MEDIO = ConverterDecimal(r["custoMedio"].ToString());
            produto.Preco.VENDA1 = Math.Round(ConverterDecimal(r["precoVenda"].ToString()), 3);
            produto.Preco.VENDA2 = 0;
            produto.Preco.DTINICIOPROMOCAO = inicioPromo.ToString("yyyy-MM-dd");
            produto.Preco.DTINICIOPROMOCAO = finalPromo.ToString("yyyy-MM-dd");
            produto.Preco.MARGEM = ConverterDecimal(r["margemDesejada"].ToString());
            produto.Preco.VENDA1_ANTERIOR = Math.Round(precoAnterior, 3);
            produto.Preco.IDFAMILIA = ConverterInt32(r["fkFamilia"].ToString());

            produto.Estoque = new ProdutoEstoque();
            produto.Estoque.Loja = loja;
            produto.Estoque.Estoque_Atual = ConverterDecimal(r["estoqueAtual"].ToString());
            produto.Estoque.Estoque_Minimo = ConverterDecimal(r["estoqueMin"].ToString());
            produto.Estoque.Cobertura_Estoque = 0;

            produto.Tributacao = new ProdutoTributacao();
            produto.Tributacao.Loja = loja;
            produto.Tributacao.Origem = "0 - NACIONAL";
            produto.Tributacao.Tipo_Produto = "00 - MERCADORIA PARA REVENDA";
            AjustaTributacaoProduto(produto);

            return produto;
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
            CorrigirCamposProdutoPreco(produto.Preco);

            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{produto.Id},");
            stringBuilder.Append($"{produto.Preco.LOJA},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.CUSTO.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.CUSTO_MEDIO.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA1.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA2.ToString())},");
            stringBuilder.Append($"'{produto.Preco.DTINICIOPROMOCAO}',");
            stringBuilder.Append($"'{produto.Preco.DTFINALPROMOCAO}',");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.MARGEM.ToString())},");
            stringBuilder.Append($"{produto.Preco.IDFAMILIA},");
            stringBuilder.Append($"{AjustaStringDecimal(produto.Preco.VENDA1_ANTERIOR > 0 ? produto.Preco.VENDA1_ANTERIOR.ToString() : "0.00")}");
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
        private void CorrigirCamposProdutoPreco(ProdutoPreco preco)
        {
            if (preco.VENDA1 > 99999.99M)
                preco.VENDA1 = 0;

            if (preco.VENDA1_ANTERIOR > 99999.99M)
                preco.VENDA1_ANTERIOR = 0;
        }
        private void CorrigirProdutoSemArvoreMercadologica()
        {
            try
            {
                AbrirConexaoMysql();
                int grupo = 0, subgrupo = 0, subgrupo1 = 0;
                string comandoRecuperaArvoreMercadologica = @"SELECT idGrupo, idSubGrupo, idSubGrupo1 
                                                                FROM subgrupo1 
                                                                WHERE nome = 'A CLASSIFICAR' LIMIT 1;";
                MySqlCommand command = new MySqlCommand(comandoRecuperaArvoreMercadologica, connecctionMysql);

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        grupo = ConverterInt32(rdr["idGrupo"].ToString());
                        subgrupo = ConverterInt32(rdr["idSubGrupo"].ToString());
                        subgrupo1 = ConverterInt32(rdr["idSubGrupo1"].ToString());
                    }
                }

                if (grupo > 0 && subgrupo > 0 && subgrupo1 > 0)
                {
                    string comando = $@"UPDATE produto SET
                                        idGrupo = {grupo},
                                        idSubGrupo = {subgrupo},
                                        idSubGrupo1 = {subgrupo1}
                                       WHERE (idGrupo = 0 OR idSubGrupo = 0 OR idSubGrupo1 = 0);";

                    command = new MySqlCommand(comando, connecctionMysql);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao corrigir árvore mercadológica produtos");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
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

                    if (cont == mConfig.Qtde_Importar)
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
                    contadorImportacao.Cont_Grupo++;

                    if (cont == mConfig.Qtde_Importar)
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
            finally { FecharConexaoMysql(); }
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
                    contadorImportacao.Cont_SubGrupo++;
                    strGrupo.AppendLine(RetornaLinhaInserirSubGrupo(g));

                    if (cont == mConfig.Qtde_Importar)
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
                    contadorImportacao.Cont_SubGrupo1++;

                    if (cont == mConfig.Qtde_Importar)
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
            finally { FecharConexaoMysql(); }
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
        private void AdicionarGrupoAClassificar()
        {
            try
            {
                AbrirConexaoMysql();
                string comando = @"INSERT INTO grupo (NOME) VALUES ('A CLASSIFICAR');";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
                long idGrupo = command.LastInsertedId;

                if (idGrupo > 0)
                    AdicionarSubGrupoAClassificar(idGrupo);
            }
            catch (Exception ex)
            {
                Logar("Erro ao inserir grupo A CLASSIFICAR");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
        }
        private void AdicionarSubGrupoAClassificar(long idGrupo)
        {
            try
            {
                AbrirConexaoMysql();
                string comando = $@"INSERT INTO subgrupo (nome, idgrupo) VALUES ('A CLASSIFICAR', {idGrupo});";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
                long idSubGrupo = command.LastInsertedId;

                if (idSubGrupo > 0)
                    AdicionarSubGrupo1AClassificar(idGrupo, idSubGrupo);
            }
            catch (Exception ex)
            {
                Logar("Erro ao inserir subgrupo A CLASSIFICAR");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
        }
        private void AdicionarSubGrupo1AClassificar(long idGrupo, long idSubGrupo)
        {
            try
            {
                AbrirConexaoMysql();
                string comando = $@"INSERT INTO subgrupo1 (nome, idgrupo, idsubgrupo) VALUES ('A CLASSIFICAR', {idGrupo}, {idSubGrupo});";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logar("Erro ao inserir subgrupo1 A CLASSIFICAR");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
        }
        private int RetornaIdDeptoPorCategoria(long idCategoria)
        {
            foreach (DataRow r in ImportacaoImperium.Dt_SubGrupo.Select($"id = {idCategoria}"))
                return ConverterInt32(r["fkDepto"].ToString());

            return 0;
        }
        private void CorrigirArvoreMercadologica()
        {
            int idSubGrupo = CorrecaoSubGrupo();
            CorrecaoSubGrupo1(idSubGrupo);
        }
        private int CorrecaoSubGrupo()
        {
            try
            {
                AbrirConexaoMysql();
                string comandoNovoId = "SELECT MAX(idsubgrupo) + 1 FROM subgrupo;";
                MySqlCommand command = new MySqlCommand(comandoNovoId, connecctionMysql);
                int novoId = 0;
                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.Read())
                        novoId = ConverterInt32(rdr[0].ToString());
                }

                if (novoId > 0)
                {
                    string comando = $@"UPDATE subgrupo SET idsubgrupo = {novoId} WHERE idsubgrupo IS NULL AND nome = 'A CLASSIFICAR';";
                    command = new MySqlCommand(comando, connecctionMysql);
                    command.ExecuteNonQuery();
                    return novoId;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Logar("Erro ao corrigir subgrupo");
                Logar(ex.Message);
                return 0;
            }
            finally { FecharConexaoMysql(); }
        }
        private void CorrecaoSubGrupo1(int idSubGrupo)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoNovoId = "SELECT MAX(idsubgrupo1) + 1 FROM subgrupo1;";
                MySqlCommand command = new MySqlCommand(comandoNovoId, connecctionMysql);
                int novoId = 0;
                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.Read())
                        novoId = ConverterInt32(rdr[0].ToString());
                }

                if (novoId > 0)
                {
                    string comando = $@"UPDATE subgrupo1 SET idsubgrupo1 = {novoId}, idsubgrupo = {idSubGrupo} WHERE idsubgrupo1 IS NULL AND nome = 'A CLASSIFICAR';";
                    command = new MySqlCommand(comando, connecctionMysql);
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Logar("Erro ao corrigir subgrupo1");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
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

                    if (cont == mConfig.Qtde_Importar)
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
            finally { FecharConexaoMysql(); }
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

                    if (cont == mConfig.Qtde_Importar)
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

                    if (cont == mConfig.Qtde_Importar)
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

        #region CONTAS PAGAR

        private ContaPagar RetornaContasPagarPorDataRow(DataRow r)
        {
            ContaPagar pagar = new ContaPagar();
            pagar.Id_Fornecedor = ConverterInt64(r["fkEntidade"].ToString());
            pagar.Numero_Doc = r["id"].ToString();
            pagar.Data_Entrada = ConverterDateTime(r["dtEntrada"].ToString());
            pagar.Data_Emissao = pagar.Data_Entrada;
            pagar.Data_Vencimento = ConverterDateTime(r["dtVencAtual"].ToString());
            pagar.Valor_Doc = ConverterDecimal(r["valorAtual"].ToString());
            pagar.Id_Pc1 = 0;
            pagar.Obs = r["nroNF"].ToString();
            pagar.Data_Pagto = ConverterDateTime(r["dtPago"].ToString());
            pagar.Tipo = "C";
            pagar.Numero_Cheque = "";
            pagar.Id_Banco = 0;
            pagar.Id_Pedido = 0;
            pagar.Situacao = "A"; //TODO: MUDAR A SITUACAO DE ACORDO COM O VALOR PAGO
            pagar.Id_Cheque = 0;
            pagar.Id_NF_Entrada = 0;
            pagar.Parcelado = "nao";
            pagar.Loja = ConverterInt32(r["fkLoja"].ToString());
            pagar.Previsao = "0"; //TODO: VERIFICAR A UTLIDADE DO CAMPO
            pagar.Id_Pc2 = 0;
            pagar.Id_Tipo_Cobranca = 0; //TODO: VERIFICAR A UTLIDADE DO CAMPO
            pagar.Valor_Desconto = 0;
            pagar.Historico = r["referencia"].ToString();
            pagar.Valor_Cheque = 0;
            pagar.Valor_Complemento = 0;
            pagar.Duplicata = "";
            pagar.Valor_Acrescimo = 0;
            pagar.Valor_Custas_Cartorio = 0;
            pagar.Valor_Total_Pagar = ConverterDecimal(r["valorAtual"].ToString());
            pagar.Id_Sub_Categoria = 0;
            pagar.Id_NF_Entrada_GNRE = 0;
            pagar.Tipo_Documento = "NF";
            pagar.Id_Tranferencia = 0;

            return pagar;
        }
        private void ExecutaComandoContasPagar(List<ContaPagar> lstPagar)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE pagar;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO pagar(
                                    idFornecedor, nr_docto, dt_entrada, dt_emissao, dt_vencto, vl_docto, idPc1, obs, dt_pagto, tipo, ncheque, idBanco, idPedido, situacao, idCheque, idNfEntrada, parcelado, loja, previsao, idPc2, idTipoCobranca, vlDesconto, historico, valorcheque, vl_complemento, duplicata, vlAcrescimo, vlCustasCartorio, vlTotalPagar, idsubcategoria, idNfEntrada_gnre, TipoDocumento, idTransferencia
                                    ) VALUES ";
                StringBuilder strBuilderPagar = new StringBuilder(comando);
                int cont = 0;

                foreach (ContaPagar p in lstPagar)
                {
                    strBuilderPagar.AppendLine(RetornaLinhaInserirContasPagar(p));
                    contadorImportacao.Cont_Contas_Pagar++;
                    cont++;

                    if (cont == mConfig.Qtde_Importar)
                    {
                        InsertBanco(strBuilderPagar, command);
                        strBuilderPagar.Clear();
                        strBuilderPagar.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderPagar, command);
                    strBuilderPagar.Clear();
                    strBuilderPagar.Append(comando);
                    cont = 0;
                }

                CorrigirCamposContasPagar();
            }
            catch (Exception)
            {

                throw;
            }
            finally { FecharConexaoMysql(); }
        }
        private string RetornaLinhaInserirContasPagar(ContaPagar pagar)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{pagar.Id_Fornecedor},");
            stringBuilder.Append($"'{pagar.Numero_Doc}',");
            stringBuilder.Append($"'{pagar.Data_Entrada:yyyy-MM-dd}',");
            stringBuilder.Append($"'{pagar.Data_Emissao:yyyy-MM-dd}',");
            stringBuilder.Append($"'{pagar.Data_Vencimento:yyyy-MM-dd}',");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Doc.ToString())},");
            stringBuilder.Append($"{pagar.Id_Pc1},");
            stringBuilder.Append($"'{pagar.Obs}',");
            stringBuilder.Append($"'{pagar.Data_Pagto:yyyy-MM-dd}',");
            stringBuilder.Append($"'{pagar.Tipo}',");
            stringBuilder.Append($"'{pagar.Numero_Cheque}',");
            stringBuilder.Append($"{pagar.Id_Banco},");
            stringBuilder.Append($"{pagar.Id_Pedido},");
            stringBuilder.Append($"'{pagar.Situacao}',");
            stringBuilder.Append($"{pagar.Id_Cheque},");
            stringBuilder.Append($"{pagar.Id_NF_Entrada},");
            stringBuilder.Append($"'{pagar.Parcelado}',");
            stringBuilder.Append($"{pagar.Loja},");
            stringBuilder.Append($"'{pagar.Previsao}',");
            stringBuilder.Append($"{pagar.Id_Pc2},");
            stringBuilder.Append($"{pagar.Id_Tipo_Cobranca},");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Desconto.ToString())},");
            stringBuilder.Append($"'{pagar.Historico}',");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Cheque.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Complemento.ToString())},");
            stringBuilder.Append($"'{pagar.Duplicata}',");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Acrescimo.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Custas_Cartorio.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(pagar.Valor_Total_Pagar.ToString())},");
            stringBuilder.Append($"{pagar.Id_Sub_Categoria},");
            stringBuilder.Append($"{pagar.Id_NF_Entrada_GNRE},");
            stringBuilder.Append($"'{pagar.Tipo_Documento}',");
            stringBuilder.Append($"{pagar.Id_Tranferencia}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        private void CorrigirCamposContasPagar()
        {
            try
            {
                AbrirConexaoMysql();
                string comando = @"UPDATE pagar SET situacao = NULL, dt_pagto = NULL WHERE dt_pagto < dt_vencto;";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);
                command.ExecuteNonQuery();

                string comando2 = @"UPDATE pagar SET situacao = 'B' WHERE dt_pagto IS NOT NULL;";
                command = new MySqlCommand(comando2, connecctionMysql);
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Logar("Erro ao corrigir campos pagar");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
        }
        #endregion

        #region CONTAS RECEBER

        private ContaReceber RetornaContaReceberPorDataRow(DataRow r)
        {
            ContaReceber receber = new ContaReceber();
            receber.Id_Cliente = ConverterInt64(r["fkEntidade"].ToString());
            receber.Numero_Venda = r["id"].ToString();
            receber.Valor_Vista = ConverterDecimal(r["valorAtual"].ToString());
            receber.Data_Venda = ConverterDateTime(r["dtReferencia"].ToString());
            receber.Situacao = r["pago"].ToString() == "1" ? "P" : "A";
            receber.Data_Vencimento = ConverterDateTime(r["dtVencAtual"].ToString());
            receber.Loja = ConverterInt32(r["fkLoja"].ToString());
            receber.ECF = 999;
            receber.Tipo_Cobranca = "BL";
            receber.Obs = $"IMPORTADO - {r["referencia"]}";
            receber.Id_Pc1 = 0;
            receber.Id_Pc2 = 0;

            return receber;
        }
        private void ExecutaComandoContasReceber(List<ContaReceber> lstReceber)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE debito;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO debito(
                                    IDCLIENTE, NR_VENDA, VL_VISTA, DT_VENDA, SITUACAO, DT_VENC, loja, ECF, TipoCobranca, observacao, idpc1, idpc2
                                    ) VALUES ";
                StringBuilder strBuilderReceber = new StringBuilder(comando);
                int cont = 0;

                foreach (ContaReceber r in lstReceber)
                {
                    strBuilderReceber.AppendLine(RetornaLinhaInserirContaReceber(r));
                    cont++;
                    contadorImportacao.Cont_Contas_Receber++;

                    if (cont == mConfig.Qtde_Importar)
                    {
                        InsertBanco(strBuilderReceber, command);
                        strBuilderReceber.Clear();
                        strBuilderReceber.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderReceber, command);
                    strBuilderReceber.Clear();
                    strBuilderReceber.Append(comando);
                    cont = 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao executar comando de inserção de contas a receber");
                Logar(ex.Message);
            }
            finally { FecharConexaoMysql(); }
        }
        private string RetornaLinhaInserirContaReceber(ContaReceber receber)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{receber.Id_Cliente},");
            stringBuilder.Append($"'{receber.Numero_Venda}',");
            stringBuilder.Append($"{AjustaStringDecimal(receber.Valor_Vista.ToString())},");
            stringBuilder.Append($"'{receber.Data_Venda:yyyy-MM-dd}',");
            stringBuilder.Append($"'{receber.Situacao}',");
            stringBuilder.Append($"'{receber.Data_Vencimento:yyyy-MM-dd}',");
            stringBuilder.Append($"{receber.Loja},");
            stringBuilder.Append($"{receber.ECF},");
            stringBuilder.Append($"'{receber.Tipo_Cobranca}',");
            stringBuilder.Append($"'{receber.Obs}',");
            stringBuilder.Append($"{receber.Id_Pc1},");
            stringBuilder.Append($"{receber.Id_Pc2}");
            stringBuilder.Append("),");
            
            return stringBuilder.ToString();
        }

        #endregion

        #region NF ENTRADA

        private NotaEntrada RetornaNotaEntradaPorDataRow(DataRow r)
        {
            NotaEntrada nota = new NotaEntrada();
            nota.CFOP = r["fkCFOP"].ToString();
            nota.Numero_FOP = r["nroNF"].ToString();
            nota.Serie_Fop = r["serie"].ToString();
            nota.Cnpj_Emitente_Fop = r["emitCNPJ"].ToString();
            nota.Numero = r["nroNF"].ToString();
            nota.Valor_Total = ConverterDecimal(r["valorNF"].ToString());
            nota.Valor_Base_Icms = ConverterDecimal(r["baseCalculoICMS"].ToString());
            nota.Valor_Icms = ConverterDecimal(r["valorICMS"].ToString());
            nota.Valor_Outras = ConverterDecimal(r["valorOutros"].ToString());
            nota.Valor_IPI = ConverterDecimal(r["valorIPI"].ToString());
            nota.Valor_Base_Icms_ST = ConverterDecimal(r["baseCalculoST"].ToString());
            nota.Valor_Icms_ST = ConverterDecimal(r["valorST"].ToString());
            nota.Id_Fornecedor = ConverterInt64(r["fkEntidade"].ToString());
            nota.Data_Emissao = ConverterDateTime(r["dEmi"].ToString());
            nota.Data_Entrada = ConverterDateTime(r["dhSaiEnt"].ToString());
            nota.Obs = "IMPORTADO";
            nota.Serie = ConverterInt32(r["serie"].ToString());
            nota.Especie = "NFe";
            nota.Modelo = "55";
            nota.Loja = ConverterInt32(r["fkLoja"].ToString());
            nota.Situacao = "E";
            nota.Chave_Eletronica = r["chaveAcesso"].ToString();
            nota.Protocolo = r["protocolo"].ToString();
            nota.Usuario = "IMPORTACAO";

            return nota;
        }
        private void ExecutaComandoNotaEntrada(List<NotaEntrada> lstNota)
        {
            var codigosFiscais = CarregaCodigoFiscal();

            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE nfentrada; TRUNCATE itensnfentrada;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO nfentrada (idNFEntrada, NumeroNF, ValorTotalNF, ValorBaseIcms, ValorICMS, Outras, IPI, BaseIcmsSubst, ValorICMSSubst, idFornecedor, Dt_Emissao, Dt_Entrada, Obs, Serie, Especie, Modelo, loja, situacao, ChaveEletronica, ProtocoloNfe, usuario, idCodigoFiscal) VALUES ";

                string comandoItens = @"INSERT INTO itensnfentrada (idProduto, valortotalcx, quantidade, margem, tributacao, valoripi, descvalor, descPorcento, acresValor, acresporcento, fretevalor, freteporcento, precovista, idNFEntrada, prsugestao, MargemIva,
                  ReducaoIcms, ReducaoIcmsSt, cfop, percentual_fcp, vl_base_fcp, vl_fcp, percentual_fcp_st, vl_base_fcp_st, vl_fcp_st) VALUES ";

                StringBuilder strBuilderNotaEntrada = new StringBuilder(comando);
                StringBuilder strBuilderItens = new StringBuilder(comandoItens);
                int cont = 0;

                foreach (NotaEntrada nota in lstNota)
                {
                    cont++;
                    contadorImportacao.Cont_Nota_Entrada++;

                    nota.Id = contadorImportacao.Cont_Nota_Entrada;
                    nota.Id_Codigo_Fiscal = codigosFiscais.FirstOrDefault(c => c.Value.Replace(",", "") == nota.CFOP).Key;

                    strBuilderNotaEntrada.AppendLine(RetornaLinhaInserirNotaEntrada(nota));

                    foreach (ItemEntrada i in nota.Itens)
                    {
                        i.Id_NF = nota.Id;
                        strBuilderItens.AppendLine(RetornaLinhaInserirItemEntrada(i));
                    }

                    if (cont == mConfig.Qtde_Importar)
                    {
                        InsertBanco(strBuilderNotaEntrada, command);
                        InsertBanco(strBuilderItens, command);

                        strBuilderNotaEntrada.Clear();
                        strBuilderItens.Clear();
                        strBuilderNotaEntrada.Append(comando);
                        strBuilderItens.Append(comandoItens);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderNotaEntrada, command);
                    InsertBanco(strBuilderItens, command);

                    strBuilderNotaEntrada.Clear();
                    strBuilderItens.Clear();
                    strBuilderNotaEntrada.Append(comando);
                    strBuilderItens.Append(comandoItens);
                    cont = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally { FecharConexaoMysql(); }
        }
        private string RetornaLinhaInserirNotaEntrada(NotaEntrada nota)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{nota.Id},");
            stringBuilder.Append($"'{nota.Numero}',");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Total.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Base_Icms.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Icms.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Outras.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_IPI.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Base_Icms_ST.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(nota.Valor_Icms_ST.ToString())},");
            stringBuilder.Append($"{nota.Id_Fornecedor},");
            stringBuilder.Append($"'{nota.Data_Emissao:yyyy-MM-dd}',");
            stringBuilder.Append($"'{nota.Data_Entrada:yyyy-MM-dd}',");
            stringBuilder.Append($"'{nota.Obs}',");
            stringBuilder.Append($"{nota.Serie},");
            stringBuilder.Append($"'{nota.Especie}',");
            stringBuilder.Append($"'{nota.Modelo}',");
            stringBuilder.Append($"{nota.Loja},");
            stringBuilder.Append($"'{nota.Situacao}',");
            stringBuilder.Append($"'{nota.Chave_Eletronica}',");
            stringBuilder.Append($"'{nota.Protocolo}',");
            stringBuilder.Append($"'{nota.Usuario}',");
            stringBuilder.Append($"{nota.Id_Codigo_Fiscal}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }
        private ItemEntrada RetornaItemEntradaPorDataRow(DataRow r)
        {
            ItemEntrada itemEntrada = new ItemEntrada();
            itemEntrada.Id_Produto = RetornaIdProdutoPorEan1(r["cProd"].ToString());
            itemEntrada.Cod_Produto = r["cProd"].ToString();
            itemEntrada.Valor_Total = ConverterDecimal(r["vProd"].ToString());
            itemEntrada.Qtde = ConverterDecimal(r["qCom"].ToString());
            itemEntrada.Valor_Unitario = ConverterDecimal(r["vUnCom"].ToString());
            itemEntrada.Margem = 0;
            itemEntrada.Valor_Tributacao = ConverterDecimal(r["valorICMS"].ToString());
            itemEntrada.Valor_Ipi = ConverterDecimal(r["valorIPI"].ToString());
            itemEntrada.Valor_Desconto = ConverterDecimal(r["vDesc"].ToString());
            itemEntrada.Perc_Desconto = itemEntrada.Valor_Desconto > 0 ? Math.Round((itemEntrada.Valor_Desconto / itemEntrada.Valor_Total), 2) : 0;
            itemEntrada.Valor_Acrescimo = 0;
            itemEntrada.Perc_Acrescimo = 0;
            itemEntrada.Valor_Frete = ConverterDecimal(r["vFrete"].ToString());
            itemEntrada.Perc_Frete = itemEntrada.Valor_Frete > 0 ? Math.Round(itemEntrada.Valor_Frete / itemEntrada.Valor_Total, 2) : 0;
            itemEntrada.Preco_Vista = ConverterDecimal(r["vUnCom"].ToString());
            itemEntrada.Id_NF = 0; //TODO: IMPLEMENTAR METODO PARA RECUPERAR ID NOTA
            itemEntrada.Preco_Sugestao = 0;
            itemEntrada.Margem_Iva = 0;
            itemEntrada.Reducao_Icms = ConverterDecimal(r["icmsReduzBC"].ToString());
            itemEntrada.Reducao_Icms_ST = ConverterDecimal(r["icmsReduzBCST"].ToString());
            itemEntrada.CFOP = r["cfop"].ToString();
            itemEntrada.Perc_FCP = ConverterDecimal(r["percFcp"].ToString());
            itemEntrada.Valor_Base_FCP = ConverterDecimal(r["baseFcp"].ToString());
            itemEntrada.Valor_FCP = ConverterDecimal(r["valorFcp"].ToString());
            itemEntrada.Perc_FCP_ST = ConverterDecimal(r["percFcpSt"].ToString());
            itemEntrada.Valor_Base_FCP = ConverterDecimal(r["baseFcpSt"].ToString());
            itemEntrada.Valor_FCP_ST = ConverterDecimal(r["valorFcpSt"].ToString());

            return itemEntrada;
        }
        private List<ItemEntrada> RetornaItensNotaEntrada(string numero, string serie, string cnpjEmitente)
        {
            List<ItemEntrada> lstItens = new List<ItemEntrada>();
            try
            {
                foreach (DataRow r in ImportacaoImperium.Dt_Nota_Entrada_Itens.Select($"nroNF = {numero} AND serie = {serie} AND emitCNPJ = {cnpjEmitente}"))
                    lstItens.Add(RetornaItemEntradaPorDataRow(r));

                return lstItens;
            }
            catch (Exception ex)
            {
                Logar("Erro ao recuperar itens da nota de entrada");
                Logar(ex.Message);
                return new List<ItemEntrada>();
            }
        }
        private string RetornaLinhaInserirItemEntrada(ItemEntrada item)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{item.Id_Produto},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Total.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Qtde.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Margem.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Tributacao.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Ipi.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Desconto.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_Desconto.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Acrescimo.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_Acrescimo.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Frete.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_Frete.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Preco_Vista.ToString())},");
            stringBuilder.Append($"{item.Id_NF},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Preco_Sugestao.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Margem_Iva.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Reducao_Icms.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.CFOP.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_FCP.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Base_FCP.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_FCP.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_FCP_ST.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_Base_FCP_ST.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Valor_FCP_ST.ToString())},");
            stringBuilder.Append($"{AjustaStringDecimal(item.Perc_FCP_ST.ToString())}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
        }

        #endregion

        #region VENDA

        private ItemVenda RetornaItemVendaPorDataRow(DataRow r)
        {
            ItemVenda itemVenda = new ItemVenda();
            itemVenda.Id_Produto = RetornaIdProdutoPorEan1(r["codigo_fop"].ToString());
            itemVenda.CodigoEan = ConverterInt64(r["codigo_fop"].ToString());
            itemVenda.Valor = ConverterDecimal(r["valor"].ToString());
            itemVenda.Qtde = ConverterDecimal(r["quantidade"].ToString());
            itemVenda.ECF = ConverterInt32(r["ecf"].ToString());
            itemVenda.Modelo = "2D";
            itemVenda.Desconto = ConverterDecimal(r["descontoItem"].ToString());
            itemVenda.Loja = ConverterInt32(r["loja"].ToString());
            itemVenda.Datamov = ConverterDateTime(r["datamov"].ToString()).ToString("yyyy-MM-ss");
            itemVenda.Hora_Cupom = ConverterDateTime(r["datamov"].ToString()).ToString("HH:mm");
            itemVenda.Custo_Produto = ConverterDecimal(r["custoProduto"].ToString());
            itemVenda.Situacao = r["situacao"].ToString();
            itemVenda.Valor_Unitario = ConverterDecimal(r["valor_unitario"].ToString());

            return itemVenda;
        }
        private void ExecutaComandoItemVenda(List<ItemVenda> lstItemVenda)
        {
            try
            {
                AbrirConexaoMysql();
                string comandoTruncar = @"TRUNCATE itensvenda;";
                MySqlCommand command = new MySqlCommand(comandoTruncar, connecctionMysql);
                command.ExecuteNonQuery();

                string comando = @"INSERT INTO itensvenda (cupom, idProduto, codigoEan, valor, quantidade, ecf, modelo, descontoItem, loja, datamov, custoProduto, hora_cupom, idVendedor, situacao, valor_unitario) VALUES ";

                StringBuilder strBuilderItemVenda = new StringBuilder(comando);
                int cont = 0;

                foreach (ItemVenda item in lstItemVenda)
                {
                    cont++;
                    contadorImportacao.Cont_Venda++;

                    item.Cupom = contadorImportacao.Cont_Venda;

                    strBuilderItemVenda.AppendLine(RetornaLinhaInserirItemVenda(item));

                    if (cont == mConfig.Qtde_Importar)
                    {
                        InsertBanco(strBuilderItemVenda, command);
                        strBuilderItemVenda.Clear();
                        strBuilderItemVenda.Append(comando);
                        cont = 0;
                    }
                }

                if (cont > 0)
                {
                    InsertBanco(strBuilderItemVenda, command);
                    strBuilderItemVenda.Clear();
                    strBuilderItemVenda.Append(comando);
                    cont = 0;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private string RetornaLinhaInserirItemVenda(ItemVenda item)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            stringBuilder.Append($"{item.Cupom},");
            stringBuilder.Append($"{item.Id_Produto},");
            stringBuilder.Append($"'{item.CodigoEan}',");
            stringBuilder.Append($"{item.Valor},");
            stringBuilder.Append($"{item.Qtde},");
            stringBuilder.Append($"{item.ECF},");
            stringBuilder.Append($"'{item.Modelo}',");
            stringBuilder.Append($"{item.Desconto},");
            stringBuilder.Append($"{item.Loja},");
            stringBuilder.Append($"'{item.Datamov}',");
            stringBuilder.Append($"{item.Custo_Produto},");
            stringBuilder.Append($"'{item.Hora_Cupom}',");
            stringBuilder.Append($"{item.Id_Vendedor},");
            stringBuilder.Append($"'{item.Situacao}',");
            stringBuilder.Append($"{item.Valor_Unitario}");
            stringBuilder.Append("),");

            return stringBuilder.ToString();
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
        private void HabilitaCheckBox()
        {
            if (ImportacaoImperium != null)
            {
                chkProdutos.Enabled = ImportacaoImperium.Dt_Produto.Rows.Count > 0;
                chkClientes.Enabled = ImportacaoImperium.Lista_Clientes.Count > 0;
                chkContasReceber.Enabled = chkClientes.Enabled && ImportacaoImperium.Dt_Contas_Receber.Rows.Count > 0;
                chkFornecedores.Enabled = ImportacaoImperium.Lista_Fornecedores.Count > 0;
                chkContasPagar.Enabled = chkFornecedores.Enabled && ImportacaoImperium.Dt_Contas_Pagar.Rows.Count > 0;
                chkVenda.Enabled = chkProdutos.Enabled && ImportacaoImperium.Dt_Vendas.Rows.Count > 0;
            }
        }
        private void InformaContadorRegistrosCarregados()
        {
            lblProdutos.Text = ImportacaoImperium.Dt_Produto != null ? ImportacaoImperium.Dt_Produto.Rows.Count.ToString() : "0";
            lblClientes.Text = ImportacaoImperium.Lista_Clientes != null ? ImportacaoImperium.Lista_Clientes.Count.ToString() : "0";
            lblContaReceber.Text = ImportacaoImperium.Dt_Contas_Receber != null ? ImportacaoImperium.Dt_Contas_Receber.Rows.Count.ToString() : "0";
            lblFornecedores.Text = ImportacaoImperium.Lista_Fornecedores != null ? ImportacaoImperium.Lista_Fornecedores.Count.ToString() : "0";
            lblFamilias.Text = ImportacaoImperium.Dt_Familia != null ? ImportacaoImperium.Dt_Familia.Rows.Count.ToString() : "0";
            lblItensFornecedor.Text = ImportacaoImperium.Dt_Itens_Fornecedor != null ? ImportacaoImperium.Dt_Itens_Fornecedor.Rows.Count.ToString() : "0";
            lblContaPagar.Text = ImportacaoImperium.Dt_Contas_Pagar != null ? ImportacaoImperium.Dt_Contas_Pagar.Rows.Count.ToString() : "0";
            lblGrupo.Text = ImportacaoImperium.Dt_Grupo != null ? ImportacaoImperium.Dt_Grupo.Rows.Count.ToString() : "0";
            lblSubGrupo.Text = ImportacaoImperium.Dt_SubGrupo != null ? ImportacaoImperium.Dt_SubGrupo.Rows.Count.ToString() : "0";
            lblSubGrupo1.Text = ImportacaoImperium.Dt_SubGrupo1 != null ? ImportacaoImperium.Dt_SubGrupo1.Rows.Count.ToString() : "0";
            lblNFEntrada.Text = ImportacaoImperium.Dt_Nota_Entrada != null ? ImportacaoImperium.Dt_Nota_Entrada.Rows.Count.ToString() : "0";
            lblItemVenda.Text = ImportacaoImperium.Dt_Vendas != null ? ImportacaoImperium.Dt_Vendas.Rows.Count.ToString() : "0";
        }
        private void InformaContadorRegistrosImportados()
        {
            lblContProdutos.Text = contadorImportacao.Cont_Produtos.ToString();
            lblContClientes.Text = contadorImportacao.Cont_Clientes.ToString();
            lblContContaReceber.Text = contadorImportacao.Cont_Contas_Receber.ToString();
            lblContFornecedores.Text = contadorImportacao.Cont_Fornecedores.ToString();
            lblContContaPagar.Text = contadorImportacao.Cont_Contas_Pagar.ToString();
            lblContFamilias.Text = contadorImportacao.Cont_Familias.ToString();
            lblContItensFornecedor.Text = contadorImportacao.Cont_ItensFornecedor.ToString();
            lblContGrupos.Text = contadorImportacao.Cont_Grupo.ToString();
            lblContSubGrupo.Text = contadorImportacao.Cont_SubGrupo.ToString();
            lblContSubGrupo1.Text = contadorImportacao.Cont_SubGrupo1.ToString();
            lblContNFEntrada.Text = contadorImportacao.Cont_Nota_Entrada.ToString();
            lblContItemVenda.Text = contadorImportacao.Cont_Venda.ToString();
        }
        private void CriarDiretorioLogs()
        {
            string caminho = Directory.GetCurrentDirectory() + "\\LOGS\\";

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);
        }
        private void Logar(string mensagem)
        {
            try
            {
                CriarDiretorioLogs();

                string nomeArquivo = $"LOG_{DateTime.Now:yyyyMMdd}.log";
                string caminhoArquivo = $"{Directory.GetCurrentDirectory()}\\LOGS\\{nomeArquivo}";

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
        private long RetornaIdProdutoPorEan1(string ean1)
        {
            try
            {
                AbrirConexaoMysql();
                string comando = $@"SELECT idproduto FROM produto WHERE ean1 = {ean1} LIMIT 1";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.Read())
                        return ConverterInt64(rdr["idproduto"].ToString());
                    else
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Logar("Erro ao recuperar idproduto pelo ean1");
                Logar(ex.Message);
                return 0;
            }
            finally { FecharConexaoMysql(); }
        }
        private Dictionary<long, string> CarregaCodigoFiscal()
        {
            Dictionary<long, string> dicCodigoFiscal = new Dictionary<long, string>();
            try
            {
                AbrirConexaoMysql();
                string comando = @"SELECT idCodigoFiscal, CodigoFiscal FROM codigoFiscal;";
                MySqlCommand command = new MySqlCommand(comando, connecctionMysql);

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                        dicCodigoFiscal.Add(ConverterInt64(rdr["idCodigoFiscal"].ToString()), rdr["CodigoFiscal"].ToString());
                }

                return dicCodigoFiscal;
            }
            catch (Exception ex)
            {
                Logar("Erro ao recuperar tabela codigofiscal");
                Logar(ex.Message);
                return dicCodigoFiscal;
            }
            finally { FecharConexaoMysql(); }
        }
        private void ControleTela(bool ativa)
        {
            grpTabelas.Enabled = ativa;
            grpDadosCarregados.Enabled = ativa;
            grpDadosImportados.Enabled = ativa;
            btnCarregar.Enabled = ativa;
            btnImportar.Enabled = ativa;
        }
        private bool VerificaArquivoINI()
        {
            string caminho = Directory.GetCurrentDirectory() + "\\config.ini";
            return File.Exists(caminho);
        }
        private void CriaArquivoIni()
        {
            string caminho = Directory.GetCurrentDirectory() + "\\config.ini";

            File.Create(caminho).Close();

            using (StreamWriter sw = new StreamWriter(caminho))
            {
                sw.WriteLine("[IMPERIUM]");
                sw.WriteLine("servidorMySQL=localhost");
                sw.WriteLine("usuarioMySQL=root");
                sw.WriteLine("senhaMySQL=root");
                sw.WriteLine("bancoMySQL=db_imperium");
                sw.WriteLine("");
                sw.WriteLine("[FOP]");
                sw.WriteLine("[tipoConexao (1 = Autenticacao Windows, 2 = Usuario/Senha)]");
                sw.WriteLine("tipoConexao=1");
                sw.WriteLine("servidorSQLServer=localhost");
                sw.WriteLine("usuarioSQLServer=");
                sw.WriteLine("senhaSQLServer=");
                sw.WriteLine("bancoSQLServer=sc2010");
                sw.WriteLine();
                sw.WriteLine("[PARAMETROS]");
                sw.WriteLine("mConfig.Qtde_Importar=1000");

                sw.Flush();
                sw.Close();
            }
        }
        private void CarregarConfiguracoes()
        {
            string caminho = Directory.GetCurrentDirectory() + "\\config.ini";

            foreach (string linha in File.ReadAllLines(caminho))
            {
                var valores = linha.Split('=');

                switch (valores[0].ToUpper())
                {
                    case "SERVIDORMYSQL":
                        mConfig.Servidor_MySQL = valores[1];
                        break;
                    case "USUARIOMYSQL":
                        mConfig.Usuario_MySQL = valores[1];
                        break;
                    case "SENHAMYSQL":
                        mConfig.Senha_MySQL = valores[1];
                        break;
                    case "BANCOMYSQL":
                        mConfig.Banco_MySQL = valores[1];
                        break;
                    case "TIPOCONEXAO":
                        mConfig.Tipo_Conexao = valores[1];
                        break;
                    case "SERVIDORSQLSERVER":
                        mConfig.Servidor_SQLServer = valores[1];
                        break;
                    case "USUARIOSQLSERVER":
                        mConfig.Usuario_SQLServer = valores[1];
                        break;
                    case "SENHASQLSERVER":
                        mConfig.Senha_SQLServer = valores[1];
                        break;
                    case "BANCOSQLSERVER":
                        mConfig.Banco_SQLServer = valores[1];
                        break;
                    case "mConfig.Qtde_Importar":
                        mConfig.Qtde_Importar = ConverterInt64(valores[1]);
                        break;
                    default:
                        break;
                }
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
