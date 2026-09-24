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
        private Timer? timer;
        private CheckBox pc1 = null!, pc2 = null!;
        private RadioButton black = null!, image = null!, text = null!;
        private TextBox message = null!;
        private Label status = null!, imageLabel = null!;
        private Button execute = null!, restore = null!;

        public MainForm(RdSistemasAuthResult result)
        {
            auth=result; deviceId=DeviceIdentity.GetDeviceId();
            BuildUi(); StartRemoteLoop();
        }

        private void BuildUi()
        {
            Text="RD Screen Guard"; StartPosition=FormStartPosition.CenterScreen; ClientSize=new Size(1120,760); BackColor=Color.FromArgb(4,8,13); Font=new Font("Segoe UI",10);
            Add(new Label{Text="🛡 RD SCREEN GUARD",ForeColor=Color.White,Font=new Font("Segoe UI",24,FontStyle.Bold),AutoSize=true,Location=new Point(30,25)});
            Add(new Label{Text="CONTROLE DE TELAS REMOTO",ForeColor=Color.FromArgb(70,180,255),Font=new Font("Segoe UI",10,FontStyle.Bold),AutoSize=true,Location=new Point(34,70)});
            status=new Label{Text="● Conectando...",ForeColor=Color.Gold,Font=new Font("Segoe UI",11,FontStyle.Bold),AutoSize=true,Location=new Point(850,45)};Add(status);

            Panel devices=Panel("🖥 COMPUTADORES CONECTADOS",new Point(25,110),new Size(420,300));
            pc1=Check("COMPUTADOR 01",new Point(20,60)); pc2=Check("COMPUTADOR 02",new Point(20,125));
            pc1.CheckedChanged+=(s,e)=>{if(pc1.Checked)pc2.Checked=false;}; pc2.CheckedChanged+=(s,e)=>{if(pc2.Checked)pc1.Checked=false;};
            devices.Controls.Add(pc1);devices.Controls.Add(pc2);
            devices.Controls.Add(new Label{Text=$"ID deste computador: {deviceId}",ForeColor=Color.Silver,AutoSize=true,Location=new Point(20,195)});
            devices.Controls.Add(new Label{Text=$"Empresa: {auth.Empresa??"-"}",ForeColor=Color.Silver,AutoSize=true,Location=new Point(20,225)});
            devices.Controls.Add(new Label{Text=$"Plano: {auth.Plano??"-"}",ForeColor=Color.Silver,AutoSize=true,Location=new Point(20,255)});Add(devices);

            Panel actions=Panel("⚙ AÇÃO DE TELA",new Point(465,110),new Size(390,300));
            black=Radio("Tela Preta — Escurece a tela do computador selecionado.",new Point(20,60),true); image=Radio("Usar Imagem — Mostra uma imagem na tela.",new Point(20,110)); text=Radio("Mensagem Personalizada — Mostra um aviso.",new Point(20,160));
            actions.Controls.Add(black);actions.Controls.Add(image);actions.Controls.Add(text);
            actions.Controls.Add(new Label{Text="CONFIGURAÇÕES DA MENSAGEM / IMAGEM",ForeColor=Color.FromArgb(70,180,255),Font=new Font("Segoe UI",9,FontStyle.Bold),AutoSize=true,Location=new Point(20,205)});
            message=new TextBox{Multiline=true,ForeColor=Color.White,BackColor=Color.FromArgb(10,18,28),Location=new Point(20,230),Size=new Size(350,55),Text="MANUTENÇÃO EM ANDAMENTO\r\nPOR FAVOR AGUARDE..."}; actions.Controls.Add(message);Add(actions);

            execute=Button("▶  EXECUTAR AÇÃO",new Point(465,430),new Size(390,58),Color.FromArgb(0,105,235));execute.Click+=async(s,e)=>await ExecuteAsync();Add(execute);
            restore=Button("■  RESTAURAR TELA",new Point(465,500),new Size(390,58),Color.FromArgb(190,45,45));restore.Click+=async(s,e)=>await RestoreAsync();Add(restore);
            Button choose=Button("🖼  SELECIONAR IMAGEM",new Point(880,500),new Size(210,48),Color.FromArgb(25,35,48));choose.Click+=(s,e)=>ChooseImage();Add(choose);
            imageLabel=new Label{Text="Nenhuma imagem selecionada",ForeColor=Color.Silver,Size=new Size(210,80),Location=new Point(880,560),TextAlign=ContentAlignment.MiddleCenter};Add(imageLabel);

            Panel info=Panel("ℹ COMPUTADOR SELECIONADO",new Point(25,430),new Size(420,210)); info.Controls.Add(new Label{Text="Marque COMPUTADOR 01 ou COMPUTADOR 02.\r\n\r\nO comando será enviado somente ao computador\r\nselecionado quando ele estiver conectado ao\r\nRD Screen Guard.",ForeColor=Color.Gainsboro,AutoSize=false,Size=new Size(370,125),Location=new Point(20,55)});Add(info);
            Add(new Label{Text="● Sistema conectado ao RD Sistemas     |     RD Screen Guard",ForeColor=Color.FromArgb(80,220,120),AutoSize=true,Location=new Point(30,680)});
            FormClosed+=(s,e)=>{timer?.Stop();RestoreLocal();};
        }

        private void Add(Control c)=>Controls.Add(c);
        private Panel Panel(string title,Point p,Size s){var x=new Panel{Location=p,Size=s,BackColor=Color.FromArgb(10,17,25),BorderStyle=BorderStyle.FixedSingle};x.Controls.Add(new Label{Text=title,ForeColor=Color.FromArgb(70,180,255),Font=new Font("Segoe UI",11,FontStyle.Bold),AutoSize=true,Location=new Point(18,15)});return x;}
        private CheckBox Check(string t,Point p)=>new CheckBox{Text=t,ForeColor=Color.White,AutoSize=true,Font=new Font("Segoe UI",11,FontStyle.Bold),Location=p};
        private RadioButton Radio(string t,Point p,bool c=false)=>new RadioButton{Text=t,ForeColor=Color.White,AutoSize=true,Font=new Font("Segoe UI",9,FontStyle.Bold),Location=p,Checked=c};
        private Button Button(string t,Point p,Size s,Color c){var b=new Button{Text=t,Location=p,Size=s,BackColor=c,ForeColor=Color.White,Font=new Font("Segoe UI",10,FontStyle.Bold),FlatStyle=FlatStyle.Flat,Cursor=Cursors.Hand};b.FlatAppearance.BorderSize=0;return b;}

        private void ChooseImage(){using var d=new OpenFileDialog{Title="Escolher imagem de aviso",Filter="Imagens|*.png;*.jpg;*.jpeg;*.bmp"};if(d.ShowDialog()!=DialogResult.OK)return;imageFile=d.FileName;imageLabel.Text="Imagem selecionada:\r\n"+Path.GetFileName(imageFile);image.Checked=true;}
        private string? Target(){if(pc1.Checked)return deviceId;if(pc2.Checked)return remoteDeviceId;return null;}

        private async Task ExecuteAsync()
        {
            string? target=Target();if(target==null){MessageBox.Show("Selecione COMPUTADOR 01 ou COMPUTADOR 02.","RD Screen Guard");return;}
            string cmd;string? txt=null,img=null;
            if(black.Checked)cmd="black";
            else if(image.Checked){if(!File.Exists(imageFile)){MessageBox.Show("Selecione uma imagem primeiro.","RD Screen Guard");return;}cmd="image";img=Convert.ToBase64String(File.ReadAllBytes(imageFile));}
            else{if(string.IsNullOrWhiteSpace(message.Text)){MessageBox.Show("Digite a mensagem primeiro.","RD Screen Guard");return;}cmd="text";txt=message.Text.Trim();}
            execute.Enabled=false;try{if(target==deviceId){Apply(cmd,txt,img);return;}if(string.IsNullOrWhiteSpace(auth.AccessToken)){MessageBox.Show("A API ainda precisa retornar o token de controle remoto.","RD Screen Guard");return;}if(!await RemoteCommandService.EnviarComandoAsync(auth.AccessToken,target,cmd,txt,img))MessageBox.Show("Não foi possível enviar o comando ao computador selecionado.","RD Screen Guard");}finally{execute.Enabled=true;}
        }

        private async Task RestoreAsync(){string? target=Target();if(target==null){MessageBox.Show("Selecione um computador.","RD Screen Guard");return;}if(target==deviceId){RestoreLocal();return;}if(!string.IsNullOrWhiteSpace(auth.AccessToken))await RemoteCommandService.EnviarComandoAsync(auth.AccessToken,target,"restore");}

        private void StartRemoteLoop(){if(string.IsNullOrWhiteSpace(auth.AccessToken)){status.Text="● Login conectado";return;}timer=new Timer{Interval=5000};timer.Tick+=async(s,e)=>{await UpdateDevicesAsync();await PollAsync();};timer.Start();_ = UpdateDevicesAsync();_ = PollAsync();}

        private async Task UpdateDevicesAsync(){if(string.IsNullOrWhiteSpace(auth.AccessToken))return;bool ok=await RemoteCommandService.RegistrarComputadorAsync(auth.AccessToken,deviceId,"COMPUTADOR 01");var list=await RemoteCommandService.ObterComputadoresAsync(auth.AccessToken,deviceId);var remote=list.Find(x=>x.Id!=deviceId&&x.Online);remoteDeviceId=remote?.Id;pc1.Text=remote==null?"COMPUTADOR 01 (este computador)":"COMPUTADOR 01 (este computador) • Online";pc2.Text=remote==null?"COMPUTADOR 02 • aguardando conexão":$"{(string.IsNullOrWhiteSpace(remote.Nome)?"COMPUTADOR 02":remote.Nome)} • Online";status.Text=ok?"● Conectado":"● Aguardando servidor";status.ForeColor=ok?Color.FromArgb(60,230,120):Color.Gold;}

        private async Task PollAsync(){if(string.IsNullOrWhiteSpace(auth.AccessToken))return;var c=await RemoteCommandService.BuscarComandoAsync(auth.AccessToken,deviceId);if(c==null)return;try{Apply(c.Tipo,c.Texto,c.ImagemBase64);}finally{await RemoteCommandService.ConfirmarComandoAsync(auth.AccessToken,deviceId,c.Id);}}

        private void Apply(string cmd,string? txt,string? img){if(cmd.Equals("restore",StringComparison.OrdinalIgnoreCase)){RestoreLocal();return;}RestoreLocal();foreach(Screen s in Screen.AllScreens){OverlayForm o;if(cmd.Equals("image",StringComparison.OrdinalIgnoreCase)&&!string.IsNullOrWhiteSpace(img)){string f=Path.Combine(Path.GetTempPath(),"rdsg_"+Guid.NewGuid().ToString("N")+".png");File.WriteAllBytes(f,Convert.FromBase64String(img));o=new OverlayForm(s,f,null);}else if(cmd.Equals("text",StringComparison.OrdinalIgnoreCase))o=new OverlayForm(s,null,txt??"AVISO");else o=new OverlayForm(s,null,null);overlays.Add(o);o.Show();}}
        private void RestoreLocal(){foreach(var o in overlays.ToArray())if(!o.IsDisposed)o.Close();overlays.Clear();}
    }
}
