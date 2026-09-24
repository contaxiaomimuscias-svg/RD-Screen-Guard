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

        // Timer explicitamente do Windows Forms
        private System.Windows.Forms.Timer? timer;

        private CheckBox pc1 = null!;
        private CheckBox pc2 = null!;

        private RadioButton black = null!;
        private RadioButton image = null!;
        private RadioButton text = null!;

        private TextBox message = null!;

        private Label status = null!;
        private Label imageLabel = null!;

        private Button execute = null!;
        private Button restore = null!;

        public MainForm(RdSistemasAuthResult result)
        {
            auth = result;
            deviceId = DeviceIdentity.GetDeviceId();

            BuildUi();
            StartRemoteLoop();
        }

        private void BuildUi()
        {
            Text = "RD Screen Guard";

            StartPosition = FormStartPosition.CenterScreen;

            ClientSize = new Size(1120, 760);

            BackColor = Color.FromArgb(4, 8, 13);

            Font = new Font(
                "Segoe UI",
                10
            );

            Add(
                new Label
                {
                    Text = "🛡 RD SCREEN GUARD",
                    ForeColor = Color.White,
                    Font = new Font(
                        "Segoe UI",
                        24,
                        FontStyle.Bold
                    ),
                    AutoSize = true,
                    Location = new Point(30, 25)
                }
            );

            Add(
                new Label
                {
                    Text = "CONTROLE DE TELAS REMOTO",
                    ForeColor = Color.FromArgb(
                        70,
                        180,
                        255
                    ),
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold
                    ),
                    AutoSize = true,
                    Location = new Point(34, 70)
                }
            );

            status = new Label
            {
                Text = "● Conectando...",
                ForeColor = Color.Gold,
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold
                ),
                AutoSize = true,
                Location = new Point(850, 45)
            };

            Add(status);

            // ==============================
            // COMPUTADORES
            // ==============================

            Panel devices = CriarPainel(
                "🖥 COMPUTADORES CONECTADOS",
                new Point(25, 110),
                new Size(420, 300)
            );

            pc1 = CriarCheck(
                "COMPUTADOR 01",
                new Point(20, 60)
            );

            pc2 = CriarCheck(
                "COMPUTADOR 02",
                new Point(20, 125)
            );

            pc1.CheckedChanged += (s, e) =>
            {
                if (pc1.Checked)
                    pc2.Checked = false;
            };

            pc2.CheckedChanged += (s, e) =>
            {
                if (pc2.Checked)
                    pc1.Checked = false;
            };

            devices.Controls.Add(pc1);
            devices.Controls.Add(pc2);

            devices.Controls.Add(
                new Label
                {
                    Text =
                        $"ID deste computador: {deviceId}",
                    ForeColor = Color.Silver,
                    AutoSize = true,
                    Location = new Point(20, 195)
                }
            );

            devices.Controls.Add(
                new Label
                {
                    Text =
                        $"Empresa: {auth.Empresa ?? "-"}",
                    ForeColor = Color.Silver,
                    AutoSize = true,
                    Location = new Point(20, 225)
                }
            );

            devices.Controls.Add(
                new Label
                {
                    Text =
                        $"Plano: {auth.Plano ?? "-"}",
                    ForeColor = Color.Silver,
                    AutoSize = true,
                    Location = new Point(20, 255)
                }
            );

            Add(devices);

            // ==============================
            // AÇÕES
            // ==============================

            Panel actions = CriarPainel(
                "⚙ AÇÃO DE TELA",
                new Point(465, 110),
                new Size(390, 300)
            );

            black = CriarRadio(
                "Tela Preta — Escurece a tela do computador selecionado.",
                new Point(20, 60),
                true
            );

            image = CriarRadio(
                "Usar Imagem — Mostra uma imagem na tela.",
                new Point(20, 110)
            );

            text = CriarRadio(
                "Mensagem Personalizada — Mostra um aviso.",
                new Point(20, 160)
            );

            actions.Controls.Add(black);
            actions.Controls.Add(image);
            actions.Controls.Add(text);

            actions.Controls.Add(
                new Label
                {
                    Text =
                        "CONFIGURAÇÕES DA MENSAGEM / IMAGEM",
                    ForeColor = Color.FromArgb(
                        70,
                        180,
                        255
                    ),
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold
                    ),
                    AutoSize = true,
                    Location = new Point(20, 205)
                }
            );

            message = new TextBox
            {
                Multiline = true,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(
                    10,
                    18,
                    28
                ),
                Location = new Point(20, 230),
                Size = new Size(350, 55),
                Text =
                    "MANUTENÇÃO EM ANDAMENTO\r\n" +
                    "POR FAVOR AGUARDE..."
            };

            actions.Controls.Add(message);

            Add(actions);

            // ==============================
            // EXECUTAR
            // ==============================

            execute = CriarButton(
                "▶  EXECUTAR AÇÃO",
                new Point(465, 430),
                new Size(390, 58),
                Color.FromArgb(
                    0,
                    105,
                    235
                )
            );

            execute.Click += async (s, e) =>
                await ExecuteAsync();

            Add(execute);

            // ==============================
            // RESTAURAR
            // ==============================

            restore = CriarButton(
                "■  RESTAURAR TELA",
                new Point(465, 500),
                new Size(390, 58),
                Color.FromArgb(
                    190,
                    45,
                    45
                )
            );

            restore.Click += async (s, e) =>
                await RestoreAsync();

            Add(restore);

            // ==============================
            // IMAGEM
            // ==============================

            Button choose = CriarButton(
                "🖼  SELECIONAR IMAGEM",
                new Point(880, 500),
                new Size(210, 48),
                Color.FromArgb(
                    25,
                    35,
                    48
                )
            );

            choose.Click += (s, e) =>
                ChooseImage();

            Add(choose);

            imageLabel = new Label
            {
                Text =
                    "Nenhuma imagem selecionada",

                ForeColor = Color.Silver,

                Size = new Size(
                    210,
                    80
                ),

                Location = new Point(
                    880,
                    560
                ),

                TextAlign =
                    ContentAlignment.MiddleCenter
            };

            Add(imageLabel);

            // ==============================
            // INFORMAÇÃO
            // ==============================

            Panel info = CriarPainel(
                "ℹ COMPUTADOR SELECIONADO",
                new Point(25, 430),
                new Size(420, 210)
            );

            info.Controls.Add(
                new Label
                {
                    Text =
                        "Marque COMPUTADOR 01 ou COMPUTADOR 02.\r\n\r\n" +
                        "O comando será enviado somente ao computador\r\n" +
                        "selecionado quando ele estiver conectado ao\r\n" +
                        "RD Screen Guard.",

                    ForeColor = Color.Gainsboro,

                    AutoSize = false,

                    Size = new Size(
                        370,
                        125
                    ),

                    Location = new Point(
                        20,
                        55
                    )
                }
            );

            Add(info);

            Add(
                new Label
                {
                    Text =
                        "● Sistema conectado ao RD Sistemas     |     RD Screen Guard",

                    ForeColor =
                        Color.FromArgb(
                            80,
                            220,
                            120
                        ),

                    AutoSize = true,

                    Location =
                        new Point(
                            30,
                            680
                        )
                }
            );

            FormClosed += (s, e) =>
            {
                timer?.Stop();
                RestoreLocal();
            };
        }

        // ==============================
        // COMPONENTES
        // ==============================

        private void Add(Control control)
        {
            Controls.Add(control);
        }

        private Panel CriarPainel(
            string title,
            Point location,
            Size size)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(
                    10,
                    17,
                    25
                ),
                BorderStyle =
                    BorderStyle.FixedSingle
            };

            panel.Controls.Add(
                new Label
                {
                    Text = title,
                    ForeColor =
                        Color.FromArgb(
                            70,
                            180,
                            255
                        ),
                    Font = new Font(
                        "Segoe UI",
                        11,
                        FontStyle.Bold
                    ),
                    AutoSize = true,
                    Location = new Point(
                        18,
                        15
                    )
                }
            );

            return panel;
        }

        private CheckBox CriarCheck(
            string text,
            Point location)
        {
            return new CheckBox
            {
                Text = text,
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold
                ),
                Location = location
            };
        }

        private RadioButton CriarRadio(
            string text,
            Point location,
            bool checkedValue = false)
        {
            return new RadioButton
            {
                Text = text,
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                ),
                Location = location,
                Checked = checkedValue
            };
        }

        private Button CriarButton(
            string text,
            Point location,
            Size size,
            Color color)
        {
            Button button = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                ),
                FlatStyle =
                    FlatStyle.Flat,
                Cursor =
                    Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        // ==============================
        // IMAGEM
        // ==============================

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

            imageLabel.Text =
                "Imagem selecionada:\r\n" +
                Path.GetFileName(
                    imageFile
                );

            image.Checked = true;
        }

        // ==============================
        // COMPUTADOR ALVO
        // ==============================

        private string? Target()
        {
            if (pc1.Checked)
                return deviceId;

            if (pc2.Checked)
                return remoteDeviceId;

            return null;
        }

        // ==============================
        // EXECUTAR
        // ==============================

        private async Task ExecuteAsync()
        {
            string? target =
                Target();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione COMPUTADOR 01 ou COMPUTADOR 02.",
                    "RD Screen Guard"
                );

                return;
            }

            string command;

            string? textValue = null;
            string? imageBase64 = null;

            if (black.Checked)
            {
                command = "black";
            }
            else if (image.Checked)
            {
                if (!File.Exists(imageFile))
                {
                    MessageBox.Show(
                        "Selecione uma imagem primeiro.",
                        "RD Screen Guard"
                    );

                    return;
                }

                command = "image";

                imageBase64 =
                    Convert.ToBase64String(
                        File.ReadAllBytes(
                            imageFile
                        )
                    );
            }
            else
            {
                if (string.IsNullOrWhiteSpace(
                    message.Text))
                {
                    MessageBox.Show(
                        "Digite a mensagem primeiro.",
                        "RD Screen Guard"
                    );

                    return;
                }

                command = "text";

                textValue =
                    message.Text.Trim();
            }

            execute.Enabled = false;

            try
            {
                // Este computador
                if (target == deviceId)
                {
                    Apply(
                        command,
                        textValue,
                        imageBase64
                    );

                    return;
                }

                // Computador remoto
                if (string.IsNullOrWhiteSpace(
                    auth.AccessToken))
                {
                    MessageBox.Show(
                        "A API ainda precisa retornar o token de controle remoto.",
                        "RD Screen Guard"
                    );

                    return;
                }

                bool enviado =
                    await RemoteCommandService
                        .EnviarComandoAsync(
                            auth.AccessToken,
                            target,
                            command,
                            textValue,
                            imageBase64
                        );

                if (!enviado)
                {
                    MessageBox.Show(
                        "Não foi possível enviar o comando ao computador selecionado.",
                        "RD Screen Guard"
                    );
                }
            }
            finally
            {
                execute.Enabled = true;
            }
        }

        // ==============================
        // RESTAURAR
        // ==============================

        private async Task RestoreAsync()
        {
            string? target =
                Target();

            if (target == null)
            {
                MessageBox.Show(
                    "Selecione um computador.",
                    "RD Screen Guard"
                );

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
                        "restore"
                    );
            }
        }

        // ==============================
        // CONEXÃO
        // ==============================

        private void StartRemoteLoop()
        {
            if (string.IsNullOrWhiteSpace(
                auth.AccessToken))
            {
                status.Text =
                    "● Login conectado";

                status.ForeColor =
                    Color.Gold;

                return;
            }

            // CORREÇÃO DO ERRO CS0104
            timer =
                new System.Windows.Forms.Timer
                {
                    Interval = 5000
                };

            timer.Tick += async (s, e) =>
            {
                await UpdateDevicesAsync();
                await PollAsync();
            };

            timer.Start();

            _ = UpdateDevicesAsync();
            _ = PollAsync();
        }

        // ==============================
        // ATUALIZAR COMPUTADORES
        // ==============================

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
                        "COMPUTADOR 01"
                    );

            var list =
                await RemoteCommandService
                    .ObterComputadoresAsync(
                        auth.AccessToken,
                        deviceId
                    );

            var remote =
                list.Find(
                    x =>
                        x.Id != deviceId &&
                        x.Online
                );

            remoteDeviceId =
                remote?.Id;

            pc1.Text =
                "COMPUTADOR 01 (este computador)" +
                (registered
                    ? " • Online"
                    : " • Aguardando conexão");

            pc2.Text =
                remote == null
                    ? "COMPUTADOR 02 • aguardando conexão"
                    : (
                        string.IsNullOrWhiteSpace(
                            remote.Nome)
                            ? "COMPUTADOR 02 • Online"
                            : remote.Nome +
                              " • Online"
                      );

            status.Text =
                registered
                    ? "● Conectado"
                    : "● Aguardando servidor";

            status.ForeColor =
                registered
                    ? Color.FromArgb(
                        60,
                        230,
                        120
                    )
                    : Color.Gold;
        }

        // ==============================
        // RECEBER COMANDO
        // ==============================

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
                        deviceId
                    );

            if (command == null)
                return;

            try
            {
                Apply(
                    command.Tipo,
                    command.Texto,
                    command.ImagemBase64
                );
            }
            finally
            {
                await RemoteCommandService
                    .ConfirmarComandoAsync(
                        auth.AccessToken,
                        deviceId,
                        command.Id
                    );
            }
        }

        // ==============================
        // APLICAR COMANDO
        // ==============================

        private void Apply(
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
                        StringComparison.OrdinalIgnoreCase
                    )
                    &&
                    !string.IsNullOrWhiteSpace(
                        imageBase64)
                )
                {
                    string file =
                        Path.Combine(
                            Path.GetTempPath(),
                            "rdsg_" +
                            Guid.NewGuid()
                                .ToString("N") +
                            ".png"
                        );

                    File.WriteAllBytes(
                        file,
                        Convert.FromBase64String(
                            imageBase64
                        )
                    );

                    overlay =
                        new OverlayForm(
                            screen,
                            file,
                            null
                        );
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
                            "AVISO"
                        );
                }
                else
                {
                    overlay =
                        new OverlayForm(
                            screen,
                            null,
                            null
                        );
                }

                overlays.Add(
                    overlay
                );

                overlay.Show();
            }
        }

        // ==============================
        // RESTAURAR LOCAL
        // ==============================

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
