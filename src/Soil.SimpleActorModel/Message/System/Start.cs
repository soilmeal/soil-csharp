namespace Soil.SimpleActorModel.Message.System;

public class Start : SystemMessage
{
    public static readonly Start Instance = new();

    private Start()
    {
    }
}
