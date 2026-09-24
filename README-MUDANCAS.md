# RD Screen Guard — atualização

Arquivos novos/alterados:
- MainForm.cs — painel no estilo da arte, seleção Computador 01/02, tela preta, imagem, mensagem e restaurar.
- LoginForm.cs — mantém login RD Sistemas e lembrança do e-mail.
- Program.cs — passa a sessão autenticada para o painel.
- RdSistemasAuthService.cs — usa `rd_screenguard` e lê `access_token`/`token`.
- RemoteCommandService.cs — comunicação com a API para registrar computadores, listar dispositivos, enviar comandos e receber comandos.
- DeviceIdentity.cs — cria ID único persistente do computador.

IMPORTANTE: o controle remoto entre os dois PCs exige que o `index.php` do RD Sistemas implemente as ações `device_register`, `devices`, `send_command`, `poll` e `ack` e devolva um token no login. Sem essa parte, o programa abre e funciona localmente, mas os comandos entre PCs não terão servidor para retransmitir.
