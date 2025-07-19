
using ImGuiNET;

namespace Emulator.VirtalMachine;

public static class Cpu
{

    private static bool running = false;
    private static bool doStep = false;
    private static int clockCount = 0;

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
        set
        {
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
        Console.WriteLine("CPU Reseted");
        Flags = 0;
        progCounter = Bus.ReadAddr(0xFFFC);
    }
    public static void Interrupt()
    {

    }

    private static void Update(double delta)
    {
        Tick();
    }
    private static void Debug(double delta)
    {
        ImGui.Begin("CPU");

        ImGui.Text($"Running: {running && !flag_break}");

        ImGui.Text($"PC: {progCounter:X4}");
        ImGui.Text($"SP: {stackPointer:X2}");
        ImGui.Text($"A:  {accumulator:X2}");
        ImGui.Text($"X:  {indexX:X2}");
        ImGui.Text($"Y:  {indexY:X2}");

        // flags
        ImGui.Text("Flags: "); ImGui.SameLine();
        if (flag_negative) ImGui.Text("N"); else ImGui.TextDisabled("N"); ImGui.SameLine();
        if (flag_overflow) ImGui.Text("O"); else ImGui.TextDisabled("O"); ImGui.SameLine();
        ImGui.TextDisabled("-"); ImGui.SameLine();
        if (flag_break) ImGui.Text("B"); else ImGui.TextDisabled("B"); ImGui.SameLine();
        if (flag_decimal) ImGui.Text("D"); else ImGui.TextDisabled("D"); ImGui.SameLine();
        if (flag_interrupt) ImGui.Text("I"); else ImGui.TextDisabled("I"); ImGui.SameLine();
        if (flag_zero) ImGui.Text("Z"); else ImGui.TextDisabled("Z"); ImGui.SameLine();
        if (flag_carry) ImGui.Text("C"); else ImGui.TextDisabled("C");

        ImGui.Text($"Clock:  {clockCount}");

        ImGui.Separator();

        if (running) { if (ImGui.Button("stop")) running = false; }
        else { if (ImGui.Button("run")) running = true; }

        ImGui.SameLine();

        if (ImGui.Button("Reset")) Reset();

        ImGui.End();
    }


    public static void Tick()
    {
        if (!running)
        {
            if (!doStep) return;
            doStep = false;
        }

        byte opCode = ReadCounter();

        var (operation, mode) = DecodeOpCode(opCode);
        clockCount++;

        //_lastExecutedInstructions.Add(new ExecutionFrame(
        //    (ushort)(progCounter - 1),
        //    operation, mode, flags
        //));
        //if (_lastExecutedInstructions.Count >= 60)
        //{
        //    _lastExecutedInstructions.RemoveRange(0, _lastExecutedInstructions.Count - 60);
        //}
        //_executionHistoryUpdated = true;

    }

