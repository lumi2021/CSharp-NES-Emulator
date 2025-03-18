using ImGuiNET;

namespace Emulator.VirtualMachine;

public static class Ppu
{

    public static void Init()
    {
        // Initialize the PPU
        Program.Update += Update;
        Program.Debug += Debug;
    }

    private static void Update(double delta)
    {
        ImGui.Begin("VGA");
        ImGui.Text("b");
        ImGui.End();
    }
    private static void Debug(double delta)
    {
        ImGui.Begin("PPU");
        ImGui.Text("a");
        ImGui.End();
    }

}
