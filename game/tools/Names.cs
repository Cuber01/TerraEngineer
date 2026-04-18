using Godot;

namespace TerraEngineer;

[Tool]
public static class Names
{
    // Node names
    public static class Node
    {
        public static readonly NodePath Main = "Main";
        public static readonly NodePath SpecialTiles = "SpecialTiles";
        public static readonly NodePath Level = "Level";
        public static readonly NodePath AnimatedSprite2D = "AnimatedSprite2D";
        public static readonly NodePath PinJoint2D = "PinJoint2D";
    }

    // Properties, metadata, custom tile data
    public static class Properties
    {
        public static readonly StringName CurrentLevel = "CurrentLevel";
        public static readonly StringName LevelName = "level_name";
        public static readonly StringName SpecialType = "special_type";
        public static readonly StringName Bosses = "bosses";
        public static readonly StringName KingFrog = "king_frog";
        public static readonly StringName OpenDoors = "open_doors";
        public static readonly StringName BackgroundColor = "background_color";
    }

    // Shader data
    public static class Shader
    {
        public static readonly StringName Delta = "delta_time";
        public static readonly StringName Palette = "palette";
        public static readonly StringName Run = "run";
        public static readonly StringName Blink = "Blink";
        
    }

    // Filesystem paths
    public static class Paths
    {
        public static readonly StringName Res = "res:/";
        public static readonly StringName Save0 = "/saves/save0.json";
        public static readonly StringName NewSave = "/saves/new_save.json";
        public static readonly StringName Zombie = "res://entities/mobs/creatures/zombie/Zombie.tscn";
    }

    // Node paths from root
    public static class NodePaths
    {
        public static readonly NodePath Popup = "/root/Main/GUI/Popup";
        public static readonly NodePath DialogueBalloon = "/root/Main/GUI/Dialogue";
        public static readonly NodePath Player = "/root/Main/Player";
    }

    // Json save sections
    public static class SaveSections
    {
        public static readonly StringName SavePointData = "savepoint_data";
        public static readonly StringName SavePointPosition = "position";
        public static readonly StringName SavePointLevel = "level";
        
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
        public static readonly StringName Idle = "idle";
        public static readonly StringName Jump = "start-jump";
        public static readonly StringName Fall = "fly-downwards";
        public static readonly StringName Fly = "fly-upwards";
        public static readonly StringName Walk = "walk";
        public static readonly StringName Land = "land";
        public static readonly StringName Dash = "dash";
    }

    public static class Actions
    {
        public static readonly StringName Weapon0 = "weapon_0";
        public static readonly StringName Weapon1 = "weapon_1";
        public static readonly StringName Weapon2 = "weapon_2";
        public static readonly StringName Weapon3 = "weapon_3";
        public static readonly StringName WeaponNext = "weapon_next";
        public static readonly StringName GunHandleNext = "gunhandle_next";
        public static readonly StringName Attack = "attack";
        public static readonly StringName Dash = "dash";
        public static readonly StringName Jump = "jump";
        
        public static readonly StringName GroupWeapon = "GroupWeapon";
    }

    public static class Other
    {
        public static readonly StringName Start = "start";
        public static readonly StringName Editor = "editor";
    }
}