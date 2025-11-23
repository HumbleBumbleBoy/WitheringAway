using Godot;
using System;

public partial class AttackManager : Node
{
    public int Attack;
    public int HowManyAttacks;
    public int CurrentAttack;
    public int CurrentHowManyAttacks;

    public void Initialize()
    {
        CurrentAttack = Attack;
        CurrentHowManyAttacks = HowManyAttacks;
    }

    public void UpdateLabels()
    {
        var parent = GetParent<Witheringaway.Game_elements.Cards.BaseCardTemplate.BaseCardTemplate>();
        if (parent != null)
        {
            parent.CardOverlay.GetNode<RichTextLabel>("AttackLabel").Text = CurrentAttack.ToString();
            parent.CardOverlay.GetNode<RichTextLabel>("AttackAmountLabel").Text = CurrentHowManyAttacks.ToString();
            
            parent.CardOnFieldOverlay.GetNode<RichTextLabel>("AttackLabel").Text = CurrentAttack.ToString();
            parent.CardOnFieldOverlay.GetNode<RichTextLabel>("AttackAmountLabel").Text = CurrentHowManyAttacks.ToString();
        }
    }
}
