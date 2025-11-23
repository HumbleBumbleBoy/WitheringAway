using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;

namespace Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

[GlobalClass]
public partial class BaseCardTemplate : Control
{
    
    // hacky ass shit
    private static bool _isDraggingAnyCard;
    
    
    [Export] public BaseCard? CardData { get; set; }
    [Export] public AttackManager? AttackManager; 
    [Export] public Sprite2D? CardOverlay;
    [Export] public Sprite2D? CardArt;
    [Export] public Sprite2D? CardOnFieldOverlay;
    [Export] public Sprite2D? CardDescription;
    [Export] public Sprite2D? CardBackground;
    [Export] public Sprite2D? CardBackside;
    [Export] public RichTextLabel? CardName;
    [Export] public Node? AudioFolder;
    [Export] public Node? AnimationFolder;
    
    public StateMachine<BaseCardTemplate> StateMachine { get; }
    
    private bool isHovering;
    public bool IsFlipped;
    public bool IsCardPlayable;
    public bool IsCheckingDescription;
    // idk if i should be doing it like that so im putting it between comments in case i want to change it
    private double _mouseDownTime;
    private Vector2 _mouseDownPosition;
    private Vector2 _mouseDownCardPosition;
    public bool IsCardInField;
    public Vector2 PlacedAreaLocation;
    public string? PlacedAreaName;
    // end of segment
    private TurnManager? turnManager;
    
    private float _timeSinceLastHealthUpdate;
    private float _timeSinceLastDefenseUpdate;
    private float _timeSinceLastTimeOnFieldUpdate;

    public BaseCardTemplate()
    {
        StateMachine = this.CreateStateMachine(this);
    }

    public override void _Process(double delta)
    {
        if (IsCardInField)
        {
            // Hacky way to detect hovering over the card only on the art part
            var rect = CardArt!.GetRect();
            rect = new Rect2(rect.Position, rect.Size);
            rect.Position += new Vector2(0, rect.Size.Y / 2f);
            rect.Size = new Vector2(rect.Size.X, rect.Size.Y * 0.65f);
            
            isHovering = rect.HasPoint(GetLocalMousePosition());
            CardOverlay?.Hide();
        }

        IsCheckingDescription = isHovering && Input.IsActionPressed("check_card_description");
        
        CheckAppearance();
        UpdateVisuals();
    }

    public override void _Ready()
    {
        CardOnFieldOverlay?.Hide();
        
        turnManager = GetTree().CurrentScene.GetNode<TurnManager>("TurnManager");

        AttackManager = GetNode<AttackManager>("AttackManager");

        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.SetMaxHealth(CardData?.Health ?? healthComponent.MaxHealth);
        
        var defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        defenseComponent.SetDefense(CardData?.Defense ?? defenseComponent.Defense);

        var timeOnFieldComponent = this.GetOrAddComponent<TimeOnFieldComponent>();
        timeOnFieldComponent.SetTimeOnField(CardData?.TimeLeftOnField ?? timeOnFieldComponent.TimeOnField);
        
        var costComponent = this.GetOrAddComponent<CostComponent>();
        costComponent.SetCost(CardData?.Cost ?? costComponent.Cost);
        
        AttackManager.Attack = CardData?.Attack??0;
        AttackManager.HowManyAttacks = CardData?.HowManyAttacks??0;
        AttackManager.Initialize();

        CustomMinimumSize = new Vector2(32, 48);
        UpdateVisuals();

        var draggable = this.FirstComponent<DraggableComponent>();
        if (draggable is null) return;
        
        draggable.CanStartDrag += () => !_isDraggingAnyCard;
        
        draggable.DragStarted += _ =>
        {
            _isDraggingAnyCard = true;
            StateMachine.ChangeState(new DraggingCard());
        };
        
        draggable.DragEnded += (_, _) =>
        {
            if (StateMachine.CurrentState is not DraggingCard) return;
            
            _isDraggingAnyCard = false;

            if (IsValidDropPosition())
            {
                var duelist = GetTree().GetFirstNodeInGroup("PlayerDuelist") as Duelist;
                duelist?.TakeSouls(CardData?.Cost ?? 0);
                StateMachine.ChangeState(new CardEnteredField(true));
            }
            else
            {
                StateMachine.ChangeState(new CardInHand());
            }
        };
    }
    
    public int GetCurrentHealth()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        return healthComponent.CurrentHealth;
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
    
    public async Task AttackOnce(Control target, Callable? onMidAttack = null)
    {
        var originalPosition = GlobalPosition;
        var directionToTarget = (target.GlobalPosition - GlobalPosition).Normalized();
        var attackPosition = originalPosition + directionToTarget * 20;
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", attackPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenCallback(onMidAttack ?? _AttackCard(target) ?? Callable.From(() => {}));
        tween.TweenProperty(this, "global_position", originalPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        await ToSignal(tween, "finished");

        await this.Wait(0.3f);
    }
    
    public async Task Attack(Control target, Callable? onMidAttack = null)
    {
        for (var i = 0; i < (AttackManager?.HowManyAttacks ?? 1); i++)
        {
            await AttackOnce(target, onMidAttack);
        }
    }

    private Callable? _AttackCard(Control node)
    {
        if (node is BaseCardTemplate card)
        {
            return Callable.From(() =>
            {
                _ = PlaySound("Attack");
                card.TakeDamage(AttackManager?.Attack ?? 0);
                _ = card.PlaySound("Hurt");
                _ = card.PlayAnimation("Hurt");
            });
        }

        return null;
    }

    public bool ShouldDie()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        return healthComponent.CurrentHealth <= 0;
    }
    
