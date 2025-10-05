using Godot;
using TENamespace;

namespace TerraEngineer.entities.mobs;

public partial class Mob : CharacterBody2D
{
    [Export] public ComponentManager CM;
    public Vector2 velocity;
    
    
    public virtual void Die() {}
}