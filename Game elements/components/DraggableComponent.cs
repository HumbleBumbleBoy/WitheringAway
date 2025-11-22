using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class DraggableComponent : Component
{
    
    public delegate void DragStartedEventHandler(Vector2 initialPosition);
    public delegate void DragEventHandler(Vector2 oldPosition, Vector2 newPosition);
    public delegate void DragEndedEventHandler(Vector2 initialPosition, Vector2 finalPosition);
    
    public event DragStartedEventHandler? DragStarted;
    public event DragEventHandler? DragEvent;
    public event DragEndedEventHandler? DragEnded;
    
    /// <summary>
    /// The minimum time in seconds the mouse button must be held down to initiate a drag.
    /// Note: Either this or DragThresholdDistance must be exceeded to start a drag.
    /// </summary>
    [Export] public double DragSecondsThreshold = 0.2;
    
    /// <summary>
    /// The minimum distance in pixels the mouse must move to initiate a drag.
    /// Note: Either this or DragThresholdSeconds must be exceeded to start a drag.
    /// </summary>
    [Export] public double DragDistanceThreshold = 10.0;

    [Export] private NodePath? _dragContainerPath;
    
    public Control? DragContainer => GetNodeOrNull<Control>(_dragContainerPath);

    private bool _isMouseDown;
    private ulong _mouseDownTime;
    private Vector2 _initialNodePosition;
    private Vector2 _initialMousePosition;
    
    private bool _isDragging;
    private Vector2 _oldMousePosition;
    
    private bool _isConnected;

    public override void _Ready()
    {
        if (DragContainer is null) return;
        
        DragContainer.GuiInput += _OnMouseEvent;
        DragContainer.GuiInput += _OnMouseMotionEvent;
        
        _isConnected = true;
    }

    public override void _ExitTree()
    {
        if (DragContainer is null) return;
        if (!_isConnected) return;
        
        DragContainer.GuiInput -= _OnMouseEvent;
        DragContainer.GuiInput -= _OnMouseMotionEvent;
        
        _isConnected = false;
    }

    private void _StartDragging()
    {
        if (_isDragging) return;
        _isDragging = true;
        DragStarted?.Invoke(_initialNodePosition);
    }
    
    private void _StopDragging()
    {
        if (!_isDragging) return;
        _isDragging = false;
        DragEnded?.Invoke(_initialNodePosition, DragContainer!.GlobalPosition);
    }
    
    private void _CaptureMouse(Vector2? initialMousePosition = null)
    {
        if (DragContainer is null) return;
        
        _isMouseDown = true;
        _mouseDownTime = Time.GetTicksMsec();
        _initialNodePosition = DragContainer.GlobalPosition;
        _initialMousePosition = initialMousePosition ?? DragContainer.GetGlobalMousePosition();
        
        _oldMousePosition = _initialMousePosition;
    }
    
    private void _ReleaseMouse()
    {
        _isMouseDown = false;
        _StopDragging();
    }

    private void _OnMouseEvent(InputEvent @event)
    {
        if (DragContainer is null)
        {
            _ReleaseMouse();
            return;
        }
        
        if (@event is not InputEventMouseButton mouseEvent) return;
        
        GD.Print($"Mouse event: ButtonIndex={mouseEvent.ButtonIndex}, Pressed={mouseEvent.Pressed}");
        
        switch (mouseEvent.ButtonIndex)
        {
            case MouseButton.Left when mouseEvent.Pressed:
                _CaptureMouse(mouseEvent.GlobalPosition);
                break;
            
            case MouseButton.Left when !mouseEvent.Pressed:
                _ReleaseMouse();
                break;
        }
    }
    
    private void _OnMouseMotionEvent(InputEvent @event)
    {
        if (DragContainer is null)
        {
            _ReleaseMouse();
            return;
        }
        
        if (!_isMouseDown) return;
        
        if (@event is not InputEventMouseMotion mouseMotion) return;
        
        GD.Print("Mouse motion event detected during drag check.");

        var currentMousePosition = mouseMotion.GlobalPosition;
        
        if (_isDragging)
        {
            DragEvent?.Invoke(_oldMousePosition, currentMousePosition);
            _oldMousePosition = currentMousePosition;
            return;
        }

        var elapsedTime = (Time.GetTicksMsec() - _mouseDownTime) / 1000.0;
        if (elapsedTime >= DragSecondsThreshold)
        {
            _StartDragging();
            return;
        }

        var distance = currentMousePosition.DistanceTo(_initialMousePosition);
        if (distance >= DragDistanceThreshold)
        {
            _StartDragging();
        }
    }
    
}