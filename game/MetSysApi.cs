using Godot.Collections;

namespace TerraEngineer.game;

using Godot;

public static class MetSysApi
{
    private static GodotObject metSys;
    private static readonly int biomeCount = 4;

    static MetSysApi()
    {
        var mainTree = Engine.GetMainLoop() as SceneTree;
        metSys = mainTree.Root.GetNodeOrNull<GodotObject>("MetSys");
    }
    
    public static int CurrentLayer => (int)metSys.Get(Names.MetSys.CurrentLayer);
    
    public static Vector3I LastPlayerPosition => (Vector3I)metSys.Get(Names.MetSys.LastPlayerPosition);

    public static Vector2 GetCellSizeOffset()
    {
        return (Vector2)metSys.Call(Names.MetSys.GetCellSizeOffset);
    }
    
    public static bool IsCellDiscovered(Vector3I cellCoords, bool includeMapped = true)
    {
        return (bool)metSys.Call(Names.MetSys.IsCellDiscovered, cellCoords, includeMapped);
    }
    
    public static void DiscoverAll()
    {
        for(int i = 0; i < biomeCount; i++)
            DiscoverCellGroup(i);
    }
    
    public static void DiscoverCell(Vector3I cellCoords)
    {
        metSys.Call(Names.MetSys.DiscoverCell, cellCoords);
    }
    
    public static void DiscoverCellGroup(int groupId)
    {
        metSys.Call(Names.MetSys.DiscoverCellGroup, groupId);
    }
    
    // Explore effect
    public static void VisitCell(Vector3I cellCoords)
    {
        metSys.Call(Names.MetSys.VisitCell, cellCoords);
    }

    public static GodotObject MakeMapView(Node wrapperNode, Vector2I minCell, Vector2I maxCell, int layer)
    {
        return metSys.Call(Names.MetSys.MakeMapView, wrapperNode, minCell, maxCell, layer).As<GodotObject>();
    }

    public static Node2D AddPlayerLocation(Node wrapperNode)
    {
        return metSys.Call(Names.MetSys.AddPlayerLocation, wrapperNode).As<Node2D>();
    }

    public static Vector2I GetCurrentFlatCoords()
    {
        return (Vector2I)metSys.Call(Names.MetSys.GetCurrentFlatCoords);
    }
    
    public static string GetBiomeName(Vector3I cellCoords)
    {
        GodotObject mapData = metSys.Get(Names.MetSys.MapData).As<GodotObject>();

        var groupCache = mapData.Get(Names.MetSys.GroupCache).As<Dictionary>();

        if (groupCache.ContainsKey(cellCoords))
        {
            int[] groupsForCell = groupCache[cellCoords].As<int[]>();
            if (groupsForCell != null && groupsForCell.Length > 0)
            {
                string[] groupNames = mapData.Get(Names.MetSys.GroupNames).As<string[]>();
                int firstGroupKey = groupsForCell[0];
                
                return groupNames[firstGroupKey];
            }
        }

        return Names.MetSys.BiomeNotFound;
    }

    public static void Move(this GodotObject mapView, Vector2I moveOffset)
    {
        mapView.Call(Names.MetSys.Move, moveOffset);
    }

    public static void MoveTo(this GodotObject mapView, Vector3I targetCoords)
    {
        mapView.Call(Names.MetSys.MoveTo, targetCoords);
    }

    public static void UpdateAll(this GodotObject mapView)
    {
        mapView.Call(Names.MetSys.UpdateAll);
    }
}