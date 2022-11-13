namespace Library
{
    public class PasswordGenerator
    {
        private const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        private const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NUMERIC_CHARACTERS = "0123456789";
        private const string SPECIAL_CHARACTERS = @"!#$%&*@\";
        private const int MIN_CHARACTERS = 8;
        private const int MAX_CHARACTERS = 16;
        private const string WOWEL_CHARACTERS = "aeiouAEIOU";
        private const string SILENT_CHARACTERS = "bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";
        private static string AllCharacters => new string(LOWERCASE_CHARACTERS.Union(UPPERCASE_CHARACTERS).Union(NUMERIC_CHARACTERS).Union(SPECIAL_CHARACTERS).Union(WOWEL_CHARACTERS).Union(SILENT_CHARACTERS).ToArray());
        private static Random Random => new Random();

        public static string GenerateRandomPassword()
        {
            //Random random = new Random();
            int passwordLength = Random.Next(MIN_CHARACTERS, MAX_CHARACTERS);
            string password = "";


            #region Fill Password
            // Başta bize gereken her karakter tipinden üretelim, en son yerlerini karışık yaptırırız
            for (int i = 0; i < passwordLength; i++)
            {
                if (!HasLowerCaseCharacter(password))
                {
                    var newChar = GenerateLowerCaseCharacter();
                    password += newChar;
                }
                else if (!HasUpperCaseCharacter(password))
                {
                    var newChar = GenerateUpperCaseCharacter();
                    password += newChar;
                }
                else if (!HasNumericCharacter(password))
                {
                    var newChar = GenerateNumericCharacter();
                    password += newChar;
                }
                else if (!HasSpecialCharacter(password))
                {
                    var newChar = GenerateSpecialCharacter();
                    password += newChar;
                }
                else if (!HasWovelCharacter(password))
                {
                    var newChar = GenerateWovelCharacter();
                    password += newChar;
                }
                else if (!HasSilentCharacter(password))
                {
                    var newChar = GenerateSilentCharacter();
                    password += newChar;
                }
                else
                {
                    var newChar = GenerateRandomCharacter();
                    password += newChar;
                }
            }
            #endregion

            password = MixUpPassword(password);
            return password;
        }

        #region Control Methods
        private static bool HasLowerCaseCharacter(string password)
        {
            return password.Any(x => char.IsLower(x));
        }
        private static bool HasUpperCaseCharacter(string password)
        {
            return password.Any(x => char.IsUpper(x));
        }
        private static bool HasNumericCharacter(string password)
        {
            return password.Any(x => char.IsDigit(x));
        }
        private static bool HasSpecialCharacter(string password)
        {
            return password.Any(x => char.IsLetterOrDigit(x));
        }
        private static bool HasWovelCharacter(string password)
        {
            return password.Any(x => WOWEL_CHARACTERS.Contains(x));
        }
        private static bool HasSilentCharacter(string password)
        {
            return password.Any(x => SILENT_CHARACTERS.Contains(x));
        }
        #endregion

        #region Generators
        private static char GenerateLowerCaseCharacter()
        {
            return LOWERCASE_CHARACTERS[Random.Next(LOWERCASE_CHARACTERS.Length)];
        }

        private static char GenerateUpperCaseCharacter()
        {
            return UPPERCASE_CHARACTERS[Random.Next(UPPERCASE_CHARACTERS.Length)];
        }
        private static char GenerateNumericCharacter()
        {
            return NUMERIC_CHARACTERS[Random.Next(NUMERIC_CHARACTERS.Length)];
        }
        private static char GenerateSpecialCharacter()
        {
            return SPECIAL_CHARACTERS[Random.Next(SPECIAL_CHARACTERS.Length)];
        }
        private static char GenerateWovelCharacter()
        {
            return WOWEL_CHARACTERS[Random.Next(WOWEL_CHARACTERS.Length)];
        }
        private static char GenerateSilentCharacter()
        {
            return SILENT_CHARACTERS[Random.Next(SILENT_CHARACTERS.Length)];
        }
        private static char GenerateRandomCharacter()
        {
            return AllCharacters[Random.Next(AllCharacters.Length)];
        }
        #endregion

        /// <summary>
        /// Şifreyi sessiz ile sesli harfler art arda gelecek, sonrasında semboller, sayılar ve en son da sessiz veya sesli harflerden geriye kalan harfler gelecek şekilde ayarlar
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string MixUpPassword(string password)
        {
            //char[] randomArray = password.ToCharArray().OrderBy(x => Random.Next()).ToArray();
            //string result = new string(randomArray);
            //return result;
            string finalPassword = "";
            List<char> unusedLetters = new List<char>();

            #region Bir sessiz bir sesli harf eklenir ve kullanılmayan harfler bulunur
            var silentLetters = password.Where(x => SILENT_CHARACTERS.Contains(x)).ToList();
            var wowelLetters = password.Where(x => WOWEL_CHARACTERS.Contains(x)).ToList();
            var allLetters = silentLetters.Concat(wowelLetters).ToList();

            int silentCharactersCount = silentLetters.Count();
            int wowelCharactersCount = wowelLetters.Count();

            int syllableCount = silentCharactersCount > wowelCharactersCount ? wowelCharactersCount : silentCharactersCount < wowelCharactersCount ? silentCharactersCount : silentCharactersCount;

            List<char> usedLetters = new List<char>();

            for (int i = 0; i < syllableCount; i++)
            {
                finalPassword += silentLetters[i];
                finalPassword += wowelLetters[i];
                usedLetters.Add(silentLetters[i]);
                usedLetters.Add(wowelLetters[i]);
            }
            unusedLetters = allLetters.Except(usedLetters).ToList();
            #endregion

            #region Semboller eklenir
            var symbols = password.Where(x => SPECIAL_CHARACTERS.Contains(x)).ToList();

            for (int i = 0; i < symbols.Count(); i++)
            {
                finalPassword += symbols[i];
            }
            #endregion

            #region Sayılar eklenir
            var numbers = password.Where(x => NUMERIC_CHARACTERS.Contains(x)).ToList();
            for (int i = 0; i < numbers.Count(); i++)
            {
                finalPassword += numbers[i];
            }
            #endregion

            #region Kullanılmamış harfler eklenir
            for (int i = 0; i < unusedLetters.Count(); i++)
            {
                finalPassword += unusedLetters[i];
            }
            #endregion

            return finalPassword;
        }
    }
}