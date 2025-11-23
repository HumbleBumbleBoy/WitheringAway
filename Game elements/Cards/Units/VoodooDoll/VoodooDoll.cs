using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class VoodooDoll : BaseCardTemplate
{
    // on take damage, deal damage back to atacker

    protected override void OnDamageTaken(BaseCardTemplate? attacker, int damage, bool isPlayer)
    {
        base.OnDamageTaken(attacker, damage, isPlayer);

        if (attacker == null || !IsInstanceValid(attacker)) return;
        
        var oldDamage = GetAttackDamage();
        SetAttackDamage(damage);
        
        Attack(attacker, isPlayer: isPlayer);
            
        SetAttackDamage(oldDamage);
    }
}
