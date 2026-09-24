using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RD.ScreenGuard
{
    public class RdSistemasAuthResult
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = "";
        public string? Empresa { get; set; }
        public string? Plano { get; set; }
        public string? Validade { get; set; }
        public string? Tipo { get; set; }
        public int DiasRestantes { get; set; }
        public string? AccessToken { get; set; }
    }

    public static class RdSistemasAuthService
    {
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private const string LoginUrl = "https://rdsistemas.online/index.php?api=rd_screenguard&action=login";

        public static async Task<RdSistemasAuthResult> LoginAsync(string email,string senha)
        {
            try
            {
                using var dados=new FormUrlEncodedContent(new Dictionary<string,string>{{"action","login"},{"email",email},{"senha",senha}});
                using HttpResponseMessage resposta=await httpClient.PostAsync(LoginUrl,dados);
                string conteudo=await resposta.Content.ReadAsStringAsync();
                if(!resposta.IsSuccessStatusCode){try{using var erro=JsonDocument.Parse(conteudo);string msg=Texto(erro.RootElement,"message","mensagem","erro");if(!string.IsNullOrWhiteSpace(msg))return new RdSistemasAuthResult{Mensagem=msg};}catch{}return new RdSistemasAuthResult{Mensagem="Servidor RD Sistemas recusou a conexão. Código HTTP: "+(int)resposta.StatusCode};}
                if(string.IsNullOrWhiteSpace(conteudo))return new RdSistemasAuthResult{Mensagem="O servidor RD Sistemas não retornou uma resposta."};
                using JsonDocument json=JsonDocument.Parse(conteudo);var root=json.RootElement;bool sucesso=Bool(root,"success","sucesso","ok","logged");string status=Texto(root,"status");if(!sucesso&&(status.Equals("success",StringComparison.OrdinalIgnoreCase)||status.Equals("ok",StringComparison.OrdinalIgnoreCase)))sucesso=true;string mensagem=Texto(root,"message","mensagem","erro");if(string.IsNullOrWhiteSpace(mensagem))mensagem=sucesso?"Login realizado com sucesso.":"E-mail ou senha incorretos.";
                return new RdSistemasAuthResult{Sucesso=sucesso,Mensagem=mensagem,Empresa=Texto(root,"empresa","empresa_nome","company","company_name"),Plano=Texto(root,"plano","plan"),Validade=Texto(root,"validade","validade_plano","expira","expiry"),Tipo=Texto(root,"tipo","type"),DiasRestantes=Inteiro(root,"dias_restantes","diasRestantes"),AccessToken=Texto(root,"access_token","token","session_token")};
            }
            catch(HttpRequestException){return new RdSistemasAuthResult{Mensagem="Não foi possível acessar rdsistemas.online."};}
            catch(TaskCanceledException){return new RdSistemasAuthResult{Mensagem="Tempo de conexão com o servidor esgotado."};}
            catch(JsonException){return new RdSistemasAuthResult{Mensagem="O servidor RD Sistemas não retornou um JSON válido."};}
            catch(Exception ex){return new RdSistemasAuthResult{Mensagem="Erro de conexão: "+ex.Message};}
        }
        private static bool Bool(JsonElement r,params string[] n){foreach(var x in n)if(r.TryGetProperty(x,out var v)){if(v.ValueKind==JsonValueKind.True)return true;if(v.ValueKind==JsonValueKind.Number&&v.TryGetInt32(out var i))return i==1;if(v.ValueKind==JsonValueKind.String){var s=v.GetString()??"";if(s=="1"||s.Equals("true",StringComparison.OrdinalIgnoreCase)||s.Equals("ok",StringComparison.OrdinalIgnoreCase)||s.Equals("success",StringComparison.OrdinalIgnoreCase))return true;}}return false;}
        private static string Texto(JsonElement r,params string[] n){foreach(var x in n)if(r.TryGetProperty(x,out var v))return v.ValueKind==JsonValueKind.String?v.GetString()??"":v.ToString();return "";}
        private static int Inteiro(JsonElement r,params string[] n){foreach(var x in n)if(r.TryGetProperty(x,out var v)){if(v.ValueKind==JsonValueKind.Number&&v.TryGetInt32(out var i))return i;if(v.ValueKind==JsonValueKind.String&&int.TryParse(v.GetString(),out var j))return j;}return 0;}
    }
}
