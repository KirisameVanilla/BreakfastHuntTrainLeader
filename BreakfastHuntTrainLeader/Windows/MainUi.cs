using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader.Windows;

public class MainUi : Window, IDisposable
{
    private int selectedServer = 0;
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
        int counter = 0;
        ImGui.BeginTable("Marks", 2);
        ImGui.TableSetupColumn("序号");
        ImGui.TableSetupColumn("服务器");
        ImGui.TableSetupColumn("区域");
        ImGui.TableSetupColumn("坐标");
        ImGui.TableSetupColumn("播报");
        ImGui.TableHeadersRow();
        foreach (var mark in Plugin.Config.Marks)
        {
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.Text($"{counter+1}");

            ImGui.TableNextColumn();
            ImGui.Text(mark.Server);

            ImGui.TableNextColumn();
            ImGui.Text(mark.Territory.ExtractPlaceName());

            ImGui.TableNextColumn();
            ImGui.Text($"({mark.Position.X}, {mark.Position.Y})");

            ImGui.TableNextColumn();
            if (ImGui.Button($"播报{counter}"))
            {
                
            }
            if (ImGui.Button($"删除{counter}"))
            {
                Plugin.Config.Marks.RemoveAt(counter);
                Plugin.Config.SaveConfig();
            }
            counter++;
        }
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.Text($"{counter + 1}");

        ImGui.TableNextColumn();
        ImGuiWidget.ServerSelectCombo(ref selectedServer);

        ImGui.TableNextColumn();

        ImGui.TableNextColumn();

        ImGui.TableNextColumn();
        using (ImRaii.Disabled(selectedServer==0))
        {
            if (ImGui.Button("添加"))
            {
                var newMark = new HuntMark(selectedServer);
                if (!newMark.InitByFlag()) {}
                else
                {
                    Plugin.Config.Marks.Add(newMark);
                    Plugin.Config.SaveConfig();
                }
            }
        }
        
    }
}
