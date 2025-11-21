using System.Collections.Generic;
using Godot;

namespace Witheringaway.Game_elements.components;

public partial class Component : Node
{
    public override void _Ready()
    {
        AddToGroup(GetClass());
    }
    
    public override void _ExitTree()
    {
        RemoveFromGroup(GetClass());
    }

    public static List<T> AllIn<T>(Node node) where T : Component
    {
        var components = new List<T>();
        
        foreach (var component in node.GetChildren())
        {
            if (component is T typedComponent)
            {
                components.Add(typedComponent);
            }
            
            components.AddRange(AllIn<T>(component));
        }
        
        return components;
    }
    
    public static T? FirstIn<T>(Node node) where T : Component
    {
        foreach (var component in node.GetChildren())
        {
            if (component is T typedComponent)
            {
                return typedComponent;
            }
            
            var foundComponent = FirstIn<T>(component);
            if (foundComponent != null)
            {
                return foundComponent;
            }
        }
        
        return null;
    }
}