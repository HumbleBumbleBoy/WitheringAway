using System.Collections.Generic;
using Godot;

namespace Witheringaway.Game_elements.components;

public static class ComponentExtensions
{
    public static List<T> AllComponents<T>(this Node node) where T : Component => Component.AllIn<T>(node);

    public static T? FirstComponent<T>(this Node node) where T : Component => Component.FirstIn<T>(node);
    
    public static void RemoveAllComponents<T>(this Node node) where T : Component => Component.RemoveAllIn<T>(node);
    
    public static void AddComponent<T>(this Node node) where T : Component, new()
    {
        var component = new T();
        
        var componentsNode = node.GetNodeOrNull("components") ?? new Node();
        if (componentsNode.GetParent() == null)
        {
            componentsNode.Name = "components";
            node.AddChild(componentsNode);
        }
        
        node.AddChild(component);
    }
    
    public static void GetOrAddComponent<T>(this Node node) where T : Component, new()
    {
        var existingComponent = node.FirstComponent<T>();
        if (existingComponent == null)
        {
            node.AddComponent<T>();
        }
    }
}