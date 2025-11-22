using System.Text.RegularExpressions;
using Godot;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;

[GlobalClass]
public partial class BaseCardTemplate : Control
{
    [Export] public BaseCard? CardData { get; set; }
    [Export] public HealthManager? healthManager;
    [Export] public AttackManager? attackManager; 
    [Export] public CostManager? costManager;
    [Export] public Sprite2D? cardOverlay;
    [Export] public Sprite2D? cardArt;
    [Export] public Sprite2D? cardOnFieldOverlay;
    [Export] public Sprite2D? cardDescription;
    [Export] public Sprite2D? cardBackground;
    [Export] public Sprite2D? cardBackside;
    [Export] public RichTextLabel? cardName;
    [Export] public Node? audioFolder;
    
    public StateMachine<BaseCardTemplate> StateMachine { get; }
    
    private bool isHovering;
    public bool isFlipped;
    public bool isCardPlayable;
    public bool isCheckingDescription;
    // idk if i should be doing it like that so im putting it between comments in case i want to change it
    private double _mouseDownTime;
    private Vector2 _mouseDownPosition;
    private Vector2 _mouseDownCardPosition;
    private bool _isMouseDown;
    public bool isCardInField;
    public Vector2 whereIsAreaWePlaceOurCardIn;
    public string? nameOfAreaPlaceOurCardIn;
    private const float HOLD_DURATION = 1.0f;
    private const float MOVE_TOLERANCE = 10f;
    // end of segment
    private TurnManager? turnManager;

    public BaseCardTemplate()
    {
        StateMachine = this.CreateStateMachine(this);
    }

    public override void _Process(double delta)
    {
        CheckForHold();
        CheckApperance();
    }

    public override void _Ready()
    {
        turnManager = GetTree().CurrentScene.GetNode<TurnManager>("TurnManager");

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

        var draggable = this.FirstComponent<DraggableComponent>();
        if (draggable is null) return;
        draggable.DragStarted += _ =>
        {
            StateMachine.ChangeState(new DraggingCard());
        };
        
        draggable.DragEnded += (_, _) =>
        {
            if (StateMachine.CurrentState is not DraggingCard) return;
                
            IState<BaseCardTemplate> nextState = CheckIfValidDropPosition() ? new CardEnteredField() : new CardInHand();
            StateMachine.ChangeState(nextState);
        };
    }
    
    private void UpdateVisuals()
    {
        healthManager?.UpdateLabels();
        attackManager?.UpdateLabels(); 
        if (!isCardInField) { costManager?.UpdateLabels(); }

        cardName.Text = CardData?.Name ?? "";
        cardArt.Texture = CardData?.Art;
        cardDescription.GetNode<RichTextLabel>("DescriptionLabel").Text = CardData?.Description;
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
        if (isHovering && !isFlipped && !isCardInField)
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

    public void onMouseEntered()
    {
        isHovering = true;
        if (!isFlipped && !isCardInField)
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

    public void onAreaEntered(Area2D area)  // when card colides with either player or enemy placable position
    {
        if (area.IsInGroup("PlacablePosition") && turnManager.canPlayerPlaceCards) // <- we need canplayerplacecards for both player and enemy side of the board
        {
            whereIsAreaWePlaceOurCardIn = area.GlobalPosition;
            nameOfAreaPlaceOurCardIn = area.Name;
            isCardPlayable = true;
        } else isCardPlayable = false;
    }

    public void onAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlacablePosition"))
        {
            
            isCardPlayable = false;
        }
    }

    private bool CheckIfValidDropPosition()
{
        if (!isCardPlayable) return false;
        
        FieldData fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
        string numberOnly = Regex.Replace(nameOfAreaPlaceOurCardIn, @"[^\d]", "");
        int laneIndex = int.Parse(numberOnly) - 1;
        
        return !fieldData.IsLaneOccupied(laneIndex, true);
    }
}

