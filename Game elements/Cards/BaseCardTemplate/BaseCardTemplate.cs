using Godot;
using System;

[GlobalClass]
public partial class BaseCardTemplate : Control
{
    [Export] public BaseCard CardData { get; set; }

    [Export] private HealthManager healthManager;
    [Export] private AttackManager attackManager; 
    [Export] private CostManager costManager;
    [Export] private Sprite2D cardOverlay;
    
    public override void _Ready()
    {
        healthManager = GetNode<HealthManager>("HealthManager");
        attackManager = GetNode<AttackManager>("AttackManager");
        costManager = GetNode<CostManager>("CostManager");

        healthManager.Health = CardData.Health;
        healthManager.Defense = CardData.Defense;
        healthManager.TimeLeftOnField = CardData.TimeLeftOnField;
        healthManager.Initialize();
        
        attackManager.Attack = CardData.Attack;
        attackManager.HowManyAttacks = CardData.HowManyAttacks;
        attackManager.Initialize();
        
        costManager.Cost = CardData.Cost;
        costManager.Initialize();

        CustomMinimumSize = new Vector2(32, 48);
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("HealthLabel").Text = healthManager.CurrentHealth.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("DefenseLabel").Text = healthManager.CurrentDefense.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("AttackLabel").Text = attackManager.CurrentAttack.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("CostLabel").Text = costManager.CurrentCost.ToString();
        GetNode<RichTextLabel>("NameLabel").Text = CardData.Name.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("TimeLeftLabel").Text = healthManager.TimeLeftOnField.ToString();
        GetNode<Sprite2D>("CardDescription").GetNode<RichTextLabel>("DescriptionLabel").Text = CardData.Description.ToString();
        GetNode<Sprite2D>("CardArt").Texture = CardData.Art;
    }

    public void onMouseEntered()
    {
        Scale = new Vector2(4.5f, 4.5f);
        GD.Print("mouse entered");
        cardOverlay.Show();
    }

    public void onMouseExited()
    {
        Scale = new Vector2(4.0f, 4.0f);
        cardOverlay.Hide();
    }
}
