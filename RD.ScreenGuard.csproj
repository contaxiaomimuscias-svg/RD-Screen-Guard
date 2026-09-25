using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private Panel cardPc1 = null!;
        private Panel cardPc2 = null!;

        private Button btnExecute = null!;
        private Button btnRestore = null!;
        private Button btnImage = null!;

        private Color BackgroundColor =
            Color.FromArgb(7, 11, 18);

        private Color CardColor =
            Color.FromArgb(13, 20, 30);

        private Color CardBorder =
            Color.FromArgb(31, 43, 58);

        private Color Blue =
            Color.FromArgb(0, 150, 255);

        private Color Green =
            Color.FromArgb(45, 210, 120);

        private Color Red =
            Color.FromArgb(235, 70, 75);

        public MainForm(RdSistemasAuthResult result)
        {
            auth = result;
            deviceId = DeviceIdentity.GetDeviceId();

            InitializeModernForm();
            BuildModernUi();
            StartRemoteLoop();
        }

        private void InitializeModernForm()
        {
            Text = "RD Screen Guard";

            StartPosition =
                FormStartPosition.CenterScreen;

            ClientSize =
                new Size(1180, 760);

            MinimumSize =
                new Size(1050, 700);

            BackColor =
                BackgroundColor;

            Font =
                new Font(
                    "Segoe UI",
                    9F);

            DoubleBuffered = true;
        }

        private void BuildModernUi()
        {
            // =====================================================
            // CABEÇALHO
            // =====================================================

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = Color.FromArgb(9, 15, 24)
            };

            Controls.Add(header);

            Label logo = new Label
            {
                Text = "◈",
                ForeColor = Blue,
                Font = new Font(
                    "Segoe UI",
                    34,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(28, 22)
            };

            header.Controls.Add(logo);

            Label title = new Label
            {
                Text = "RD SCREEN GUARD",
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    24,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(78, 20)
            };

            header.Controls.Add(title);

            Label subtitle = new Label
            {
                Text = "CONTROLE DE TELAS REMOTO",
                ForeColor =
                    Color.FromArgb(
                        110,
                        170,
                        220),
                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(82, 61)
            };

            header.Controls.Add(subtitle);

            Panel statusCard =
                CreateRoundedPanel(
                    Color.FromArgb(
                        14,
                        25,
                        36),
                    150);

            statusCard.Location =
                new Point(
                    980,
                    30);

            statusCard.Size =
                new Size(
                    165,
                    45);

            header.Controls.Add(statusCard);

            lblStatus = new Label
            {
                Text = "● CONECTANDO",
                ForeColor = Color.Gold,
                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(
                        18,
                        13)
            };

            statusCard.Controls.Add(
                lblStatus);

            // =====================================================
            // ÁREA PRINCIPAL
            // =====================================================

            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding =
                    new Padding(
                        24,
                        20,
                        24,
                        20),
                BackColor =
                    BackgroundColor
            };

            Controls.Add(content);

            // =====================================================
            // CARD COMPUTADORES
            // =====================================================

            Panel computers =
                CreateCard(
                    "COMPUTADORES",
                    "Selecione qual computador receberá o comando.");

            computers.Location =
                new Point(
                    24,
                    20);

            computers.Size =
                new Size(
                    500,
                    325);

            content.Controls.Add(computers);

            // PC 01

            cardPc1 =
                CreateComputerCard(
                    "COMPUTADOR 01",
                    "Este computador",
                    true);

            cardPc1.Location =
                new Point(
                    20,
                    65);

            cardPc1.Size =
                new Size(
                    455,
                    85);

            computers.Controls.Add(
                cardPc1);

            pc1 =
                CreateComputerCheck();

            pc1.Location =
                new Point(
                    15,
                    18);

            pc1.CheckedChanged +=
                (s, e) =>
                {
                    if (pc1.Checked)
                    {
                        pc2.Checked = false;
                        DestacarComputador(cardPc1);
                        RemoverDestaque(cardPc2);
                    }
                };

            cardPc1.Controls.Add(pc1);

            lblPc1Status = new Label
            {
                Text = "● Verificando...",
                ForeColor = Color.Gold,
                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(
                        38,
                        47)
            };

            cardPc1.Controls.Add(
                lblPc1Status);

            // PC 02

            cardPc2 =
                CreateComputerCard(
                    "COMPUTADOR 02",
                    "Computador remoto",
                    false);

            cardPc2.Location =
                new Point(
                    20,
                    165);

            cardPc2.Size =
                new Size(
                    455,
                    85);

            computers.Controls.Add(
                cardPc2);

            pc2 =
                CreateComputerCheck();

            pc2.Location =
                new Point(
                    15,
                    18);

            pc2.CheckedChanged +=
                (s, e) =>
                {
                    if (pc2.Checked)
                    {
                        pc1.Checked = false;
                        DestacarComputador(cardPc2);
                        RemoverDestaque(cardPc1);
                    }
                };

            cardPc2.Controls.Add(pc2);

            lblPc2Status = new Label
            {
                Text =
                    "● Aguardando conexão",
                ForeColor = Color.Gray,
                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold),
                AutoSize = true,
                Location =
                    new Point(
                        38,
                        47)
            };

            cardPc2.Controls.Add(
                lblPc2Status);

            Label company = new Label
            {
                Text =
                    $"Empresa: {auth.Empresa ?? "-"}",
                ForeColor =
                    Color.FromArgb(
                        150,
                        165,
                        180),
                AutoSize = true,
                Location =
                    new Point(
                        20,
                        270)
            };

            computers.Controls.Add(
                company);

            Label device = new Label
            {
                Text =
                    $"ID: {deviceId}",
                ForeColor =
                    Color.FromArgb(
                        105,
                        120,
                        135),
                AutoSize = true,
                Location =
                    new Point(
                        20,
                        294)
            };

            computers.Controls.Add(device);

            // =====================================================
            // CARD AÇÃO
            // =====================================================

            Panel actions =
                CreateCard(
                    "AÇÃO DE TELA",
                    "Escolha o tipo de proteção.");

            actions.Location =
                new Point(
                    545,
                    20);

            actions.Size =
                new Size(
                    610,
                    325);

            content.Controls.Add(actions);

            // Tela preta

            rbBlack =
                CreateRadio(
                    "TELA PRETA",
                    "Escurece completamente a tela.",
                    true);

            rbBlack.Location =
                new Point(
                    25,
                    67);

            actions.Controls.Add(
                rbBlack);

            // Imagem

            rbImage =
                CreateRadio(
                    "IMAGEM",
                    "Exibe uma imagem personalizada.");

            rbImage.Location =
                new Point(
                    25,
                    112);

            actions.Controls.Add(
                rbImage);

            // Mensagem

            rbText =
                CreateRadio(
                    "MENSAGEM",
                    "Exibe um aviso personalizado.");

            rbText.Location =
                new Point(
                    25,
                    157);

            actions.Controls.Add(
                rbText);

            Label messageTitle =
                new Label
                {
                    Text =
                        "MENSAGEM / CONFIGURAÇÃO",
                    ForeColor =
                        Color.FromArgb(
                            110,
                            180,
                            240),
                    Font =
                        new Font(
                            "Segoe UI",
                            8.5F,
                            FontStyle.Bold),
                    AutoSize = true,
                    Location =
                        new Point(
                            300,
                            67)
                };

            actions.Controls.Add(
                messageTitle);

            txtMessage =
                new TextBox
                {
                    Multiline = true,
                    ForeColor = Color.White,
                    BackColor =
                        Color.FromArgb(
                            8,
                            14,
                            22),
                    BorderStyle =
                        BorderStyle.FixedSingle,
                    Font =
                        new Font(
                            "Segoe UI",
                            9.5F),
                    Location =
                        new Point(
                            300,
                            92),
                    Size =
                        new Size(
                            280,
                            80),
                    Text =
                        "MANUTENÇÃO EM ANDAMENTO\r\n" +
                        "POR FAVOR AGUARDE..."
                };

            actions.Controls.Add(
                txtMessage);

            btnImage =
                CreateModernButton(
                    "🖼  ESCOLHER IMAGEM",
                    Blue);

            btnImage.Location =
                new Point(
                    300,
                    188);

            btnImage.Size =
                new Size(
                    280,
                    42);

            btnImage.Click +=
                (s, e) =>
                    ChooseImage();

            actions.Controls.Add(
                btnImage);

            lblImage =
                new Label
                {
                    Text =
                        "Nenhuma imagem selecionada",
                    ForeColor =
                        Color.FromArgb(
                            115,
                            130,
                            145),
                    Font =
                        new Font(
                            "Segoe UI",
                            8.5F),
                    AutoSize = false,
                    TextAlign =
                        ContentAlignment.MiddleCenter,
                    Location =
                        new Point(
                            300,
                            238),
                    Size =
                        new Size(
                            280,
                            40)
                };

            actions.Controls.Add(
                lblImage);

            // =====================================================
            // CARD AÇÕES
            // =====================================================

            Panel commandCard =
                CreateCard(
                    "CONTROLE",
                    "Execute a ação no computador selecionado.");

            commandCard.Location =
                new Point(
                    24,
                    370);

            commandCard.Size =
                new Size(
                    1131,
                    170);

            content.Controls.Add(
                commandCard);

            btnExecute =
                CreateModernButton(
                    "▶   EXECUTAR AÇÃO",
                    Color.FromArgb(
                        0,
                        125,
                        245));

            btnExecute.Location =
                new Point(
                    25,
                    70);

            btnExecute.Size =
                new Size(
                    520,
                    65);

            btnExecute.Click +=
                async (s, e) =>
                    await ExecuteAsync();

            commandCard.Controls.Add(
                btnExecute);

            btnRestore =
                CreateModernButton(
                    "■   RESTAURAR TELA",
                    Color.FromArgb(
                        190,
                        45,
                        55));

            btnRestore.Location =
                new Point(
                    575,
                    70);

            btnRestore.Size =
                new Size(
                    520,
                    65);

            btnRestore.Click +=
                async (s, e) =>
                    await RestoreAsync();

            commandCard.Controls.Add(
                btnRestore);

            // =====================================================
            // RODAPÉ
            // =====================================================

            Label footer =
                new Label
                {
                    Text =
                        "RD SCREEN GUARD   •   RD SISTEMAS   •   Proteção de tela",

                    ForeColor =
                        Color.FromArgb(
                            85,
                            105,
                            120),

                    AutoSize = true,

                    Location =
                        new Point(
                            28,
                            565)
                };

            content.Controls.Add(
                footer);

            // Seleciona PC01 visualmente
            pc1.Checked = true;
        }

        // =========================================================
        // CARD
        // =========================================================

        private Panel CreateCard(
            string title,
            string description)
        {
            Panel panel =
                CreateRoundedPanel(
                    CardColor,
                    12);

            panel.BorderStyle =
                BorderStyle.FixedSingle;

            Label titleLabel =
                new Label
                {
                    Text = title,
                    ForeColor =
                        Color.FromArgb(
                            95,
                            185,
                            255),
                    Font =
                        new Font(
                            "Segoe UI",
                            12,
                            FontStyle.Bold),
                    AutoSize = true,
                    Location =
                        new Point(
                            20,
                            16)
                };

            panel.Controls.Add(
                titleLabel);

            Label descriptionLabel =
                new Label
                {
                    Text = description,
                    ForeColor =
                        Color.FromArgb(
                            115,
                            130,
                            145),
                    Font =
                        new Font(
                            "Segoe UI",
                            8.5F),
                    AutoSize = true,
                    Location =
                        new Point(
                            20,
                            40)
                };

            panel.Controls.Add(
                descriptionLabel);

            return panel;
        }

        private Panel CreateRoundedPanel(
            Color color,
            int radius)
        {
            Panel panel =
                new Panel
                {
                    BackColor = color
                };

            panel.Paint +=
                (sender, e) =>
                {
                    Graphics g =
                        e.Graphics;

                    g.SmoothingMode =
                        SmoothingMode.AntiAlias;

                    using GraphicsPath path =
                        RoundedPath(
                            panel.ClientRectangle,
                            radius);

                    using Pen pen =
                        new Pen(
                            CardBorder,
                            1);

                    g.DrawPath(
                        pen,
                        path);
                };

            return panel;
        }

        private GraphicsPath RoundedPath(
            Rectangle rect,
            int radius)
        {
            GraphicsPath path =
                new GraphicsPath();

            int diameter =
                radius * 2;

            Rectangle arc =
                new Rectangle(
                    rect.X,
                    rect.Y,
                    diameter,
                    diameter);

            path.AddArc(
                arc,
                180,
                90);

            arc.X =
                rect.Right -
                diameter;

            path.AddArc(
                arc,
                270,
                90);

            arc.Y =
                rect.Bottom -
                diameter;

            path.AddArc(
                arc,
                0,
                90);

            arc.X =
                rect.X;

            path.AddArc(
                arc,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        // =========================================================
        // COMPUTADOR
        // =========================================================

        private Panel CreateComputerCard(
            string name,
            string description,
            bool local)
        {
            Panel card =
                CreateRoundedPanel(
                    Color.FromArgb(
                        10,
                        18,
                        28),
                    10);

            Label icon =
                new Label
                {
                    Text = local
                        ? "▣"
                        : "▤",
                    ForeColor =
                        Blue,
                    Font =
                        new Font(
                            "Segoe UI",
                            18,
                            FontStyle.Bold),
                    AutoSize = true,
                    Location =
                        new Point(
                            405,
                            14)
                };

            card.Controls.Add(
                icon);

            return card;
        }

        private CheckBox CreateComputerCheck()
        {
            return new CheckBox
            {
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

        private void DestacarComputador(
            Panel panel)
        {
            panel.BackColor =
                Color.FromArgb(
                    15,
                    34,
                    52);
        }

        private void RemoverDestaque(
            Panel panel)
        {
            panel.BackColor =
                Color.FromArgb(
                    10,
                    18,
                    28);
        }

        // =========================================================
        // RADIO
        // =========================================================

        private RadioButton CreateRadio(
            string title,
            string description,
            bool selected = false)
        {
            RadioButton radio =
                new RadioButton
                {
                    Text =
                        title +
                        "   —   " +
                        description,

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            9F,
                            FontStyle.Bold),

                    AutoSize = true,

                    Checked =
                        selected
                };

            return radio;
        }

        // =========================================================
        // BOTÃO
        // =========================================================

        private Button CreateModernButton(
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

                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            button.FlatAppearance.BorderSize =
                0;

            button.MouseEnter +=
                (s, e) =>
                {
                    button.BackColor =
                        ControlPaint.Light(
                            color,
                            0.15f);
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
        // IMAGEM
        // =========================================================

        private void ChooseImage()
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
                "Imagem selecionada:\r\n" +
                Path.GetFileName(
                    imageFile);

            rbImage.Checked = true;
        }

        // =========================================================
        // COMPUTADOR ALVO
        // =========================================================

        private string? GetTarget()
        {
            if (pc1.Checked)
                return deviceId;

            if (pc2.Checked)
                return remoteDeviceId;

            return null;
        }

        // =========================================================
        // EXECUTAR
        // =========================================================

        private async Task ExecuteAsync()
        {
            string? target =
                GetTarget();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione um computador.",
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
                if (!File.Exists(imageFile))
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
                if (target == deviceId)
                {
                    ApplyCommand(
                        command,
                        textValue,
                        imageBase64);

                    return;
                }

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
                        "Não foi possível enviar o comando ao computador selecionado.",
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

        private async Task RestoreAsync()
        {
            string? target =
                GetTarget();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione um computador.",
                    "RD Screen Guard");

                return;
            }

            if (target == deviceId)
            {
                RestoreLocal();
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

        private void StartRemoteLoop()
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
                    await UpdateDevicesAsync();
                    await PollAsync();
                };

            timer.Start();

            _ = UpdateDevicesAsync();
            _ = PollAsync();
        }

        // =========================================================
        // ATUALIZAR COMPUTADORES
        // =========================================================

        private async Task UpdateDevicesAsync()
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

            var list =
                await RemoteCommandService
                    .ObterComputadoresAsync(
                        auth.AccessToken,
                        deviceId);

            var remote =
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

        private async Task PollAsync()
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
                ApplyCommand(
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
        // APLICAR COMANDO LOCAL
        // =========================================================

        private void ApplyCommand(
            string command,
            string? textValue,
            string? imageBase64)
        {
            if (command.Equals(
                "restore",
                StringComparison.OrdinalIgnoreCase))
            {
                RestoreLocal();
                return;
            }

            RestoreLocal();

            foreach (Screen screen
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
        // RESTAURAR LOCAL
        // =========================================================

        private void RestoreLocal()
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
