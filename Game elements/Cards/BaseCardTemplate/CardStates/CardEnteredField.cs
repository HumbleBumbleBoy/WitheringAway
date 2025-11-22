using Godot;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

public class CardEnteredField(bool isPlayer) : IState<BaseCardTemplate>
{   
    public Node2D? Field;
    public int indexOfLane;

    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        var handManager = isPlayer
            ? card.GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager
            : card.GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;
        
        if (handManager == null)
        {
            GD.PrintErr("HandManager not found in the scene tree.");
            return null;
        }
        
        GD.Print(card.Name + " entered field");
        card.isCardInField = true;
        card.audioFolder?.GetNode<AudioStreamPlayer>("PlaceDown").Play();

        // GET FIELD REFERENCE FIRST (while card still has parent)
        Field = card.GetTree().GetFirstNodeInGroup("GameField") as Node2D;
        
        // REMOVE FROM HAND
        handManager.RemoveCardFromHand(card);
        
        // NOW reparent to field
        string areaName = card.nameOfAreaPlaceOurCardIn;
        string numberOnly = System.Text.RegularExpressions.Regex.Replace(areaName, @"[^\d]", "");
        indexOfLane = int.Parse(numberOnly)-1;
        
        var fieldSide = isPlayer ? "PlayerSide" : "EnemySide";
        var fieldArea = isPlayer ? "PlayerArea" : "EnemyArea";
        
        Field.GetNode<Control>(fieldSide).GetNode<HBoxContainer>(fieldArea).GetNode<VBoxContainer>("FrontLane").GetNode<Control>("Position" + numberOnly).AddChild(card);
        card.Position = Vector2.Zero;

        var fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        fieldData.PlayCardOnSpecificLane(indexOfLane, card, isPlayer);

        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        card.cardArt.Scale = new Vector2(0.6f, 0.6f);
        card.cardOnFieldOverlay.Scale = new Vector2(1.0f, 1.0f);
        card.cardOnFieldOverlay?.Show();
        card.cardBackground?.Hide();
        card.cardOverlay?.Hide();
        card.cardName?.Hide();
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(card.Name + "exited field");  // probably died
        card.isCardInField = false;

        return new CardDied();
    }
}
