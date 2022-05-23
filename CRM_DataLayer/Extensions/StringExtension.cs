namespace CRM.DataLayer.Extensions
{
    // Class for secure logging emails
    public static class StringExtension
    {
        public static string Encryptor(this string str)
        {
            var mas = str.ToCharArray();
            for (int i = 1; i < mas.Length - 1; i++)
                mas[i] = '*';
            return String.Join("", mas);
        }
    }
}
