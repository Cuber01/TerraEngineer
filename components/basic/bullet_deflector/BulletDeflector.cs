using Godot;
using System;

public partial class BulletDeflector : Node2D
{
    [ExportGroup("Internal")]
    
    [Export] private CollisionShape2D hitbox;

    public void ChangeState(bool active)
    {
        hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, !active);
    }
}
