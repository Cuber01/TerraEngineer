using Godot;

namespace TENamespace.health;

public partial class Health : Component
{
    public delegate void InvincibilityEndedEventHandler();
    public event InvincibilityEndedEventHandler InvincibilityEnded;
    
    public delegate void HealthChangedEventHandler(int currentHealth, int amount);
    public event HealthChangedEventHandler HealthChanged;
    
    public delegate void MaxHealthChangedEventHandler(int newMax);
    public event MaxHealthChangedEventHandler MaxHealthChanged;
    
    [Export] public int MaxHealth;
    private int health;

    [Export] private float invincibilityTimeOnHit = 1f;
    private bool invincible = false;
    private float invincibilityDelay = 0;

    public override void _Ready()
    {
        health = MaxHealth;
    }
    
    // Does not check for invincibility or anything!
    public void SetHealth(int health) => this.health = health;
    
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
        HealthChanged?.Invoke(health, amount);
    }

    public void ChangeMaxHealth(int amount)
    {
        MaxHealth += amount;
        MaxHealthChanged?.Invoke(MaxHealth);
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