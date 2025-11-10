using Godot;

namespace TerraEngineer;

public static class Names
{
    // Node names
    public static class Node
    {
        public static readonly NodePath Main = "Main";
        public static readonly NodePath SpecialTiles = "SpecialTiles";
        public static readonly NodePath Level = "Level";
    }

    // Properties, metadata, custom tile data
    public static class Properties
    {
        public static readonly StringName CurrentLevel = "CurrentLevel";
        public static readonly StringName LevelName = "level_name";
        public static readonly StringName SpecialType = "special_type";
    }

    // Shader data
    public static class Shader
    {
        public static readonly StringName Delta = "delta_time";
        public static readonly StringName Run = "run";
        public static readonly StringName Blink = "blink";
    }

    // Filesystem paths
    public static class Paths
    {
        public static readonly StringName Save0 = "res://saves/save0.json";
    }

    // Json save sections
    public static class SaveSections
    {
        public static readonly StringName PlayerInventory = "player_inventory";
        public static readonly StringName RemovedTiles = "removed_tiles";
    }

    // Animation names
    public static class Animations
    {
        public static readonly StringName Selected = "selected";
        public static readonly StringName Unselected = "unselected";
        public static readonly StringName NoFruit = "no_fruit";
        public static readonly StringName Default = "default";
    }
}