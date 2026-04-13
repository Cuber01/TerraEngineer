using Godot;

namespace TerraEngineer.game.sprite;

[Tool]
[GlobalClass]
public partial class PlayerWrapper : SpriteWrapper
{
    private Sprite2D sprite;
    private AnimationPlayer player;
    
    public override void Init(Node2D node)
    {
        sprite = (Sprite2D)node;
        player = sprite.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        player.AnimationFinished += (animName) => InvokeAnimationFinished();
        Initialized = true;
        
        if(player == null)
            GD.PrintErr("Animation Player Wrapper: Cannot find AnimationPlayer node.");
    } 
    
    public override void Play(string anim, float customSpeed=1f) 
    {
        player.Play(anim, -1D, customSpeed);
    }
    
    public override void Flip() => sprite.FlipH = !sprite.FlipH;
    public override bool GetFlipH() => sprite.FlipH;
    
    public override void SetFrame(int num)
    {
        throw new System.NotImplementedException();
    }
    
    public override void SetTexture(Texture2D texture)
    {
        throw new System.NotImplementedException();
    }
}