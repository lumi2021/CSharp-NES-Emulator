using Emulator.RomSpecific;
using Emulator.VirtalMachine;

namespace Emulator.Mappers;

public abstract class Mapper(NesRom rom)
{
    public readonly NesRom romReference = rom;


    public abstract void Process_CPU_Read();
    public abstract void Process_CPU_Write();
    public abstract void Process_PPU_Read();
    public abstract void Process_PPU_Write();
}
