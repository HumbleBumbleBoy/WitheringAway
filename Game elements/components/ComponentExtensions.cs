using System.Collections.Generic;
using Godot;

namespace Witheringaway.Game_elements.components;

public static class ComponentExtensions
{
    public static List<T> AllComponents<T>(this Node node) where T : Component
    {
        return Component.AllIn<T>(node);
    }
    
    public static T? FirstComponent<T>(this Node node) where T : Component
    {
        return Component.FirstIn<T>(node);
    }
}