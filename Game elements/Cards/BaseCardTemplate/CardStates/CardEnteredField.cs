using Godot;
using System;

public partial class CardEnteredField : CardState
{   

    public int indexOfLane;
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)  // FIX CARD COMING BACK TO HAND AFTER DRAWING
    {
        GD.Print(card.Name + " entered field");

        card.GlobalPosition = card.whereIsAreaWePlaceOurCardIn;
        string areaName = card.nameOfAreaPlaceOurCardIn;
        string numberOnly = System.Text.RegularExpressions.Regex.Replace(areaName, @"[^\d]", "");
        indexOfLane = int.Parse(numberOnly)-1; // we're doing -1 cause i named all position starting from 1 like a silly goose

        GD.Print(indexOfLane);

        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        fieldData.playCardOnSpecificLane(indexOfLane, card, true);
        GD.Print(fieldData.playerCardsOnField);

        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        card.isCardInField = true;
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
