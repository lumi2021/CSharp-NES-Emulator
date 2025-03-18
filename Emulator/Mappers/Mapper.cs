using Emulator.RomSpecific;
using Emulator.VirtalMachine;

namespace Emulator.Mappers;

public abstract class Mapper(NesRom rom)
{
    public readonly NesRom romReference = rom;

    public byte Read(ushort address, ReadingAs device) => ProcessRead(ProcessAddress(address, device), device);
    public void Write(ushort address, byte value, ReadingAs device) => ProcessWrite(ProcessAddress(address, device), value, device);

    protected abstract ushort ProcessAddress(ushort address, ReadingAs device);

    protected virtual byte ProcessRead(ushort address, ReadingAs device)
    {
        // CPU RAM / PPU pattern tables
        if (address < 0x2000)
            return device != ReadingAs.PPU ? Rom.ReadPrg(address) : Rom.ReadChr(address);

        // Picture PU registers
        else if (address >= 0x2000 && address < 0x4000)
            return 0; //sys.Ppu.ReadRegister(address);

        // Audio PU registers
        else if (address == 0x4015)
            return 0; //sys.Apu.ReadStatus();

        // Input shit
        else if (address == 0x4016)
            return 0; //sys.Joy1.InputBitRegister;
        else if (address == 0x4017)
        {
            //Console.WriteLine("Reading joy 2");
            return 0;
        }

        // CHR PRG RAM and ROM
        else if (address >= 0x4020)
            return Rom.ReadPrg(address);

        else // Open Bus
        {
            Console.WriteLine($"Reading in address {address:X} not implemented!");
            return 0;
        }

    }
    protected virtual void ProcessWrite(ushort address, byte value, ReadingAs device)
    {
        // CPU RAM / PPU pattern tables
        if (address < 0x2000)
        {
            if (device != ReadingAs.PPU)
                Ram.WriteAddress(address, value);
            else Console.WriteLine($"CHR ROM is not writealbe!");
        }

        // Picture PU registers
        else if ((address >= 0x2000 && address < 0x4000) || address == 0x4014)
            return; //sys.Ppu.WriteRegister(address, value);

        // Audio PU registers
        else if (address >= 0x4000 && address <= 0x4013) return; //sys.Apu.Write(address, value);
        else if (address == 0x4015) return; //sys.Apu.Write(address, value);

        // Input shit
        else if (address == 0x4016)
        {
            return; //sys.Joy1.Mode = value == 0 ? JoyControllerMode.Read : JoyControllerMode.Write;
            //sys.Joy2.Mode = value == 0 ? JoyControllerMode.Read : JoyControllerMode.Write;
        }

        // CHR PRG RAM and ROM
        else if (address >= 0x4020)
        {
            Console.WriteLine($"PRG ROM addr ${address:X4} is not writeable!");
        }

        else
        {
            //Console.WriteLine($"Writing in address ${address:X} not implemented!");
        }

    }
}

public enum ReadingAs : byte
{
    CPU,
    PPU,
    None
}
