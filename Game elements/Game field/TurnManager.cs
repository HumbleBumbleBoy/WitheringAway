using Godot;

public partial class TurnManager : Node
{
    public int Turn = 0;
    public int CurrentTurn;

    public void ChangeTurnBy(int amount, bool isAdded)
    {
        if (isAdded) { CurrentTurn += amount; }
        else CurrentTurn -= amount;
    }
}
