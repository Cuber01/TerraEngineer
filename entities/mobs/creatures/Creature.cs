using TENamespace.basic.shader;
using TENamespace.health;

namespace TerraEngineer.entities.mobs.creatures;

public partial class Creature : Entity
{
    public override void _Ready()
    {
        CM.GetComponent<Health>().HealthChanged += (_, amount) =>
        {
            if (amount < 0)
            {
                CM.TryGetComponent<Shader>()?.SetShader("blink");
                CM.TryGetComponent<Shader>()?.ToggleShader(true);
            }
        };
		
        CM.GetComponent<Health>().InvincibilityEnded += () =>
        {
            CM.TryGetComponent<Shader>()?.ToggleShader(false);
        };
    }
}