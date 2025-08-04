namespace ImportadorFopImperium
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkLojas = new System.Windows.Forms.CheckBox();
            this.chkFamilias = new System.Windows.Forms.CheckBox();
            this.chkProdutos = new System.Windows.Forms.CheckBox();
            this.chkContasPagar = new System.Windows.Forms.CheckBox();
            this.chkMovCaixa = new System.Windows.Forms.CheckBox();
            this.chkFornecedores = new System.Windows.Forms.CheckBox();
            this.chkClientes = new System.Windows.Forms.CheckBox();
            this.chkNFEntrada = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.chkContasReceber = new System.Windows.Forms.CheckBox();
            this.chkSecoes = new System.Windows.Forms.CheckBox();
            this.chkItensFornecedor = new System.Windows.Forms.CheckBox();
            this.barraProgresso = new System.Windows.Forms.ProgressBar();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnCarregar = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblUsuarios = new System.Windows.Forms.Label();
            this.lblLojas = new System.Windows.Forms.Label();
            this.lblMapa = new System.Windows.Forms.Label();
            this.lblContaPagar = new System.Windows.Forms.Label();
            this.lblMovCaixa = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblNFEntrada = new System.Windows.Forms.Label();
            this.lblProdutos = new System.Windows.Forms.Label();
            this.lblContaReceber = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblFornecedores = new System.Windows.Forms.Label();
            this.lblFamilias = new System.Windows.Forms.Label();
            this.lblClientes = new System.Windows.Forms.Label();
            this.lblSecoes = new System.Windows.Forms.Label();
            this.lblItensFornecedor = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblContUsuario = new System.Windows.Forms.Label();
            this.lblContLojas = new System.Windows.Forms.Label();
            this.lblContMapa = new System.Windows.Forms.Label();
            this.lblContContaPagar = new System.Windows.Forms.Label();
            this.lblContMovCaixa = new System.Windows.Forms.Label();
            this.lblContNFEntrada = new System.Windows.Forms.Label();
            this.lblContContaReceber = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblContProdutos = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblContFamilias = new System.Windows.Forms.Label();
            this.lblContFornecedores = new System.Windows.Forms.Label();
            this.lblContClientes = new System.Windows.Forms.Label();
            this.lblContSecoes = new System.Windows.Forms.Label();
            this.lblContItensFornecedor = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Controls.Add(this.label7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 46);
            this.panel1.TabIndex = 46;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Window;
            this.label7.Location = new System.Drawing.Point(3, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(384, 25);
            this.label7.TabIndex = 0;
            this.label7.Text = "Importador FOP para Imperium";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkLojas);
            this.groupBox2.Controls.Add(this.chkFamilias);
            this.groupBox2.Controls.Add(this.chkProdutos);
            this.groupBox2.Controls.Add(this.chkContasPagar);
            this.groupBox2.Controls.Add(this.chkMovCaixa);
            this.groupBox2.Controls.Add(this.chkFornecedores);
            this.groupBox2.Controls.Add(this.chkClientes);
            this.groupBox2.Controls.Add(this.chkNFEntrada);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.chkContasReceber);
            this.groupBox2.Controls.Add(this.chkSecoes);
            this.groupBox2.Controls.Add(this.chkItensFornecedor);
            this.groupBox2.Location = new System.Drawing.Point(8, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 378);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            // 
            // chkLojas
            // 
            this.chkLojas.AutoSize = true;
            this.chkLojas.Enabled = false;
            this.chkLojas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLojas.Location = new System.Drawing.Point(6, 50);
            this.chkLojas.Name = "chkLojas";
            this.chkLojas.Size = new System.Drawing.Size(56, 17);
            this.chkLojas.TabIndex = 34;
            this.chkLojas.Text = "Lojas";
            this.chkLojas.UseVisualStyleBackColor = true;
            // 
            // chkFamilias
            // 
            this.chkFamilias.AutoSize = true;
            this.chkFamilias.Enabled = false;
            this.chkFamilias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFamilias.Location = new System.Drawing.Point(22, 289);
            this.chkFamilias.Name = "chkFamilias";
            this.chkFamilias.Size = new System.Drawing.Size(71, 17);
            this.chkFamilias.TabIndex = 25;
            this.chkFamilias.Text = "Familias";
            this.chkFamilias.UseVisualStyleBackColor = true;
            // 
            // chkProdutos
            // 
            this.chkProdutos.AutoSize = true;
            this.chkProdutos.Enabled = false;
            this.chkProdutos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkProdutos.Location = new System.Drawing.Point(6, 182);
            this.chkProdutos.Name = "chkProdutos";
            this.chkProdutos.Size = new System.Drawing.Size(76, 17);
            this.chkProdutos.TabIndex = 26;
            this.chkProdutos.Text = "Produtos";
            this.chkProdutos.UseVisualStyleBackColor = true;
            this.chkProdutos.CheckedChanged += new System.EventHandler(this.chkProdutos_CheckedChanged);
            // 
            // chkContasPagar
            // 
            this.chkContasPagar.AutoSize = true;
            this.chkContasPagar.Enabled = false;
            this.chkContasPagar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkContasPagar.Location = new System.Drawing.Point(22, 151);
            this.chkContasPagar.Name = "chkContasPagar";
            this.chkContasPagar.Size = new System.Drawing.Size(114, 17);
            this.chkContasPagar.TabIndex = 30;
            this.chkContasPagar.Text = "Contas a Pagar";
            this.chkContasPagar.UseVisualStyleBackColor = true;
            // 
            // chkMovCaixa
            // 
            this.chkMovCaixa.AutoSize = true;
            this.chkMovCaixa.Enabled = false;
            this.chkMovCaixa.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMovCaixa.Location = new System.Drawing.Point(22, 269);
            this.chkMovCaixa.Name = "chkMovCaixa";
            this.chkMovCaixa.Size = new System.Drawing.Size(86, 17);
            this.chkMovCaixa.TabIndex = 32;
            this.chkMovCaixa.Text = "Mov Caixa";
            this.chkMovCaixa.UseVisualStyleBackColor = true;
            // 
            // chkFornecedores
            // 
            this.chkFornecedores.AutoSize = true;
            this.chkFornecedores.Enabled = false;
            this.chkFornecedores.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFornecedores.Location = new System.Drawing.Point(6, 131);
            this.chkFornecedores.Name = "chkFornecedores";
            this.chkFornecedores.Size = new System.Drawing.Size(103, 17);
            this.chkFornecedores.TabIndex = 23;
            this.chkFornecedores.Text = "Fornecedores";
            this.chkFornecedores.UseVisualStyleBackColor = true;
            this.chkFornecedores.CheckedChanged += new System.EventHandler(this.chkFornecedores_CheckedChanged);
            // 
            // chkClientes
            // 
            this.chkClientes.AutoSize = true;
            this.chkClientes.Enabled = false;
            this.chkClientes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkClientes.Location = new System.Drawing.Point(6, 73);
            this.chkClientes.Name = "chkClientes";
            this.chkClientes.Size = new System.Drawing.Size(72, 17);
            this.chkClientes.TabIndex = 22;
            this.chkClientes.Text = "Clientes";
            this.chkClientes.UseVisualStyleBackColor = true;
            this.chkClientes.CheckedChanged += new System.EventHandler(this.chkClientes_CheckedChanged);
            // 
            // chkNFEntrada
            // 
            this.chkNFEntrada.AutoSize = true;
            this.chkNFEntrada.Enabled = false;
            this.chkNFEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNFEntrada.Location = new System.Drawing.Point(22, 249);
            this.chkNFEntrada.Name = "chkNFEntrada";
            this.chkNFEntrada.Size = new System.Drawing.Size(88, 17);
            this.chkNFEntrada.TabIndex = 31;
            this.chkNFEntrada.Text = "NF Entrada";
            this.chkNFEntrada.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(9, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 13);
            this.label15.TabIndex = 15;
            this.label15.Text = "TABELAS";
            // 
            // chkContasReceber
            // 
            this.chkContasReceber.AutoSize = true;
            this.chkContasReceber.Enabled = false;
            this.chkContasReceber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkContasReceber.Location = new System.Drawing.Point(18, 91);
            this.chkContasReceber.Name = "chkContasReceber";
            this.chkContasReceber.Size = new System.Drawing.Size(128, 17);
            this.chkContasReceber.TabIndex = 29;
            this.chkContasReceber.Text = "Contas a Receber";
            this.chkContasReceber.UseVisualStyleBackColor = true;
            // 
            // chkSecoes
            // 
            this.chkSecoes.AutoSize = true;
            this.chkSecoes.Enabled = false;
            this.chkSecoes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSecoes.Location = new System.Drawing.Point(22, 205);
            this.chkSecoes.Name = "chkSecoes";
            this.chkSecoes.Size = new System.Drawing.Size(67, 17);
            this.chkSecoes.TabIndex = 24;
            this.chkSecoes.Text = "Seções";
            this.chkSecoes.UseVisualStyleBackColor = true;
            // 
            // chkItensFornecedor
            // 
            this.chkItensFornecedor.AutoSize = true;
            this.chkItensFornecedor.Enabled = false;
            this.chkItensFornecedor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkItensFornecedor.Location = new System.Drawing.Point(22, 228);
            this.chkItensFornecedor.Name = "chkItensFornecedor";
            this.chkItensFornecedor.Size = new System.Drawing.Size(123, 17);
            this.chkItensFornecedor.TabIndex = 28;
            this.chkItensFornecedor.Text = "Itens Fornecedor";
            this.chkItensFornecedor.UseVisualStyleBackColor = true;
            // 
            // barraProgresso
            // 
            this.barraProgresso.Location = new System.Drawing.Point(8, 450);
            this.barraProgresso.Name = "barraProgresso";
            this.barraProgresso.Size = new System.Drawing.Size(780, 23);
            this.barraProgresso.TabIndex = 50;
            // 
            // btnImportar
            // 
            this.btnImportar.Enabled = false;
            this.btnImportar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportar.Image = ((System.Drawing.Image)(resources.GetObject("btnImportar.Image")));
            this.btnImportar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportar.Location = new System.Drawing.Point(671, 396);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(117, 34);
            this.btnImportar.TabIndex = 49;
            this.btnImportar.Text = "Importar\r\n";
            this.btnImportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportar.UseVisualStyleBackColor = true;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // btnCarregar
            // 
            this.btnCarregar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCarregar.Image = ((System.Drawing.Image)(resources.GetObject("btnCarregar.Image")));
            this.btnCarregar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCarregar.Location = new System.Drawing.Point(671, 356);
            this.btnCarregar.Name = "btnCarregar";
            this.btnCarregar.Size = new System.Drawing.Size(117, 34);
            this.btnCarregar.TabIndex = 48;
            this.btnCarregar.Text = "Carregar";
            this.btnCarregar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCarregar.UseVisualStyleBackColor = true;
            this.btnCarregar.Click += new System.EventHandler(this.btnCarregar_Click);
            // 
            // btnFechar
            // 
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btnFechar.Image = ((System.Drawing.Image)(resources.GetObject("btnFechar.Image")));
            this.btnFechar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFechar.Location = new System.Drawing.Point(671, 52);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(117, 30);
            this.btnFechar.TabIndex = 67;
            this.btnFechar.Text = "&Fechar";
            this.btnFechar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblUsuarios);
            this.groupBox3.Controls.Add(this.lblLojas);
            this.groupBox3.Controls.Add(this.lblMapa);
            this.groupBox3.Controls.Add(this.lblContaPagar);
            this.groupBox3.Controls.Add(this.lblMovCaixa);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.lblNFEntrada);
            this.groupBox3.Controls.Add(this.lblProdutos);
            this.groupBox3.Controls.Add(this.lblContaReceber);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.lblFornecedores);
            this.groupBox3.Controls.Add(this.lblFamilias);
            this.groupBox3.Controls.Add(this.lblClientes);
            this.groupBox3.Controls.Add(this.lblSecoes);
            this.groupBox3.Controls.Add(this.lblItensFornecedor);
            this.groupBox3.Location = new System.Drawing.Point(197, 52);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(106, 378);
            this.groupBox3.TabIndex = 68;
            this.groupBox3.TabStop = false;
            // 
            // lblUsuarios
            // 
            this.lblUsuarios.AutoSize = true;
            this.lblUsuarios.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuarios.Location = new System.Drawing.Point(6, 320);
            this.lblUsuarios.Name = "lblUsuarios";
            this.lblUsuarios.Size = new System.Drawing.Size(11, 13);
            this.lblUsuarios.TabIndex = 26;
            this.lblUsuarios.Text = ".";
            // 
            // lblLojas
            // 
            this.lblLojas.AutoSize = true;
            this.lblLojas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLojas.Location = new System.Drawing.Point(7, 50);
            this.lblLojas.Name = "lblLojas";
            this.lblLojas.Size = new System.Drawing.Size(11, 13);
            this.lblLojas.TabIndex = 25;
            this.lblLojas.Text = ".";
            // 
            // lblMapa
            // 
            this.lblMapa.AutoSize = true;
            this.lblMapa.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapa.Location = new System.Drawing.Point(6, 310);
            this.lblMapa.Name = "lblMapa";
            this.lblMapa.Size = new System.Drawing.Size(11, 13);
            this.lblMapa.TabIndex = 24;
            this.lblMapa.Text = ".";
            // 
            // lblContaPagar
            // 
            this.lblContaPagar.AutoSize = true;
            this.lblContaPagar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContaPagar.Location = new System.Drawing.Point(7, 152);
            this.lblContaPagar.Name = "lblContaPagar";
            this.lblContaPagar.Size = new System.Drawing.Size(11, 13);
            this.lblContaPagar.TabIndex = 21;
            this.lblContaPagar.Text = ".";
            // 
            // lblMovCaixa
            // 
            this.lblMovCaixa.AutoSize = true;
            this.lblMovCaixa.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMovCaixa.Location = new System.Drawing.Point(6, 270);
            this.lblMovCaixa.Name = "lblMovCaixa";
            this.lblMovCaixa.Size = new System.Drawing.Size(11, 13);
            this.lblMovCaixa.TabIndex = 23;
            this.lblMovCaixa.Text = ".";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(7, 31);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "Carregados";
            // 
            // lblNFEntrada
            // 
            this.lblNFEntrada.AutoSize = true;
            this.lblNFEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNFEntrada.Location = new System.Drawing.Point(6, 249);
            this.lblNFEntrada.Name = "lblNFEntrada";
            this.lblNFEntrada.Size = new System.Drawing.Size(11, 13);
            this.lblNFEntrada.TabIndex = 22;
            this.lblNFEntrada.Text = ".";
            // 
            // lblProdutos
            // 
            this.lblProdutos.AutoSize = true;
            this.lblProdutos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProdutos.Location = new System.Drawing.Point(7, 183);
            this.lblProdutos.Name = "lblProdutos";
            this.lblProdutos.Size = new System.Drawing.Size(11, 13);
            this.lblProdutos.TabIndex = 10;
            this.lblProdutos.Text = ".";
            // 
            // lblContaReceber
            // 
            this.lblContaReceber.AutoSize = true;
            this.lblContaReceber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContaReceber.Location = new System.Drawing.Point(7, 91);
            this.lblContaReceber.Name = "lblContaReceber";
            this.lblContaReceber.Size = new System.Drawing.Size(11, 13);
            this.lblContaReceber.TabIndex = 19;
            this.lblContaReceber.Text = ".";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(7, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(47, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Dados";
            // 
            // lblFornecedores
            // 
            this.lblFornecedores.AutoSize = true;
            this.lblFornecedores.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFornecedores.Location = new System.Drawing.Point(7, 132);
            this.lblFornecedores.Name = "lblFornecedores";
            this.lblFornecedores.Size = new System.Drawing.Size(11, 13);
            this.lblFornecedores.TabIndex = 7;
            this.lblFornecedores.Text = ".";
            // 
            // lblFamilias
            // 
            this.lblFamilias.AutoSize = true;
            this.lblFamilias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFamilias.Location = new System.Drawing.Point(6, 289);
            this.lblFamilias.Name = "lblFamilias";
            this.lblFamilias.Size = new System.Drawing.Size(11, 13);
            this.lblFamilias.TabIndex = 9;
            this.lblFamilias.Text = ".";
            // 
            // lblClientes
            // 
            this.lblClientes.AutoSize = true;
            this.lblClientes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientes.Location = new System.Drawing.Point(7, 74);
            this.lblClientes.Name = "lblClientes";
            this.lblClientes.Size = new System.Drawing.Size(11, 13);
            this.lblClientes.TabIndex = 6;
            this.lblClientes.Text = ".";
            // 
            // lblSecoes
            // 
            this.lblSecoes.AutoSize = true;
            this.lblSecoes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecoes.Location = new System.Drawing.Point(7, 204);
            this.lblSecoes.Name = "lblSecoes";
            this.lblSecoes.Size = new System.Drawing.Size(11, 13);
            this.lblSecoes.TabIndex = 8;
            this.lblSecoes.Text = ".";
            // 
            // lblItensFornecedor
            // 
            this.lblItensFornecedor.AutoSize = true;
            this.lblItensFornecedor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItensFornecedor.Location = new System.Drawing.Point(6, 228);
            this.lblItensFornecedor.Name = "lblItensFornecedor";
            this.lblItensFornecedor.Size = new System.Drawing.Size(11, 13);
            this.lblItensFornecedor.TabIndex = 18;
            this.lblItensFornecedor.Text = ".";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblContUsuario);
            this.groupBox4.Controls.Add(this.lblContLojas);
            this.groupBox4.Controls.Add(this.lblContMapa);
            this.groupBox4.Controls.Add(this.lblContContaPagar);
            this.groupBox4.Controls.Add(this.lblContMovCaixa);
            this.groupBox4.Controls.Add(this.lblContNFEntrada);
            this.groupBox4.Controls.Add(this.lblContContaReceber);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.lblContProdutos);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.lblContFamilias);
            this.groupBox4.Controls.Add(this.lblContFornecedores);
            this.groupBox4.Controls.Add(this.lblContClientes);
            this.groupBox4.Controls.Add(this.lblContSecoes);
            this.groupBox4.Controls.Add(this.lblContItensFornecedor);
            this.groupBox4.Location = new System.Drawing.Point(309, 52);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(106, 378);
            this.groupBox4.TabIndex = 69;
            this.groupBox4.TabStop = false;
            // 
            // lblContUsuario
            // 
            this.lblContUsuario.AutoSize = true;
            this.lblContUsuario.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContUsuario.Location = new System.Drawing.Point(7, 331);
            this.lblContUsuario.Name = "lblContUsuario";
            this.lblContUsuario.Size = new System.Drawing.Size(11, 13);
            this.lblContUsuario.TabIndex = 27;
            this.lblContUsuario.Text = ".";
            // 
            // lblContLojas
            // 
            this.lblContLojas.AutoSize = true;
            this.lblContLojas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContLojas.Location = new System.Drawing.Point(7, 50);
            this.lblContLojas.Name = "lblContLojas";
            this.lblContLojas.Size = new System.Drawing.Size(11, 13);
            this.lblContLojas.TabIndex = 26;
            this.lblContLojas.Text = ".";
            // 
            // lblContMapa
            // 
            this.lblContMapa.AutoSize = true;
            this.lblContMapa.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContMapa.Location = new System.Drawing.Point(7, 310);
            this.lblContMapa.Name = "lblContMapa";
            this.lblContMapa.Size = new System.Drawing.Size(11, 13);
            this.lblContMapa.TabIndex = 25;
            this.lblContMapa.Text = ".";
            // 
            // lblContContaPagar
            // 
            this.lblContContaPagar.AutoSize = true;
            this.lblContContaPagar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContContaPagar.Location = new System.Drawing.Point(7, 153);
            this.lblContContaPagar.Name = "lblContContaPagar";
            this.lblContContaPagar.Size = new System.Drawing.Size(11, 13);
            this.lblContContaPagar.TabIndex = 22;
            this.lblContContaPagar.Text = ".";
            // 
            // lblContMovCaixa
            // 
            this.lblContMovCaixa.AutoSize = true;
            this.lblContMovCaixa.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContMovCaixa.Location = new System.Drawing.Point(7, 270);
            this.lblContMovCaixa.Name = "lblContMovCaixa";
            this.lblContMovCaixa.Size = new System.Drawing.Size(11, 13);
            this.lblContMovCaixa.TabIndex = 24;
            this.lblContMovCaixa.Text = ".";
            // 
            // lblContNFEntrada
            // 
            this.lblContNFEntrada.AutoSize = true;
            this.lblContNFEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContNFEntrada.Location = new System.Drawing.Point(7, 249);
            this.lblContNFEntrada.Name = "lblContNFEntrada";
            this.lblContNFEntrada.Size = new System.Drawing.Size(11, 13);
            this.lblContNFEntrada.TabIndex = 23;
            this.lblContNFEntrada.Text = ".";
            // 
            // lblContContaReceber
            // 
            this.lblContContaReceber.AutoSize = true;
            this.lblContContaReceber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContContaReceber.Location = new System.Drawing.Point(7, 92);
            this.lblContContaReceber.Name = "lblContContaReceber";
            this.lblContContaReceber.Size = new System.Drawing.Size(11, 13);
            this.lblContContaReceber.TabIndex = 20;
            this.lblContContaReceber.Text = ".";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(7, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(83, 13);
            this.label19.TabIndex = 18;
            this.label19.Text = "Importados";
            // 
            // lblContProdutos
            // 
            this.lblContProdutos.AutoSize = true;
            this.lblContProdutos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContProdutos.Location = new System.Drawing.Point(7, 179);
            this.lblContProdutos.Name = "lblContProdutos";
            this.lblContProdutos.Size = new System.Drawing.Size(11, 13);
            this.lblContProdutos.TabIndex = 10;
            this.lblContProdutos.Text = ".";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(7, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(47, 13);
            this.label17.TabIndex = 17;
            this.label17.Text = "Dados";
            // 
            // lblContFamilias
            // 
            this.lblContFamilias.AutoSize = true;
            this.lblContFamilias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContFamilias.Location = new System.Drawing.Point(7, 290);
            this.lblContFamilias.Name = "lblContFamilias";
            this.lblContFamilias.Size = new System.Drawing.Size(11, 13);
            this.lblContFamilias.TabIndex = 9;
            this.lblContFamilias.Text = ".";
            // 
            // lblContFornecedores
            // 
            this.lblContFornecedores.AutoSize = true;
            this.lblContFornecedores.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContFornecedores.Location = new System.Drawing.Point(7, 133);
            this.lblContFornecedores.Name = "lblContFornecedores";
            this.lblContFornecedores.Size = new System.Drawing.Size(11, 13);
            this.lblContFornecedores.TabIndex = 7;
            this.lblContFornecedores.Text = ".";
            // 
            // lblContClientes
            // 
            this.lblContClientes.AutoSize = true;
            this.lblContClientes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContClientes.Location = new System.Drawing.Point(7, 71);
            this.lblContClientes.Name = "lblContClientes";
            this.lblContClientes.Size = new System.Drawing.Size(11, 13);
            this.lblContClientes.TabIndex = 6;
            this.lblContClientes.Text = ".";
            // 
            // lblContSecoes
            // 
            this.lblContSecoes.AutoSize = true;
            this.lblContSecoes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContSecoes.Location = new System.Drawing.Point(7, 200);
            this.lblContSecoes.Name = "lblContSecoes";
            this.lblContSecoes.Size = new System.Drawing.Size(11, 13);
            this.lblContSecoes.TabIndex = 8;
            this.lblContSecoes.Text = ".";
            // 
            // lblContItensFornecedor
            // 
            this.lblContItensFornecedor.AutoSize = true;
            this.lblContItensFornecedor.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContItensFornecedor.Location = new System.Drawing.Point(7, 228);
            this.lblContItensFornecedor.Name = "lblContItensFornecedor";
            this.lblContItensFornecedor.Size = new System.Drawing.Size(11, 13);
            this.lblContItensFornecedor.TabIndex = 19;
            this.lblContItensFornecedor.Text = ".";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 485);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnFechar);
            this.Controls.Add(this.barraProgresso);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.btnCarregar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Name = "frmPrincipal";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkLojas;
        private System.Windows.Forms.CheckBox chkFamilias;
        private System.Windows.Forms.CheckBox chkProdutos;
        private System.Windows.Forms.CheckBox chkContasPagar;
        private System.Windows.Forms.CheckBox chkMovCaixa;
        private System.Windows.Forms.CheckBox chkFornecedores;
        private System.Windows.Forms.CheckBox chkClientes;
        private System.Windows.Forms.CheckBox chkNFEntrada;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chkContasReceber;
        private System.Windows.Forms.CheckBox chkSecoes;
        private System.Windows.Forms.CheckBox chkItensFornecedor;
        private System.Windows.Forms.ProgressBar barraProgresso;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.Button btnCarregar;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblUsuarios;
        private System.Windows.Forms.Label lblLojas;
        private System.Windows.Forms.Label lblMapa;
        private System.Windows.Forms.Label lblContaPagar;
        private System.Windows.Forms.Label lblMovCaixa;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblNFEntrada;
        private System.Windows.Forms.Label lblProdutos;
        private System.Windows.Forms.Label lblContaReceber;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblFornecedores;
        private System.Windows.Forms.Label lblFamilias;
        private System.Windows.Forms.Label lblClientes;
        private System.Windows.Forms.Label lblSecoes;
        private System.Windows.Forms.Label lblItensFornecedor;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblContUsuario;
        private System.Windows.Forms.Label lblContLojas;
        private System.Windows.Forms.Label lblContMapa;
        private System.Windows.Forms.Label lblContContaPagar;
        private System.Windows.Forms.Label lblContMovCaixa;
        private System.Windows.Forms.Label lblContNFEntrada;
        private System.Windows.Forms.Label lblContContaReceber;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblContProdutos;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblContFamilias;
        private System.Windows.Forms.Label lblContFornecedores;
        private System.Windows.Forms.Label lblContClientes;
        private System.Windows.Forms.Label lblContSecoes;
        private System.Windows.Forms.Label lblContItensFornecedor;
    }
}

