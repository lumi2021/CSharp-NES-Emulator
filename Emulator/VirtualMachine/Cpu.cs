
using ImGuiNET;

namespace Emulator.VirtalMachine;

public static class Cpu
{

    private static bool running = false;

    private static ushort progCounter = 0;
    private static byte stackPointer = 0;

    private static byte accumulator = 0;
    private static byte indexX = 0;
    private static byte indexY = 0;

    private static bool flag_negative = false;
    private static bool flag_overflow = false;
    private static bool flag_break = false;
    private static bool flag_decimal = false;
    private static bool flag_interrupt = false;
    private static bool flag_zero = false;
    private static bool flag_carry = false;
    private static byte Flags
    {
        get
        {
            var val = 0;
            val |= flag_negative ? 1 << 7 : 0;
            val |= flag_overflow ? 1 << 6 : 0;
            val |= flag_break ? 1 << 4 : 0;
            val |= flag_decimal ? 1 << 3 : 0;
            val |= flag_interrupt ? 1 << 2 : 0;
            val |= flag_zero ? 1 << 1 : 0;
            val |= flag_carry ? 1 << 0 : 0;
            return (byte)val;
        }
        set {
            flag_negative = (value & (1 << 7)) != 0;
            flag_overflow = (value & (1 << 6)) != 0;
            flag_break = (value & (1 << 4)) != 0;
            flag_decimal = (value & (1 << 3)) != 0;
            flag_interrupt = (value & (1 << 2)) != 0;
            flag_zero = (value & (1 << 1)) != 0;
            flag_carry = (value & (1 << 0)) != 0;
        }
    }

    public static void Init()
    {
        // Initialize the CPU
        Program.Update += Update;
        Program.Debug += Debug;
    }
    public static void Reset()
    {
        Flags = 0;

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
