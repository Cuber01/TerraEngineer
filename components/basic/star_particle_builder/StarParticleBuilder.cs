using Godot;
using TerraEngineer.entities.objects;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.particle_builder;

public partial class StarParticleBuilder : Component
{
    [Export] private PackedScene particleScene;
    [Export] private int extraYSpread = 2;
    
    [Export] private CurveXyzTexture leftVelCurve;
    [Export] private CurveXyzTexture rightVelCurve;
    [Export] private CurveXyzTexture downVelCurve;
    [Export] private CurveXyzTexture upVelCurve;

    [Export] private Texture2D mushroomTexture;
    [Export] private Texture2D grassTexture;
    [Export] private Texture2D iceTexture;
    [Export] private Texture2D desertTexture;
    
    private Node main;

    public override void _Ready()
    {
        main = GetTree().GetRoot().GetNode("Main");
    }

    public GpuParticles2D Build(Vector2 position, Vector2 directionNormal, Biomes biome, Node2D parent=null)
    {
        GpuParticles2D instance = (GpuParticles2D)particleScene.Instantiate();

        instance.OneShot = true;
        
        ParticleProcessMaterial material = (ParticleProcessMaterial)instance.ProcessMaterial;

        material.Direction = new Vector3(directionNormal.X, directionNormal.Y*extraYSpread, 0); // Initial velocity
        
        if (directionNormal.X > 0)
            material.DirectionalVelocityCurve = rightVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.X < 0)
            material.DirectionalVelocityCurve = leftVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.Y > 0)
            material.DirectionalVelocityCurve = downVelCurve.Duplicate(true) as CurveXyzTexture;
        else 
            material.DirectionalVelocityCurve = upVelCurve.Duplicate(true) as CurveXyzTexture;

        switch (biome)
        {
            case Biomes.Forest:
                instance.Texture = grassTexture;
                break;
            case Biomes.Desert:
                instance.Texture = desertTexture;
                break;
            case Biomes.Ice:
                instance.Texture = iceTexture;
                break;
            case Biomes.Mushroom:
                instance.Texture = mushroomTexture;
                break;
        }
        
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