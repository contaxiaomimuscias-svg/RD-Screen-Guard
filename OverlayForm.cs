using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class OverlayForm : Form
    {
        public OverlayForm(Screen tela, string? imagem)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;

            Bounds = tela.Bounds;

            TopMost = true;
            ShowInTaskbar = false;

            BackColor = Color.Black;

            KeyPreview = true;

            if (!string.IsNullOrWhiteSpace(imagem))
            {
                BackgroundImage = Image.FromFile(imagem);
                BackgroundImageLayout = ImageLayout.Stretch;
            }

            CriarBotaoSaida();

            KeyDown += OverlayForm_KeyDown;
        }

        private void CriarBotaoSaida()
        {
            Button sair = new Button
            {
                Text = "SAIR  (ESC)",
                AutoSize = true,
                Location = new Point(20, 20),

                BackColor = Color.FromArgb(210, 0, 0, 0),
                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                ),

                FlatStyle = FlatStyle.Flat,

                Padding = new Padding(
                    12,
                    7,
                    12,
                    7
                ),

                Cursor = Cursors.Hand
            };

            sair.FlatAppearance.BorderSize = 0;

            sair.Click += (sender, e) =>
            {
                Close();
            };

            Controls.Add(sair);
        }

        private void OverlayForm_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            Image? imagem = BackgroundImage;

            BackgroundImage = null;

            if (imagem != null)
            {
                imagem.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}
