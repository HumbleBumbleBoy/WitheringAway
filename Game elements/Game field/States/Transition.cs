using Godot;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public class Transition : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        GD.Print("Transition State: Decaying cards and drawing new ones.");
        
        var enemyHand = context.GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;
        if (!enemyHand?.HasMoreCards() ?? true)
        {
            GD.Print("Enemy has no more cards. Player wins!");
            return null;
        }
        enemyHand.GetTopCard();
        GD.Print("Enemy drew a card.");
        
        var playerHand = context.GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager;
        if (!playerHand?.HasMoreCards() ?? true)
        {
            GD.Print("Player has no more cards. Enemy wins!");
            return null;
        }
        playerHand.GetTopCard();
        GD.Print("Player drew a card.");
        
        var playerDuelist = (context.GetTree().GetFirstNodeInGroup("PlayerDuelist") as Duelist)!;
        var enemyDuelist = (context.GetTree().GetFirstNodeInGroup("EnemyDuelist") as Duelist)!;
        
        playerDuelist.GiveSouls(1);
        enemyDuelist.GiveSouls(1);
        
        playerDuelist.RefreshSouls();
        enemyDuelist.RefreshSouls();
        
        var fieldData = context.GetNode<FieldData>("/root/GameScene/FieldData");
        _DecayCards(fieldData?.PlayerCardsOnField ?? [], true);
        _DecayCards(fieldData?.EnemyCardsOnField ?? [], false);
        
        context.GetTree().CreateTimer(2).Timeout += () =>
        {
            if (context.StateMachine.CurrentState == this)
            {
                context.StateMachine.ChangeState(new PlayerTurn());
            }
        };
        
        return null;
    }
    
    private static void _DecayCards(BaseCardTemplate?[] cards, bool isPlayer)
    {
        foreach (var card in cards)
        {
            if (!GodotObject.IsInstanceValid(card))
            {
                continue;
            }
            
            var timeOnField = card.FirstComponent<TimeOnFieldComponent>();
            timeOnField?.SubtractTimeOnField(1);
            
            if (timeOnField is { TimeOnField: <= 0 })
            {
                card.StateMachine.ChangeState(new CardDied(isPlayer));
            }
        }
    }
}