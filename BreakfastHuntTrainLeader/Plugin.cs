using Dalamud.Plugin;
using System;
using BreakfastHuntTrainLeader.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using OmenTools;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader;

public class Plugin : IDalamudPlugin
{
    private MainUi MainUi { get; init; }
    private readonly WindowSystem windowSystem = new("BreakfastHuntTrainLeader");
    private const string Command = "/hunhuan";

    public static Configuration Config { get; set; } = null!;
    public static TaskHelper Tasks = new();

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        DService.Init(pluginInterface);
        try
        {
            Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        }
        catch
        {
            Config = new Configuration();
            Config.SaveConfig();
            HelpersOm.NotificationError("配置文件加载失败，已重置配置文件。", "BreakfastHuntTrainLeader");
        }

        MainUi = new MainUi();
        windowSystem.AddWindow(MainUi);

        DService.Command.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = """
                          Open main window.
                          打开主窗口。
                          """
        });

        pluginInterface.UiBuilder.Draw += DrawUi;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleMainUi;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
    }

    public void Dispose()
    {
        Tasks.Dispose();
        windowSystem.RemoveAllWindows();
        MainUi.Dispose();
        DService.Command.RemoveHandler(Command);
        GC.SuppressFinalize(this);
    }

    private void DrawUi() => windowSystem.Draw();
    public void ToggleMainUi() => MainUi.Toggle();
    private void OnCommand(string command, string args) => ToggleMainUi();
}
