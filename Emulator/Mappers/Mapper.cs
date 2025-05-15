using Emulator.RomSpecific;
using Emulator.VirtalMachine;

namespace Emulator.Mappers;

public abstract class Mapper(NesRom rom)
{
    public readonly NesRom romReference = rom;

    public byte Read(ushort address) => Process_CPU_Read(address);
    public void Write(ushort address, byte value) => Process_CPU_Write(address, value);
    public byte Video_Read(ushort address) => Process_PPU_Read(address);
    public void Video_Write(ushort address, byte value) => Process_PPU_Write(address, value);


    protected abstract byte Process_CPU_Read(ushort address);
    protected abstract void Process_CPU_Write(ushort address, byte value);
    protected abstract byte Process_PPU_Read(ushort address);
    protected abstract void Process_PPU_Write(ushort address, byte value);
}
