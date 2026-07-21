using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using TerraEngineer;
using TerraEngineer.game;
using GodotDict = Godot.Collections.Dictionary<Godot.StringName, Godot.Variant>;
using GodotArray = Godot.Collections.Array;

public partial class SaveData : Node
{
    // Used for objects in current level to know when a state of some data they might need has changed
    public delegate void RealtimeDataChangedHandler(bool switchedOn);
    public static event RealtimeDataChangedHandler RealtimeDataChanged;
    
    private static GodotDict data;
    private static Json json = new Json();    

    private static readonly string SavePath = OS.HasFeature(Names.Other.Editor) ?
        Names.Paths.Res + Names.Paths.Save0 :
        OS.GetExecutablePath().GetBaseDir() + Names.Paths.Save0;
    
    public override void _Ready()
    {
        loadSaveFile(SavePath);
    }

    public override void _Process(double delta)
    {
        #if DEBUG
        if (Input.IsActionJustPressed("f1"))
        {
            SetAddValue(Names.SaveSections.SavePointData,
                Names.SaveSections.SavePointPosition,
                GetNode<Player>(Names.NodePaths.Player).GlobalPosition);
        
            StringName levelName = (StringName)GetNode<Node2D>(Names.NodePaths.Level).GetMeta(Names.Properties.LevelName);
        
            SetAddValue(Names.SaveSections.SavePointData,
                Names.SaveSections.SavePointLevel,
                levelName);
            
            WriteChanges();
            GD.Print("DEBUG TOOLS: Custom savepoint set.");
        }
        #endif
    }

    public static void WriteChanges()
    {
        addMetSysData();
        String newJsonText = Json.Stringify(data);

        if (newJsonText == "")
        {
            throw new Exception("Failed to stringify data.");
        }
        
        FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(newJsonText);
        file.Close();
    }

    public static void SetAddValue(string sectionKey, string key, Variant value, bool fireEvent = false)
    {
        if (!data.ContainsKey(sectionKey))
        {
            data[sectionKey] = new GodotDict();
        }
        
        ((GodotDict)data[sectionKey] )[key] = value;
        if(fireEvent) RealtimeDataChanged?.Invoke((bool)value);
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

    public static Variant ReadValue(string sectionKey, string key)
    {
        if (data.TryGetValue(sectionKey, out Variant output))
        {
            GodotDict dict = (GodotDict)output;
            if (dict.TryGetValue(key, out Variant value))
            {
                return value;    
            }
        }
        return new Variant();
    }
    
    public static Variant ReadFromArray(string sectionKey, string key, int index) 
        => ((GodotArray)((GodotDict)data[sectionKey] )[key])[index];

    private static void addMetSysData()
    {
        if (data.ContainsKey(Names.SaveSections.Map))
        {
            data[Names.SaveSections.Map] = MetSysApi.GetSaveData();
        }
        else
        {
            data.Add(Names.SaveSections.Map, MetSysApi.GetSaveData());
        }
    }
    
    private static void loadMetSysData()
    {
        MetSysApi.ResetState();
        
        Dictionary mapData = (Dictionary)data[Names.SaveSections.Map];
        Dictionary discoveredCells = (Dictionary)mapData["discovered_cells"];

        List<Vector3I> vecKeys = new List<Vector3I>();
        Dictionary updatedDiscoveredCells = new Dictionary();
        
        foreach (string key in discoveredCells.Keys)
        {
            Vector3I converted = StringToVec3I(key);
            vecKeys.Add(converted);
            updatedDiscoveredCells.Add(converted, discoveredCells[key]);
        }
        
        mapData["discovered_cells"] = updatedDiscoveredCells;

        MetSysApi.SetSaveData(mapData);
        
        foreach (Vector3I vec in vecKeys)
        {
            MetSysApi.VisitCell(vec);
        }
    }

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
            throw new Exception($"Save file {path} does not exist");
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

        if (data.TryGetValue(Names.SaveSections.Map, out Variant value))
        {
            loadMetSysData();   
        }
        
    }
    
    // We need to convert it to a really weird array in order for the comparison to work...
    public static GodotArray VecToParseableArray(Vector2I coords)
        => new GodotArray() {(float)coords.X, (float)coords.Y};
    
    public static Vector2 StringToVec2(String vecString)
    { 
        string[] parts = vecString.Split(',');
        float x = float.Parse(parts[0].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        float y = float.Parse(parts[1].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        return new Vector2(x, y);
    }
    
    public static Vector3I StringToVec3I(string vecString)
    { 
        string[] parts = vecString.Split(',');
        int x = int.Parse(parts[0].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        int y = int.Parse(parts[1].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        int z = int.Parse(parts[2].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        return new Vector3I(x, y, z);
    }
    
    public static Vector3 StringToVec3(String vecString)
    { 
        string[] parts = vecString.Split(',');
        float x = float.Parse(parts[0].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        float y = float.Parse(parts[1].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        float z = float.Parse(parts[2].Trim(" ()[]".ToCharArray()), CultureInfo.InvariantCulture);
        return new Vector3(x, y, z);
    }
}
