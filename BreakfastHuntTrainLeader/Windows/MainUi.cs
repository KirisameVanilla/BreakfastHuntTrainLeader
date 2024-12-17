using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BreakfastHuntTrainLeader.Windows;

public class MainUi : Window, IDisposable
{
    private int selectedServer = 0;
    private int instanceIdInput = 0;
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
    private bool selected = false;
    public override void Draw()
    {
        int counter = 0;
        if (ImGui.BeginTable("Marks", 6))
        {
            ImGui.TableSetupColumn("序号", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("服务器", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("区域", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("分线", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("坐标", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("播报", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();
            List<HuntMark> marks = [.. Plugin.Config.Marks];
            foreach (var mark in marks)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{counter + 1}");

                ImGui.TableNextColumn();
                ImGui.Text(mark.Server);

                ImGui.TableNextColumn();
                ImGui.Text(mark.Territory);

                ImGui.TableNextColumn();
                if (ImGui.InputInt($"##分线{counter}", ref instanceIdInput))
                {
                    Plugin.Config.Marks[counter].InstanceId = (uint)instanceIdInput;
                    Plugin.Config.SaveConfig();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"({mark.Position.X}, {mark.Position.Y})");

                ImGui.TableNextColumn();
                if (ImGui.Button($"播报##{counter}"))
                {
                    mark.Relay();
                }
                ImGui.SameLine();
                if (ImGui.Button($"删除##{counter}"))
                {
                    Plugin.Config.Marks.RemoveAt(counter);
                    Plugin.Config.SaveConfig();
                }
                counter++;
            }

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text($"{counter + 1}");
            ImGui.TableNextColumn();
            selected = ImGuiWidget.ServerSelectCombo(ref selectedServer) || selected;
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            using (ImRaii.Disabled(!selected))
            {
                if (ImGui.Button("添加"))
                {
                    var newMark = new HuntMark(selectedServer);
                    if (!newMark.InitByFlag()) { }
                    else
                    {
                        Plugin.Config.Marks.Add(newMark);
                        Plugin.Config.SaveConfig();
                        selected = false;
                    }
                }
            }
            ImGui.EndTable();
        }
    }
}
