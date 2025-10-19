using Godot;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.particle_builder;

public partial class ParticleBuilder : Component
{
    [Export] private PackedScene particleScene;
    [Export] private CurveXyzTexture leftVelCurve;
    [Export] private CurveXyzTexture rightVelCurve;
    [Export] private CurveXyzTexture downVelCurve;
    [Export] private CurveXyzTexture upVelCurve;

    private Node main;

    public override void _Ready()
    {
        main = GetTree().GetRoot().GetNode("Main");
    }

    public GpuParticles2D Build(Vector2 position, Vector2 directionNormal, Node2D parent=null)
    {
        GpuParticles2D instance = (GpuParticles2D)particleScene.Instantiate();

        instance.OneShot = true;
        
        // Change velocity curve - is there really no better way to do this?
        ParticleProcessMaterial material = (ParticleProcessMaterial)instance.ProcessMaterial;

        material.Direction = new Vector3(directionNormal.X, directionNormal.Y*2, 0); // Initial velocity
        
        if (directionNormal.X > 0)
            material.DirectionalVelocityCurve = rightVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.X < 0)
            material.DirectionalVelocityCurve = leftVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.Y > 0)
            material.DirectionalVelocityCurve = downVelCurve.Duplicate(true) as CurveXyzTexture;
        else 
            material.DirectionalVelocityCurve = upVelCurve.Duplicate(true) as CurveXyzTexture;

        instance.ProcessMaterial = material.Duplicate() as ParticleProcessMaterial;
        
        instance.GlobalPosition = position;
        if (parent != null)
        {
            parent.AddChild(instance);
        }
        else
        {
            Node2D level = (Node2D)main.Get("CurrentLevel");
            level.AddChild(instance);    
        }
        
        return instance;
    }
}