using EMILtools.Systems;
using static FishInputAuthority;
using static PlayerController;

public class FishInputAuthority : InputAuthority<FishInputReader, PlayerInputMap, Subordinates>
{
    public enum Subordinates { Fish }
}