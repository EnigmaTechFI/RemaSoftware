using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RemaSoftware.WebApp;

public static class ExtensionMethods
{
    private static readonly char[][] valuesArrays = new []
    {
        "abcdefghijklmnopqrstuvwxyz".ToCharArray(),
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
        "1234567890".ToCharArray(),
        "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray()
    };
    public static string GenerateRandomPassword(this IHtmlHelper helper)
    {
        string result = "";
        var rand = new Random();
        for (int i = 0; i <= 10; i++)
        {
            var value = valuesArrays[rand.Next(valuesArrays.Length)];
            result += value[rand.Next(value.Length)];
        }

        return result;
    }
}