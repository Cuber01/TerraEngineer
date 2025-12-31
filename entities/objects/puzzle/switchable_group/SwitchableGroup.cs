using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

[Tool]
public partial class SwitchableGroup : Node2D, ISwitchable
{
    [ExportToolButton("Update group palettes")]
    public Callable MyToolButton => Callable.From(updateGroupPalette);
    
    [Export] private Texture2D GroupPalette
    {
        get => _groupPalette;
        set
        {
            _groupPalette = value;
            updateGroupPalette();
        }
    }
    private Texture2D _groupPalette;
    
    [Export] private Texture2D BlocksGroupPalette
    {
        get => _blocksGroupPalette;
        set
        {
            _blocksGroupPalette = value;
            updateGroupPalette();
        }
    }
    private Texture2D _blocksGroupPalette;
    
    [Export] public StringName[] SavePropertiesNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchersNeededToSwitch { get; set; }
    [Export] public Node2D[] SwitchableGroupMembers;
    [Export] public bool GroupSwitchedOn { get; set; }

    public List<ISwitcher> Switchers { get; set; }
    public Node2D Me { get; set; }

    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        ((ISwitchable)this).Init(this);
        
        updateGroupPalette();
    }

    public void OnSwitch(bool switchedOn)
    {
        foreach (var switchable in SwitchableGroupMembers)
        {
            ((ISwitchableDependent)switchable).OnSwitch(switchedOn);
        }
    }
    
    private void updateGroupPalette()
    {
        if(SwitchableGroupMembers == null) 
            return;
        
        foreach (var switchable in SwitchableGroupMembers)
        {
            AnimatedSprite2D sprite = switchable.GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D);
            ((ShaderMaterial)sprite.Material)?.SetShaderParameter(Names.Shader.Palette, _blocksGroupPalette);
        }
        
        
        foreach (var switcher in SwitchersNeededToSwitch)
        {
            AnimatedSprite2D sprite = switcher.GetNode<AnimatedSprite2D>(Names.Node.AnimatedSprite2D);
            ((ShaderMaterial)sprite.Material)?.SetShaderParameter(Names.Shader.Palette, _groupPalette);
        }
    }
}