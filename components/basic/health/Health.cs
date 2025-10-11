using Godot;

namespace TENamespace.health;

public partial class Health : Component
{
    public delegate void InvincibilityEndedEventHandler();
    public event InvincibilityEndedEventHandler InvincibilityEnded;
    
    [Export] private int maxHealth;
    private int health;

    [Export] private float invincibilityTimeOnHit = 1f;
    private bool invincible = false;
    private float invincibilityDelay = 0;
    
    public void ChangeHealth(int amount)
    {
        if (invincible && amount < 0)
        {
            return;
        }
        
        health += amount;
        if (health <= 0)
        {
            Actor.Die();
        }
        else if (invincibilityTimeOnHit > 0)
        {
            MakeInvincible();
        }
    }
    
    public void MakeInvincible()
    {
        invincible = true;
        TimerManager.Schedule(invincibilityTimeOnHit, (t) =>
        {
            invincible = false;
            InvincibilityEnded?.Invoke();
        });
    }
}