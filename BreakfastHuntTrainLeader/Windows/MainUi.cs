using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using OmenTools;
using OmenTools.Helpers;
using OmenTools.ImGuiOm;

namespace BreakfastHuntTrainLeader.Windows;

public class MainUi : Window, IDisposable
{
    private int  selectedServer     = 0;
    private int  instanceIdInput    = 0;
    private bool selected           = false;
    private bool initialized        = false;
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
        if (!initialized)
        {
            var pc = DService.ClientState.LocalPlayer;
            if (pc?.CurrentWorld.Value.DataCenter.Value == null) return;
            var dataCenter = pc.CurrentWorld.Value.DataCenter.Value.Name.ExtractText();

            initialized = true;
            if (Plugin.Config.大区名 != dataCenter)
            {
                Plugin.Config.Marks.Clear();
                Plugin.Config.大区名 = dataCenter;
                Plugin.Config.SaveConfig();
            }
        }

        if (Plugin.Config.大区名 != "莫古力" && Plugin.Config.大区名 != "豆豆柴")
        {
            ImGui.TextColored(new(255,0,0,255) ,"本插件为莫古力区和豆豆柴区特供，用于早餐S车带车，其他区无使用本插件的需求。");
            return;
        }

        using (ImRaii.TabBar("##tabBar"))
        {
            using (ImRaii.TabItem("怪物列表"))
                DrawMarks();
            using (ImRaii.TabItem("设置"))
                DrawSettings();
        }
    }

    private void DrawMarks()
    {
        if (ImGui.Checkbox("编辑模式", ref Plugin.Config.编辑模式))
            Plugin.Config.SaveConfig();
        var counter = 0;
        using (ImRaii.Table("Marks", 6, ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable))
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
                if (Plugin.Config.编辑模式)
                {
                    instanceIdInput = (int)mark.InstanceId;
                    if (ImGui.InputInt($"##分线{counter}", ref instanceIdInput))
                    {
                        Plugin.Config.Marks[counter].InstanceId = (uint)instanceIdInput;
                        Plugin.Config.SaveConfig();
                    }
                }
                else
                {
                    ImGui.Text($"{mark.InstanceId}线");
                }

                ImGui.TableNextColumn();
                if (ImGuiOm.ButtonIcon($"{counter}FlagMark", FontAwesomeIcon.Flag, useStaticFont: true))
                    if (mark.FlagMark())
                        HelpersOm.NotificationSuccess("已标记", "BreakfastHuntTrainLeader");
                    else
                        HelpersOm.NotificationError("标记失败", "BreakfastHuntTrainLeader");

                ImGui.SameLine();
                ImGui.Text($"({mark.MapPos.X:F1}, {mark.MapPos.Y:F1})");

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
            if (Plugin.Config.编辑模式)
            {
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
                        if (!newMark.InitByFlag())
                        {
                            HelpersOm.NotificationError("添加失败，当前未设置flag", "BreakfastHuntTrainLeader");
                        }
                        else
                        {
                            Plugin.Config.Marks.Add(newMark);
                            Plugin.Config.SaveConfig();
                            selected = false;
                        }
                    }
                }
            }
        }
    }

    private static void DrawSettings()
    {
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
                ImGui.InputText("##同服S怪", ref Plugin.Config.同服扩散模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                    Plugin.Config.SaveConfig();
            }

            ImGui.Text("跨服S怪:");
            ImGuiWidget.HelpMarker("""
                               {0}: 分线显示控制。若怪物未设置分线，即为0线，则会不显示
                               {1}: 服务器名
                               """);
            using (ImRaii.PushIndent())
            {
                ImGui.InputText("##跨服S怪", ref Plugin.Config.跨服扩散模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                    Plugin.Config.SaveConfig();
            }

            ImGui.Text("分线模板:");
            ImGuiWidget.HelpMarker("{2}: 分线显示控制的内容。若怪物未设置分线，即为0线，则本项不会显示在扩散中");
            using (ImRaii.PushIndent())
            {
                ImGui.InputText("##分线模板", ref Plugin.Config.分线模板, 256);
                if (ImGui.IsItemDeactivatedAfterEdit())
                    Plugin.Config.SaveConfig();
            }
        }
    }
}
