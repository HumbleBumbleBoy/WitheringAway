using System;
using System.Linq;
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
    private float _timeSinceLastAttackUpdate;
    private float _timeSinceLastAttackAmountUpdate;
    
    private (RichTextLabel hand, RichTextLabel field) _visualHealthLabels = (default, default);
    private (RichTextLabel hand, RichTextLabel field) _visualDefenseLabels = (default, default);
    private (RichTextLabel hand, RichTextLabel field) _visualTimeOnFieldLabels = (default, default);
    private (RichTextLabel hand, RichTextLabel field) _visualAttackLabels = (default, default);
    private (RichTextLabel hand, RichTextLabel field) _visualAttackAmountLabels = (default, default);
    
    private HealthComponent _healthComponent = new();
    private DefenseComponent _defenseComponent = new();
    private TimeOnFieldComponent _timeOnFieldComponent = new();
    private AttackComponent _attackComponent = new();
    private CostComponent _costComponent = new();
    
    private int _tempHealthBuff = 0;
    private int _tempAttackDamageBuff = 0;

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
        _visualHealthLabels = (CardOverlay!.GetNode<RichTextLabel>("HealthLabel"), CardOnFieldOverlay!.GetNode<RichTextLabel>("HealthLabel"));
        _visualDefenseLabels = (CardOverlay!.GetNode<RichTextLabel>("DefenseLabel"), CardOnFieldOverlay!.GetNode<RichTextLabel>("DefenseLabel"));
        _visualTimeOnFieldLabels = (CardOverlay!.GetNode<RichTextLabel>("TimeLeftLabel"), CardOnFieldOverlay!.GetNode<RichTextLabel>("TimeLeftLabel"));
        _visualAttackLabels = (CardOverlay!.GetNode<RichTextLabel>("AttackLabel"), CardOnFieldOverlay!.GetNode<RichTextLabel>("AttackLabel"));
        _visualAttackAmountLabels = (CardOverlay!.GetNode<RichTextLabel>("AttackAmountLabel"), CardOnFieldOverlay!.GetNode<RichTextLabel>("AttackAmountLabel"));
        
        CardOnFieldOverlay?.Hide();
        
        turnManager = GetTree().CurrentScene.GetNode<TurnManager>("TurnManager");

        _healthComponent = this.GetOrAddComponent<HealthComponent>();
        _healthComponent.SetMaxHealth(CardData?.Health ?? _healthComponent.MaxHealth);
        
        _defenseComponent = this.GetOrAddComponent<DefenseComponent>();
        _defenseComponent.SetDefense(CardData?.Defense ?? _defenseComponent.Defense);

        _timeOnFieldComponent = this.GetOrAddComponent<TimeOnFieldComponent>();
        _timeOnFieldComponent.SetTimeOnField(CardData?.TimeLeftOnField ?? _timeOnFieldComponent.TimeOnField);
        
        _costComponent = this.GetOrAddComponent<CostComponent>();
        _costComponent.SetCost(CardData?.Cost ?? _costComponent.Cost);
        
        _attackComponent = this.GetOrAddComponent<AttackComponent>();
        _attackComponent.SetAttackDamage(CardData?.Attack ?? _attackComponent.AttackDamage);
        _attackComponent.SetAttackCount(CardData?.HowManyAttacks ?? _attackComponent.AttackCount);
        
        CustomMinimumSize = new Vector2(32, 48);
        CheckAppearance();
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

            var isValid = IsValidDropPosition();
            if (isValid.Item1)
            {
                Duelist.PlayerDuelist.TakeSouls(CardData?.Cost ?? 0);
                StateMachine.ChangeState(new CardEnteredField(true, isValid.Item2));
            }
            else
            {
                var result = GetHoveredFriendlyPlaceable();
                if (result != null)
                {
                    var fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
                    var cardOnField = fieldData.GetCardOnSpecificLane(result.Value.lane, true);
                    DropOnCard(cardOnField, true, result.Value.lane);
                    return;
                }
                
                result = GetHoveredEnemyPlaceable();
                if (result != null)
                {
                    var fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
                    var cardOnField = fieldData.GetCardOnSpecificLane(result.Value.lane, false);
                    DropOnCard(cardOnField, false, result.Value.lane);
                    return;
                }
                
                var duelistHovered = GetHoveredDuelist();
                if (duelistHovered != null)
                {
                    DropOnDuelist(duelistHovered, true);
                    return;
                }
                
                Drop(true);
            }
        };
    }
    
    public virtual bool ShouldKeepDragging()
    {
        return true;
    }
    
    public void AddTimeOnField(int amount)
    {
        _timeOnFieldComponent.AddTimeOnField(amount);
        UpdateVisualTimeOnField();
    }
    
    public void SubtractTimeOnField(int amount)
    {
        _timeOnFieldComponent.SubtractTimeOnField(amount);
        UpdateVisualTimeOnField();
    }

    public void SetFlipped(bool flipped)
    {
        IsFlipped = flipped;
        CheckAppearance();
    }

    public async Task Kill()
    {
        await Task.WhenAll(
            PlayAnimation("Dying", 0.2f),
            PlaySound("Death")
        );
        
        QueueFree();
    }
    
    public int GetCurrentHealth()
    {
        return _healthComponent.CurrentHealth + _tempHealthBuff;
    }
    
    public void Heal(int amount)
    {
        _healthComponent.SetHealth(_healthComponent.CurrentHealth + amount);
    }
    
    public void BuffHealth(int amount)
    {
        _healthComponent.SetMaxHealth(amount, false);
        _healthComponent.SetHealth(_healthComponent.CurrentHealth + amount);
    }
    
    public void TempBuffHealth(int amount)
    {
        _tempHealthBuff += amount;
    }
    
    public void TempDebuffHealth(int amount)
    {
        _tempHealthBuff -= amount;
        if (_tempHealthBuff < 0) _tempHealthBuff = 0;
    }
    
    public int GetAttackDamage()
    {
        return _attackComponent.AttackDamage + _tempAttackDamageBuff;
    }
    
    public void SetAttackDamage(int damage)
    {
        _attackComponent.SetAttackDamage(damage);
    }
    
    public void BuffAttackDamage(int damage)
    {
        var newDamage = _attackComponent.AttackDamage + damage;
        _attackComponent.SetAttackDamage(newDamage);
    }
    
    public void DebuffAttackDamage(int damage)
    {
        var newDamage = _attackComponent.AttackDamage - damage;
        if (newDamage < 0) newDamage = 0;
        _attackComponent.SetAttackDamage(newDamage);
    }
    
    public void TempBuffAttackDamage(int damage)
    {
        _tempAttackDamageBuff += damage;
    }
    
    public void TempDebuffAttackDamage(int damage)
    {
        _tempAttackDamageBuff -= damage;
        if (_tempAttackDamageBuff < 0) _tempAttackDamageBuff = 0;
    }
    
    public int GetAttackCount()
    {
        return _attackComponent.AttackCount;
    }
    
    public int GetCost(bool isPlayer)
    {
        var cards = FieldData.Instance.GetCardsOnField(isPlayer);
        var countCrooked = cards.Count(card => card is Crooked);
        
        return Math.Max(_costComponent.Cost - countCrooked, 0);
    }
    
    public void TakeDamage(BaseCardTemplate? attacker, int damage, bool isPlayer = true)
    {
        var remainingDamage = _defenseComponent.AbsorbDamage(damage);
        
        var damageToTemp = Mathf.Min(remainingDamage, _tempHealthBuff);
        _tempHealthBuff -= damageToTemp;
        remainingDamage -= damageToTemp;
        
        _healthComponent.TakeDamage(remainingDamage);
        
        var originalPosition = GlobalPosition;
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", originalPosition + new Vector2(5, -5), 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "global_position", originalPosition - new Vector2(5, -5), 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this, "global_position", originalPosition, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        
        _ = PlaySound("Hurt");
        _ = PlayAnimation("Hurt");
        OnDamageTaken(attacker, damage, isPlayer);
        
        tween.TweenCallback(Callable.From(() => tween.Dispose()));
    }
    
    public async Task AttackOnce(Control target, Callable? onMidAttack = null, bool isPlayer = true)
    {
        var originalPosition = GlobalPosition;
        var directionToTarget = (target.GlobalPosition - GlobalPosition).Normalized();
        var attackPosition = originalPosition + directionToTarget * 20;
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", attackPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        tween.TweenCallback(onMidAttack ?? _AttackCard(target, isPlayer) ?? Callable.From(() => {}));
        if (target is BaseCardTemplate bct)
        {
            OnAttackLanded(bct, isPlayer);
        }
        tween.TweenProperty(this, "global_position", originalPosition, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        await ToSignal(tween, "finished");
        
        tween.Dispose();

        await this.Wait(0.3f);
    }
    
    public async Task Attack(Control target, Callable? onMidAttack = null, bool isPlayer = true)
    {
        for (var i = 0; i < GetAttackCount(); i++)
        {
            await AttackOnce(target, onMidAttack, isPlayer);
        }
    }

    private Callable? _AttackCard(Control node, bool isPlayer = true)
    {
        if (node is BaseCardTemplate card)
        {
            return Callable.From(() =>
            {
                _ = PlaySound("Attack");
                card.TakeDamage(this, GetAttackDamage(), isPlayer);
            });
        }

        return null;
    }

    public bool ShouldDie()
    {
        return _healthComponent.CurrentHealth <= 0;
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

    public override void _GuiInput(InputEvent @event)
    {
        var draggable = this.FirstComponent<DraggableComponent>();
        draggable?.OnDragControlInput(@event);
        
        AcceptEvent();
    }

    private void UpdateVisuals()
    {
        UpdateVisualHealth();
        UpdateVisualDefense();
        UpdateVisualTimeOnField();
        UpdateVisualCost();
        UpdateVisualDamage();

        CardName!.Text = CardData?.Name ?? "";
        CardArt!.Texture = CardData?.Art;
        CardDescription!.GetNode<RichTextLabel>("DescriptionLabel").Text = CardData?.Description;
    }

    private void UpdateVisualHealth()
    {
        _UpdateTextLabels(_visualHealthLabels.hand, _visualHealthLabels.field, GetCurrentHealth(), ref _timeSinceLastHealthUpdate);
    }
    
    private void UpdateVisualDefense()
    {
        _UpdateTextLabels(_visualDefenseLabels.hand, _visualDefenseLabels.field, _defenseComponent.Defense, ref _timeSinceLastDefenseUpdate);
    }
    
    private void UpdateVisualTimeOnField()
    {
        _UpdateTextLabels(_visualTimeOnFieldLabels.hand, _visualTimeOnFieldLabels.field, _timeOnFieldComponent.TimeOnField, ref _timeSinceLastTimeOnFieldUpdate);
    }
    
    private void UpdateVisualCost()
    {
        var costLabel = CardOverlay!.GetNode<RichTextLabel>("CostLabel");
        
        costLabel.Text = GetCost(true).ToString();
    }

    private void UpdateVisualDamage()
    {
        _UpdateTextLabels(_visualAttackLabels.hand, _visualAttackLabels.field, GetAttackDamage(), ref _timeSinceLastAttackUpdate);
        
        _UpdateTextLabels(_visualAttackAmountLabels.hand, _visualAttackAmountLabels.field, _attackComponent.AttackCount, ref _timeSinceLastAttackAmountUpdate);
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

    protected virtual void CheckAppearance()
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
            IsCardPlayable = true;
            PlacedAreaName = area.Name;
            PlacedAreaLocation = area.GlobalPosition;
        }
    }

    public void onAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlacablePosition"))
        {
            IsCardPlayable = false;
            PlacedAreaName = null;
            PlacedAreaLocation = Vector2.Zero;
        }
    }

    protected virtual (bool, int) IsValidDropPosition()
    {
        if (!IsCardPlayable) return (false, 0);
        if (PlacedAreaName is null) return (false, 0);
        if (turnManager?.StateMachine.CurrentState is not PlayerTurn) return (false, 0);
        
        var playerDuelist = Duelist.PlayerDuelist;
        if (playerDuelist.CurrentSouls < (CardData?.Cost ?? 0)) return (false, 0);
        
        var fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
        var numberOnly = NumberRegex().Replace(PlacedAreaName, "");
        var laneIndex = int.Parse(numberOnly) - 1;
        
        return (!fieldData.IsLaneOccupied(laneIndex, true), laneIndex);
    }
    
    public virtual void OnCombatEnd(int lane, bool isPlayer)
    {
        // can be overridden
    }
    
    public virtual void OnCombatStart(int lane, bool isPlayer)
    {
        // can be overridden
    }
    
    public virtual void OnEnemyExitField(BaseCardTemplate card, int laneIndex)
    {
        // can be overridden
    }
    
    public virtual void OnFriendlyExitField(BaseCardTemplate card, int laneIndex)
    {
        // can be overridden
    }
    
    public virtual void OnEnemyEnterField(BaseCardTemplate card, int laneIndex)
    {
        // can be overridden
    }
    
    public virtual void OnFriendlyEnterField(BaseCardTemplate card, int laneIndex)
    {
        // can be overridden
    }
    
    public virtual void OnAttackLanded(BaseCardTemplate? target, bool isPlayer)
    {
        // can be overridden
    }

    public virtual void OnSelfEnterField(bool isPlayer)
    {
        
    }
    
    protected virtual void OnDamageTaken(BaseCardTemplate? attacker, int damage, bool isPlayer)
    {
        // can be overridden
    }

    protected virtual void DropOnCard(BaseCardTemplate? card, bool isPlayer, int laneIndex)
    {
        StateMachine.ChangeState(new CardInHand());
        _ = PlaySound("FailedToPlace");
    }
    
    protected virtual void DropOnDuelist(Duelist duelis, bool isPlayer)
    {
        StateMachine.ChangeState(new CardInHand());
        _ = PlaySound("FailedToPlace");
    }

    protected virtual void Drop(bool isPlayer)
    {
        StateMachine.ChangeState(new CardInHand());
        _ = PlaySound("FailedToPlace");
    }

    private (Node2D position, int lane)? GetHoveredFriendlyPlaceable()
    {
        var tree = GetTree();
        foreach (var node in tree.GetNodesInGroup("PlacablePosition"))
        {
            if (node is not Area2D position) continue;
            var overlaps = position.OverlapsArea(GetNode<Area2D>("DetectionArea"));
            if (!overlaps)
            {
                continue;
            } 
            
            var numberOnly = NumberRegex().Replace(position.Name, "");
            var laneIndex = int.Parse(numberOnly) - 1;
            return (position, laneIndex);
        }

        return null;
    }

    private (Node2D position, int lane)? GetHoveredEnemyPlaceable()
    {
        var tree = GetTree();
        foreach (var node in tree.GetNodesInGroup("EnemyPlacablePosition"))
        {
            if (node is not Area2D position) continue;
            var overlaps = position.OverlapsArea(GetNode<Area2D>("DetectionArea"));
            if (!overlaps)
            {
                continue;
            } 
            
            var numberOnly = NumberRegex().Replace(position.Name, "");
            var laneIndex = int.Parse(numberOnly) - 1;
            return (position, laneIndex);
        }

        return null;
    }
    
    private Duelist? GetHoveredDuelist()
    {
        var playerDuelist = GetTree().GetFirstNodeInGroup("PlayerCharacter") as Area2D;
        var enemyDuelist = GetTree().GetFirstNodeInGroup("EnemyCharacter") as Area2D;
        
        if (playerDuelist?.OverlapsArea(GetNode<Area2D>("DetectionArea")) == true)
        {
            return Duelist.PlayerDuelist;
        }
        
        if (enemyDuelist?.OverlapsArea(GetNode<Area2D>("DetectionArea")) == true)
        {
            return Duelist.EnemyDuelist;
        }
        
        return null;
    }
    
    [GeneratedRegex(@"[^\d]")]
    private static partial Regex NumberRegex();
}