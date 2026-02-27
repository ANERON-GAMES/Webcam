using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

public class PlayerPrefs2
{
    private static Dictionary<string, string> keyValuePairs;
    private static byte[] sessionKey;
    private static byte[] encryptedMasterKey;

    static PlayerPrefs2()
    {
        LoadKeysAndValues();
    }

    private static void InitializeEncryptionKey()
    {
        if (sessionKey != null) return;

        sessionKey = new byte[32];
        RandomNumberGenerator.Create().GetBytes(sessionKey);

        string master = GetEncryptionKey();
        encryptedMasterKey = EncryptWithSession(master);
    }

    private static byte[] EncryptData(string plainText)
    {
        byte[] keyBytes = GetKeyBytes();
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes; // 32 байта благодаря хэшу
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Генерируем случайный IV каждый раз

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Записываем IV в начало потока
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return msEncrypt.ToArray();
            }
        }
    }

    private static string DecryptData(byte[] cipherText)
    {
        byte[] keyBytes = GetKeyBytes();
        if (cipherText.Length < 16)
        {
            throw new InvalidDataException("Encrypted data is too short to contain IV.");
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes; // 32 байта благодаря хэшу
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Извлекаем IV из первых 16 байт
            byte[] iv = new byte[16];
            Array.Copy(cipherText, 0, iv, 0, 16);
            aes.IV = iv;

            if (cipherText.Length == 16)
            {
                // Если только IV, значит данные пустые
                return string.Empty;
            }

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText, 16, cipherText.Length - 16))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }

    private static byte[] GetKeyBytes()
    {
        InitializeEncryptionKey();
        string masterKey = DecryptWithSession(encryptedMasterKey);
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(masterKey));
        }
    }

    public static void SetInt(string key, int value, int defaultValue = 0)
    {
        if (!keyValuePairs.ContainsKey(key))
        {
            keyValuePairs[key] = defaultValue.ToString();
        }

        string valueString = value.ToString();
        SetValue(key, valueString);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        string valueString;
        if (keyValuePairs.TryGetValue(key, out valueString))
        {
            int value;
            if (int.TryParse(valueString, out value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    public static void SetFloat(string key, float value, float defaultValue = 0f)
    {
        if (!keyValuePairs.ContainsKey(key))
        {
            keyValuePairs[key] = defaultValue.ToString("R");
        }

        string valueString = value.ToString("R");
        SetValue(key, valueString);
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        string valueString;
        if (keyValuePairs.TryGetValue(key, out valueString))
        {
            float value;
            if (float.TryParse(valueString, out value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    public static void SetString(string key, string value, string defaultValue = null)
    {
        if (!keyValuePairs.ContainsKey(key))
        {
            keyValuePairs[key] = defaultValue;
        }

        SetValue(key, value);
    }

    public static string GetString(string key, string defaultValue = null)
    {
        string value;
        if (keyValuePairs.TryGetValue(key, out value))
        {
            return value;
        }
        return defaultValue;
    }

    public static bool HasKey(string key)
    {
        return keyValuePairs.ContainsKey(key);
    }

    public static void DeleteKey(string key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            keyValuePairs.Remove(key);
            SaveKeysAndValues();
        }
        else
        {
            Debug2.LogWarning($"<color=#FF8C00>Key '{key}' not found in PlayerPrefs2.</color>");
        }
    }

    public static void DeleteAll()
    {
        keyValuePairs.Clear();
        string savePath = GetSavePath();
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public static void Save()
    {
        SaveKeysAndValues();
    }

    private static void SetValue(string key, string value)
    {
        keyValuePairs[key] = value;
        SaveKeysAndValues();
    }

    private static void LoadKeysAndValues()
    {
        keyValuePairs = new Dictionary<string, string>();

        string savePath = GetSavePath();

        if (File.Exists(savePath))
        {
            try
            {
                byte[] encryptedData = File.ReadAllBytes(savePath);
                string decryptedText = DecryptData(encryptedData);

                using (StringReader reader = new StringReader(decryptedText))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null) break;
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] pair = line.Split('=');
                            if (pair.Length == 2)
                            {
                                string key = pair[0];
                                string value = pair[1];
                                keyValuePairs[key] = value;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug2.LogError("Error reading or decrypting saved data: " + e.Message);
                // Если файл поврежден или не дешифруется, удаляем его и очищаем данные
                File.Delete(savePath);
                keyValuePairs.Clear();
            }
        }
    }

    private static void SaveKeysAndValues()
    {
        string savePath = GetSavePath();

        try
        {
            if (keyValuePairs.Count == 0)
            {
                // Если нет данных, удаляем файл
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in keyValuePairs)
            {
                sb.AppendLine(pair.Key + "=" + pair.Value);
            }
            string plainText = sb.ToString();

            byte[] encryptedData = EncryptData(plainText);
            File.WriteAllBytes(savePath, encryptedData);
        }
        catch (Exception e)
        {
            Debug2.LogError("Error encrypting or saving data: " + e.Message);
        }
    }

    public static string GetSavePath()
    {
        string fileName = "save2.dat";
        string savePath = Path.Combine(Application.persistentDataPath, fileName);
        return savePath;
    }

    public static List<string> GetAllKeys()
    {
        return new List<string>(keyValuePairs.Keys);
    }

    public static void SetAllKeys(Dictionary<string, string> values)
    {
        foreach (var kvp in values)
        {
            keyValuePairs[kvp.Key] = kvp.Value;
        }
        Save();
    }

    public static void LogAllValues()
    {
        if (keyValuePairs.Count == 0)
        {
            Debug2.Log("<color=white>No data saved.</color>");
            return;
        }

        string logMessage = "List of all data:\n";
        foreach (var kvp in keyValuePairs)
        {
            logMessage += $"Key: {kvp.Key}, Value: {kvp.Value}\n";
        }

        Debug2.Log(logMessage);
    }

    public static string GetEncryptionKey()
    {
        return "a_kcL(&ho<–tt(WN^Vgx4pEUFFkROP:X";
    }

    public static byte[] CompressData(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return data;
        }

        using (MemoryStream compressedStream = new MemoryStream())
        using (GZipStream gzip = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return compressedStream.ToArray();
        }
    }

    public static byte[] DecompressData(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return data;
        }

        try
        {
            using (MemoryStream compressedStream = new MemoryStream(data))
            using (GZipStream gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (MemoryStream outputStream = new MemoryStream())
            {
                gzip.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }
        catch (InvalidDataException)
        {
            return data;
        }
    }

    private static byte[] EncryptWithSession(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = sessionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return msEncrypt.ToArray();
            }
        }
    }

    private static string DecryptWithSession(byte[] cipherText)
    {
        if (cipherText.Length < 16)
        {
            throw new InvalidDataException("Encrypted master key is too short to contain IV.");
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = sessionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[16];
            Array.Copy(cipherText, 0, iv, 0, 16);
            aes.IV = iv;

            if (cipherText.Length == 16)
            {
                return string.Empty;
            }

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText, 16, cipherText.Length - 16))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }

}
