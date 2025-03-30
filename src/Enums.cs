namespace MaulAliveCreatures;

public class Enums
{
    public static SoundID scavcry;

    public static SoundID munch;

    public static SoundID yummers;

    public static void RegisterValues()
    {
        scavcry = new SoundID("scavcry", true);
        munch = new SoundID("munch", true);
        yummers = new SoundID("yummers", true);
    }

    public static void UnregisterValues()
    {
        Unregister<SoundID>((ExtEnum<SoundID>)(object)scavcry);
        Unregister<SoundID>((ExtEnum<SoundID>)(object)munch);
        Unregister<SoundID>((ExtEnum<SoundID>)(object)yummers);
    }

    private static void Unregister<T>(ExtEnum<T> extEnum) where T : ExtEnum<T>
    {
        extEnum?.Unregister();
    }
}