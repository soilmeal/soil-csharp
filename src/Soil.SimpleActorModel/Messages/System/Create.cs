namespace Soil.SimpleActorModel.Messages.System;

public class Create : SystemMessage
{
    public static readonly Create Instance = new();

    private Create()
    {
    }
}
