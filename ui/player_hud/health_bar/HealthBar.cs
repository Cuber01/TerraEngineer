using Godot;
using System;
using TENamespace.health;
using TerraEngineer.ui.player_hud;

public partial class HealthBar : TextureProgressBar, IConnectable<Player>
{
    private Health hpComponent;
    
    public void Connect(Player player)
    {
        hpComponent = player.CM.GetComponent<Health>();
        
        Value = hpComponent.MaxHealth;
        MaxValue = hpComponent.MaxHealth;
        
        hpComponent.HealthChanged += onHealthChanged;
        hpComponent.MaxHealthChanged += onMaxHealthChanged;
    }

    public void Disconnect(Player player)
    {
        hpComponent.HealthChanged -= onHealthChanged;
        hpComponent.MaxHealthChanged -= onMaxHealthChanged;
    }

    private void onHealthChanged(int health, int amount)
    {
        Value = health;
    }

    private void onMaxHealthChanged(int maxHealth)
    {
        MaxValue = maxHealth;
    }
}
