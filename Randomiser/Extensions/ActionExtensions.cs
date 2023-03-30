namespace Randomiser.Extensions
{
    public static class ActionExtensions
    {
        public static bool Is(this RandomiserAction action, AbilityType ability)
        {
            // TODO Really need to rework actions
            return action?.Action == "SK" && action.Parameters?.Length >= 1 && action.Parameters[0] == ((int)ability).ToString();
        }
    }
}
