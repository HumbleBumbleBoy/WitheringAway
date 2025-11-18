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
        var parent = GetParent<BaseCardTemplate>();
        if (parent != null)
        {
            parent.cardOverlay.GetNode<RichTextLabel>("HealthLabel").Text = CurrentHealth.ToString();
            parent.cardOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = CurrentDefense.ToString();
            parent.cardOverlay.GetNode<RichTextLabel>("TimeLeftLabel").Text = TimeLeftOnField.ToString();
            
            parent.cardOnFieldOverlay.GetNode<RichTextLabel>("HealthLabel").Text = CurrentHealth.ToString();
            parent.cardOnFieldOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = CurrentDefense.ToString();
            parent.cardOnFieldOverlay.GetNode<RichTextLabel>("TimeLeftLabel").Text = TimeLeftOnField.ToString();
        }
    }
}
