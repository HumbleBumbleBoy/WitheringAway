using Godot;
using System;

public partial class HealthManager : Node
{
    public int Health;
    public int Defense;
    public int CurrentHealth;
    public int CurrentDefense;
    public int TimeLeftOnField;

    public void Initialize()
    {
        CurrentHealth = Health;
        CurrentDefense = Defense;
    }

    public void UpdateLabels()
    {
        // Labels are in the parent's overlay nodes
        var parent = GetParent<Witheringaway.Game_elements.Cards.BaseCardTemplate.BaseCardTemplate>();
        if (parent != null)
        {
            parent.CardOverlay.GetNode<RichTextLabel>("HealthLabel").Text = CurrentHealth.ToString();
            parent.CardOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = CurrentDefense.ToString();
            parent.CardOverlay.GetNode<RichTextLabel>("TimeLeftLabel").Text = TimeLeftOnField.ToString();
            
            parent.CardOnFieldOverlay.GetNode<RichTextLabel>("HealthLabel").Text = CurrentHealth.ToString();
            parent.CardOnFieldOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = CurrentDefense.ToString();
            parent.CardOnFieldOverlay.GetNode<RichTextLabel>("TimeLeftLabel").Text = TimeLeftOnField.ToString();
        }
    }
}
