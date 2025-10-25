using Godot;
using TENamespace.basic.builders;
using TerraEngineer.entities.objects;
using TerraEngineer.entities.projectiles;

namespace TENamespace.basic.particle_builder;

public partial class StarParticleSpawner : Spawner<GpuParticles2D, StarParticleSpawner>
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
    
    public override GpuParticles2D Build()
    {
        Instance.OneShot = true;
        return Instance;
    }

    public StarParticleSpawner SetBiome(Biomes biome)
    {
        switch (biome)
        {
            case Biomes.Forest:
                Instance.Texture = grassTexture;
                break;
            case Biomes.Desert:
                Instance.Texture = desertTexture;
                break;
            case Biomes.Ice:
                Instance.Texture = iceTexture;
                break;
            case Biomes.Mushroom:
                Instance.Texture = mushroomTexture;
                break;
        }

        return this;
    }

    public StarParticleSpawner SetDirectionNormal(Vector2 directionNormal)
    {
        ParticleProcessMaterial material = (ParticleProcessMaterial)Instance.ProcessMaterial;
        if (directionNormal.X > 0)
            material.DirectionalVelocityCurve = rightVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.X < 0)
            material.DirectionalVelocityCurve = leftVelCurve.Duplicate(true) as CurveXyzTexture;
        else if (directionNormal.Y > 0)
            material.DirectionalVelocityCurve = downVelCurve.Duplicate(true) as CurveXyzTexture;
        else 
            material.DirectionalVelocityCurve = upVelCurve.Duplicate(true) as CurveXyzTexture;
        
        material.Direction = new Vector3(directionNormal.X, directionNormal.Y*extraYSpread, 0); 
        Instance.ProcessMaterial = material.Duplicate() as ParticleProcessMaterial;

        return this;
    }
    

}