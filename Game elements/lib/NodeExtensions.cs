using System.Threading.Tasks;
using Godot;

namespace Witheringaway.Game_elements.lib;

public static class NodeExtensions
{
    public static async Task Wait(this Node node, float seconds)
    {
        await node.ToSignal(node.GetTree().CreateTimer(seconds), "timeout");
    }
}