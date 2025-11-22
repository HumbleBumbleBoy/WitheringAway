using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;

[GlobalClass]
public partial class BaseCardTemplate : Control
{
    [Export] public BaseCard? CardData { get; set; }
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
    [Export] public Node? animationFolder;
    
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
    
    private float _timeSinceLastHealthUpdate = 0f;

    public BaseCardTemplate()
    {
        StateMachine = this.CreateStateMachine(this);
    }

    public override void _Process(double delta)
    {
        CheckForHold();
        CheckApperance();
        UpdateVisuals();
    }

    public override void _Ready()
    {
        cardOnFieldOverlay?.Hide();
        
        turnManager = GetTree().CurrentScene.GetNode<TurnManager>("TurnManager");

        attackManager = GetNode<AttackManager>("AttackManager");
        costManager = GetNode<CostManager>("CostManager");

        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.SetMaxHealth(CardData?.Health ?? healthComponent.MaxHealth);
        
        var defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        defenseComponent.SetDefense(CardData?.Defense ?? defenseComponent.Defense);

        /*
        healthManager.Health = CardData?.Health??0;
        healthManager.Defense = CardData?.Defense??0;
        healthManager.TimeLeftOnField = CardData?.TimeLeftOnField??0;
        healthManager.Initialize();*/
        
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
                
            IState<BaseCardTemplate> nextState = CheckIfValidDropPosition() ? new CardEnteredField(true) : new CardInHand();
            StateMachine.ChangeState(nextState);
        };
    }
    
    public void TakeDamage(int damage)
    {
        var defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        var remainingDamage = defenseComponent.AbsorbDamage(damage);
        
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.TakeDamage(remainingDamage);
        
        var originalPosition = GlobalPosition;
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", originalPosition + new Vector2(5, -5), 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "global_position", originalPosition - new Vector2(5, -5), 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this, "global_position", originalPosition, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
    }
    
    public async Task AttackOnce(BaseCardTemplate targetCard)
    {
        await PlaySound("Attack");
        
        var originalPosition = GlobalPosition;
        var directionToTarget = (targetCard.GlobalPosition - GlobalPosition).Normalized();
        var attackPosition = originalPosition + directionToTarget * 20;
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", attackPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenCallback(Callable.From(() => 
        {
            targetCard.TakeDamage(attackManager?.Attack ?? 0);
            _ = targetCard.PlaySound("Hurt");
        }));
        tween.TweenProperty(this, "global_position", originalPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        await ToSignal(tween, "finished");

        await this.Wait(0.3f);
    }
    
    public async Task Attack(BaseCardTemplate targetCard)
    {
        for (var i = 0; i < (attackManager?.HowManyAttacks ?? 1); i++)
        {
            await AttackOnce(targetCard);
        }
    }

    public bool ShouldDie()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        return healthComponent.CurrentHealth <= 0;
    }
    
    public async Task PlaySound(string soundName)
    {
        var soundPlayer = audioFolder?.GetNodeOrNull(soundName) as AudioStreamPlayer;
        soundPlayer?.Play();
        if (soundPlayer != null)
        {
            await ToSignal(soundPlayer, "finished");
        }
    }

    public async Task PlayAnimation(string animationName, float delay = 0.0f)
    {
        if (animationFolder?.GetNodeOrNull(animationName) is AnimatedSprite2D animationPlayer)
        {
            if (delay > 0.0f)
            {
                await this.Wait(delay);
            }
            
            animationPlayer.Show();
            animationPlayer.Play("execute");
            await ToSignal(animationPlayer, "animation_finished");
            animationPlayer.Hide();
        }
    }

    public void DisableArt()
    {
        cardArt?.Hide();
        cardOnFieldOverlay?.Hide();
        cardBackground?.Hide();
        cardName?.Hide();
        cardDescription?.Hide();
    }
    
    private void UpdateVisuals()
    {
        UpdateVisualHealth();
        UpdateVisualDefense();
        
        attackManager?.UpdateLabels(); 
        if (!isCardInField) { costManager?.UpdateLabels(); }

        cardName.Text = CardData?.Name ?? "";
        cardArt.Texture = CardData?.Art;
        cardDescription.GetNode<RichTextLabel>("DescriptionLabel").Text = CardData?.Description;
    }

    private void UpdateVisualHealth()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        
        var healthLabel = cardOverlay.GetNode<RichTextLabel>("HealthLabel");
        var healthOnFieldLabel = cardOnFieldOverlay.GetNode<RichTextLabel>("HealthLabel");
        if (healthLabel.Text.Length == 0 || healthLabel.Text == "0")
        {
            healthLabel.Text = healthComponent.CurrentHealth.ToString();
            healthOnFieldLabel.Text = healthComponent.CurrentHealth.ToString();
            return;
        }
        
        var currentHealthLabelValue = int.Parse(healthLabel.Text);
        if (currentHealthLabelValue != healthComponent.CurrentHealth)
        {
            var difference = healthComponent.CurrentHealth - currentHealthLabelValue;
            var speed = 0.2f - Mathf.Clamp(Mathf.Abs(difference) * 0.1f / 5.0f, 0.0f, 0.2f);
            if (_timeSinceLastHealthUpdate >= speed)
            {
                var direction = Mathf.Sign(difference);
                
                currentHealthLabelValue += direction;
                _timeSinceLastHealthUpdate = 0f;
            }
        }
        
        _timeSinceLastHealthUpdate += (float) GetProcessDeltaTime();
        
        healthLabel.Text = currentHealthLabelValue.ToString();
        healthOnFieldLabel.Text = currentHealthLabelValue.ToString();
    }
    
    private void UpdateVisualDefense()
    {
        var defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        cardOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = defenseComponent.Defense.ToString();
        cardOnFieldOverlay.GetNode<RichTextLabel>("DefenseLabel").Text = defenseComponent.Defense.ToString();
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
        if (area.IsInGroup("PlacablePosition"))
        {
            nameOfAreaPlaceOurCardIn = area.Name;
            whereIsAreaWePlaceOurCardIn = area.GlobalPosition;
        }
        
        isCardPlayable = turnManager.canPlayerPlaceCards;
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

