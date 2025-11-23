using Godot;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public class Transition : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        var enemyHand = context.GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;
        if (!enemyHand?.HasMoreCards() ?? true)
        {
            return null;
        }
        enemyHand.GetTopCard();
        
        var playerHand = context.GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager;
        if (!playerHand?.HasMoreCards() ?? true)
        {
            return null;
        }
        playerHand.GetTopCard();
        
        var playerDuelist = Duelist.PlayerDuelist;
        var enemyDuelist = Duelist.EnemyDuelist;
        
        playerDuelist.GiveSouls(1);
        enemyDuelist.GiveSouls(1);
        
        playerDuelist.RefreshSouls();
        enemyDuelist.RefreshSouls();
        
        _DecayCards(FieldData.Instance.GetCardsOnField(true), true);
        _DecayCards(FieldData.Instance.GetCardsOnField(false), false);
        
        context.GetTree().CreateTimer(2).Timeout += () =>
        {
            if (context.StateMachine.CurrentState != this) return;

            context.CurrentRound++;
            context.StateMachine.ChangeState(new PlayerTurn());
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