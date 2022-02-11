namespace Soil.SimpleActorModel.Messages.System;

public class Start : SystemMessage
{
    public static readonly Start Instance = new();

    private Start()
    {
    }
}
