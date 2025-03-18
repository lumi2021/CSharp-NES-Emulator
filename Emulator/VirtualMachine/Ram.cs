using System.Runtime.InteropServices.Marshalling;

namespace Emulator.VirtalMachine;

public static class Ram
{
    
    private static byte[] _data = new byte[0x800];

    public static byte ReadAddress(ushort addr) => _data[addr % 0x800];
    public static void WriteAddress(ushort addr, byte val) => _data[addr % 0x800] = val;
}
