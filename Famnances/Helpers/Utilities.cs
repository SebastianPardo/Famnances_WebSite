namespace Famnances.Helpers
{
    public class Utilities
    {
        public static bool StringEmptyOrNull(string[] strings)
        {
            foreach (string s in strings)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
