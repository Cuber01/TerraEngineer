using Godot;
using System;
using System.Collections.Generic;
using Godot.NativeInterop;
using TerraEngineer;
using GodotDict = Godot.Collections.Dictionary<Godot.StringName, Godot.Variant>;
using GodotArray = Godot.Collections.Array;

public partial class SaveData : Node
{
    // Used for objects in current level to know when a state of some data they might need has changed
    public delegate void RealtimeDataChangedHandler(bool switchedOn);
    public static event RealtimeDataChangedHandler RealtimeDataChanged;
    
    private static GodotDict data;
    private static Json json = new Json();    

    public override void _Ready()
    {
        loadSaveFile(Names.Paths.Save0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("f1"))
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

    public static void SetValue(string sectionKey, string key, Variant value, bool fireEvent = false)
    {
        if(fireEvent) RealtimeDataChanged?.Invoke((bool)value);
        ((GodotDict)data[sectionKey] )[key] = value;
    }

    public static void SetValueInArray(string sectionKey, string key, int index, Variant value, bool fireEvent = false)
    {
        if(fireEvent) RealtimeDataChanged?.Invoke((bool)value);
        ((GodotArray)((GodotDict)data[sectionKey])[key])[index] = value;
    }

    public static void RemoveValueInArray(string sectionKey, string key, Variant value)
        => ((GodotArray)( (GodotDict)data[sectionKey] )[key]).Remove(value);
    
    public static void AddValueToArray(string sectionKey, string key, Variant value, bool fireEvent = false)
    { 
        if(fireEvent) RealtimeDataChanged?.Invoke((bool)value);
        
        GodotDict dict = (GodotDict)data[sectionKey];
        if (!dict.ContainsKey(key))
        {
            dict[key] = new GodotArray();
        }
        
        ((GodotArray)dict[key]).Add(value);
    }

    public static Variant? ReadValue(string sectionKey, string key)
    {
        GodotDict dict = (GodotDict)data[sectionKey];
        if (dict.TryGetValue(key, out Variant value))
        {
            return value;    
        }
        else
        {
            return null;
        }
    }
    
    public static Variant ReadFromArray(string sectionKey, string key, int index) 
        => ((GodotArray)((GodotDict)data[sectionKey] )[key])[index];

    public static List<StringName> ReadInventory()
    {
        List<StringName> items = new();
        GodotDict playerInventory = (GodotDict)data[Names.SaveSections.PlayerInventory];
        
        foreach(StringName key in playerInventory.Keys)
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
    
    // We need to convert it to a really weird array in order for the comparison to work...
    public static GodotArray VecToParseableArray(Vector2I coords)
        => new GodotArray() {(float)coords.X, (float)coords.Y};
    
    public static Vector2 StringToVec(String vecString)
    { 
        string[] parts = vecString.Split(',');
        float x = float.Parse(parts[0].Remove(0,1));
        float y = float.Parse(parts[1].Remove(parts[1].Length-1,1));
        return new Vector2(x, y);
    }
}
