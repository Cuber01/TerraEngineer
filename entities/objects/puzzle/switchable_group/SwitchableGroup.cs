using System.Collections.Generic;
using Godot;
using System;

namespace TerraEngineer.entities.objects.puzzle.switchable_group;

[Tool]
public partial class SwitchableGroup : Node2D
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


    // Groups of switchers - if one of the groups is switched, all switchables are
    [Export] public Node2D[] SwitcherGroups = Array.Empty<Node2D>();
    
    // Switchables switched on or off
    [Export] public Node2D[] SwitchableGroupMembers = Array.Empty<Node2D>();
    
    [Export] public bool GroupSwitchedOn { get; set; }

    public List<ISwitcher> Switchers { get; set; }
    public Node2D Me { get; set; }

    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        
        
        updateGroupPalette();
        
        if (GroupSwitchedOn)
        {
            OnSwitch(GroupSwitchedOn);
        }
    }

    public void OnSwitch(bool switchedOn)
    {
        GroupSwitchedOn = switchedOn;
        foreach (var switchable in SwitchableGroupMembers)
        {
            ((ISwitchable)switchable).OnSwitch(switchedOn);
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
        
        foreach (var node in SwitcherGroups)
        {
            SwitcherGroup switcherGroup = (SwitcherGroup)node;
            if (!switcherGroup.IsInit)
            {
                switcherGroup.Init(this);
            }
            
            foreach (var switcher in switcherGroup.SwitchersToSwitch)
            {
                AnimatedSprite2D sprite = ((Node2D)switcher).GetNodeOrNull<AnimatedSprite2D>(Names.Node.AnimatedSprite2D);
                ((ShaderMaterial)sprite?.Material)?.SetShaderParameter(Names.Shader.Palette, _groupPalette);    
            }
        }
    }

    // Signal that overrides usual checks
    private void onSpecialSignal()
    {
        GroupSwitchedOn = !GroupSwitchedOn;
        OnSwitch(GroupSwitchedOn);
    }

 
}