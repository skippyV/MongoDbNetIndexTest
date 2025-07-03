using System.Text;

namespace MongoDbTesting.Utils
{
    public static class StringStuff
    {
        public static string RemoveWhitespacesUsingStringBuilder(string source)
        {
            var builder = new StringBuilder(source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (!char.IsWhiteSpace(c))
                    builder.Append(c);
            }
            return source.Length == builder.Length ? source : builder.ToString();
        }
    }

}
