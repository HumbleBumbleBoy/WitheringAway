using Godot;
using System;

public partial class CostManager : Node
{
    public int Cost;
    public int CurrentCost;

    public void Initialize()
    {
        CurrentCost = Cost;
    }

    public void UpdateLabels()
    {
        var parent = GetParent<BaseCardTemplate>();
        if (parent != null)
        {
            parent.cardOverlay.GetNode<RichTextLabel>("CostLabel").Text = CurrentCost.ToString();
        }
    }
}
