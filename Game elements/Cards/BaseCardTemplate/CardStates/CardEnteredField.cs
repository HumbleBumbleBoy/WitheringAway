using Godot;
using System;
using System.Linq;
using Witheringaway.Game_elements.lib;

public class CardEnteredField : IState<BaseCardTemplate>
{   
    public Node2D? Field;
    public int indexOfLane;
    public PlayerHandManager? playerHandManager;

    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " entered field");
        card.isCardInField = true;
        card.audioFolder?.GetNode<AudioStreamPlayer>("PlaceDown").Play();

        // GET FIELD REFERENCE FIRST (while card still has parent)
        Field = card.GetParent().GetParent().GetParent().GetParent().GetNode<Node2D>("Field");
        
        // GET HAND MANAGER FIRST (while card still has parent)
        playerHandManager = card.GetParent().GetParent().GetParent().GetNode<PlayerHandManager>("PlayerHandManager");
        
        // REMOVE FROM HAND
        playerHandManager.RemoveCardFromHand(card);
        
        // NOW reparent to field
        string areaName = card.nameOfAreaPlaceOurCardIn;
        string numberOnly = System.Text.RegularExpressions.Regex.Replace(areaName, @"[^\d]", "");
        indexOfLane = int.Parse(numberOnly)-1;
        
        Field.GetNode<Control>("PlayerSide").GetNode<HBoxContainer>("PlayerArea").GetNode<VBoxContainer>("FrontLane").GetNode<Control>("Position" + numberOnly).AddChild(card);
        card.Position = Vector2.Zero;

        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        fieldData.PlayCardOnSpecificLane(indexOfLane, card, true);

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
