using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;

public class EnemyTurn : IState<TurnManager>
{
    private EnemyHandManager? enemyHandManager;
    private Node? enemyDeckManager;
    private HBoxContainer? enemyCardContainer;
    private TurnManager? _turnManager;
    private CancellationTokenSource tokenSource = new();

    public IState<TurnManager>? OnEnter(TurnManager turnManager, IState<TurnManager>? previousState)
    {
        _turnManager = turnManager;
        turnManager.canEnemyPlaceCards = true; // is even needed??

        enemyDeckManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<Node>("EnemyDeckManager");
        enemyHandManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<EnemyHandManager>("EnemyHandManager");
        enemyCardContainer = enemyHandManager.GetNode<HBoxContainer>("CardContainer");

        try
        {
            PlayCards(tokenSource.Token);
        } catch (OperationCanceledException _){}

        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager turnManager, IState<TurnManager>? nextState)
    {
        turnManager.canEnemyPlaceCards = false;
        tokenSource.Dispose();
        return null;
    }

    private void PlayCards(CancellationToken cancellationToken)
    {
        var enemyPlacablePositions = _turnManager.GetTree().GetNodesInGroup("EnemyPlacablePosition");
        for (var i = 0; i < enemyPlacablePositions.Count; i++)
        {
            var position = enemyPlacablePositions[i];
            var positionTaken = position.GetChildren().Any(child => child is BaseCardTemplate);

            if (!positionTaken)
            {
                position.GetTree().CreateTimer((i + 1) * 0.1).Timeout += () => { PlaceSingleCard(cancellationToken, position); };
            }
        }
    }

    private void PlaceSingleCard(CancellationToken cancellationToken, Node cardPosition)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested(); 
            return;
        }   

        var card = enemyHandManager.PlayCard(100); // so rich
        
        if (card == null) 
        { 
            tokenSource.Cancel();
            _turnManager.StateMachine.ChangeState(new Combat());
            return;
        }

        enemyHandManager.RemoveCardFromHand(card);
        cardPosition.AddChild(card);
    }
}
