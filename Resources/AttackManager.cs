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
        var parent = GetParent<BaseCardTemplate>();
        if (parent != null)
        {
            parent.cardOverlay.GetNode<RichTextLabel>("AttackLabel").Text = CurrentAttack.ToString();
            parent.cardOverlay.GetNode<RichTextLabel>("AttackAmountLabel").Text = CurrentHowManyAttacks.ToString();
            
            parent.cardOnFieldOverlay.GetNode<RichTextLabel>("AttackLabel").Text = CurrentAttack.ToString();
            parent.cardOnFieldOverlay.GetNode<RichTextLabel>("AttackAmountLabel").Text = CurrentHowManyAttacks.ToString();
        }
    }
}
