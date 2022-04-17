namespace Randomiser
{
    public class RandomiserActionResult
    {
        public readonly string text;
        public readonly char? decoration;

        public RandomiserActionResult(string text, char? decoration = null)
        {
            this.text = text;
            this.decoration = decoration;
        }
    }
}
