using Godot;
using System;
using System.Linq;

public partial class CardEnteredField : CardState
{   
    public Node2D? Field;
    public int indexOfLane;
    public EnemyHandManager? playerHandManager;
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)  // FIX CARD COMING BACK TO HAND AFTER DRAWING
    {
        GD.Print(card.Name + " entered field");
        card.isCardInField = true;
        card.audioFolder?.GetNode<AudioStreamPlayer>("PlaceDown").Play();

        // GET FIELD REFERENCE FIRST (while card still has parent)
        Field = card.GetParent().GetParent().GetParent().GetParent().GetNode<Node2D>("Field");
        
        // GET HAND MANAGER FIRST (while card still has parent)
        playerHandManager = card.GetParent().GetParent().GetParent().GetNode<EnemyHandManager>("PlayerHandManager");
        
        // REMOVE FROM HAND
        playerHandManager.RemoveCardFromHand(card);
        
        // NOW reparent to field
        string areaName = card.nameOfAreaPlaceOurCardIn;
        string numberOnly = System.Text.RegularExpressions.Regex.Replace(areaName, @"[^\d]", "");
        indexOfLane = int.Parse(numberOnly)-1;
        
        Field.GetNode<Control>("PlayerSide").GetNode<HBoxContainer>("PlayerArea").GetNode<VBoxContainer>("FrontLane").GetNode<Control>("Position" + numberOnly).AddChild(card);
        card.Position = Vector2.Zero;

        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        fieldData.playCardOnSpecificLane(indexOfLane, card, true);

        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        card.cardArt.Scale = new Vector2(0.6f, 0.6f);
        card.cardOnFieldOverlay.Scale = new Vector2(1.0f, 1.0f);
        card.cardOnFieldOverlay?.Show();
        card.cardBackground?.Hide();
        card.cardOverlay?.Hide();
        card.cardName?.Hide();
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + "exited field");  // probably died
        card.isCardInField = false;
        optionalState = new CardDied();
    }
}
