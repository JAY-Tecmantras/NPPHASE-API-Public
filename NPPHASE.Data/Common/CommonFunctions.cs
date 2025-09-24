using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.Common
{
    public static class CommonFunctions
    {
        public static readonly Random rand = new Random();
        public const string Alphabet = "0123456789";

        public static string GenerateAlphabaticNumber(int size)
        {

            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }
    }
}
