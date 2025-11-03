using Godot;
using TENamespace;
using TENamespace.health;

namespace TerraEngineer.entities.mobs;

public partial class Mob : CharacterBody2D
{
    [Export] public ComponentManager CM;
    [Export] public CollisionTeam Team;
    
    [Export] protected AnimatedSprite2D Sprite;
    [Export] public DirectionX Facing = DirectionX.Right;
    [Export] public ShaderMaterial BlinkShader;

    public Vector2 velocity;

    // Used to stop edge cases in which non-garbage collected objects will try to interact with disposed Godot nodes via timed callbacks.
    public bool Dead  = false;

    public override void _Ready()
    {
        if (Facing == DirectionX.Left)
        {
            FlipSprite();
        }
    }

    protected void FlipSprite() => Sprite.FlipH = !Sprite.FlipH;
    
    public void Flip(DirectionX side=DirectionX.None)
    {
        if (side == DirectionX.None || (int)side == -(int)Facing)
        {
            FlipEffect();
        }
    }

    protected virtual void FlipEffect()
    {
        Facing = (DirectionX)(-(int)Facing);
        Sprite.FlipH = !Sprite.FlipH;   
    }
    
    public void SetShader(AnimatedSprite2D sprite, ShaderMaterial shader) =>
        sprite.Material = shader;

    public void ToggleShader(AnimatedSprite2D sprite, bool enabled)
    {
        var shaderMaterial = (ShaderMaterial)sprite.Material;
        shaderMaterial.SetShaderParameter("run", enabled); 
    }

    public virtual void Die()
    {
        if (!Dead)
        {
            CallDeferred(Node.MethodName.QueueFree);    
        }
        Dead = true;
    }
}