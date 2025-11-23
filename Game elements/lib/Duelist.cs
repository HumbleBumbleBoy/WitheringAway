using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.components;

namespace Witheringaway.Game_elements.lib;

[GlobalClass]
public partial class Duelist : Control
{
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
        MaxSouls = StartingSouls;
        CurrentSouls = StartingSouls;
        
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        healthComponent.OnHealthChanged += UpdateHealthLabel;
        UpdateHealthLabel(healthComponent.MaxHealth, healthComponent.MaxHealth, healthComponent.MaxHealth);
        
        RefreshSouls();
    }
    
    public int GetCurrentHealth()
    {
        var healthComponent = this.GetOrAddComponent<HealthComponent>();
        return healthComponent.CurrentHealth;
    }
    
    public async Task TakeDamage(int amount)
    {
        if (WouldYouLookAtTheTime == 3)
        {
            WouldYouLookAtTheTime++;
            UpdateBlockSprite(WouldYouLookAtTheTime);

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