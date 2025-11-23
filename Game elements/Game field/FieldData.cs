using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib.manager;

public partial class FieldData : Node
{
    
    public static FieldData Instance { get; private set; }
    
    public readonly BaseCardTemplate?[] EnemyCardsOnField = new BaseCardTemplate[5];
    public readonly BaseCardTemplate?[] PlayerCardsOnField = new BaseCardTemplate[5];
    
    public BaseCardTemplate?[] GetCardsOnField(bool isPlayer)
    {
        var array = isPlayer ? PlayerCardsOnField : EnemyCardsOnField;
        
        for (var lane = 0; lane < array.Length; lane++)
        {
            var card = array[lane];
            if (IsInstanceValid(card)) continue;
            
            array[lane] = null;
        }
        
        return array;
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
    
    public void SpawnOnLane(int whichLane, BaseCardTemplate whichCard, bool isPlayer)
    {
        if (isPlayer)
        {
            PlayerCardsOnField[whichLane] = whichCard;
        }
        else
        {
            EnemyCardsOnField[whichLane] = whichCard;
        }
        
        whichCard.PlacedAreaName = "Position" + whichLane;
        
        var Field = GetTree().GetFirstNodeInGroup("GameField") as Node2D;
        
        var fieldSide = isPlayer ? "PlayerSide" : "EnemySide";
        var fieldArea = isPlayer ? "PlayerArea" : "EnemyArea";
        Field.GetNode<Control>(fieldSide).GetNode<HBoxContainer>(fieldArea).GetNode<VBoxContainer>("FrontLane").GetNode<Control>("Position" + (whichLane + 1)).AddChild(whichCard);
        
        whichCard.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        whichCard.CardArt.Scale = new Vector2(0.6f, 0.6f);
        whichCard.CardOnFieldOverlay.Scale = new Vector2(1.0f, 1.0f);
        whichCard.CardOnFieldOverlay?.Show();
        whichCard.CardBackground?.Hide();
        whichCard.CardOverlay?.Hide();
        whichCard.CardName?.Hide();

        whichCard.OnSelfEnterField(isPlayer);

        var friendlies = GetCardsOnField(isPlayer);
        for (var lane = 0; lane < friendlies.Length; lane++)
        {
            var friendly = friendlies[lane];
            friendly?.OnFriendlyEnterField(whichCard, lane);
        }

        var enemies = GetCardsOnField(!isPlayer);
        for (var lane = 0; lane < enemies.Length; lane++)
        {
            var enemy = enemies[lane];
            enemy?.OnEnemyEnterField(whichCard, lane);
        }
    }
    
    public bool SpawnOnRandomLane(BaseCardTemplate whichCard, bool isPlayer)
    {
        var cardsOnField = GetCardsOnField(isPlayer);
        var emptyLanes = new List<int>();

        for (var lane = 0; lane < cardsOnField.Length; lane++)
        {
            if (cardsOnField[lane] == null)
            {
                emptyLanes.Add(lane);
            }
        }

        if (emptyLanes.Count == 0)
        {
            return false;
        }

        var randomIndex = new Random().Next(emptyLanes.Count);
        var selectedLane = emptyLanes[randomIndex];

        SpawnOnLane(selectedLane, whichCard, isPlayer);
        return true;
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