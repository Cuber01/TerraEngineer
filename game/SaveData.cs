using Godot;
using System;
using System.Collections.Generic;

public partial class SaveData : Node
{
    private static ConfigFile data = new();
    private const string saveSlotPath = "res://saves/save0.cfg";    

    public override void _Ready()
    {
        loadSaveFile(saveSlotPath);
    }

    public static void WriteChanges() => data.Save(saveSlotPath);
    
    public static void SetValue(string section, string key, Variant value) => data.SetValue(section, key, value);  

    public static Variant ReadValue(string section, string key) => data.GetValue(section, key);

    public static List<string> ReadInventory()
    {
        List<string> items = new();
        
        foreach(string key in data.GetSectionKeys("player_inventory"))
        {
            if ((bool)data.GetValue("player_inventory", key))
            {
                items.Add(key);    
            }
            
        }
        return items;
    }
    
    public static Dictionary<string, Variant> ReadSection(string section)
    {
        Dictionary<string, Variant> dict = new();
        
        foreach(string key in data.GetSectionKeys(section))
        {
            dict.Add(key, ReadValue(section, key));
        }
        return dict;
    }

    private void loadSaveFile(string path)
    {
        Error err = data.Load(path);

        if (err != Error.Ok)
        {
            throw new Exception($"Error loading save at {path}");
        }
    }
}
