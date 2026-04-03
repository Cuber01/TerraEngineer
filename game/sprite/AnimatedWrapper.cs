using Godot;

namespace TerraEngineer.game.sprite;

[Tool]
[GlobalClass]
public partial class AnimatedWrapper : SpriteWrapper
{
    private AnimatedSprite2D animSprite;

    public override void Init(Node2D node)
    {
        animSprite = (AnimatedSprite2D)node;
        animSprite.AnimationFinished += InvokeAnimationFinished;
        Initialized = true;
    }

    public override void Play(string anim) => animSprite.Play(anim);
    public override void Flip() => animSprite.FlipH = !animSprite.FlipH;
    public override bool GetFlipH() => animSprite.FlipH;
    public override void SetFrame(int num) => animSprite.Frame = num;

    public override void SetTexture(Texture2D texture)
    {
        animSprite.SpriteFrames = (SpriteFrames)animSprite.SpriteFrames.Duplicate();
        animSprite.SpriteFrames.SetFrame(Names.Animations.Default, 0, (Texture2D)texture.Duplicate());    
    }
    
}