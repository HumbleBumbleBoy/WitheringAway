using System;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;
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

        StartPlaying(turnManager);

        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager turnManager, IState<TurnManager>? nextState)
    {
        turnManager.canEnemyPlaceCards = false;
        return null;
    }

    private async void StartPlaying(TurnManager turnManager)
    {
        try
        {
            if (enemyHandManager?.GetNodeOrNull("first_round_setup_done") == null)
            {
                // first round setup
                var firstRoundSetupDone = new Node();
                firstRoundSetupDone.Name = "first_round_setup_done";
                enemyHandManager?.AddChild(firstRoundSetupDone);

                for (var i = 0; i < 5; i++)
                {
                    enemyHandManager?.GetTopCard();
                    await turnManager.Wait(0.2f);
                }
            }

            Play(turnManager);
        }
        catch (Exception e)
        {
            GD.PrintErr("Error during enemy turn: " + e.Message);
        }
    }

    private async void Play(TurnManager turnManager)
    {
        try
        {
            await PlaceAllCards(turnManager);
        } catch (Exception e)
        {
            GD.PrintErr("Error while placing enemy cards: " + e.Message);
            GD.PrintErr(e.ToString());
        }
        
        if (turnManager.StateMachine.CurrentState == this)
        {
            turnManager.StateMachine.ChangeState(new Combat());
        }
    }

    private async Task PlaceAllCards(TurnManager turnManager)
    {
        var enemyPlacablePositions = turnManager.GetTree().GetNodesInGroup("EnemyPlacablePosition");
        var playerDuelist = Duelist.PlayerDuelist;
        var enemyDuelist = Duelist.EnemyDuelist;
        
        {
            // When the player's health is low, try to kill them as fast as possible
            if (playerDuelist.GetCurrentHealth() < 5)
            {
                 for (var i = 0; i < enemyPlacablePositions.Count; i++)
                 {
                     var playerCard = fieldData?.GetCardOnSpecificLane(i, true);
                     if (playerCard != null) continue;
                     
                     var enemyCard = fieldData?.GetCardOnSpecificLane(i, false);
                     if (enemyCard != null) continue;
                     
                     // Try to find a card that can kill the player directly
                     var cardToPlay = enemyHandManager?.FindCard(
                         enemyDuelist.CurrentSouls,
                         cardPredicate =>
                         {
                             var cardDamage = cardPredicate.GetAttackDamage();
                             return cardDamage >= playerDuelist.GetCurrentHealth();
                         }
                     );
                     
                     await turnManager.Wait(0.2f);
                     if (!PlaceSingleCard(turnManager, enemyPlacablePositions[i], enemyDuelist, cardToPlay))
                     {
                         return;
                     }
                 }
            }
        }
        
        {
            // Try blocking the player cards
            for (var i = 0; i < enemyPlacablePositions.Count; i++)
            {
                var playerCard = fieldData?.GetCardOnSpecificLane(i, true);
                if (playerCard == null) continue;

                var enemyCard = fieldData?.GetCardOnSpecificLane(i, false);
                if (enemyCard != null) continue;

                var cardToPlay = enemyHandManager?.FindCard(
                    enemyDuelist.CurrentSouls,
                    cardPredicate =>
                    {
                        var playerHealth = playerCard.GetCurrentHealth();
                        var cardHealth = cardPredicate.GetCurrentHealth();
                        
                        var playerDamage = playerCard.GetAttackDamage();
                        var cardDamage = cardPredicate.GetAttackDamage();

                        if (cardHealth - playerDamage > 0)
                        {
                            return true;
                        }
                        
                        if (playerHealth - cardDamage <= 0)
                        {
                            return true;
                        }
                        
                        return enemyDuelist.GetCurrentHealth() < 10;
                    }
                );
                
                if (cardToPlay == null) continue;
                
                await turnManager.Wait(0.2f);
                if (!PlaceSingleCard(turnManager, enemyPlacablePositions[i], enemyDuelist, cardToPlay))
                {
                    return;
                }
            }
        }

        {
            // Place on any free lane
            for (var i = 0; i < enemyPlacablePositions.Count; i++)
            {
                var position = enemyPlacablePositions[i];
                var positionTaken = fieldData?.IsLaneOccupied(i, false) ?? true;

                if (positionTaken) continue;

                await turnManager.Wait(0.2f);
                if (!PlaceSingleCard(turnManager, position, enemyDuelist))
                {
                    return;
                }
            }
        }
    }

    private bool PlaceSingleCard(TurnManager turnManager, Node cardPosition, Duelist enemyDuelist, BaseCardTemplate? card = null)
    {
        card ??= enemyHandManager?.FindCard(enemyDuelist.CurrentSouls);

        if (card == null)
        {
            turnManager.StateMachine.ChangeState(new Combat());
            return false;
        }

        card.PlacedAreaName = cardPosition.Name;

        if (cardPosition is Node2D node)
        {
            card.PlacedAreaLocation = node.GlobalPosition;
        }

        var cost = card.FirstComponent<CostComponent>();

        enemyDuelist.TakeSouls(cost?.Cost ?? 0);
        card.StateMachine.ChangeState(new CardEnteredField(false));

        return true;
    }
}