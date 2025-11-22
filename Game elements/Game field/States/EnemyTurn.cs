using System.Linq;
using Godot;
using Witheringaway.Game_elements.lib;

public class EnemyTurn : IState<TurnManager>
{
    private EnemyHandManager? enemyHandManager;
    private FieldData? fieldData;

    public IState<TurnManager>? OnEnter(TurnManager turnManager, IState<TurnManager>? previousState)
    {
        turnManager.canEnemyPlaceCards = true; // is even needed??

        enemyHandManager = turnManager.GetTree().GetFirstNodeInGroup("EnemyHandManager") as EnemyHandManager;
        fieldData = turnManager.GetNode<FieldData>("/root/GameScene/FieldData");

        PlayCards(turnManager);

        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager turnManager, IState<TurnManager>? nextState)
    {
        turnManager.canEnemyPlaceCards = false;
        return null;
    }

    private void PlayCards(TurnManager turnManager)
    {
        var keepPlaying = true;
        
        var enemyPlacablePositions = turnManager.GetTree().GetNodesInGroup("EnemyPlacablePosition");
        for (var i = 0; i < enemyPlacablePositions.Count; i++)
        {
            var position = enemyPlacablePositions[i];
            var positionTaken = position.GetChildren().Any(child => child is BaseCardTemplate);

            if (positionTaken) continue;
            
            position.GetTree().CreateTimer((i + 1) * 0.1).Timeout += () =>
            {
                if (!keepPlaying)
                {
                    return;
                }
                    
                keepPlaying = PlaceSingleCard(turnManager, position);
            };
        }
    }

    private bool PlaceSingleCard(TurnManager turnManager, Node cardPosition)
    {
        var card = enemyHandManager.FindCard(100); // so rich
        
        if (card == null) 
        {
            turnManager.StateMachine.ChangeState(new Combat());
            return false;
        }

        card.nameOfAreaPlaceOurCardIn = cardPosition.Name;

        if (cardPosition is Node2D node)
        {
            card.whereIsAreaWePlaceOurCardIn = node.GlobalPosition;   
        }
        
        card.StateMachine.ChangeState(new CardEnteredField(false));
        
        return true;
    }
}
