namespace UnknownCampServer.Core.Services
{
    public interface IPasswordService
    {
        byte[] GenerateSalt();
        string HashPasswordPBKDF2(string password, byte[] salt);
        bool VerifyPassword(string enteredPassword, string storedPasswordHash);
    }
}
