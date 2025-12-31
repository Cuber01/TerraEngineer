using Godot;
using TENamespace.health;
using Shader = TENamespace.basic.shader.Shader;

namespace TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Creature : Entity
{
    // Override this.
    public virtual void Init() {}
    
    // Do not override!
    public override void _Ready()
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        CM.GetComponent<Health>().HealthChanged += (_, amount) =>
        {
            if (amount < 0)
            {
                CM.TryGetComponent<Shader>()?.SetShader(Names.Shader.Blink);
                CM.TryGetComponent<Shader>()?.ToggleShader(true);
            }
        };
		
        CM.GetComponent<Health>().InvincibilityEnded += () =>
        {
            CM.TryGetComponent<Shader>()?.ToggleShader(false);
        };
        
        Init();
    }

    protected void FlipIfHitWall()
    {
        for(int i = 0; i < GetSlideCollisionCount(); i++)
        {
            Vector2 normal = GetSlideCollision(i).GetNormal();
            if (normal == new Vector2(-(int)Facing, 0))
            {
                Flip();
            }
        }
    }

}