using Godot;
using TENamespace.health;
using Shader = TENamespace.basic.shader.Shader;

namespace TerraEngineer.entities.mobs.creatures;

[Tool]
public partial class Creature : Entity
{
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
    }
}