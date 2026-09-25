using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RD.ScreenGuard
{
    public class LoginForm : Form
    {
        private TextBox txtEmail = null!;
        private TextBox txtSenha = null!;
        private Button btnEntrar = null!;
        private Label lblStatus = null!;
        private CheckBox chkLembrar = null!;

        public string EmailAutenticado { get; private set; } = "";

        // Resultado completo retornado pelo servidor RD Sistemas
        public RdSistemasAuthResult? ResultadoLogin { get; private set; }

        public LoginForm()
        {
            Text = "RD Screen Guard - Login";
            Width = 430;
            Height = 430;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(18, 18, 22);

            CriarInterface();
        }

        private void CriarInterface()
        {
            Label titulo = new Label
            {
                Text = "RD SCREEN GUARD",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(55, 45)
            };

            Label subtitulo = new Label
            {
                Text = "Entre com sua conta RD Sistemas",
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(85, 90)
            };

            Label lblEmail = new Label
            {
                Text = "E-mail",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(55, 135)
            };

            txtEmail = new TextBox
            {
                Width = 310,
                Location = new Point(55, 160),
                Font = new Font("Segoe UI", 11)
            };

            Label lblSenha = new Label
            {
                Text = "Senha",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(55, 205)
            };

            txtSenha = new TextBox
            {
                Width = 310,
                Location = new Point(55, 230),
                Font = new Font("Segoe UI", 11),
                UseSystemPasswordChar = true
            };

            chkLembrar = new CheckBox
            {
                Text = "Lembrar meu e-mail",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(55, 270)
            };

            btnEntrar = new Button
            {
                Text = "ENTRAR",
                Width = 310,
                Height = 45,
                Location = new Point(55, 305),
                BackColor = Color.FromArgb(0, 150, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.Click += BtnEntrar_Click;

            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.OrangeRed,
                AutoSize = false,
                Width = 310,
                Height = 40,
                Location = new Point(55, 355),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(titulo);
            Controls.Add(subtitulo);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblSenha);
            Controls.Add(txtSenha);
            Controls.Add(chkLembrar);
            Controls.Add(btnEntrar);
            Controls.Add(lblStatus);

            AcceptButton = btnEntrar;
        }

        private async void BtnEntrar_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                lblStatus.Text = "Digite seu e-mail.";
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                lblStatus.Text = "Digite sua senha.";
                txtSenha.Focus();
                return;
            }

            btnEntrar.Enabled = false;
            btnEntrar.Text = "CONECTANDO...";
            lblStatus.ForeColor = Color.LightGray;
            lblStatus.Text = "Verificando sua conta...";

            try
            {
                RdSistemasAuthResult resultado =
                    await RdSistemasAuthService.LoginAsync(email, senha);

                if (resultado.Sucesso)
                {
                    // Guarda os dados de autenticação para o MainForm
                    ResultadoLogin = resultado;
                    EmailAutenticado = email;

                    lblStatus.ForeColor = Color.LightGreen;
                    lblStatus.Text = "Login realizado com sucesso.";

                    await Task.Delay(300);

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                ResultadoLogin = null;

                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = resultado.Mensagem;
            }
            catch (Exception ex)
            {
                ResultadoLogin = null;

                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "Erro: " + ex.Message;
            }
            finally
            {
                btnEntrar.Enabled = true;
                btnEntrar.Text = "ENTRAR";
            }
        }
    }
}
