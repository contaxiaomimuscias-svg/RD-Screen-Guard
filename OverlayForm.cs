using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class OverlayForm : Form
    {
        public OverlayForm(
            Screen tela,
            string? imagem,
            string? textoAviso)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = tela.Bounds;

            TopMost = true;
            ShowInTaskbar = false;

            BackColor = Color.Black;

            // Imagem de fundo
            if (!string.IsNullOrWhiteSpace(imagem))
            {
                BackgroundImage = Image.FromFile(imagem);
                BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Texto do aviso
            if (!string.IsNullOrWhiteSpace(textoAviso))
            {
                Label aviso = new Label
                {
                    Text = textoAviso,
                    Dock = DockStyle.Fill,

                    ForeColor = Color.White,
                    BackColor = Color.Transparent,

                    Font = new Font(
                        "Segoe UI",
                        42,
                        FontStyle.Bold
                    ),

                    TextAlign = ContentAlignment.MiddleCenter,

                    Padding = new Padding(50)
                };

                Controls.Add(aviso);
            }
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            Image? imagem = BackgroundImage;

            BackgroundImage = null;

            imagem?.Dispose();

            base.OnFormClosed(e);
        }
    }
}
