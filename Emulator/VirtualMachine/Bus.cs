using Emulator.Mappers;

namespace Emulator.VirtalMachine;

public class Bus
{
    public static byte Data { get; set; } = 0;
    public static ushort Address { get; set; } = 0;

    public static Mapper Mapper { get => Rom.Mapper; }


    public static void DoRead()
    {
        Mapper.Process_CPU_Read();
    }
    public static void DoWrite()
    {
        Mapper.Process_CPU_Write();
    }


    public static byte Read(ushort address)
    {
        Address = address;
        DoRead();
        return Data;
    }
    public static void Write(ushort address, byte data)
    {
        Address = address;
        Data = data;
        DoWrite();
    }


    public static ushort ReadAddr(ushort address)
    {
        int addr = 0;

        Address = address;
        DoRead();
        addr |= Data;

        Address = (ushort)(address + 1);
        DoRead();
        addr |= Data << 8;

        return (ushort)addr;
    }
    public static void WriteAddr(ushort address, ushort value)
    {
        var b1 = (byte)(value & 0xFF);
        var b2 = (byte)(value >> 8 & 0xFF);

        Address = address;
        Data = b1;
        DoWrite();

        Address = (byte)(address + 1);
        Data = b2;
        DoWrite();
    }

}
