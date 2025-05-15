using Emulator.RomSpecific;

namespace Emulator.Mappers;

internal class NROM(NesRom r) : Mapper(r)
{
    
    protected override byte Process_CPU_Read(ushort address)
    {
        throw new NotImplementedException();
    }
    protected override void Process_CPU_Write(ushort address, byte value)
    {
        throw new NotImplementedException();
    }
    protected override byte Process_PPU_Read(ushort address)
    {
        throw new NotImplementedException();
    }
    protected override void Process_PPU_Write(ushort address, byte value)
    {
        throw new NotImplementedException();
    }
}
