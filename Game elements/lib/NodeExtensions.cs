using System.Threading.Tasks;
using Godot;

namespace Witheringaway.Game_elements.lib;

public static class NodeExtensions
{
    public static async Task Wait(this Node node, float seconds)
    {
        var timer = node.GetTree().CreateTimer(seconds);
        await node.ToSignal(timer, "timeout");
        
        timer.Dispose();
    }
    
    public static T? FindNodeByType<T>(this Node parent) where T : Node
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is T typedChild)
            {
                return typedChild;
            }

            var result = FindNodeByType<T>(child);
            return result;
        }

        return null;
    }
}