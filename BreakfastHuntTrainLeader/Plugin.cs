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
    internal MainUi MainUi { get; init; }
    public readonly WindowSystem WindowSystem = new("BreakfastHuntTrainLeader");
    private const string Command = "/hunhuan";
    internal static Configuration Config { get; set; } = null!;
    public IDalamudPluginInterface PluginInterface { get; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
        DService.Init(PluginInterface);
        try
        {
            Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        }
        catch
        {
            Config = new Configuration();
            Config.SaveConfig();
            NotifyHelper.NotificationError("配置文件加载失败，已重置配置文件。", "BreakfastHuntTrainLeader");
        }

        MainUi = new MainUi();

        WindowSystem.AddWindow(MainUi);
        DService.Command.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = """
                          Open main window.
                          打开主窗口。
                          """
        });

        PluginInterface.UiBuilder.Draw += DrawUi;

        PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainUi.Dispose();
        DService.Command.RemoveHandler(Command);
        GC.SuppressFinalize(this);
    }

    private void DrawUi() => WindowSystem.Draw();
    public void ToggleMainUi() => MainUi.Toggle();
    private void OnCommand(string command, string args) => ToggleMainUi();
}
