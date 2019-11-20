using System;
using System.Linq;
using System.Text;

namespace NetCoreStarter.Utils
{
    public class StringGenerators
    {
        private static readonly Random Random = new Random();

        public static string GenerateToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string GenerateRandomString(int length)
        {
            var stringBuilder = new StringBuilder(length);
            var chArray = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var random = new Random((int)DateTime.Now.Ticks);
            for (var index = 0; index < length; ++index)
                stringBuilder.Append(chArray[random.Next(chArray.Length)]);
            return stringBuilder.ToString().ToUpper();
        }
        public static string GenerateRandomNumber(int length)
        {
            var stringBuilder = new StringBuilder(length);
            var chArray = "0123456789".ToCharArray();
            var random = new Random((int)DateTime.Now.Ticks);
            for (var index = 0; index < length; ++index)
                stringBuilder.Append(chArray[random.Next(chArray.Length)]);
            return stringBuilder.ToString();
        }

        public static string GenerateCode(int length,
           string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            var charArray = charset.ToCharArray();
            var charLength = charArray.Length;
            var output = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                output.Append(charArray[rand.Next(charLength)]);
            }
            return output.ToString();
        }
    }
}
