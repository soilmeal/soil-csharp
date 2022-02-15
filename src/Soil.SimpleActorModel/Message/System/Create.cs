namespace Soil.SimpleActorModel.Message.System;

internal class Create : SystemMessage
{
    public static readonly Create Instance = new();

    private Create()
    {
    }
}
