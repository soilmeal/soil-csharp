namespace Soil.SimpleActorModel.Message.System;

public class Stop : SystemMessage
{
    public static readonly Stop Instance = new();

    private Stop()
    {
    }
}
