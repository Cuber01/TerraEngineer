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

    private void loadSaveFile(string path)
    {
        Error err = data.Load(path);

        if (err != Error.Ok)
        {
            throw new Exception($"Error loading save at {path}");
        }
    }
}
