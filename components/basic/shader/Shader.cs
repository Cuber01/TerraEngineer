using System.Collections.Generic;
using Godot;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic.shader;

public partial class Shader : Component
{
    [Export] private ShaderMaterial[] importedShaders;
    private Dictionary<string, ShaderMaterial> shaders = new();
    private AnimatedSprite2D sprite;
    private ShaderMaterial material;
    
    private bool running = false;
    private float deltaTime = 0f;
    
    public override void Init(Mob actor)
    {
        base.Init(actor);
        
        sprite = Actor.Sprite;
        material = (ShaderMaterial)sprite.Material;
        
        foreach (ShaderMaterial shader in importedShaders)
        {
            shaders.Add(shader.GetName(), shader);
        }
    }

    public override void Update(float delta)
    {
        if (running)
        {
            deltaTime += delta;
            material.SetShaderParameter("delta_time", deltaTime); 
        }
    }

    public void SetShader(string name)
    {
        material = shaders[name];
        sprite.Material = material;
    }
        
    public void ToggleShader(bool enabled)
    {
        running = enabled;
        material.SetShaderParameter("delta_time", 0f);
        material.SetShaderParameter("run", enabled);

        if (!enabled)
        {
            deltaTime = 0f;
        }
    }
}