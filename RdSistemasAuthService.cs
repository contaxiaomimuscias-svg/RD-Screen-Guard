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
    }

    public static class RdSistemasAuthService
    {
        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        // API exclusiva do RD Screen Guard
        private const string LoginUrl =
            "https://rdsistemas.online/index.php?api=rd_screenguard&action=login";

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
                    try
                    {
                        using JsonDocument erroJson =
                            JsonDocument.Parse(conteudo);

                        string mensagemErro =
                            ObterTexto(
                                erroJson.RootElement,
                                "message",
                                "mensagem",
                                "erro"
                            );

                        if (!string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            return new RdSistemasAuthResult
                            {
                                Sucesso = false,
                                Mensagem = mensagemErro
                            };
                        }
                    }
                    catch
                    {
                        // Resposta não era JSON.
                    }

                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem =
                            "Servidor RD Sistemas recusou a conexão. Código HTTP: "
                            + (int)resposta.StatusCode
                    };
                }

                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem =
                            "O servidor RD Sistemas não retornou uma resposta."
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

                    string status =
                        ObterTexto(root, "status");

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

                    string mensagem =
                        ObterTexto(
                            root,
                            "message",
                            "mensagem",
                            "erro"
                        );

                    if (string.IsNullOrWhiteSpace(mensagem))
                    {
                        mensagem = sucesso
                            ? "Login realizado com sucesso."
                            : "E-mail ou senha incorretos.";
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
                        ),

                        Tipo = ObterTexto(
                            root,
                            "tipo",
                            "type"
                        ),

                        DiasRestantes =
                            ObterInteiro(
                                root,
                                "dias_restantes",
                                "diasRestantes"
                            )
                    };
                }
                catch (JsonException)
                {
                    return new RdSistemasAuthResult
                    {
                        Sucesso = false,
                        Mensagem =
                            "O servidor RD Sistemas não retornou um JSON válido."
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
                    Mensagem =
                        "Tempo de conexão com o servidor esgotado."
                };
            }
            catch (Exception ex)
            {
                return new RdSistemasAuthResult
                {
                    Sucesso = false,
                    Mensagem =
                        "Erro de conexão: " + ex.Message
                };
            }
        }

        private static bool ObterBooleano(
            JsonElement root,
            params string[] nomes)
        {
            foreach (string nome in nomes)
            {
                if (!root.TryGetProperty(
                    nome,
                    out JsonElement valor))
                {
                    continue;
                }

                if (valor.ValueKind ==
                    JsonValueKind.True)
                {
                    return true;
                }

                if (valor.ValueKind ==
                    JsonValueKind.False)
                {
                    return false;
                }

                if (valor.ValueKind ==
                    JsonValueKind.String)
                {
                    string texto =
                        valor.GetString() ?? "";

                    if (texto == "1" ||
                        texto.Equals(
                            "true",
                            StringComparison.OrdinalIgnoreCase) ||
                        texto.Equals(
                            "ok",
                            StringComparison.OrdinalIgnoreCase) ||
                        texto.Equals(
                            "success",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                if (valor.ValueKind ==
                    JsonValueKind.Number &&
                    valor.TryGetInt32(
                        out int numero))
                {
                    return numero == 1;
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
                if (!root.TryGetProperty(
                    nome,
                    out JsonElement valor))
                {
                    continue;
                }

                if (valor.ValueKind ==
                    JsonValueKind.String)
                {
                    return valor.GetString() ?? "";
                }

                return valor.ToString();
            }

            return "";
        }

        private static int ObterInteiro(
            JsonElement root,
            params string[] nomes)
        {
            foreach (string nome in nomes)
            {
                if (!root.TryGetProperty(
                    nome,
                    out JsonElement valor))
                {
                    continue;
                }

                if (valor.ValueKind ==
                    JsonValueKind.Number &&
                    valor.TryGetInt32(
                        out int numero))
                {
                    return numero;
                }

                if (valor.ValueKind ==
                    JsonValueKind.String &&
                    int.TryParse(
                        valor.GetString(),
                        out int numeroTexto))
                {
                    return numeroTexto;
                }
            }

            return 0;
        }
    }
}
