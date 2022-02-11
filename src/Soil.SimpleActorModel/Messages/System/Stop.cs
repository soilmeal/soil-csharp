namespace Soil.SimpleActorModel.Messages.System;

public class Stop : SystemMessage
{
    public static readonly Stop Instance = new();

    private Stop()
    {
    }
}
