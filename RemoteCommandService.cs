using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RD.ScreenGuard
{
    public class ScreenGuardDevice
    {
        public string Id { get; set; } = "";
        public string Nome { get; set; } = "";
        public bool Online { get; set; }
    }

    public class ScreenGuardCommand
    {
        public string Id { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string? Texto { get; set; }
        public string? ImagemBase64 { get; set; }
    }

    public static class RemoteCommandService
    {
        private const string ApiUrl =
            "https://rdsistemas.online/index.php?api=rd_screenguard";

        private static readonly HttpClient Http =
            new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

        public static async Task<bool> RegistrarComputadorAsync(
            string token, string deviceId, string nome)
        {
            try
            {
                using var request = CriarRequest(token, new Dictionary<string, string>
                {
                    ["action"] = "device_register",
                    ["device_id"] = deviceId,
                    ["device_name"] = nome
                });

                using var response = await Http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public static async Task<List<ScreenGuardDevice>> ObterComputadoresAsync(
            string token, string deviceId)
        {
            try
            {
                using var request = CriarRequest(token, new Dictionary<string, string>
                {
                    ["action"] = "devices",
                    ["device_id"] = deviceId
                });

                using var response = await Http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<ScreenGuardDevice>();

                using JsonDocument doc =
                    JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                if (!doc.RootElement.TryGetProperty("devices", out var lista) ||
                    lista.ValueKind != JsonValueKind.Array)
                    return new List<ScreenGuardDevice>();

                var resultado = new List<ScreenGuardDevice>();

                foreach (var item in lista.EnumerateArray())
                {
                    resultado.Add(new ScreenGuardDevice
                    {
                        Id = Texto(item, "id", "device_id"),
                        Nome = Texto(item, "nome", "name", "device_name"),
                        Online = Bool(item, "online")
                    });
                }

                return resultado;
            }
            catch { return new List<ScreenGuardDevice>(); }
        }

        public static async Task<bool> EnviarComandoAsync(
            string token,
            string deviceId,
            string tipo,
            string? texto = null,
            string? imagemBase64 = null)
        {
            try
            {
                var dados = new Dictionary<string, string>
                {
                    ["action"] = "send_command",
                    ["device_id"] = deviceId,
                    ["command"] = tipo
                };

                if (!string.IsNullOrWhiteSpace(texto))
                    dados["text"] = texto;

                if (!string.IsNullOrWhiteSpace(imagemBase64))
                    dados["image"] = imagemBase64;

                using var request = CriarRequest(token, dados);
                using var response = await Http.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public static async Task<ScreenGuardCommand?> BuscarComandoAsync(
            string token, string deviceId)
        {
            try
            {
                using var request = CriarRequest(token, new Dictionary<string, string>
                {
                    ["action"] = "poll",
                    ["device_id"] = deviceId
                });

                using var response = await Http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;

                using JsonDocument doc =
                    JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                var root = doc.RootElement;

                if (!Bool(root, "has_command", "hasCommand"))
                    return null;

                if (!root.TryGetProperty("command", out var command))
                    return null;

                return new ScreenGuardCommand
                {
                    Id = Texto(command, "id"),
                    Tipo = Texto(command, "type", "command"),
                    Texto = Texto(command, "text", "texto"),
                    ImagemBase64 = Texto(command, "image", "imagem")
                };
            }
            catch { return null; }
        }

        public static async Task ConfirmarComandoAsync(
            string token, string deviceId, string commandId)
        {
            if (string.IsNullOrWhiteSpace(commandId))
                return;

            try
            {
                using var request = CriarRequest(token, new Dictionary<string, string>
                {
                    ["action"] = "ack",
                    ["device_id"] = deviceId,
                    ["command_id"] = commandId
                });

                using var response = await Http.SendAsync(request);
            }
            catch { }
        }

        private static HttpRequestMessage CriarRequest(
            string token, Dictionary<string, string> dados)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Fallback caso o servidor/proxy não entregue
                // o cabeçalho Authorization ao PHP.
                dados["access_token"] = token;
            }

            request.Content = new FormUrlEncodedContent(dados);
            return request;
        }

        private static string Texto(
            JsonElement root, params string[] nomes)
        {
            foreach (var nome in nomes)
            {
                if (root.TryGetProperty(nome, out var valor))
                {
                    return valor.ValueKind == JsonValueKind.String
                        ? valor.GetString() ?? ""
                        : valor.ToString();
                }
            }

            return "";
        }

        private static bool Bool(
            JsonElement root, params string[] nomes)
        {
            foreach (var nome in nomes)
            {
                if (!root.TryGetProperty(nome, out var valor))
                    continue;

                if (valor.ValueKind == JsonValueKind.True)
                    return true;

                if (valor.ValueKind == JsonValueKind.Number &&
                    valor.TryGetInt32(out var numero))
                    return numero == 1;

                if (valor.ValueKind == JsonValueKind.String)
                {
                    string texto = valor.GetString() ?? "";
                    return texto == "1" ||
                           texto.Equals("true",
                               StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;
        }
    }
}
