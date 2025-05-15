using Emulator.Mappers;
using Emulator.RomSpecific;

namespace Emulator.VirtalMachine;

public class Rom
{
    
    private static NesRom _rom_data = null!;
    public static Mapper Mapper { get => _rom_data.Mapper; }

    public static void Load(byte[] rom_data)
    {
        _rom_data = new(rom_data);
    }

    public static byte ReadPrg(ushort addr) => _rom_data.PrgData[addr];
    public static void WritePrg(ushort addr, byte val) => throw new Exception();

    public static byte ReadChr(ushort addr) => _rom_data.ChrData[addr];
    public static void WriteChr(ushort addr, byte val) => throw new Exception();
}
