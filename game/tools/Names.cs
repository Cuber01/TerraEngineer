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
        public static readonly NodePath CollisionShape2D = "CollisionShape2D";
        public static readonly StringName GridContainer = "GridContainer";
        public static readonly StringName Blowtorch = "Blowtorch";
        public static readonly StringName MetSys = "MetSys";
        public static readonly StringName AnimationPlayer = "AnimationPlayer";
    }

    // Properties, metadata, custom tile data
    public static class Properties
    {
        public static readonly StringName CurrentLevel = "CurrentLevel";
        public static readonly StringName LevelName = "level_name";
        public static readonly StringName SpecialType = "special_type";
        public static readonly StringName Bosses = "bosses";
        public static readonly StringName KingFrog = "king_frog";
        public static readonly StringName Collider = "collider";
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
        public static readonly NodePath Level = "/root/Main/Level";
        public static readonly NodePath Map = "/root/Main/GUI/MapScreen/Panel";
        public static readonly NodePath Inventory = "/root/Main/GUI/InventoryScreenStarter";
        public static readonly NodePath PlayerHUD = "/root/Main/GUI/PlayerHUD";
        public static readonly NodePath GuiMediator = "/root/Main/GUI";
        public static readonly NodePath WorldManager = "/root/Main";
    }

    // Json save sections
    public static class SaveSections
    {
        public static readonly StringName SavePointData = "savepoint_data";
        public static readonly StringName SavePointPosition = "position";
        public static readonly StringName SavePointLevel = "level";
        public static readonly StringName Map = "map";
        
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
        public static readonly StringName Locked = "locked";
        
        // Creature animations
        public static readonly StringName Charge = "charge";
        public static readonly StringName Stuck = "stuck";
        public static readonly StringName Attack = "attack";
        public static readonly StringName Hide = "hide";
        
        // Object animations
        public static readonly StringName Grown = "grown";
        public static readonly StringName ThreeOn = "3-on";
        public static readonly StringName ThreeOff = "3-off";
        public static readonly StringName Closed = "closed";
        public static readonly StringName Open = "open";
        public static readonly StringName Closing = "closing";
        
        // Terminal animations
        public static readonly StringName Green = "green";
        public static readonly StringName Blue = "blue";
        public static readonly StringName Red = "red";
        public static readonly StringName Yellow = "yellow";
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
         public static readonly StringName OpenMap = "open_map";
         public static readonly StringName OpenInventory = "open_inventory";
         public static readonly StringName Quit = "ui_cancel";
         
         public static readonly StringName Up = "ui_up";
         public static readonly StringName Down = "ui_down";
         public static readonly StringName Left = "ui_left";
         public static readonly StringName Right = "ui_right";
         
         public static readonly StringName GroupWeapon = "GroupWeapon";
         public static readonly StringName GroupMenus = "GroupMenus";
     }

     public static class CollisionLayers
     {
         public const int Ground = 1;
         public const int Player = 2;
         public const int Enemy = 3;
         public const int Objects = 4;
         public const int Terraformables = 5;
         public const int Platforms = 6;
         public const int Bullets = 7;
         public const int BulletDeflectors = 8;
     }
     
     public static class MapMarkers
     {
         public const int UncollectedCollectible = 0;
         public const int CollectedCollectible = 1;
         public const int Heal = 2;
         public const int Heart = 3;
         public const int Save = 4;
         public const int Boss = 5;
         public const int Bang = 6;
         public const int Teleport = 7;
     }

     public static class Other
     {
         public static readonly StringName Start = "start";
         public static readonly StringName Editor = "editor";
     }

     public static class MetSys
     {
         public static readonly StringName CurrentLayer = "current_layer";
         public static readonly StringName LastPlayerPosition = "last_player_position";
         public static readonly StringName MapData = "map_data";
         public static readonly StringName GroupCache = "group_cache";
         public static readonly StringName GroupNames = "group_names";

         public static readonly StringName GetCellSizeOffset = "getCellSizeOffset";
         public static readonly StringName MakeMapView = "make_map_view";
         public static readonly StringName AddPlayerLocation = "add_player_location";
         public static readonly StringName GetCurrentFlatCoords = "get_current_flat_coords";
         public static readonly StringName Move = "move";
         public static readonly StringName MoveTo = "move_to";
         public static readonly StringName UpdateAll = "update_all";
         public static readonly StringName DiscoverCell = "discover_cell";
         public static readonly StringName IsCellDiscovered = "is_cell_discovered";
         public static readonly StringName DiscoverCellGroup = "discover_cell_group";
         public static readonly StringName VisitCell = "visit_cell";
         
         public static readonly StringName Offset = "offset";
         public static readonly StringName GetSaveData = "get_save_data";
         public static readonly StringName SetSaveData = "set_save_data";
         public static readonly StringName ResetState = "reset_state";

         public static readonly StringName BiomeNotFound = "???";

         public static readonly StringName GetObjectCoords = "get_object_coords";
         public static readonly StringName GetObjectId = "get_object_id";
         public static readonly StringName IsObjectIdStored = "is_object_id_stored";
         public static readonly StringName StoreObject = "store_object";
         public static readonly StringName RegisterStorableObject = "register_storable_object";
         public static readonly StringName RegisterStorableObjectWithMarker = "register_storable_object_with_marker";
         public static readonly StringName RemoveCustomMarker = "remove_custom_marker";
         public static readonly StringName AddCustomMarker = "add_custom_marker";
         public static readonly StringName GetCurrentCoords = "get_current_coords";
         public static readonly StringName GetMarkerAt = "get_marker_at";
     }
 }
