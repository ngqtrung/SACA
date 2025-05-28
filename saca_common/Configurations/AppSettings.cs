﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace SACA_Common.Configurations;

public class AppSettings
{
    private static AppSettings _instance = null!;
    private static readonly object ObjLocked = new();
    private IConfiguration _configuration = null!;

    public static AppSettings Instance
    {
        get
        {
            if (_instance == null)
                lock (ObjLocked)
                {
                    if (_instance == null) _instance = new AppSettings();
                }

            return _instance;
        }
    }

    protected AppSettings() { }
    public void SetConfiguration(IConfiguration configuration) { _configuration = configuration; }

    //public bool GetBool(string key, bool defaultValue = false)
    //{
    //    try
    //    {
    //        return _configuration.GetSection("StringValue").GetChildren().FirstOrDefault((IConfigurationSection x) => x.Key == key)!.Value.ToBool();
    //    }
    //    catch
    //    {
    //        return defaultValue;
    //    }
    //}

    public string GetConnection(string key, string defaultValue = "")
    {
        try { return _configuration.GetConnectionString(key)!; }
        catch { return defaultValue; }
    }

    //public int GetInt32(string key, int defaultValue = 0)
    //{
    //    try
    //    {
    //        return _configuration.GetSection("StringValue").GetChildren().FirstOrDefault((IConfigurationSection x) => x.Key == key)!.Value.ToInt();
    //    }
    //    catch
    //    {
    //        return defaultValue;
    //    }
    //}

    public string GetString(string key, string defaultValue = "")
    {
        try
        {
            var text = _configuration.GetSection("StringValue").GetChildren().FirstOrDefault((x) => x.Key == key)?.Value;
            return string.IsNullOrEmpty(text) ? defaultValue : text;
        }
        catch { return defaultValue; }
    }

    public static T Get<T>(string? key = null)
    {
        if (string.IsNullOrWhiteSpace(key)) return Instance._configuration.Get<T>()!;
        var section = Instance._configuration.GetSection(key);
        return section.Get<T>()!;
    }

    public static T Get<T>(string key, T defaultValue)
    {
        if (Instance._configuration.GetSection(key) == null) return defaultValue;
        if (string.IsNullOrWhiteSpace(key)) return Instance._configuration.Get<T>()!;
        return Instance._configuration.GetSection(key).Get<T>()!;
    }
}