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
    }

    public static class RdSistemasAuthService
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        private const string LoginUrl =
            "https://rdsistemas.online/index.php?api=rdtechclean&action=login";

        public static async Task<RdSistemasAuthResult> LoginAsync(
            string email,
            string senha)
        {
            try
            {
                using var dados = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        ["action"] = "login",
                        ["email"] = email,
                        ["senha"] = senha
                    });

                using HttpResponseMessage resposta =
                    await httpClient.PostAsync(LoginUrl, dados);

                string conteudo =
                    await resposta.Content.ReadAsStringAsync();

                if (!resposta.IsSuccessStatusCode)
                {
                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem = "Não foi possível conectar ao servidor RD Sistemas."
                    };
                }

                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem = "O servidor não retornou uma resposta."
                    };
                }

                try
                {
                    using JsonDocument json =
                        JsonDocument.Parse(conteudo);

                    JsonElement root = json.RootElement;

                    bool sucesso = ObterBooleano(
                        root,
                        "success",
                        "sucesso",
                        "ok",
                        "logged"
                    );

                    string status = ObterTexto(
                        root,
                        "status"
                    );

                    if (!sucesso &&
                        status.Equals(
                            "success",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        sucesso = true;
                    }

                    if (!sucesso &&
                        status.Equals(
                            "ok",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        sucesso = true;
                    }

                    string mensagem = ObterTexto(
                        root,
                        "message",
                        "mensagem",
                        "erro"
                    );

                    if (string.IsNullOrWhiteSpace(mensagem))
                    {
                        mensagem = sucesso
                            ? "Login realizado com sucesso."
                            : "E-mail ou senha inválidos.";
                    }

                    return new RdSistemasAuthResult
                    {
                        Sucesso = sucesso,
                        Mensagem = mensagem,
                        Empresa = ObterTexto(
                            root,
                            "empresa",
                            "empresa_nome",
                            "company",
                            "company_name"
                        ),
                        Plano = ObterTexto(
                            root,
                            "plano",
                            "plan"
                        ),
                        Validade = ObterTexto(
                            root,
                            "validade",
                            "validade_plano",
                            "expira",
                            "expiry"
                        )
                    };
                }
                catch (JsonException)
                {
                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem = "Resposta inválida do servidor RD Sistemas."
                    };
                }
            }
            catch (HttpRequestException)
            {
                return new RdSistemasAuthResult
                {
                    Sucesso = false,
                    Mensagem =
                        "Não foi possível acessar rdsistemas.online."
                };
            }
            catch (TaskCanceledException)
            {
                return new RdSistemasAuthResult
                {
                    Sucesso = false,
                    Mensagem = "Tempo de conexão esgotado."
                };
            }
            catch (Exception ex)
            {
                return new RdSistemasAuthResult
                {
                    Sucesso = false,
                    Mensagem = "Erro: " + ex.Message
                };
            }
        }

        private static bool ObterBooleano(
            JsonElement root,
            params string[] nomes)
        {
            foreach (string nome in nomes)
            {
                if (root.TryGetProperty(nome, out JsonElement valor))
                {
                    if (valor.ValueKind == JsonValueKind.True)
                        return true;

                    if (valor.ValueKind == JsonValueKind.False)
                        return false;

                    if (valor.ValueKind == JsonValueKind.String)
                    {
                        string texto =
                            valor.GetString() ?? "";

                        if (texto == "1" ||
                            texto.Equals(
                                "true",
                                StringComparison.OrdinalIgnoreCase))
                            return true;
                    }

                    if (valor.ValueKind == JsonValueKind.Number &&
                        valor.TryGetInt32(out int numero))
                    {
                        return numero == 1;
                    }
                }
            }

            return false;
        }

        private static string ObterTexto(
            JsonElement root,
            params string[] nomes)
        {
            foreach (string nome in nomes)
            {
                if (root.TryGetProperty(nome, out JsonElement valor))
                {
                    if (valor.ValueKind == JsonValueKind.String)
                        return valor.GetString() ?? "";

                    return valor.ToString();
                }
            }

            return "";
        }
    }
}
