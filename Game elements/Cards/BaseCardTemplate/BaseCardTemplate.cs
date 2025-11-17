using Godot;
using System;

[GlobalClass]
public partial class BaseCardTemplate : Control
{
    [Export] public BaseCard? CardData { get; set; }
    [Export] private HealthManager? healthManager;
    [Export] private AttackManager? attackManager; 
    [Export] private CostManager? costManager;
    [Export] private Sprite2D? cardOverlay;
    [Export] private Sprite2D? cardOnFieldOverlay;
    [Export] private Sprite2D? cardDescription;
    [Export] private Sprite2D? cardBackside;
    protected CardState? currentState;
    private bool isHovering;
    public bool isFlipped;
    public bool isCardPlayable;
    public bool isCheckingDescription;
    // idk if i should be doing it like that so im putting it between comments in case i want to change it
    private double _mouseDownTime;
    private Vector2 _mouseDownPosition;
    private bool _isMouseDown;
    private const float HOLD_DURATION = 1.0f;
    private const float MOVE_TOLERANCE = 10f;
    // end of segment
    
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
            {
                // Mouse pressed on this card
                _mouseDownTime = Time.GetTicksMsec();
                _mouseDownPosition = GetGlobalMousePosition();
                _isMouseDown = true;
            }
            else
            {
                // Mouse released - hide description
                if (isCheckingDescription)
                {
                    isCheckingDescription = false;
                }
                _isMouseDown = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        CheckForHold();
        CheckApperance();
        currentState?.Update(delta);
    }

    public override void _Ready()
    {
        healthManager = GetNode<HealthManager>("HealthManager");
        attackManager = GetNode<AttackManager>("AttackManager");
        costManager = GetNode<CostManager>("CostManager");

        healthManager.Health = CardData?.Health??0;
        healthManager.Defense = CardData?.Defense??0;
        healthManager.TimeLeftOnField = CardData?.TimeLeftOnField??0;
        healthManager.Initialize();
        
        attackManager.Attack = CardData?.Attack??0;
        attackManager.HowManyAttacks = CardData?.HowManyAttacks??0;
        attackManager.Initialize();
        
        costManager.Cost = CardData?.Cost??0;
        costManager.Initialize();

        CustomMinimumSize = new Vector2(32, 48);
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("HealthLabel").Text = healthManager?.CurrentHealth.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("DefenseLabel").Text = healthManager?.CurrentDefense.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("AttackLabel").Text = attackManager?.CurrentAttack.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("AttackAmountLabel").Text = attackManager?.CurrentHowManyAttacks.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("CostLabel").Text = costManager?.CurrentCost.ToString();
        GetNode<RichTextLabel>("NameLabel").Text = CardData?.Name.ToString();
        GetNode<Node2D>("CardOverlay").GetNode<RichTextLabel>("TimeLeftLabel").Text = healthManager?.TimeLeftOnField.ToString();
        GetNode<Sprite2D>("CardDescription").GetNode<RichTextLabel>("DescriptionLabel").Text = CardData?.Description.ToString();
        GetNode<Sprite2D>("CardArt").Texture = CardData?.Art;
    }

    private void CheckForHold()
    {
        if (_isMouseDown && !isCheckingDescription && !isFlipped)
        {
            // Check if mouse moved too much
            Vector2 currentPos = GetGlobalMousePosition();
            float distance = _mouseDownPosition.DistanceTo(currentPos);
            
            if (distance < MOVE_TOLERANCE)
            {
                // Check if held long enough
                double holdTime = (Time.GetTicksMsec() - _mouseDownTime) / 1000.0;
                if (holdTime >= HOLD_DURATION)
                {
                    isCheckingDescription = true;
                    _isMouseDown = false; // Prevent retriggering
                }
            }
            else
            {
                // Mouse moved too far - cancel
                _isMouseDown = false;
            }
        }
    }

    public void CheckApperance()
    {
        if (isHovering && !isFlipped)
        {
            ZIndex = 3;
            Scale = new Vector2(4.2f, 4.2f);
        } else 
        { 
            ZIndex = 1;
            Scale = new Vector2(4.0f, 4.0f); 
        }
        if (isFlipped)
        {
            cardBackside?.Show();
        } else {cardBackside?.Hide();}
        if (isCheckingDescription)
        {
            cardDescription?.Show();
        } else { cardDescription?.Hide(); }
    }

    public void ChangeState(CardState newState)
    {
        CardState? optionalState = newState;
        currentState?.Exit(this, ref optionalState);
        currentState = optionalState;
        currentState?.Enter(this, ref optionalState);
        if (optionalState != null && optionalState != newState) ChangeState(optionalState);
        else currentState = null;
    }

    public void onMouseEntered()
    {
        isHovering = true;
        if (!isFlipped)
        {
            cardOverlay?.Show();
        }
    }

    public void onMouseExited()
    {
        isHovering = false;
        cardOverlay?.Hide();

        if (isCheckingDescription)
        {
            isCheckingDescription = false;
        }
        
        _isMouseDown = false;
    }

    public void onAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlacablePosition"))
        {
            GD.Print("Card is placable");
        }
    }
}

