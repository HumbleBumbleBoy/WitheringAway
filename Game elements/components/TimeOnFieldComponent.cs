using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class TimeOnFieldComponent : Component
{
    
    public delegate void TimeOnFieldChangedEventHandler(int oldTimeOnField, int currentTimeOnField);
    
    public event TimeOnFieldChangedEventHandler? OnTimeOnFieldChanged;

    public int TimeOnField { get; private set; }
    
    public void SetTimeOnField(int amount)
    {
        amount = Mathf.Clamp(amount, 0, int.MaxValue);
        
        var oldTimeOnField = TimeOnField;
        TimeOnField = amount;
        OnTimeOnFieldChanged?.Invoke(oldTimeOnField, TimeOnField);
    }
    
    public void AddTimeOnField(int amount)
    {
        if (amount <= 0) return;
        
        SetTimeOnField(TimeOnField + amount);
    }
    
    public void SubtractTimeOnField(int amount)
    {
        if (amount <= 0) return;
        
        SetTimeOnField(TimeOnField - amount);
    }
    
    public void ResetTimeOnField()
    {
        SetTimeOnField(0);
    }
    
}