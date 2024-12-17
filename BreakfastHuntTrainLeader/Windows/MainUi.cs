using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BreakfastHuntTrainLeader.Windows;

public class MainUi : Window, IDisposable
{

    public MainUi()
        : base("BreakfastHuntTrainLeader##MainUi", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(480f, 320f),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
      ImGui.Text("UI");  
    }
}