    private static (Operation operation, AddressMode mode) DecodeOpCode(byte opCode)
    {
      
        return opCode switch
        {
            0x00 => (Operation.Brk, AddressMode.Implied),
            0x01 => (Operation.Ora, AddressMode.XIndirect),
            0x02 => (Operation.Kil, AddressMode.Implied),
            0x03 => (Operation.Slo, AddressMode.XIndirect),
            0x04 => (Operation.Nops, AddressMode.ZeroPage),
            0x05 => (Operation.Ora, AddressMode.ZeroPage),
            0x06 => (Operation.Asl, AddressMode.ZeroPage),
            0x07 => (Operation.Slo, AddressMode.ZeroPage),
            0x08 => (Operation.Php, AddressMode.Implied),
            0x09 => (Operation.Ora, AddressMode.Immediate),
            0x0A => (Operation.Asl, AddressMode.Accumulator),
            0x0B => (Operation.Anc, AddressMode.Immediate),
            0x0C => (Operation.Nops, AddressMode.Absolute),
            0x0D => (Operation.Ora, AddressMode.Absolute),
            0x0E => (Operation.Asl, AddressMode.Absolute),
            0x0F => (Operation.Slo, AddressMode.Absolute),

            // 0x10 - 0x1F
            0x10 => (Operation.Bpl, AddressMode.Relative),
            0x11 => (Operation.Ora, AddressMode.IndirectY),
            0x12 => (Operation.Kil, AddressMode.Implied),
            0x13 => (Operation.Slo, AddressMode.IndirectY),
            0x14 => (Operation.Nops, AddressMode.ZeroPageX),
            0x15 => (Operation.Ora, AddressMode.ZeroPageX),
            0x16 => (Operation.Asl, AddressMode.ZeroPageX),
            0x17 => (Operation.Slo, AddressMode.ZeroPageX),
            0x18 => (Operation.Clc, AddressMode.Implied),
            0x19 => (Operation.Ora, AddressMode.AbsoluteY),
            0x1A => (Operation.Nops, AddressMode.Implied),
            0x1B => (Operation.Slo, AddressMode.AbsoluteY),
            0x1C => (Operation.Nops, AddressMode.AbsoluteX),
            0x1D => (Operation.Ora, AddressMode.AbsoluteX),
            0x1E => (Operation.Asl, AddressMode.AbsoluteX),
            0x1F => (Operation.Slo, AddressMode.AbsoluteX),

            // 0x20 - 0x2F
            0x20 => (Operation.Jsr, AddressMode.Absolute),
            0x21 => (Operation.And, AddressMode.XIndirect),
            0x22 => (Operation.Kil, AddressMode.Implied),
            0x23 => (Operation.Rla, AddressMode.XIndirect),
            0x24 => (Operation.Bit, AddressMode.ZeroPage),
            0x25 => (Operation.And, AddressMode.ZeroPage),
            0x26 => (Operation.Rol, AddressMode.ZeroPage),
            0x27 => (Operation.Rla, AddressMode.ZeroPage),
            0x28 => (Operation.Plp, AddressMode.Implied),
            0x29 => (Operation.And, AddressMode.Immediate),
            0x2A => (Operation.Rol, AddressMode.Accumulator),
            0x2B => (Operation.Anc2, AddressMode.Immediate),
            0x2C => (Operation.Bit, AddressMode.Absolute),
            0x2D => (Operation.And, AddressMode.Absolute),
            0x2E => (Operation.Rol, AddressMode.Absolute),
            0x2F => (Operation.Rla, AddressMode.Absolute),

            // 0x30 - 0x3F
            0x30 => (Operation.Bmi, AddressMode.Relative),
            0x31 => (Operation.And, AddressMode.IndirectY),
            0x32 => (Operation.Kil, AddressMode.Implied),
            0x33 => (Operation.Rla, AddressMode.IndirectY),
            0x34 => (Operation.Nops, AddressMode.ZeroPageX),
            0x35 => (Operation.And, AddressMode.ZeroPageX),
            0x36 => (Operation.Rol, AddressMode.ZeroPageX),
            0x37 => (Operation.Rla, AddressMode.ZeroPageX),
            0x38 => (Operation.Sec, AddressMode.Implied),
            0x39 => (Operation.And, AddressMode.AbsoluteY),
            0x3A => (Operation.Nops, AddressMode.Implied),
            0x3B => (Operation.Rla, AddressMode.AbsoluteY),
            0x3C => (Operation.Nops, AddressMode.AbsoluteX),
            0x3D => (Operation.And, AddressMode.AbsoluteX),
            0x3E => (Operation.Rol, AddressMode.AbsoluteX),
            0x3F => (Operation.Rla, AddressMode.AbsoluteX),

            // 0x40 - 0x4F
            0x40 => (Operation.Rti, AddressMode.Implied),
            0x41 => (Operation.Eor, AddressMode.XIndirect),
            0x42 => (Operation.Kil, AddressMode.Implied),
            0x43 => (Operation.Sre, AddressMode.XIndirect),
            0x44 => (Operation.Nops, AddressMode.ZeroPage),
            0x45 => (Operation.Eor, AddressMode.ZeroPage),
            0x46 => (Operation.Lsr, AddressMode.ZeroPage),
            0x47 => (Operation.Sre, AddressMode.ZeroPage),
            0x48 => (Operation.Pha, AddressMode.Implied),
            0x49 => (Operation.Eor, AddressMode.Immediate),
            0x4A => (Operation.Lsr, AddressMode.Accumulator),
            0x4B => (Operation.Alr, AddressMode.Immediate),
            0x4C => (Operation.Jmp, AddressMode.Absolute),
            0x4D => (Operation.Eor, AddressMode.Absolute),
            0x4E => (Operation.Lsr, AddressMode.Absolute),
            0x4F => (Operation.Sre, AddressMode.Absolute),

            // 0x50 - 0x5F
            0x50 => (Operation.Bvc, AddressMode.Relative),
            0x51 => (Operation.Eor, AddressMode.IndirectY),
            0x52 => (Operation.Kil, AddressMode.Implied),
            0x53 => (Operation.Sre, AddressMode.IndirectY),
            0x54 => (Operation.Nops, AddressMode.ZeroPageX),
            0x55 => (Operation.Eor, AddressMode.ZeroPageX),
            0x56 => (Operation.Lsr, AddressMode.ZeroPageX),
            0x57 => (Operation.Sre, AddressMode.ZeroPageX),
            0x58 => (Operation.Cli, AddressMode.Implied),
            0x59 => (Operation.Eor, AddressMode.AbsoluteY),
            0x5A => (Operation.Nops, AddressMode.Implied),
            0x5B => (Operation.Sre, AddressMode.AbsoluteY),
            0x5C => (Operation.Nops, AddressMode.AbsoluteX),
            0x5D => (Operation.Eor, AddressMode.AbsoluteX),
            0x5E => (Operation.Lsr, AddressMode.AbsoluteX),
            0x5F => (Operation.Sre, AddressMode.AbsoluteX),

            // 0x60 - 0x6F
            0x60 => (Operation.Rts, AddressMode.Implied),
            0x61 => (Operation.Adc, AddressMode.XIndirect),
            0x62 => (Operation.Kil, AddressMode.Implied),
            0x63 => (Operation.Rra, AddressMode.XIndirect),
            0x64 => (Operation.Nops, AddressMode.ZeroPage),
            0x65 => (Operation.Adc, AddressMode.ZeroPage),
            0x66 => (Operation.Ror, AddressMode.ZeroPage),
            0x67 => (Operation.Rra, AddressMode.ZeroPage),
            0x68 => (Operation.Pla, AddressMode.Implied),
            0x69 => (Operation.Adc, AddressMode.Immediate),
            0x6A => (Operation.Ror, AddressMode.Accumulator),
            0x6B => (Operation.Arr, AddressMode.Immediate),
            0x6C => (Operation.Jmp, AddressMode.Indirect),
            0x6D => (Operation.Adc, AddressMode.Absolute),
            0x6E => (Operation.Ror, AddressMode.Absolute),
            0x6F => (Operation.Rra, AddressMode.Absolute),

            // 0x70 - 0x7F
            0x70 => (Operation.Bvs, AddressMode.Relative),
            0x71 => (Operation.Adc, AddressMode.IndirectY),
            0x72 => (Operation.Kil, AddressMode.Implied),
            0x73 => (Operation.Rra, AddressMode.IndirectY),
            0x74 => (Operation.Nops, AddressMode.ZeroPageX),
            0x75 => (Operation.Adc, AddressMode.ZeroPageX),
            0x76 => (Operation.Ror, AddressMode.ZeroPageX),
            0x77 => (Operation.Rra, AddressMode.ZeroPageX),
            0x78 => (Operation.Sei, AddressMode.Implied),
            0x79 => (Operation.Adc, AddressMode.AbsoluteY),
            0x7A => (Operation.Nops, AddressMode.Implied),
            0x7B => (Operation.Rra, AddressMode.AbsoluteY),
            0x7C => (Operation.Nops, AddressMode.AbsoluteX),
            0x7D => (Operation.Adc, AddressMode.AbsoluteX),
            0x7E => (Operation.Ror, AddressMode.AbsoluteX),
            0x7F => (Operation.Rra, AddressMode.AbsoluteX),

            // 0x80 - 0x8F
            0x80 => (Operation.Nops, AddressMode.Immediate),
            0x81 => (Operation.Sta, AddressMode.XIndirect),
            0x82 => (Operation.Nops, AddressMode.Immediate),
            0x83 => (Operation.Sax, AddressMode.XIndirect),
            0x84 => (Operation.Sty, AddressMode.ZeroPage),
            0x85 => (Operation.Sta, AddressMode.ZeroPage),
            0x86 => (Operation.Stx, AddressMode.ZeroPage),
            0x87 => (Operation.Sax, AddressMode.ZeroPage),
            0x88 => (Operation.Dey, AddressMode.Implied),
            0x89 => (Operation.Nops, AddressMode.Immediate),
            0x8A => (Operation.Txa, AddressMode.Implied),
            0x8B => (Operation.Ane, AddressMode.Immediate),
            0x8C => (Operation.Sty, AddressMode.Absolute),
            0x8D => (Operation.Sta, AddressMode.Absolute),
            0x8E => (Operation.Stx, AddressMode.Absolute),
            0x8F => (Operation.Sax, AddressMode.Absolute),

            // 0x90 - 0x9F
            0x90 => (Operation.Bcc, AddressMode.Relative),
            0x91 => (Operation.Sta, AddressMode.IndirectY),
            0x92 => (Operation.Kil, AddressMode.Implied),
            0x93 => (Operation.Sha, AddressMode.IndirectY),
            0x94 => (Operation.Sty, AddressMode.ZeroPageX),
            0x95 => (Operation.Sta, AddressMode.ZeroPageX),
            0x96 => (Operation.Stx, AddressMode.ZeroPageY),
            0x97 => (Operation.Sax, AddressMode.ZeroPageY),
            0x98 => (Operation.Tya, AddressMode.Implied),
            0x99 => (Operation.Sta, AddressMode.AbsoluteY),
            0x9A => (Operation.Txs, AddressMode.Implied),
            0x9B => (Operation.Tas, AddressMode.AbsoluteY),
            0x9C => (Operation.Shy, AddressMode.AbsoluteX),
            0x9D => (Operation.Sta, AddressMode.AbsoluteX),
            0x9E => (Operation.Shx, AddressMode.AbsoluteY),
            0x9F => (Operation.Sha, AddressMode.AbsoluteY),

            // 0xA0 - 0xAF
            0xA0 => (Operation.Ldy, AddressMode.Immediate),
            0xA1 => (Operation.Lda, AddressMode.XIndirect),
            0xA2 => (Operation.Ldx, AddressMode.Immediate),
            0xA3 => (Operation.Lax, AddressMode.XIndirect),
            0xA4 => (Operation.Ldy, AddressMode.ZeroPage),
            0xA5 => (Operation.Lda, AddressMode.ZeroPage),
            0xA6 => (Operation.Ldx, AddressMode.ZeroPage),
            0xA7 => (Operation.Lax, AddressMode.ZeroPage),
            0xA8 => (Operation.Tay, AddressMode.Implied),
            0xA9 => (Operation.Lda, AddressMode.Immediate),
            0xAA => (Operation.Tax, AddressMode.Implied),
            0xAB => (Operation.Lxa, AddressMode.Immediate),
            0xAC => (Operation.Ldy, AddressMode.Absolute),
            0xAD => (Operation.Lda, AddressMode.Absolute),
            0xAE => (Operation.Ldx, AddressMode.Absolute),
            0xAF => (Operation.Lax, AddressMode.Absolute),

            // 0xB0 - 0xBF
            0xB0 => (Operation.Bcs, AddressMode.Relative),
            0xB1 => (Operation.Lda, AddressMode.IndirectY),
            0xB2 => (Operation.Kil, AddressMode.Implied),
            0xB3 => (Operation.Lax, AddressMode.IndirectY),
            0xB4 => (Operation.Ldy, AddressMode.ZeroPageX),
            0xB5 => (Operation.Lda, AddressMode.ZeroPageX),
            0xB6 => (Operation.Ldx, AddressMode.ZeroPageY),
            0xB7 => (Operation.Lax, AddressMode.ZeroPageY),
            0xB8 => (Operation.Clv, AddressMode.Implied),
            0xB9 => (Operation.Lda, AddressMode.AbsoluteY),
            0xBA => (Operation.Tsx, AddressMode.Implied),
            0xBB => (Operation.Las, AddressMode.AbsoluteY),
            0xBC => (Operation.Ldy, AddressMode.AbsoluteX),
            0xBD => (Operation.Lda, AddressMode.AbsoluteX),
            0xBE => (Operation.Ldx, AddressMode.AbsoluteY),
            0xBF => (Operation.Lax, AddressMode.AbsoluteY),

            // 0xC0 - 0xCF
            0xC0 => (Operation.Cpy, AddressMode.Immediate),
            0xC1 => (Operation.Cmp, AddressMode.XIndirect),
            0xC2 => (Operation.Nops, AddressMode.Immediate),
            0xC3 => (Operation.Dcp, AddressMode.XIndirect),
            0xC4 => (Operation.Cpy, AddressMode.ZeroPage),
            0xC5 => (Operation.Cmp, AddressMode.ZeroPage),
            0xC6 => (Operation.Dec, AddressMode.ZeroPage),
            0xC7 => (Operation.Dcp, AddressMode.ZeroPage),
            0xC8 => (Operation.Iny, AddressMode.Implied),
            0xC9 => (Operation.Cmp, AddressMode.Immediate),
            0xCA => (Operation.Dex, AddressMode.Implied),
            0xCB => (Operation.Ane, AddressMode.Immediate),
            0xCC => (Operation.Cpy, AddressMode.Absolute),
            0xCD => (Operation.Cmp, AddressMode.Absolute),
            0xCE => (Operation.Dec, AddressMode.Absolute),
            0xCF => (Operation.Dcp, AddressMode.Absolute),

            // 0xD0 - 0xDF
            0xD0 => (Operation.Bne, AddressMode.Relative),
            0xD1 => (Operation.Cmp, AddressMode.IndirectY),
            0xD2 => (Operation.Kil, AddressMode.Implied),
            0xD3 => (Operation.Dcp, AddressMode.IndirectY),
            0xD4 => (Operation.Nops, AddressMode.ZeroPageX),
            0xD5 => (Operation.Cmp, AddressMode.ZeroPageX),
            0xD6 => (Operation.Dec, AddressMode.ZeroPageX),
            0xD7 => (Operation.Dcp, AddressMode.ZeroPageX),
            0xD8 => (Operation.Cld, AddressMode.Implied),
            0xD9 => (Operation.Cmp, AddressMode.AbsoluteY),
            0xDA => (Operation.Nops, AddressMode.Implied),
            0xDB => (Operation.Dcp, AddressMode.AbsoluteY),
            0xDC => (Operation.Nops, AddressMode.AbsoluteX),
            0xDD => (Operation.Cmp, AddressMode.AbsoluteX),
            0xDE => (Operation.Dec, AddressMode.AbsoluteX),
            0xDF => (Operation.Dcp, AddressMode.AbsoluteX),

            // 0xE0 - 0xEF
            0xE0 => (Operation.Cpx, AddressMode.Immediate),
            0xE1 => (Operation.Sbc, AddressMode.XIndirect),
            0xE2 => (Operation.Nops, AddressMode.Immediate),
            0xE3 => (Operation.Isc, AddressMode.XIndirect),
            0xE4 => (Operation.Cpx, AddressMode.ZeroPage),
            0xE5 => (Operation.Sbc, AddressMode.ZeroPage),
            0xE6 => (Operation.Inc, AddressMode.ZeroPage),
            0xE7 => (Operation.Isc, AddressMode.ZeroPage),
            0xE8 => (Operation.Inx, AddressMode.Implied),
            0xE9 => (Operation.Sbc, AddressMode.Immediate),
            0xEA => (Operation.Nop, AddressMode.Implied),
            0xEB => (Operation.Usbc, AddressMode.Immediate),
            0xEC => (Operation.Cpx, AddressMode.Absolute),
            0xED => (Operation.Sbc, AddressMode.Absolute),
            0xEE => (Operation.Inc, AddressMode.Absolute),
            0xEF => (Operation.Isc, AddressMode.Absolute),

            // 0xF0 - 0xFF
            0xF0 => (Operation.Beq, AddressMode.Relative),
            0xF1 => (Operation.Sbc, AddressMode.IndirectY),
            0xF2 => (Operation.Kil, AddressMode.Implied),
            0xF3 => (Operation.Isc, AddressMode.IndirectY),
            0xF4 => (Operation.Nops, AddressMode.ZeroPageX),
            0xF5 => (Operation.Sbc, AddressMode.ZeroPageX),
            0xF6 => (Operation.Inc, AddressMode.ZeroPageX),
            0xF7 => (Operation.Isc, AddressMode.ZeroPageX),
            0xF8 => (Operation.Sed, AddressMode.Implied),
            0xF9 => (Operation.Sbc, AddressMode.AbsoluteY),
            0xFA => (Operation.Nops, AddressMode.Implied),
            0xFB => (Operation.Isc, AddressMode.AbsoluteY),
            0xFC => (Operation.Nops, AddressMode.AbsoluteX),
            0xFD => (Operation.Sbc, AddressMode.AbsoluteX),
            0xFE => (Operation.Inc, AddressMode.AbsoluteX),
            0xFF => (Operation.Isc, AddressMode.AbsoluteX),
        };
    }


