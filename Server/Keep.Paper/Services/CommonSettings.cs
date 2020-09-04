using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Services
{
  internal class CommonSettings : ICommonSettings
  {
    // Como gerar uma chave?
    // using (var des = new System.Security.Cryptography.TripleDESCryptoServiceProvider())
    // {
    //   des.GenerateKey();
    //   Console.WriteLine(Convert.ToBase64String(des.Key));
    // }
    private readonly IAudit<CommonSettings> audit;

    private HashMap<string> cache;

    public CommonSettings(IAudit<CommonSettings> audit)
    {
      this.audit = audit;
      this.cache = LoadAllKeys();
    }

    public ICollection<string> Keys => cache.Keys;

    public string CanonicalizeKey(string key)
    {
      return key.ChangeCase(TextCase.Hyphenated | TextCase.PreserveSpecialCharacters);
    }

    public string Get(string key, string defaultValue = null)
    {
      key = CanonicalizeKey(key);
      return cache[key];
    }

    public T Get<T>(string key, T defaultValue = default)
    {
      try
      {
        key = CanonicalizeKey(key);
        var sourceValue = cache[key];
        var targetValue = Change.Try<T>(sourceValue, defaultValue);
        return targetValue;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Não foi possível ler a configuração do sistema.",
            $"Chave: {key}",
            $"Tipo esperado: {typeof(T).FullName}",
            ex));
        return defaultValue;
      }
    }

    public Ret Set(string key, object value)
    {
      key = CanonicalizeKey(key);
      var text = Change.To<string>(value);
      return SaveKey(cache, key, text);
    }

    public void Reload()
    {
      this.cache = LoadAllKeys();
    }

    private HashMap<string> LoadAllKeys()
    {
      string folder = null;
      try
      {
        folder = LocateCommonSettingsFolder();

        var entries = new HashMap<string>();

        var filepaths = Directory.GetFiles(folder, "*.key");
        foreach (var filepath in filepaths)
        {
          var key = Path.GetFileNameWithoutExtension(filepath);
          try
          {
            var value = File.ReadAllText(filepath);
            value = Api.Crypto.Decrypt(value);
            entries.Add(key, value);
          }
          catch (Exception ex)
          {
            audit.LogDanger(
              To.Text(
                "Não foi possível ler o valor da chave de configuração.",
                $"Chave: {key}",
                $"Local: {filepath}",
                ex));
          }
        }

        return entries;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Não foi possível recuperar as configurações comuns de sistema.",
            $"Local: {folder ?? "(não localizado)"}",
            ex));

        // Vamos deixar tudo como está, retornando a coleção de entradas atuais
        return this.cache ?? new HashMap<string>();
      }
    }

    private Ret SaveKey(HashMap<string> map, string key, string value)
    {
      map.Add(key, value);

      string folder = null;
      string file = null;
      try
      {
        folder = LocateCommonSettingsFolder();
        file = Path.Combine(folder, $"{key}.key");

        value = Api.Crypto.Encrypt(value);

        File.WriteAllText(file, value);

        return true;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Não foi possível gravar a configuração do sistema.",
            $"Chave: {key}",
            $"Local: {file ?? folder ?? "(não localizado)"}",
            ex));

        return ex;
      }
    }

    private string LocateCommonSettingsFolder()
    {
      var folder = new DirectoryInfo(App.Path);

      var file = Path.GetFileName(GetType().Assembly.Location);
      var current = folder;
      while ((current = current.Parent) != null)
      {
        var isPaperFolder = current.EnumerateFiles(file).Any();
        if (isPaperFolder)
        {
          folder = current;
        }
      }

      var settingsFolder = Path.Combine(folder.FullName, ".settings");
      if (!Directory.Exists(settingsFolder))
      {
        Directory.CreateDirectory(settingsFolder);
      }

      return settingsFolder;
    }
  }
}
