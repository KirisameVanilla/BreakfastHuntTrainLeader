using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using OmenTools.ImGuiOm;

namespace BreakfastHuntTrainLeader.Windows;

public class MainUi : Window, IDisposable
{
    private int selectedServer = 0;
    private int instanceIdInput = 0;
    private bool selected = false;
    private string 同服扩散模板 = Plugin.Config.同服扩散模板;
    private string 跨服扩散模板 = Plugin.Config.跨服扩散模板;
    private string 分线模板 = Plugin.Config.分线模板;
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
        if (ImGui.BeginTabBar("##tabBar"))
        {
            if (ImGui.BeginTabItem("怪物列表"))
            {
                DrawMarks();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("设置"))
            {
                DrawSettings();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    private void DrawMarks()
    {
        var counter = 0;
        if (ImGui.BeginTable("Marks", 6, ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable))
        {
            ImGui.TableSetupColumn("序号", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("服务器", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("区域", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("分线", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("坐标", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("播报", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();
            List<HuntMark> marks = [.. Plugin.Config.Marks];
            foreach (var mark in marks)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{counter + 1}");

                ImGui.SameLine();
                if (ImGuiWidget.SwapButtons(counter, Plugin.Config.Marks))
                    Plugin.Config.SaveConfig();

                ImGui.TableNextColumn();
                ImGui.Text(mark.Server);

                ImGui.TableNextColumn();
                ImGui.Text(mark.Territory);

                ImGui.TableNextColumn();
                instanceIdInput = (int)mark.InstanceId;
                ImGui.Text($"{mark.InstanceId}线");
                ImGui.SameLine();
                if (ImGui.InputInt($"##分线{counter}", ref instanceIdInput))
                {
                    Plugin.Config.Marks[counter].InstanceId = (uint)instanceIdInput;
                    Plugin.Config.SaveConfig();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"({mark.Position.X}, {mark.Position.Y})");

                ImGui.TableNextColumn();
                if (ImGui.Button($"播报##{counter}"))
                    mark.Relay();

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
            selected = ImGuiWidget.ServerSelectCombo(ref selectedServer, Plugin.Config.大区名) || selected;
            ImGui.TableNextColumn();
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

    private void DrawSettings()
    {
        ImGui.Text("请选择大区:");
        using (ImRaii.PushIndent())
        {
            if (ImGuiWidget.DataCenterSelectCombo(ref selectedServer, out var 大区名))
            {
                Plugin.Config.大区名 = 大区名;
                Plugin.Config.Marks.Clear();
                Plugin.Config.SaveConfig();
            }
        }

        ImGui.Text("扩散频道设置:");
        using (ImRaii.PushIndent())
            ImGuiWidget.ChatChannelCombo();
        ImGui.Text("扩散模板设置:");
        using (ImRaii.PushIndent())
        {
            ImGui.Text("同服S怪:");
            ImGuiWidget.HelpMarker("{0}: 分线显示控制。若怪物未设置分线，即为0线，则会不显示");
            using (ImRaii.PushIndent())
            {
                ImGui.InputText("##同服S怪", ref 同服扩散模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Plugin.Config.同服扩散模板 = 同服扩散模板;
                    Plugin.Config.SaveConfig();
                }
            }

            ImGui.Text("跨服S怪:");
            ImGuiWidget.HelpMarker("""
                               {0}: 分线显示控制。若怪物未设置分线，即为0线，则会不显示
                               {1}: 服务器名
                               """);
            using (ImRaii.PushIndent())
            {
                ImGui.InputText("##跨服S怪", ref 跨服扩散模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Plugin.Config.跨服扩散模板 = 跨服扩散模板;
                    Plugin.Config.SaveConfig();
                }
            }

            ImGui.Text("分线模板:");
            ImGuiWidget.HelpMarker("""
                                   {2}: 分线显示控制的内容。若怪物未设置分线，即为0线，则本项不会显示在扩散中
                                   """);
            using (ImRaii.PushIndent())
            {
                ImGui.InputText("##分线模板", ref 分线模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Plugin.Config.分线模板 = 分线模板;
                    Plugin.Config.SaveConfig();
                }
            }
        }
    }
}
