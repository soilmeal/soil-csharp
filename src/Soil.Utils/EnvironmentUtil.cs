using System;

namespace Soil.Utils;

public static class EnvironmentUtil
{
    private static readonly string _dotnetEnvironmentName = Environment.GetEnvironmentVariable(Constants.DotnetEnvironmentNameKey) ?? Constants.DefaultDotnetEnvrionmentName;

    public static string DotnetEnvironmentName
    {
        get
        {
            return _dotnetEnvironmentName;
        }
    }

    public static bool IsDevelopment()
    {
        return string.Equals(_dotnetEnvironmentName, Constants.DotnetEnvrionmentNameDevelopment);
    }

    public static bool IsProduction()
    {
        return string.Equals(_dotnetEnvironmentName, Constants.DotnetEnvironmentNameProduction);
    }
}
