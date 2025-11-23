using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.Game_field.States;

namespace Witheringaway.Game_elements.lib;

[GlobalClass]
public partial class Duelist : Control
{

    public static Duelist PlayerDuelist = null!;
    public static Duelist EnemyDuelist = null!;
    
    public static Duelist GetDuelist(bool isPlayer)
    {
        return isPlayer ? PlayerDuelist : EnemyDuelist;
    }
    
    [Export] public RichTextLabel HealthLabel = null!;
    
    [Export] public AnimatedSprite2D HurtAnimation = null!;
    
    [Export] public AudioStreamPlayer2D HurtSoundPlayer = null!;
    [Export] public Sprite2D BlockSprite = null!;
    [Export] public Texture2D[] BlockSprites = null!;
    
    [Export] public int StartingSouls = 1;
    [Export] public bool IsPlayer;
    
    public int WouldYouLookAtTheTime { get; private set; }

    public int MaxSouls { get; private set; }
    
    public int CurrentSouls { get; private set; }

    public override void _Ready()
    {
        if (IsPlayer)
        {
            PlayerDuelist = this;
        }
        else
        {
            EnemyDuelist = this;
        }
        
        MaxSouls = StartingSouls;
        CurrentSouls = StartingSouls;
        
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.OnHealthChanged += UpdateHealthLabel;
        UpdateHealthLabel(healthComponent.MaxHealth, healthComponent.MaxHealth, healthComponent.MaxHealth);
        
        RefreshSouls();
    }

    public override void _ExitTree()
    {
        if (IsPlayer)
        {
            PlayerDuelist = null!;
        }
        else
        {
            EnemyDuelist = null!;
        }
    }

    public int GetCurrentHealth()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        return healthComponent.CurrentHealth;
    }
    
    public async Task TakeDamage(int amount, BaseCardTemplate? from = null)
    {
        if (WouldYouLookAtTheTime == 3)
        {
            WouldYouLookAtTheTime++;
            UpdateBlockSprite(WouldYouLookAtTheTime);

            var cards = FieldData.Instance.GetCardsOnField(IsPlayer);
            foreach (var playerCard in cards)
            {
                if (!IsInstanceValid(playerCard))
                {
                    continue;
                }
                
                var timeOnFieldComponent = playerCard.GetOrAddComponent<TimeOnFieldComponent>();
                timeOnFieldComponent.SubtractTimeOnField(2);
            }
            
            HurtSoundPlayer.Play();
            await ToSignal(HurtSoundPlayer, "finished");
            
            WouldYouLookAtTheTime = 0;
            UpdateBlockSprite(WouldYouLookAtTheTime);
            return;
        }
        
        WouldYouLookAtTheTime++;
        
        UpdateBlockSprite(WouldYouLookAtTheTime);

        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.TakeDamage(amount);
        
        HurtAnimation.Show();
        HurtAnimation.Frame = 0;
        HurtAnimation.Play();
        await ToSignal(HurtAnimation, "animation_finished");
        HurtAnimation.Hide();

        if (healthComponent.CurrentHealth <= 0)
        {
            var turnManager = GetNode<TurnManager>("/root/GameScene/TurnManager");
            turnManager.StateMachine.ChangeState(new GameEnded());
            
            var music = GetNode<AudioStreamPlayer>("/root/GameScene/Music");
            music.SetStreamPaused(true); // pause the music
            
            var fields = GetNode<FieldData>("/root/GameScene/FieldData");
            foreach (var card in fields.PlayerCardsOnField)
            {
                if (!IsInstanceValid(card))
                {
                    continue;
                }
                
                card.Wait(0.2f).ContinueWith(_ => card.CallDeferred(nameof(BaseCardTemplate.DisableArt)));
                
                await card.Kill();
            }
            
            foreach (var card in fields.EnemyCardsOnField)
            {
                if (!IsInstanceValid(card))
                {
                    continue;
                }
                
                card.Wait(0.2f).ContinueWith(_ => card.CallDeferred(nameof(BaseCardTemplate.DisableArt)));
                
                await card.Kill();
            }
            
            var winScreen = GetNode<Node2D>("/root/GameScene/WinScreen");
            var winSprite = IsPlayer ? "EnemyWon" : "PlayerWon";
            
            var winNode = winScreen.GetNode<Node2D>(winSprite);
            
            winNode.Show();
            winScreen.Show();
            
            music.SetStreamPaused(false);
            return;
        }
        
        from?.OnAttackLanded(null, !IsPlayer);
    }
    
    public void GiveSouls(int amount)
    {
        MaxSouls += amount;
        CurrentSouls = MaxSouls;
    }
    
    public void TakeSouls(int amount)
    {
        CurrentSouls -= amount;
        if (CurrentSouls < 0) CurrentSouls = 0;
        
        RefreshSouls();
    }
    
    public void RefreshSouls()
    {
        if (GetTree().GetFirstNodeInGroup(IsPlayer ? "PlayerSoulCounter" : "EnemySoulCounter") is RichTextLabel soulCounter)
        {
            soulCounter.Text = CurrentSouls.ToString();
        }
    }

    private void UpdateBlockSprite(int currentBlock)
    {
        var index = currentBlock % BlockSprites.Length;
        BlockSprite.Texture = BlockSprites[index];
    }
    
    private void UpdateHealthLabel(int oldHealth, int currentHealth, int maxHealth)
    {
        HealthLabel.Text = currentHealth.ToString(); // TODO lerp it
    }
}