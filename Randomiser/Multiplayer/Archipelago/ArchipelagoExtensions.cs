using System.Text;
using Archipelago.MultiClient.Net.Helpers;
using APColour = Archipelago.MultiClient.Net.Models.Color;

namespace Randomiser.Multiplayer.Archipelago;

public static class ArchipelagoExtensions
{
    public static string ConvertMessageFormat(this LogMessage message)
    {
        var sb = new StringBuilder();
        foreach (var part in message.Parts)
        {
            if (part.Color == APColour.White || part.Color == APColour.Black)
            {
                sb.Append(part.Text);
            }
            else
            {
                var colour = GetClosestColour(part.Color, APColour.White, APColour.Red, APColour.Yellow, APColour.Blue, APColour.Green);
                sb.Append(ColourString(colour)).Append(part.Text).Append(ColourString(colour));
            }
        }
        return sb.ToString();
    }

    private static APColour GetClosestColour(APColour test, params APColour[] colours)
    {
        var closest = Distance(test, colours[0]);
        var closestColour = colours[0];
        for (var i = 1; i < colours.Length; i++)
        {
            var c = Distance(test, colours[i]);
            if (c < closest)
            {
                closest = c;
                closestColour = colours[i];
            }
        }
        return closestColour;
    }

    private static string ColourString(APColour colour)
    {
        if (colour == APColour.Red) return "@";
        if (colour == APColour.Yellow) return "#";
        if (colour == APColour.Blue) return "*";
        if (colour == APColour.Green) return "$";

        return "";
    }

    private static int Distance(APColour a, APColour b)
    {
        return (a.R - b.R) * (a.R - b.R) + (a.G - b.G) * (a.G - b.G) + (a.B - b.B) * (a.B - b.B);
    }

    public static UnityEngine.Color ToUnityColour(this APColour colour)
    {
        return new UnityEngine.Color(colour.R / 255f, colour.G / 255f, colour.B / 255f);
    }
}
