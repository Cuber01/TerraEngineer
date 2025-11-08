using Godot;
using System;
using System.Collections.Generic;
using GodotDict = Godot.Collections.Dictionary<string, Godot.Variant>;
using GodotArray = Godot.Collections.Array;

public partial class SaveData : Node
{
    private static GodotDict data;
    private static Json json = new Json();
    private const string saveSlotPath = "res://saves/save0.json";    

    public override void _Ready()
    {
        loadSaveFile(saveSlotPath);
    }

    public static void WriteChanges()
    {
        String newJsonText = Json.Stringify(data);

        if (newJsonText == "")
        {
            throw new Exception("Failed to stringify data.");
        }
        
        FileAccess file = FileAccess.Open(saveSlotPath, FileAccess.ModeFlags.Write);
        file.StoreString(newJsonText);
        file.Close();
    }
    
    public static void SetValue(string sectionKey, string key, Variant value)
        => ( (GodotDict)data[sectionKey] )[key] = value;

    public static Variant ReadValue(string sectionKey, string key) 
        => ((GodotDict)data[sectionKey] )[key];

    public static List<string> ReadInventory()
    {
        List<string> items = new();
        GodotDict playerInventory = (GodotDict)data["player_inventory"];
        
        foreach(string key in playerInventory.Keys)
        {
            if ((bool)playerInventory[key])
            {
                items.Add(key);   
            }
            
        }
        return items;
    }

    private void loadSaveFile(string path)
    {
        if (!FileAccess.FileExists(path))
        {
            throw new Exception($"File {path} does not exist");
        }

        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();
        file.Close();

        // Parse JSON string to Variant
        Error err = json.Parse(jsonString);
        if (err != Error.Ok)
        {
            throw new Exception("Failed to parse json: " + json.GetErrorMessage());
        }

        data = (GodotDict)json.GetData();
    }
}
