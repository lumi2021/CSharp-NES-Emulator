
using ImGuiNET;

namespace Emulator.VirtalMachine;

public static class Cpu
{

    private static ushort progCounter = 0;
    private static byte stackPointer = 0;

    private static byte accumulator = 0;
    private static byte indexX = 0;
    private static byte indexY = 0;

    public static void Init()
    {
        // Initialize the CPU
        Program.Update += Update;
        Program.Debug += Debug;
    }

    private static void Update(double delta)
    {
        
    }
    private static void Debug(double delta)
    {
        ImGui.Begin("CPU");

        ImGui.Text($"PC: {progCounter:X4}");
        ImGui.Text($"SP: {progCounter:X2}");
        ImGui.Text($"A:  {accumulator:X2}");
        ImGui.Text($"X:  {indexX:X2}");
        ImGui.Text($"Y:  {indexY:X2}");

        ImGui.End();
    }
}
