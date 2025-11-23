using System.Text.RegularExpressions;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

public class CardEnteredField(bool isPlayer, int? laneIndex = null) : IState<BaseCardTemplate>
{   
    public Node2D? Field;
    public int indexOfLane;

    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        var handManager = HandManager.GetHandManager(isPlayer);
        
        GD.Print(card.Name + " entered field");
        card.IsCardInField = true;
        card.AudioFolder?.GetNode<AudioStreamPlayer>("PlaceDown").Play();

        // GET FIELD REFERENCE FIRST (while card still has parent)
        Field = card.GetTree().GetFirstNodeInGroup("GameField") as Node2D;
        
        // REMOVE FROM HAND
        handManager.RemoveCardFromHand(card);
        
        // NOW reparent to field
        string numberOnly = "";
        if (laneIndex is not null)
        {
            numberOnly = (laneIndex.Value + 1).ToString();
            indexOfLane = laneIndex.Value;
        }
        else
        {
            string areaName = card.PlacedAreaName;
            numberOnly = Regex.Replace(areaName, @"[^\d]", "");
            indexOfLane = int.Parse(numberOnly) - 1;
        }

        var fieldSide = isPlayer ? "PlayerSide" : "EnemySide";
        var fieldArea = isPlayer ? "PlayerArea" : "EnemyArea";
        
        Field.GetNode<Control>(fieldSide).GetNode<HBoxContainer>(fieldArea).GetNode<VBoxContainer>("FrontLane").GetNode<Control>("Position" + numberOnly).AddChild(card);
        card.Position = Vector2.Zero;

        var fieldData = FieldData.Instance;
        fieldData.PlayCardOnSpecificLane(indexOfLane, card, isPlayer);

        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        card.CardArt.Scale = new Vector2(0.6f, 0.6f);
        card.CardOnFieldOverlay.Scale = new Vector2(1.0f, 1.0f);
        card.CardOnFieldOverlay?.Show();
        card.CardBackground?.Hide();
        card.CardOverlay?.Hide();
        card.CardName?.Hide();

        card.OnSelfEnterField(isPlayer);

        var friendlies = fieldData.GetCardsOnField(isPlayer);
        for (var lane = 0; lane < friendlies.Length; lane++)
        {
            var friendly = friendlies[lane];
            friendly?.OnFriendlyEnterField(card, lane);
        }

        var enemies = fieldData.GetCardsOnField(!isPlayer);
        for (var lane = 0; lane < enemies.Length; lane++)
        {
            var enemy = enemies[lane];
            enemy?.OnEnemyEnterField(card, lane);
        }
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(card.Name + "exited field");  // probably died
        card.IsCardInField = false;

        return null;
    }
}
