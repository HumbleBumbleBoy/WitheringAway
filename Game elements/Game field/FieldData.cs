using Godot;
using System;


public partial class FieldData : Node
{
    public BaseCardTemplate?[] enemyCardsOnField = new BaseCardTemplate[5];
    public BaseCardTemplate?[] playerCardsOnField = new BaseCardTemplate[5];

    public void PlayCardOnSpecificLane(int whichLane, BaseCardTemplate whichCard, bool isPlayer)
    {
        if (isPlayer)
        {
            playerCardsOnField[whichLane] = whichCard;
        } else { enemyCardsOnField[whichLane] = whichCard; }
    }

    public void RemoveCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer)
        {
            playerCardsOnField[whichLane] = null;
        } else { enemyCardsOnField[whichLane] = null; }
    }

    public BaseCardTemplate? GetCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer) { return playerCardsOnField[whichLane]; } 
        return enemyCardsOnField[whichLane];
    }

    public bool IsLaneOccupied(int whichLane, bool isPlayer)
    {
        if (isPlayer)
            return playerCardsOnField[whichLane] != null;
        
        return enemyCardsOnField[whichLane] != null;
    }
}
