using Emulator.RomSpecific;
using Emulator.VirtalMachine;

namespace Emulator.Mappers;

internal class NROM(NesRom r) : Mapper(r)
{

    public override void Process_CPU_Read()
    {
        ushort addr = Bus.Address;

        if (addr < 0x2000) Bus.Data = Ram.ReadAddress(addr);

        if (addr >= 0x8000) Bus.Data = Rom.ReadPrg((ushort)(addr - 0x8000));

        else throw new Exception($"Unmapped address {addr} (R)");
    }
    public override void Process_CPU_Write()
    {
        ushort addr = Bus.Address;
        byte data = Bus.Data;

        if (addr < 0x2000) Ram.WriteAddress(addr, data);

        else throw new Exception($"Unmapped address {addr} (W)");
    }

    public override void Process_PPU_Read()
    {
        throw new NotImplementedException();
    }
    public override void Process_PPU_Write()
    {
        throw new NotImplementedException();
    }
}