    public static byte ReadCounter()
    {
        return Bus.Read(progCounter++);
    }
    public static ushort ReadCounterWord()
    {
        return (ushort)(ReadCounter() | ReadCounter() << 8);
    }


    private enum Operation : byte
    {
        Adc, And, Asl, Bcc, Bcs, Beq, Bit, Bmi, Bne,
        Bpl, Brk, Bvc, Bvs, Clc, Cld, Cli, Clv, Cmp,
        Cpx, Cpy, Dec, Dex, Dey, Eor, Inc, Inx, Iny,
        Jmp, Jsr, Lda, Ldx, Ldy, Lsr, Nop, Ora, Pha,
        Php, Pla, Plp, Rol, Ror, Rti, Rts, Sbc, Sec,
        Sed, Sei, Sta, Stx, Sty, Tax, Tay, Tsx, Txa,
        Txs, Tya,

        // Illegal shit here

        Alr, Anc, Anc2, Ane, Arr, Dcp, Isc, Las, Lax,
        Lxa, Rla, Rra, Sax, Sbx, Sha, Shx, Shy, Slo,
        Sre, Tas, Usbc, Nops, Kil
    }

    private enum AddressMode : byte
    {
        Implied = 0,
        Accumulator,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Immediate,
        Indirect,
        XIndirect,
        IndirectY,
        Relative,
        ZeroPage,
        ZeroPageX,
        ZeroPageY
    }

}

