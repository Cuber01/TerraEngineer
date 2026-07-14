using Godot;

[GlobalClass]
public partial class InventoryItemData : Resource
{
    [Export] public string ItemName { get; set; } = "New Item";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
}