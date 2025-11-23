using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;

namespace Witheringaway.Game_elements.Cards;

[GlobalClass]
public partial class TrickEffect : Resource
{
    
    [Export] public int MinRound { get; set; }
    
    [Export] public int BonusDefense { get; set; }
    [Export] public int BonusHealth { get; set; }
    [Export] public int BonusDamage { get; set; }
    [Export] public int BonusAttacks { get; set; }
    [Export] public int BonusTimeOnField { get; set; }
    
    [Export] public int ExtraCardsToDraw { get; set; }
    
    /// <summary>
    /// Flat damage dealt when the trick is used.
    /// </summary>
    [Export] public int FlatDamage { get; set; }
    
    public async Task Apply(Duelist duelist)
    {
        await duelist.TakeDamage(FlatDamage);
    }

    public void Apply(BaseCardTemplate template)
    {
        var healthComponent = template.FirstComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(healthComponent.MaxHealth + BonusHealth, adjustCurrentHealth: false);
            healthComponent.Heal(BonusHealth);
        }
        
        var defenseComponent = template.FirstComponent<DefenseComponent>();
        defenseComponent?.SetDefense(defenseComponent.Defense + BonusDefense);

        var attackComponent = template.FirstComponent<AttackComponent>();
        if (attackComponent != null)
        {
            attackComponent.SetAttackDamage(attackComponent.AttackDamage + BonusDamage);
            attackComponent.SetAttackCount(attackComponent.AttackCount + BonusAttacks);
        }
        
        var timeOnFieldComponent = template.FirstComponent<TimeOnFieldComponent>();
        timeOnFieldComponent?.SetTimeOnField(timeOnFieldComponent.TimeOnField + BonusTimeOnField);
        
        template.TakeDamage(FlatDamage);
    }
    
}