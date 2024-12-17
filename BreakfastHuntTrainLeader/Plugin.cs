using Dalamud.Plugin;
using System;
using BreakfastHuntTrainLeader.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;

namespace BreakfastHuntTrainLeader;

public class Plugin : IDalamudPlugin
{
    internal MainUi MainUi { get; init; }
    public readonly WindowSystem WindowSystem = new("BreakfastHuntTrainLeader");
    private const string Command = "/hunhuan";


    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        try
        {
            Service.Config = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        }
        catch
        {
            Service.Config = new Configuration();
            Service.Config.SaveConfig();
        }

        MainUi = new MainUi();

        WindowSystem.AddWindow(MainUi);

        Service.CommandManager.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = """
                          Open main window.
                          打开主窗口。
                          """
        });

        pluginInterface.UiBuilder.Draw += DrawUi;

        Service.PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUi;
        Service.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainUi.Dispose();
        Service.CommandManager.RemoveHandler(Command);
        GC.SuppressFinalize(this);
    }

    private void DrawUi() => WindowSystem.Draw();
    public void ToggleMainUi() => MainUi.Toggle();
    private void OnCommand(string command, string args) => ToggleMainUi();


}
