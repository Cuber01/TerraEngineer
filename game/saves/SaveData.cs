using Godot;
using System;
using System.Collections.Generic;
using Godot.NativeInterop;
using TerraEngineer;
using GodotDict = Godot.Collections.Dictionary<string, Godot.Variant>;
using GodotArray = Godot.Collections.Array;

public partial class SaveData : Node
{
    private static GodotDict data;
    private static Json json = new Json();    

    public override void _Ready()
    {
        loadSaveFile(Names.Paths.Save0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("debug"))
        {
            WriteChanges();
        }
    }

    public static void WriteChanges()
    {
        String newJsonText = Json.Stringify(data);

        if (newJsonText == "")
        {
            throw new Exception("Failed to stringify data.");
        }
        
        FileAccess file = FileAccess.Open(Names.Paths.Save0, FileAccess.ModeFlags.Write);
        file.StoreString(newJsonText);
        file.Close();
    }
    
    public static void SetValue(string sectionKey, string key, Variant value)
        => ( (GodotDict)data[sectionKey] )[key] = value;
    
    public static void SetValueInArray(string sectionKey, string key, int index, Variant value)
        => ((GodotArray)( (GodotDict)data[sectionKey] )[key])[index] = value;
    
    public static void RemoveValueInArray(string sectionKey, string key, Variant value)
        => ((GodotArray)( (GodotDict)data[sectionKey] )[key]).Remove(value);
    
    public static void AddValueToArray(string sectionKey, string key, Variant value)
    { 
        GodotDict dict = (GodotDict)data[sectionKey];
        if (!dict.ContainsKey(key))
        {
            dict[key] = new GodotArray();
        }
        
        ((GodotArray)dict[key]).Add(value);
    }
    
    public static Variant ReadValue(string sectionKey, string key) 
        => ((GodotDict)data[sectionKey] )[key];
    
    public static Variant ReadFromArray(string sectionKey, string key, int index) 
        => ((GodotArray)((GodotDict)data[sectionKey] )[key])[index];

    public static List<string> ReadInventory()
    {
        List<string> items = new();
        GodotDict playerInventory = (GodotDict)data[Names.SaveSections.PlayerInventory];
        
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
