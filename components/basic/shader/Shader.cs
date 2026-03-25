using System;
using System.Collections.Generic;
using Godot;
using TerraEngineer;
using TerraEngineer.entities.mobs;

namespace TENamespace.basic.shader;

public partial class Shader : Component
{
    [Export] private ShaderMaterial[] importedShaders;
    private Dictionary<StringName, ShaderMaterial> shaders = new();
    private AnimatedSprite2D sprite;
    private ShaderMaterial material;
    
    private bool running = false;
    private float deltaTime = 0f;

    private Entity entityActor;

    public override void Init(Node2D actor)
    {
        base.Init(actor);
        if (actor is Entity entity)
        {
            entityActor = entity;
        }
        else
        {
            throw new Exception("Shader component requires Entity actor.");
        }
        
        sprite = entityActor.Sprite;
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
            material.SetShaderParameter(Names.Shader.Delta, deltaTime); 
        }
    }

    public void SetShader(StringName name)
    {
        material = shaders[name];
        sprite.Material = material;
    }
        
    public void ToggleShader(bool enabled)
    {
        running = enabled;
        material.SetShaderParameter(Names.Shader.Delta, 0f);
        material.SetShaderParameter(Names.Shader.Run, enabled);

        if (!enabled)
        {
            deltaTime = 0f;
        }
    }
}