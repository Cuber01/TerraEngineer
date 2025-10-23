using Godot;
using TENamespace.basic.builders;
using TerraEngineer.entities.objects;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.particle_builder;

public partial class StarParticleSpawner : Spawner
{
    [Export] private int extraYSpread = 2;
    
    [Export] private CurveXyzTexture leftVelCurve;
    [Export] private CurveXyzTexture rightVelCurve;
    [Export] private CurveXyzTexture downVelCurve;
    [Export] private CurveXyzTexture upVelCurve;

    [Export] private Texture2D mushroomTexture;
    [Export] private Texture2D grassTexture;
    [Export] private Texture2D iceTexture;
    [Export] private Texture2D desertTexture;

    public GpuParticles2D Build(Vector2 position, Vector2 directionNormal, Biomes biome)
    {
        GpuParticles2D instance = (GpuParticles2D)Scene.Instantiate();

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
        
        return instance;
    }
}