namespace Emulator.VirtalMachine;

public class Rom
{
    
    private ushort _mapper = 0;
    public ushort Mapper => _mapper;

    public static byte ReadPrg(ushort addr) => throw new Exception();
    public static void WritePrg(ushort addr, byte val) => throw new Exception();

    public static byte ReadChr(ushort addr) => throw new Exception();
    public static void WriteChr(ushort addr, byte val) => throw new Exception();
}