    public async Task PlaySound(string soundName)
    {
        var soundPlayer = AudioFolder?.GetNodeOrNull(soundName) as AudioStreamPlayer;
        soundPlayer?.Play();
        if (soundPlayer != null)
        {
            await ToSignal(soundPlayer, "finished");
        }
    }

    public async Task PlayAnimation(string animationName, float delay = 0.0f)
    {
        if (AnimationFolder?.GetNodeOrNull(animationName) is AnimatedSprite2D animationPlayer)
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
        CardArt?.Hide();
        CardOnFieldOverlay?.Hide();
        CardBackground?.Hide();
        CardName?.Hide();
        CardDescription?.Hide();
    }
    
    private void UpdateVisuals()
    {
        UpdateVisualHealth();
        UpdateVisualDefense();
        UpdateVisualTimeOnField();
        UpdateVisualCost();
        
        AttackManager?.UpdateLabels(); 

        CardName!.Text = CardData?.Name ?? "";
        CardArt!.Texture = CardData?.Art;
        CardDescription!.GetNode<RichTextLabel>("DescriptionLabel").Text = CardData?.Description;
    }

    private void UpdateVisualHealth()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        
        var healthLabel = CardOverlay!.GetNode<RichTextLabel>("HealthLabel");
        var healthOnFieldLabel = CardOnFieldOverlay!.GetNode<RichTextLabel>("HealthLabel");
        
        _UpdateTextLabels(healthLabel, healthOnFieldLabel, healthComponent.CurrentHealth, ref _timeSinceLastHealthUpdate);
    }
    
    private void UpdateVisualDefense()
    {
        var defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        
        var defenseLabel = CardOverlay!.GetNode<RichTextLabel>("DefenseLabel");
        var defenseOnFieldLabel = CardOnFieldOverlay!.GetNode<RichTextLabel>("DefenseLabel");
        
        _UpdateTextLabels(defenseLabel, defenseOnFieldLabel, defenseComponent.Defense, ref _timeSinceLastDefenseUpdate);
    }
    
    private void UpdateVisualTimeOnField()
    {
        var timeOnFieldComponent = this.GetOrAddComponent<TimeOnFieldComponent>();
        
        var timeOnFieldLabel = CardOverlay!.GetNode<RichTextLabel>("TimeLeftLabel");
        var timeOnFieldOnFieldLabel = CardOnFieldOverlay!.GetNode<RichTextLabel>("TimeLeftLabel");
        
        _UpdateTextLabels(timeOnFieldLabel, timeOnFieldOnFieldLabel, timeOnFieldComponent.TimeOnField, ref _timeSinceLastTimeOnFieldUpdate);
    }
    
    private void UpdateVisualCost()
    {
        var costComponent = this.GetOrAddComponent<CostComponent>();
        
        var costLabel = CardOverlay!.GetNode<RichTextLabel>("CostLabel");
        
        costLabel.Text = costComponent.Cost.ToString();
    }

    private void _UpdateTextLabels(RichTextLabel overlayLabel, RichTextLabel fieldLabel, int value, ref float timeElapsed)
    {
        if (overlayLabel.Text.Length == 0 || overlayLabel.Text == "0")
        {
            overlayLabel.Text = value.ToString();
            fieldLabel.Text = value.ToString();
            return;
        }
        
        var currentLabelValue = int.Parse(overlayLabel.Text);
        if (currentLabelValue != value)
        {
            var difference = value - currentLabelValue;
            var speed = 0.2f - Mathf.Clamp(Mathf.Abs(difference) * 0.1f / 5.0f, 0.0f, 0.2f);
            if (timeElapsed >= speed)
            {
                var direction = Mathf.Sign(difference);
                
                currentLabelValue += direction;
                timeElapsed = 0f;
            }
        }
        
        timeElapsed += (float) GetProcessDeltaTime();
        
        overlayLabel.Text = currentLabelValue.ToString();
        fieldLabel.Text = currentLabelValue.ToString();
    }

    private void CheckAppearance()
    {
        if (isHovering && !IsFlipped)
        {
            ZIndex = 3;
            Scale = new Vector2(4.2f, 4.2f);
        } else 
        { 
            ZIndex = 0;
            Scale = new Vector2(4.0f, 4.0f); 
        }
        if (IsFlipped)
        {
            CardBackside?.Show();
        } else {CardBackside?.Hide();}
        if (IsCheckingDescription)
        {
            CardDescription?.Show();
        } else { CardDescription?.Hide(); }
    }

    private void OnMouseEntered()
    {
        isHovering = true;
        if (!IsFlipped)
        {
            CardOverlay?.Show();
        }
    }

    private void OnMouseExited()
    {
        isHovering = false;
        CardOverlay?.Hide();

        IsCheckingDescription = false;
    }

    public void onAreaEntered(Area2D area)  // when card colides with either player or enemy placable position
    {
        if (area.IsInGroup("PlacablePosition"))
        {
            PlacedAreaName = area.Name;
            PlacedAreaLocation = area.GlobalPosition;
        }
        
        IsCardPlayable = turnManager.canPlayerPlaceCards;
    }

    public void onAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlacablePosition"))
        {
            IsCardPlayable = false;
        }
    }

    private bool IsValidDropPosition()
    {
        if (!IsCardPlayable) return false;
        if (PlacedAreaName is null) return false;
        
        var playerDuelist = GetTree().GetFirstNodeInGroup("PlayerDuelist") as Duelist;
        if (playerDuelist is null) return false;
        if (playerDuelist.CurrentSouls < (CardData?.Cost ?? 0)) return false;
        
        FieldData fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
        string numberOnly = Regex.Replace(PlacedAreaName, @"[^\d]", "");
        int laneIndex = int.Parse(numberOnly) - 1;
        
        GD.Print("Checking lane " + laneIndex + " for validity.");
        
        return !fieldData.IsLaneOccupied(laneIndex, true);
    }
}