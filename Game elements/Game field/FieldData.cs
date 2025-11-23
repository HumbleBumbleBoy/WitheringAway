using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class FieldData : Node
{
    public readonly BaseCardTemplate?[] EnemyCardsOnField = new BaseCardTemplate[5];
    public readonly BaseCardTemplate?[] PlayerCardsOnField = new BaseCardTemplate[5];

    public void PlayCardOnSpecificLane(int whichLane, BaseCardTemplate whichCard, bool isPlayer)
    {
        if (isPlayer)
        {
            PlayerCardsOnField[whichLane] = whichCard;
        }
        else
        {
            EnemyCardsOnField[whichLane] = whichCard;
        }
    }

    public void RemoveCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer)
        {
            PlayerCardsOnField[whichLane] = null;
        }
        else
        {
            EnemyCardsOnField[whichLane] = null;
        }
    }

    public BaseCardTemplate? GetCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer)
        {
            var playerCard = PlayerCardsOnField[whichLane];
            if (IsInstanceValid(playerCard)) return PlayerCardsOnField[whichLane];
            
            PlayerCardsOnField[whichLane] = null;
            return null;
        }
        
        var enemyCard = EnemyCardsOnField[whichLane];
        if (IsInstanceValid(enemyCard)) return EnemyCardsOnField[whichLane];
        
        EnemyCardsOnField[whichLane] = null;
        return null;
    }

    public bool IsLaneOccupied(int whichLane, bool isPlayer)
    {
        if (isPlayer)
        {
            var playerCard = PlayerCardsOnField[whichLane];
            if (IsInstanceValid(playerCard)) return PlayerCardsOnField[whichLane] != null;
            
            PlayerCardsOnField[whichLane] = null;
            return false;

        }
        
        var enemyCard = EnemyCardsOnField[whichLane];
        if (IsInstanceValid(enemyCard)) return EnemyCardsOnField[whichLane] != null;
        
        EnemyCardsOnField[whichLane] = null;
        return false;

    }
}