using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class MainForm : Form
    {
        private readonly RdSistemasAuthResult auth;
        private readonly string deviceId;

        private readonly List<OverlayForm> overlays = new();

        private string? remoteDeviceId;
        private string imageFile = "";

        private System.Windows.Forms.Timer? timer;

        private CheckBox pc1 = null!;
        private CheckBox pc2 = null!;

        private RadioButton rbBlack = null!;
        private RadioButton rbImage = null!;
        private RadioButton rbText = null!;

        private TextBox txtMessage = null!;

        private Label lblStatus = null!;
        private Label lblPc1Status = null!;
        private Label lblPc2Status = null!;
        private Label lblImage = null!;
        private Label lblEmpresa = null!;
        private Label lblDevice = null!;

        private Panel cardPc1 = null!;
        private Panel cardPc2 = null!;

        private Button btnExecute = null!;
        private Button btnRestore = null!;
        private Button btnImage = null!;

        private readonly Color Background =
            Color.FromArgb(7, 11, 18);

        private readonly Color Header =
            Color.FromArgb(10, 17, 27);

        private readonly Color Card =
            Color.FromArgb(14, 22, 33);

        private readonly Color CardDark =
            Color.FromArgb(9, 16, 25);

        private readonly Color Border =
            Color.FromArgb(35, 50, 67);

        private readonly Color Blue =
            Color.FromArgb(0, 145, 245);

        private readonly Color Green =
            Color.FromArgb(45, 210, 120);

        private readonly Color Red =
            Color.FromArgb(205, 45, 55);

        public MainForm(RdSistemasAuthResult result)
        {
            auth = result;
            deviceId = DeviceIdentity.GetDeviceId();

            CriarJanela();
            CriarInterface();
            IniciarComunicacao();
        }

        // =========================================================
        // JANELA
        // =========================================================

        private void CriarJanela()
        {
            Text = "RD Screen Guard";

            StartPosition =
                FormStartPosition.CenterScreen;

            ClientSize =
                new Size(1180, 760);

            MinimumSize =
                new Size(1050, 680);

            BackColor =
                Background;

            Font =
                new Font(
                    "Segoe UI",
                    9F);

            DoubleBuffered = true;

            FormClosed += (s, e) =>
            {
                timer?.Stop();
                RestaurarLocal();
            };
        }

        // =========================================================
        // INTERFACE
        // =========================================================

        private void CriarInterface()
        {
            CriarCabecalho();

            TableLayoutPanel principal =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Background,
                    Padding =
                        new Padding(
                            24,
                            18,
                            24,
                            18),
                    ColumnCount = 2,
                    RowCount = 2
                };

            principal.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    43F));

            principal.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    57F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    330F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(principal);

            // =====================================================
            // COMPUTADORES
            // =====================================================

            Panel computadores =
                CriarCard(
                    "COMPUTADORES",
                    "Selecione o computador que receberá a ação.");

            principal.Controls.Add(
                computadores,
                0,
                0);

            CriarComputadores(
                computadores);

            // =====================================================
            // AÇÃO
            // =====================================================

            Panel acao =
                CriarCard(
                    "AÇÃO DE TELA",
                    "Escolha o que será exibido no computador.");

            principal.Controls.Add(
                acao,
                1,
                0);

            CriarAcao(
                acao);

            // =====================================================
            // CONTROLE
            // =====================================================

            Panel controle =
                CriarCard(
                    "CONTROLE",
                    "Execute ou restaure a tela do computador selecionado.");

            principal.Controls.Add(
                controle,
                0,
                1);

            principal.SetColumnSpan(
                controle,
                2);

            CriarControle(
                controle);
        }

        // =========================================================
        // CARD PRINCIPAL
        // =========================================================

        private Panel CriarCard(
            string titulo,
            string descricao)
        {
            Panel panel =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Card,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };

            panel.Paint +=
                (s, e) =>
                {
                    using Pen pen =
                        new Pen(Border);

                    e.Graphics.DrawRectangle(
                        pen,
                        0,
                        0,
                        panel.Width - 1,
                        panel.Height - 1);
                };

            Label title =
                new Label
                {
                    Text = titulo,

                    ForeColor =
                        Color.FromArgb(
                            95,
                            185,
                            255),

                    Font =
                        new Font(
                            "Segoe UI",
                            11F,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            18,
                            16)
                };

            panel.Controls.Add(
                title);

            Label description =
                new Label
                {
                    Text = descricao,

                    ForeColor =
                        Color.FromArgb(
                            120,
                            135,
                            150),

                    Font =
                        new Font(
                            "Segoe UI",
                            8F),

                    AutoSize = true,

                    Location =
                        new Point(
                            18,
                            40)
                };

            panel.Controls.Add(
                description);

            return panel;
        }

        // =========================================================
        // CABEÇALHO
        // =========================================================

        private void CriarCabecalho()
        {
            Panel header =
                new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 100,
                    BackColor = Header
                };

            Controls.Add(header);

            Label logo =
                new Label
                {
                    Text = "RD",
                    ForeColor = Blue,

                    Font =
                        new Font(
                            "Segoe UI",
                            27,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            28,
                            20)
                };

            header.Controls.Add(
                logo);

            Label title =
                new Label
                {
                    Text = "SCREEN GUARD",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            25,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            87,
                            19)
                };

            header.Controls.Add(
                title);

            Label subtitle =
                new Label
                {
                    Text =
                        "CONTROLE DE TELAS REMOTO",

                    ForeColor =
                        Color.FromArgb(
                            100,
                            180,
                            235),

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            90,
                            61)
                };

            header.Controls.Add(
                subtitle);

            Panel status =
                new Panel
                {
                    Size =
                        new Size(
                            185,
                            46),

                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right,

                    BackColor =
                        Color.FromArgb(
                            15,
                            27,
                            39),

                    Location =
                        new Point(
                            ClientSize.Width - 215,
                            27)
                };

            header.Controls.Add(
                status);

            lblStatus =
                new Label
                {
                    Text =
                        "● AGUARDANDO SERVIDOR",

                    ForeColor =
                        Color.Gold,

                    Font =
                        new Font(
                            "Segoe UI",
                            8.5F,
                            FontStyle.Bold),

                    AutoSize = false,

                    TextAlign =
                        ContentAlignment.MiddleCenter,

                    Dock = DockStyle.Fill
                };

            status.Controls.Add(
                lblStatus);

            header.Resize +=
                (s, e) =>
                {
                    status.Left =
                        header.ClientSize.Width -
                        status.Width -
                        25;
                };
        }

        // =========================================================
        // COMPUTADORES
        // =========================================================

        private void CriarComputadores(
            Panel container)
        {
            TableLayoutPanel tabela =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 1,
                    RowCount = 4,

                    Padding =
                        new Padding(
                            18,
                            65,
                            18,
                            15)
                };

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    78));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    78));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    28));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    25));

            container.Controls.Add(
                tabela);

            // =====================================================
            // COMPUTADOR 01
            // =====================================================

            cardPc1 =
                CriarCardComputador();

            tabela.Controls.Add(
                cardPc1,
                0,
                0);

            pc1 =
                CriarCheckBox(
                    "COMPUTADOR 01");

            cardPc1.Controls.Add(
                pc1);

            pc1.Location =
                new Point(
                    15,
                    12);

            lblPc1Status =
                CriarStatusLabel(
                    "● Verificando...",
                    Color.Gold);

            cardPc1.Controls.Add(
                lblPc1Status);

            lblPc1Status.Location =
                new Point(
                    40,
                    42);

            Label icon1 =
                CriarIcone(
                    "01");

            cardPc1.Controls.Add(
                icon1);

            icon1.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            icon1.Location =
                new Point(
                    cardPc1.Width - 52,
                    23);

            // =====================================================
            // COMPUTADOR 02
            // =====================================================

            cardPc2 =
                CriarCardComputador();

            tabela.Controls.Add(
                cardPc2,
                0,
                1);

            pc2 =
                CriarCheckBox(
                    "COMPUTADOR 02");

            cardPc2.Controls.Add(
                pc2);

            pc2.Location =
                new Point(
                    15,
                    12);

            lblPc2Status =
                CriarStatusLabel(
                    "● Aguardando conexão",
                    Color.Gray);

            cardPc2.Controls.Add(
                lblPc2Status);

            lblPc2Status.Location =
                new Point(
                    40,
                    42);

            Label icon2 =
                CriarIcone(
                    "02");

            cardPc2.Controls.Add(
                icon2);

            icon2.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            icon2.Location =
                new Point(
                    cardPc2.Width - 52,
                    23);

            // =====================================================
            // EMPRESA
            // =====================================================

            lblEmpresa =
                new Label
                {
                    Text =
                        "Empresa: " +
                        (auth.Empresa ?? "-"),

                    ForeColor =
                        Color.FromArgb(
                            150,
                            165,
                            180),

                    AutoSize = true
                };

            tabela.Controls.Add(
                lblEmpresa,
                0,
                2);

            // =====================================================
            // ID
            // =====================================================

            lblDevice =
                new Label
                {
                    Text =
                        "ID: " +
                        deviceId,

                    ForeColor =
                        Color.FromArgb(
                            100,
                            115,
                            130),

                    AutoSize = true
                };

            tabela.Controls.Add(
                lblDevice,
                0,
                3);

            // =====================================================
            // SELEÇÃO PC 01
            // =====================================================

            pc1.CheckedChanged +=
                (s, e) =>
                {
                    if (!pc1.Checked)
                        return;

                    pc2.Checked = false;

                    Destacar(
                        cardPc1);

                    RemoverDestaque(
                        cardPc2);
                };

            // =====================================================
            // SELEÇÃO PC 02
            // =====================================================

            pc2.CheckedChanged +=
                (s, e) =>
                {
                    if (!pc2.Checked)
                        return;

                    pc1.Checked = false;

                    Destacar(
                        cardPc2);

                    RemoverDestaque(
                        cardPc1);
                };

            pc1.Checked = true;
        }

        private Panel CriarCardComputador()
        {
            Panel panel =
                new Panel
                {
                    Dock = DockStyle.Fill,

                    BackColor =
                        CardDark,

                    Margin =
                        new Padding(
                            0,
                            4,
                            0,
                            4)
                };

            panel.Paint +=
                (s, e) =>
                {
                    using Pen pen =
                        new Pen(Border);

                    e.Graphics.DrawRectangle(
                        pen,
                        0,
                        0,
                        panel.Width - 1,
                        panel.Height - 1);
                };

            return panel;
        }

        private CheckBox CriarCheckBox(
            string text)
        {
            return new CheckBox
            {
                Text = text,

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold),

                AutoSize = true
            };
        }

        private Label CriarStatusLabel(
            string text,
            Color color)
        {
            return new Label
            {
                Text = text,

                ForeColor = color,

                Font =
                    new Font(
                        "Segoe UI",
                        8.5F,
                        FontStyle.Bold),

                AutoSize = true
            };
        }

        private Label CriarIcone(
            string numero)
        {
            return new Label
            {
                Text = numero,

                ForeColor = Blue,

                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold),

                AutoSize = true
            };
        }

        // =========================================================
        // AÇÃO
        // =========================================================

        private void CriarAcao(
            Panel container)
        {
            TableLayoutPanel tabela =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 2,
                    RowCount = 4,

                    Padding =
                        new Padding(
                            18,
                            65,
                            18,
                            15)
                };

            tabela.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    45F));

            tabela.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    55F));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    45));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    45));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    45));

            tabela.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100));

            container.Controls.Add(
                tabela);

            // =====================================================
            // TELA PRETA
            // =====================================================

            Panel blackPanel =
                CriarOpcao(
                    "TELA PRETA",
                    "Escurecer completamente a tela.");

            tabela.Controls.Add(
                blackPanel,
                0,
                0);

            rbBlack =
                new RadioButton
                {
                    Text =
                        "TELA PRETA",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            8,
                            5),

                    Checked = true
                };

            blackPanel.Controls.Add(
                rbBlack);

            // =====================================================
            // IMAGEM
            // =====================================================

            Panel imagePanel =
                CriarOpcao(
                    "IMAGEM",
                    "Exibir uma imagem personalizada.");

            tabela.Controls.Add(
                imagePanel,
                0,
                1);

            rbImage =
                new RadioButton
                {
                    Text =
                        "IMAGEM",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            8,
                            5)
                };

            imagePanel.Controls.Add(
                rbImage);

            // =====================================================
            // MENSAGEM
            // =====================================================

            Panel textPanel =
                CriarOpcao(
                    "MENSAGEM",
                    "Exibir um aviso personalizado.");

            tabela.Controls.Add(
                textPanel,
                0,
                2);

            rbText =
                new RadioButton
                {
                    Text =
                        "MENSAGEM",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(
                            8,
                            5)
                };

            textPanel.Controls.Add(
                rbText);

            // =====================================================
            // CONFIGURAÇÃO
            // =====================================================

            Panel configuracao =
                new Panel
                {
                    Dock = DockStyle.Fill,

                    BackColor =
                        Color.FromArgb(
                            9,
                            16,
                            25),

                    Padding =
                        new Padding(
                            10)
                };

            tabela.Controls.Add(
                configuracao,
                1,
                0);

            tabela.SetRowSpan(
                configuracao,
                4);

            Label configTitle =
                new Label
                {
                    Text =
                        "CONFIGURAÇÃO",

                    ForeColor =
                        Color.FromArgb(
                            100,
                            185,
                            240),

                    Font =
                        new Font(
                            "Segoe UI",
                            8.5F,
                            FontStyle.Bold),

                    Dock = DockStyle.Top,

                    Height = 25
                };

            configuracao.Controls.Add(
                configTitle);

            txtMessage =
                new TextBox
                {
                    Multiline = true,

                    ScrollBars =
                        ScrollBars.Vertical,

                    BackColor =
                        Color.FromArgb(
                            5,
                            10,
                            17),

                    ForeColor =
                        Color.White,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    Font =
                        new Font(
                            "Segoe UI",
                            9F),

                    Dock = DockStyle.Top,

                    Height = 75,

                    Text =
                        "MANUTENÇÃO EM ANDAMENTO\r\n" +
                        "POR FAVOR AGUARDE..."
                };

            configuracao.Controls.Add(
                txtMessage);

            btnImage =
                CriarBotao(
                    "SELECIONAR IMAGEM",
                    Blue);

            btnImage.Dock =
                DockStyle.Top;

            btnImage.Height =
                40;

            btnImage.Margin =
                new Padding(
                    0,
                    10,
                    0,
                    0);

            btnImage.Click +=
                (s, e) =>
                    EscolherImagem();

            configuracao.Controls.Add(
                btnImage);

            lblImage =
                new Label
                {
                    Text =
                        "Nenhuma imagem selecionada",

                    ForeColor =
                        Color.FromArgb(
                            110,
                            125,
                            140),

                    TextAlign =
                        ContentAlignment.MiddleCenter,

                    Dock = DockStyle.Top,

                    Height = 40
                };

            configuracao.Controls.Add(
                lblImage);
        }

        private Panel CriarOpcao(
            string titulo,
            string descricao)
        {
            Panel panel =
                new Panel
                {
                    Dock = DockStyle.Fill,

                    BackColor =
                        Color.FromArgb(
                            11,
                            19,
                            29),

                    Padding =
                        new Padding(
                            5)
                };

            return panel;
        }

        // =========================================================
        // CONTROLE
        // =========================================================

        private void CriarControle(
            Panel container)
        {
            TableLayoutPanel tabela =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 2,

                    RowCount = 1,

                    Padding =
                        new Padding(
                            20,
                            62,
                            20,
                            20)
                };

            tabela.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            tabela.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            container.Controls.Add(
                tabela);

            btnExecute =
                CriarBotao(
                    "▶   EXECUTAR AÇÃO",
                    Blue);

            btnExecute.Dock =
                DockStyle.Fill;

            btnExecute.Margin =
                new Padding(
                    0,
                    0,
                    8,
                    0);

            btnExecute.Click +=
                async (s, e) =>
                    await ExecutarAsync();

            tabela.Controls.Add(
                btnExecute,
                0,
                0);

            btnRestore =
                CriarBotao(
                    "■   RESTAURAR TELA",
                    Red);

            btnRestore.Dock =
                DockStyle.Fill;

            btnRestore.Margin =
                new Padding(
                    8,
                    0,
                    0,
                    0);

            btnRestore.Click +=
                async (s, e) =>
                    await RestaurarAsync();

            tabela.Controls.Add(
                btnRestore,
                1,
                0);
        }

        // =========================================================
        // BOTÕES
        // =========================================================

        private Button CriarBotao(
            string text,
            Color color)
        {
            Button button =
                new Button
                {
                    Text = text,

                    BackColor = color,

                    ForeColor =
                        Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),

                    Cursor =
                        Cursors.Hand,

                    UseVisualStyleBackColor =
                        false
                };

            button.FlatAppearance.BorderSize =
                0;

            button.MouseEnter +=
                (s, e) =>
                {
                    button.BackColor =
                        ControlPaint.Light(
                            color,
                            0.12f);
                };

            button.MouseLeave +=
                (s, e) =>
                {
                    button.BackColor =
                        color;
                };

            return button;
        }

        // =========================================================
        // DESTAQUE
        // =========================================================

        private void Destacar(
            Panel panel)
        {
            panel.BackColor =
                Color.FromArgb(
                    14,
                    37,
                    57);
        }

        private void RemoverDestaque(
            Panel panel)
        {
            panel.BackColor =
                CardDark;
        }

        // =========================================================
        // IMAGEM
        // =========================================================

        private void EscolherImagem()
        {
            using OpenFileDialog dialog =
                new OpenFileDialog
                {
                    Title =
                        "Escolher imagem de aviso",

                    Filter =
                        "Imagens|*.png;*.jpg;*.jpeg;*.bmp"
                };

            if (dialog.ShowDialog() !=
                DialogResult.OK)
            {
                return;
            }

            imageFile =
                dialog.FileName;

            lblImage.Text =
                "Imagem: " +
                Path.GetFileName(
                    imageFile);

            rbImage.Checked = true;
        }

        // =========================================================
        // COMPUTADOR ALVO
        // =========================================================

        private string? ObterAlvo()
        {
            if (pc1.Checked)
                return deviceId;

            if (pc2.Checked)
                return remoteDeviceId;

            return null;
        }

        // =========================================================
        // EXECUTAR AÇÃO
        // =========================================================

        private async Task ExecutarAsync()
        {
            string? target =
                ObterAlvo();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione COMPUTADOR 01 ou COMPUTADOR 02.",
                    "RD Screen Guard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            string command;

            string? textValue = null;

            string? imageBase64 = null;

            if (rbBlack.Checked)
            {
                command = "black";
            }
            else if (rbImage.Checked)
            {
                if (!File.Exists(
                    imageFile))
                {
                    MessageBox.Show(
                        "Selecione uma imagem primeiro.",
                        "RD Screen Guard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                command = "image";

                imageBase64 =
                    Convert.ToBase64String(
                        File.ReadAllBytes(
                            imageFile));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(
                    txtMessage.Text))
                {
                    MessageBox.Show(
                        "Digite a mensagem.",
                        "RD Screen Guard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                command = "text";

                textValue =
                    txtMessage.Text.Trim();
            }

            btnExecute.Enabled = false;

            try
            {
                // Computador local
                if (target == deviceId)
                {
                    AplicarComando(
                        command,
                        textValue,
                        imageBase64);

                    return;
                }

                // Computador remoto
                if (string.IsNullOrWhiteSpace(
                    auth.AccessToken))
                {
                    MessageBox.Show(
                        "A sessão não recebeu o token de controle remoto.",
                        "RD Screen Guard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                bool enviado =
                    await RemoteCommandService
                        .EnviarComandoAsync(
                            auth.AccessToken,
                            target,
                            command,
                            textValue,
                            imageBase64);

                if (!enviado)
                {
                    MessageBox.Show(
                        "Não foi possível enviar o comando ao computador.",
                        "RD Screen Guard",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnExecute.Enabled = true;
            }
        }

        // =========================================================
        // RESTAURAR
        // =========================================================

        private async Task RestaurarAsync()
        {
            string? target =
                ObterAlvo();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione um computador.",
                    "RD Screen Guard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (target == deviceId)
            {
                RestaurarLocal();
                return;
            }

            if (!string.IsNullOrWhiteSpace(
                auth.AccessToken))
            {
                await RemoteCommandService
                    .EnviarComandoAsync(
                        auth.AccessToken,
                        target,
                        "restore");
            }
        }

        // =========================================================
        // COMUNICAÇÃO
        // =========================================================

        private void IniciarComunicacao()
        {
            if (string.IsNullOrWhiteSpace(
                auth.AccessToken))
            {
                lblStatus.Text =
                    "● LOGIN CONECTADO";

                lblStatus.ForeColor =
                    Color.Gold;

                return;
            }

            timer =
                new System.Windows.Forms.Timer
                {
                    Interval = 5000
                };

            timer.Tick +=
                async (s, e) =>
                {
                    await AtualizarComputadoresAsync();
                    await BuscarComandoAsync();
                };

            timer.Start();

            _ = AtualizarComputadoresAsync();
            _ = BuscarComandoAsync();
        }

        // =========================================================
        // ATUALIZAR COMPUTADORES
        // =========================================================

        private async Task AtualizarComputadoresAsync()
        {
            if (string.IsNullOrWhiteSpace(
                auth.AccessToken))
            {
                return;
            }

            bool registered =
                await RemoteCommandService
                    .RegistrarComputadorAsync(
                        auth.AccessToken,
                        deviceId,
                        "COMPUTADOR 01");

            List<ScreenGuardDevice> list =
                await RemoteCommandService
                    .ObterComputadoresAsync(
                        auth.AccessToken,
                        deviceId);

            ScreenGuardDevice? remote =
                list.Find(
                    x =>
                        x.Id != deviceId &&
                        x.Online);

            remoteDeviceId =
                remote?.Id;

            if (registered)
            {
                lblPc1Status.Text =
                    "● Online";

                lblPc1Status.ForeColor =
                    Green;
            }
            else
            {
                lblPc1Status.Text =
                    "● Aguardando servidor";

                lblPc1Status.ForeColor =
                    Color.Gold;
            }

            if (remote != null)
            {
                pc2.Text =
                    string.IsNullOrWhiteSpace(
                        remote.Nome)
                        ? "COMPUTADOR 02"
                        : remote.Nome;

                lblPc2Status.Text =
                    "● Online";

                lblPc2Status.ForeColor =
                    Green;
            }
            else
            {
                pc2.Text =
                    "COMPUTADOR 02";

                lblPc2Status.Text =
                    "● Aguardando conexão";

                lblPc2Status.ForeColor =
                    Color.Gray;
            }

            lblStatus.Text =
                registered
                    ? "● CONECTADO"
                    : "● AGUARDANDO SERVIDOR";

            lblStatus.ForeColor =
                registered
                    ? Green
                    : Color.Gold;
        }

        // =========================================================
        // RECEBER COMANDO
        // =========================================================

        private async Task BuscarComandoAsync()
        {
            if (string.IsNullOrWhiteSpace(
                auth.AccessToken))
            {
                return;
            }

            ScreenGuardCommand? command =
                await RemoteCommandService
                    .BuscarComandoAsync(
                        auth.AccessToken,
                        deviceId);

            if (command == null)
                return;

            try
            {
                AplicarComando(
                    command.Tipo,
                    command.Texto,
                    command.ImagemBase64);
            }
            finally
            {
                await RemoteCommandService
                    .ConfirmarComandoAsync(
                        auth.AccessToken,
                        deviceId,
                        command.Id);
            }
        }

        // =========================================================
        // APLICAR COMANDO
        // =========================================================

        private void AplicarComando(
            string command,
            string? textValue,
            string? imageBase64)
        {
            if (command.Equals(
                "restore",
                StringComparison.OrdinalIgnoreCase))
            {
                RestaurarLocal();
                return;
            }

            RestaurarLocal();

            foreach (
                Screen screen
                in Screen.AllScreens)
            {
                OverlayForm overlay;

                if (
                    command.Equals(
                        "image",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    !string.IsNullOrWhiteSpace(
                        imageBase64))
                {
                    string file =
                        Path.Combine(
                            Path.GetTempPath(),
                            "rd_screenguard_" +
                            Guid.NewGuid()
                                .ToString("N") +
                            ".png");

                    File.WriteAllBytes(
                        file,
                        Convert.FromBase64String(
                            imageBase64));

                    overlay =
                        new OverlayForm(
                            screen,
                            file,
                            null);
                }
                else if (
                    command.Equals(
                        "text",
                        StringComparison.OrdinalIgnoreCase))
                {
                    overlay =
                        new OverlayForm(
                            screen,
                            null,
                            textValue ??
                            "AVISO");
                }
                else
                {
                    overlay =
                        new OverlayForm(
                            screen,
                            null,
                            null);
                }

                overlays.Add(
                    overlay);

                overlay.Show();
            }
        }

        // =========================================================
        // RESTAURAR TELA LOCAL
        // =========================================================

        private void RestaurarLocal()
        {
            foreach (
                OverlayForm overlay
                in overlays.ToArray())
            {
                if (!overlay.IsDisposed)
                    overlay.Close();
            }

            overlays.Clear();
        }
    }
}
