using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class MainForm : Form
    {
        private Button btnTelaPreta;
        private Button btnImagem;
        private Button btnTexto;
        private Button btnRestaurar;
        private Button btnEscolher;

        private TextBox campoTexto;
        private Label lblImagem;

        private string imagemSelecionada = "";

        private readonly List<OverlayForm> overlays = new();

        public MainForm()
        {
            CriarInterface();
        }

        private void CriarInterface()
        {
            Text = "RD Screen Guard";

            StartPosition =
                FormStartPosition.CenterScreen;

            Size =
                new Size(700, 500);

            BackColor =
                Color.FromArgb(20, 24, 30);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            // TÍTULO
            Label titulo = new Label
            {
                Text = "RD SCREEN GUARD",

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        26,
                        FontStyle.Bold),

                AutoSize = true,

                Location =
                    new Point(35, 25)
            };

            Controls.Add(titulo);

            // SUBTÍTULO
            Label subtitulo = new Label
            {
                Text =
                    "Controle visual da tela do computador",

                ForeColor =
                    Color.Silver,

                Font =
                    new Font(
                        "Segoe UI",
                        11),

                AutoSize = true,

                Location =
                    new Point(38, 70)
            };

            Controls.Add(subtitulo);

            // TELA PRETA
            btnTelaPreta =
                CriarBotao(
                    "TELA PRETA",
                    new Point(35, 115),
                    Color.FromArgb(35, 35, 35));

            btnTelaPreta.Click +=
                (s, e) =>
                {
                    MostrarTelaPreta();
                };

            Controls.Add(btnTelaPreta);

            // IMAGEM
            btnImagem =
                CriarBotao(
                    "IMAGEM DE AVISO",
                    new Point(250, 115),
                    Color.FromArgb(0, 105, 180));

            btnImagem.Click +=
                (s, e) =>
                {
                    MostrarImagem();
                };

            Controls.Add(btnImagem);

            // TEXTO
            btnTexto =
                CriarBotao(
                    "AVISO EM TEXTO",
                    new Point(465, 115),
                    Color.FromArgb(120, 70, 180));

            btnTexto.Click +=
                (s, e) =>
                {
                    MostrarTexto();
                };

            Controls.Add(btnTexto);

            // CAMPO DE TEXTO
            Label labelTexto =
                new Label
                {
                    Text =
                        "Mensagem do aviso:",

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),

                    AutoSize = true,

                    Location =
                        new Point(38, 200)
                };

            Controls.Add(labelTexto);

            campoTexto =
                new TextBox
                {
                    Multiline = true,

                    ScrollBars =
                        ScrollBars.Vertical,

                    Font =
                        new Font(
                            "Segoe UI",
                            13),

                    ForeColor =
                        Color.White,

                    BackColor =
                        Color.FromArgb(
                            35,
                            40,
                            48),

                    Location =
                        new Point(35, 230),

                    Size =
                        new Size(
                            400,
                            100)
                };

            Controls.Add(campoTexto);

            // ESCOLHER IMAGEM
            btnEscolher =
                CriarBotaoPequeno(
                    "ESCOLHER IMAGEM",
                    new Point(465, 230));

            btnEscolher.Click +=
                (s, e) =>
                {
                    EscolherImagem();
                };

            Controls.Add(btnEscolher);

            // NOME DA IMAGEM
            lblImagem =
                new Label
                {
                    Text =
                        "Nenhuma imagem selecionada",

                    ForeColor =
                        Color.Gainsboro,

                    Font =
                        new Font(
                            "Segoe UI",
                            9),

                    AutoSize = false,

                    Location =
                        new Point(465, 285),

                    Size =
                        new Size(
                            190,
                            60)
                };

            Controls.Add(lblImagem);

            // RESTAURAR
            btnRestaurar =
                CriarBotao(
                    "RESTAURAR TELA",
                    new Point(35, 370),
                    Color.FromArgb(0, 145, 80));

            btnRestaurar.Click +=
                (s, e) =>
                {
                    Restaurar();
                };

            Controls.Add(btnRestaurar);

            // INFORMAÇÃO
            Label info =
                new Label
                {
                    Text =
                        "O aviso será aplicado em todos os monitores.",

                    ForeColor =
                        Color.Gray,

                    AutoSize = true,

                    Location =
                        new Point(250, 390)
                };

            Controls.Add(info);

            FormClosed +=
                (s, e) =>
                {
                    Restaurar();
                };
        }

        private Button CriarBotao(
            string texto,
            Point localizacao,
            Color cor)
        {
            Button botao =
                new Button
                {
                    Text = texto,

                    Location =
                        localizacao,

                    Size =
                        new Size(
                            190,
                            55),

                    BackColor =
                        cor,

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand
                };

            botao.FlatAppearance.BorderSize = 0;

            return botao;
        }

        private Button CriarBotaoPequeno(
            string texto,
            Point localizacao)
        {
            Button botao =
                new Button
                {
                    Text = texto,

                    Location =
                        localizacao,

                    Size =
                        new Size(
                            190,
                            45),

                    BackColor =
                        Color.FromArgb(
                            45,
                            50,
                            58),

                    ForeColor =
                        Color.White,

                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand
                };

            botao.FlatAppearance.BorderSize = 0;

            return botao;
        }

        private void EscolherImagem()
        {
            using OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Title =
                "Escolher imagem de aviso";

            dialog.Filter =
                "Imagens|*.png;*.jpg;*.jpeg;*.bmp|" +
                "Todos os arquivos|*.*";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                imagemSelecionada =
                    dialog.FileName;

                lblImagem.Text =
                    "Imagem selecionada:\r\n" +
                    Path.GetFileName(
                        imagemSelecionada);
            }
        }

        private void MostrarTelaPreta()
        {
            Restaurar();

            foreach (Screen tela
                in Screen.AllScreens)
            {
                OverlayForm overlay =
                    new OverlayForm(
                        tela,
                        null,
                        null);

                overlays.Add(overlay);

                overlay.Show();
            }
        }

        private void MostrarImagem()
        {
            if (string.IsNullOrWhiteSpace(
                    imagemSelecionada) ||
                !File.Exists(
                    imagemSelecionada))
            {
                MessageBox.Show(
                    "Escolha uma imagem primeiro.",
                    "RD Screen Guard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            Restaurar();

            foreach (Screen tela
                in Screen.AllScreens)
            {
                OverlayForm overlay =
                    new OverlayForm(
                        tela,
                        imagemSelecionada,
                        null);

                overlays.Add(overlay);

                overlay.Show();
            }
        }

        private void MostrarTexto()
        {
            if (string.IsNullOrWhiteSpace(
                    campoTexto.Text))
            {
                MessageBox.Show(
                    "Digite uma mensagem primeiro.",
                    "RD Screen Guard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            Restaurar();

            foreach (Screen tela
                in Screen.AllScreens)
            {
                OverlayForm overlay =
                    new OverlayForm(
                        tela,
                        null,
                        campoTexto.Text);

                overlays.Add(overlay);

                overlay.Show();
            }
        }

        private void Restaurar()
        {
            foreach (OverlayForm overlay
                in overlays.ToArray())
            {
                if (!overlay.IsDisposed)
                    overlay.Close();
            }

            overlays.Clear();
        }
    }
}
