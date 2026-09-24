using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class LoginForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtSenha;
        private Button btnEntrar;

        public LoginForm()
        {
            CriarInterface();
        }

        private void CriarInterface()
        {
            Text = "RD Screen Guard - Login";

            StartPosition =
                FormStartPosition.CenterScreen;

            Size =
                new Size(430, 390);

            BackColor =
                Color.FromArgb(20, 24, 30);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            Label titulo = new Label
            {
                Text = "RD SCREEN GUARD",

                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    24,
                    FontStyle.Bold),

                AutoSize = true,

                Location =
                    new Point(65, 35)
            };

            Controls.Add(titulo);

            Label subtitulo = new Label
            {
                Text = "Acesso ao painel de controle",

                ForeColor = Color.Silver,

                Font = new Font(
                    "Segoe UI",
                    10),

                AutoSize = true,

                Location =
                    new Point(105, 80)
            };

            Controls.Add(subtitulo);

            Label usuario = new Label
            {
                Text = "Usuário",

                ForeColor = Color.White,

                AutoSize = true,

                Location =
                    new Point(55, 125)
            };

            Controls.Add(usuario);

            txtUsuario = new TextBox
            {
                Location =
                    new Point(55, 150),

                Size =
                    new Size(310, 32),

                Font =
                    new Font(
                        "Segoe UI",
                        11)
            };

            Controls.Add(txtUsuario);

            Label senha = new Label
            {
                Text = "Senha",

                ForeColor = Color.White,

                AutoSize = true,

                Location =
                    new Point(55, 195)
            };

            Controls.Add(senha);

            txtSenha = new TextBox
            {
                Location =
                    new Point(55, 220),

                Size =
                    new Size(310, 32),

                Font =
                    new Font(
                        "Segoe UI",
                        11),

                UseSystemPasswordChar = true
            };

            Controls.Add(txtSenha);

            btnEntrar = new Button
            {
                Text = "ENTRAR",

                Location =
                    new Point(55, 275),

                Size =
                    new Size(310, 48),

                BackColor =
                    Color.FromArgb(
                        0,
                        120,
                        215),

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        11,
                        FontStyle.Bold),

                FlatStyle =
                    FlatStyle.Flat,

                Cursor =
                    Cursors.Hand
            };

            btnEntrar.FlatAppearance.BorderSize = 0;

            btnEntrar.Click +=
                (s, e) =>
                {
                    FazerLogin();
                };

            Controls.Add(btnEntrar);

            AcceptButton = btnEntrar;
        }

        private void FazerLogin()
        {
            string usuario =
                txtUsuario.Text.Trim();

            string senha =
                txtSenha.Text;

            /*
             * LOGIN TEMPORÁRIO
             *
             * Na próxima etapa vamos trocar
             * esta validação pelo login do
             * RD Sistemas.
             */

            if (usuario == "admin" &&
                senha == "1234")
            {
                DialogResult =
                    DialogResult.OK;

                Close();

                return;
            }

            MessageBox.Show(
                "Usuário ou senha incorretos.",
                "RD Screen Guard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            txtSenha.Clear();
            txtSenha.Focus();
        }
    }
}
