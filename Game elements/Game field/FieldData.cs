using Godot;
using System;


public partial class FieldData : Node
{
    public BaseCardTemplate?[] enemyCardsOnField = new BaseCardTemplate[5];
    public BaseCardTemplate?[] playerCardsOnField = new BaseCardTemplate[5];

    public void playCardOnSpecificLane(int whichLane, BaseCardTemplate whichCard, bool isPlayer)
    {
        if (isPlayer)
        {
            playerCardsOnField[whichLane] = whichCard;
        } else { enemyCardsOnField[whichLane] = whichCard; }
    }

    public void removeCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer)
        {
            playerCardsOnField[whichLane] = null;
        } else { enemyCardsOnField[whichLane] = null; }
    }

    public BaseCardTemplate? getCardOnSpecificLane(int whichLane, bool isPlayer)
    {
        if (isPlayer) { return playerCardsOnField[whichLane]; } 
        return enemyCardsOnField[whichLane];
    }
}
