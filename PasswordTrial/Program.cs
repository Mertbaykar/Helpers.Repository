
using Library;

for (int i = 0; i < 20; i++)
{
    string password = PasswordGenerator.GenerateRandomPassword();
    Console.WriteLine(password);
}