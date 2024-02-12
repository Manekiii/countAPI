namespace EpeNetWebAPI.TokenAuthentication
{
    public interface ITokenManager
    {
        bool Authenticate(string username, string password);
        TokenList NewToken(string userId);
        string tokensresult();
        bool VerifyToken(string token);
    }
}