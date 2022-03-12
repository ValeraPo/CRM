using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.DataLayer.Extensions
{
    public static class StringExtension
    {
        public static string Encryptor(this string str)
        {
            var mas = str.Split();
            for (int i = 3; i < mas.Length - 3; i++)
                mas[i] = "*";
            return String.Join("", mas); 
        }
    }
}
