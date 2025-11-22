using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class HealthComponent : Component
{
    
    public delegate void HealthChangedEventHandler(int oldHealth, int currentHealth, int maxHealth);
    public delegate void DeathEventHandler();
    
    public event HealthChangedEventHandler? OnHealthChanged;
    public event DeathEventHandler? OnDeath;

    [Export] public int MaxHealth;
    
    public int CurrentHealth { get; private set; }

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }
    
    public void SetMaxHealth(int amount, bool adjustCurrentHealth = true)
    {
        if (amount < 1) amount = 1;
        
        MaxHealth = amount;
        
        if (adjustCurrentHealth)
        {
            SetHealth(MaxHealth);
        }
    }
    
    public void SetHealth(int amount)
    {
        if (amount < 0) amount = 0;
        if (amount > MaxHealth) amount = MaxHealth;
        
        var oldHealth = CurrentHealth;
        CurrentHealth = amount;
        OnHealthChanged?.Invoke(oldHealth, CurrentHealth, MaxHealth);
        
        if (CurrentHealth == 0)
        {
            OnDeath?.Invoke();
        }
    }
    
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        
        SetHealth(CurrentHealth + amount);
    }
    
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        
        SetHealth(CurrentHealth - amount);
    }
    
    public void RestoreToFullHealth()
    {
        SetHealth(MaxHealth);
    }
    
    public void Kill()
    {
        SetHealth(0);
    }
    
}