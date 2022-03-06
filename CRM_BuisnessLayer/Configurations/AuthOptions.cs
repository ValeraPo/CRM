using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BearGoodbyeKolkhozProject.Business.Configuration
{
    public class AuthOptions
    {
        public const string Issuer = "BgkBack"; // издатель токена
        public const string Audience = "FrontEnd"; // потребитель токена

        private const string _key = "BgkBackSuperSecretKye";   // ключ для шифрации

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
    }
}
