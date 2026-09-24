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
        private Button btnRestaurar;
        private Button btnEscolher;
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
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(620, 390);
            MinimumSize = new Size(620, 390);
            BackColor = Color.FromArgb(20, 24, 30);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label titulo = new Label
            {
                Text = "RD SCREEN GUARD",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 30)
            };

            Label subtitulo = new Label
            {
                Text = "Controle visual da tela do computador",
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Location = new Point(38, 78)
            };

            btnTelaPreta = CriarBotao(
                "TELA PRETA",
                new Point(35, 125),
                Color.FromArgb(35, 35, 35)
            );

            btnImagem = CriarBotao(
                "IMAGEM DE AVISO",
                new Point(220, 125),
                Color.FromArgb(0, 105, 180)
            );

            btnRestaurar = CriarBotao(
                "RESTAURAR TELA",
                new Point(405, 125),
                Color.FromArgb(0, 145, 80)
            );

            btnEscolher = CriarBotaoPequeno(
                "ESCOLHER IMAGEM",
                new Point(35, 200)
            );

            lblImagem = new Label
            {
                Text = "Nenhuma imagem selecionada",
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(210, 208)
            };

            Label info = new Label
            {
                Text =
                    "Use o modo de tela preta ou uma imagem personalizada.\r\n" +
                    "ESC também pode ser usado para fechar o aviso.",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(38, 270)
            };

            btnTelaPreta.Click += (s, e) => MostrarTelaPreta();
            btnImagem.Click += (s, e) => MostrarImagem();
            btnRestaurar.Click += (s, e) => Restaurar();
            btnEscolher.Click += (s, e) => EscolherImagem();

            Controls.Add(titulo);
            Controls.Add(subtitulo);
            Controls.Add(btnTelaPreta);
            Controls.Add(btnImagem);
            Controls.Add(btnRestaurar);
            Controls.Add(btnEscolher);
            Controls.Add(lblImagem);
            Controls.Add(info);

            FormClosed += (s, e) => Restaurar();
        }

        private Button CriarBotao(
            string texto,
            Point localizacao,
            Color cor)
        {
            Button botao = new Button
            {
                Text = texto,
                Location = localizacao,
                Size = new Size(170, 55),
                BackColor = cor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            botao.FlatAppearance.BorderSize = 0;

            return botao;
        }

        private Button CriarBotaoPequeno(
            string texto,
            Point localizacao)
        {
            Button botao = new Button
            {
                Text = texto,
                Location = localizacao,
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(45, 50, 58),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            botao.FlatAppearance.BorderSize = 0;

            return botao;
        }

        private void EscolherImagem()
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Escolher imagem de aviso";
            dialog.Filter =
                "Imagens|*.png;*.jpg;*.jpeg;*.bmp|" +
                "Todos os arquivos|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imagemSelecionada = dialog.FileName;

                lblImagem.Text =
                    "Imagem: " +
                    Path.GetFileName(imagemSelecionada);
            }
        }

        private void MostrarTelaPreta()
        {
            Restaurar();

            foreach (Screen tela in Screen.AllScreens)
            {
                OverlayForm overlay =
                    new OverlayForm(tela, null);

                overlays.Add(overlay);

                overlay.Show();
            }
        }

        private void MostrarImagem()
        {
            if (string.IsNullOrWhiteSpace(imagemSelecionada) ||
                !File.Exists(imagemSelecionada))
            {
                MessageBox.Show(
                    "Escolha uma imagem primeiro.",
                    "RD Screen Guard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            Restaurar();

            foreach (Screen tela in Screen.AllScreens)
            {
                OverlayForm overlay =
                    new OverlayForm(
                        tela,
                        imagemSelecionada);

                overlays.Add(overlay);

                overlay.Show();
            }
        }

        private void Restaurar()
        {
            foreach (OverlayForm overlay in overlays.ToArray())
            {
                if (!overlay.IsDisposed)
                    overlay.Close();
            }

            overlays.Clear();
        }
    }
}
