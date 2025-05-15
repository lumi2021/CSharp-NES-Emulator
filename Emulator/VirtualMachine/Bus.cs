using Emulator.Mappers;

namespace Emulator.VirtalMachine;

public class Bus
{
    private static byte _last = 0;
    public static Mapper Mapper { get; private set; }


    public static void Reset(Mapper mapper)
    {
        Mapper = mapper;
    }


    public static byte Read(ushort addr)
    {

        return _last;
    }
    public static void Write(ushort addr, byte val)
    {
        
    }

}
