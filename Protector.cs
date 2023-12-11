using System.Security.Cryptography;
using System.Text;
using StockSharp.Messages;

namespace NoteApp;

public class Protector
{
    static public string UseGenerateSalt()
    {
        return GenerateSalt();
    }
    static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }
    static public string UseHashPassword(string password, string salt)
    {
        return HashPassword(password, salt);
    }
    static string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

public class Authentication
{
    private readonly Context _dbContext;
    private static int cachedUserId;

    public Authentication(Context dbContext)
    {
        _dbContext = dbContext;
    }

    public bool AuthenticateUser(string login, string password)
    {
        var passwordVerifier = new PasswordVerifier(_dbContext, login);

        bool isPasswordCorrect = passwordVerifier.VerifyPassword(password);

        int userId = GetUserId(login);
        cachedUserId = userId;

        return isPasswordCorrect;
    }

    public static int GetCachedUserId()
    {
        return cachedUserId;
    }

    private int GetUserId(string login)
    {
        int userId = _dbContext.Users
            .Where(u => u.login == login)
            .Select(u => (int)u.user_id)
            .FirstOrDefault();

        return userId;
    }
}
public class PasswordVerifier
{
    private readonly Context _dbContext;
    private readonly string storedSalt;
    private readonly string storedHashedPassword;

    public PasswordVerifier(Context dbContext, string username)
    {
        _dbContext = dbContext;
        storedSalt = GetSaltFromDatabase(username);
        storedHashedPassword = GetHashedPasswordFromDatabase(username);
    }

    private string GetSaltFromDatabase(string username)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.login == username);
        if (user != null)
        {
            return user.salt;
        }
        return null;
    }

    private string GetHashedPasswordFromDatabase(string username)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.login == username);
        return user.password;
    }

    public bool VerifyPassword(string enteredPassword)
    {
        string enteredPasswordHash = Protector.UseHashPassword(enteredPassword, storedSalt);

        return storedHashedPassword.Equals(enteredPasswordHash);
    }
}