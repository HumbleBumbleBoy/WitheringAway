using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class FieldData : Node
{
    
    public static FieldData Instance { get; private set; }
    
    public readonly BaseCardTemplate?[] EnemyCardsOnField = new BaseCardTemplate[5];
    public readonly BaseCardTemplate?[] PlayerCardsOnField = new BaseCardTemplate[5];
    

    public BaseCardTemplate?[] GetCardsOnField(bool isPlayer)
    {
        return isPlayer ? PlayerCardsOnField : EnemyCardsOnField;
    }
    
    public BaseCardTemplate? RandomCardOnField(bool isPlayer, BaseCardTemplate? exceptCard = null)
    {
        var cardsOnField = GetCardsOnField(isPlayer);
        var validCards = cardsOnField.OfType<BaseCardTemplate>().Where(card => card != exceptCard).ToList();

        if (validCards.Count == 0)
        {
            return null;
        }

        var randomIndex = new Random().Next(validCards.Count);
        return validCards[randomIndex];
    }
    
    public IEnumerable<(BaseCardTemplate? card, int lane, bool isPlayer)> GetAllCardsOnField()
    {
        for (var lane = 0; lane < PlayerCardsOnField.Length; lane++)
        {
            var card = PlayerCardsOnField[lane];
            yield return (card, lane, true);
        }

        for (var lane = 0; lane < EnemyCardsOnField.Length; lane++)
        {
            var card = EnemyCardsOnField[lane];
            yield return (card, lane, false);
        }
    }

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null!;
        }
    }

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