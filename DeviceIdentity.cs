using System;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace RD.ScreenGuard
{
    public static class DeviceIdentity
    {
        private const string RegistryPath = @"Software\RDTech\ScreenGuard";

        public static string GetDeviceId()
        {
            using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            string? id = key?.GetValue("DeviceId") as string;
            if (!string.IsNullOrWhiteSpace(id)) return id;
            string novoId = "SG-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(6));
            key?.SetValue("DeviceId", novoId);
            return novoId;
        }

        public static string GetDeviceName()
        {
            using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            return key?.GetValue("DeviceName") as string ?? "COMPUTADOR 01";
        }

        public static void SetDeviceName(string nome)
        {
            using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key?.SetValue("DeviceName", string.IsNullOrWhiteSpace(nome) ? "COMPUTADOR 01" : nome);
        }
    }
}
