using Godot;
using System;

namespace TerraEngineer.game.sprite;

[Tool]
[GlobalClass]
public abstract partial class SpriteWrapper : Resource
{
    public event Action AnimationFinished;
    protected void InvokeAnimationFinished() => AnimationFinished?.Invoke();
    
    public abstract void Init(Node2D node);
    public abstract void Play(string anim);
    public abstract void Flip();
    public abstract bool GetFlipH();
    public abstract void SetFrame(int num);
    public abstract void SetTexture(Texture2D texture);
}