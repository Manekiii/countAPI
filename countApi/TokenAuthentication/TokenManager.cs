
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastNetCoreLibrary;
using countApi.Models;

namespace EpeNetWebAPI.TokenAuthentication
{
    public class TokenManager : ITokenManager
    {

        private CountdbContext _dbContext;
        private readonly string tokenExpiry;

        private string tokenresult = string.Empty; 
        public TokenManager(CountdbContext dbContext, string tokenExpiry)
        {
            this.tokenExpiry = tokenExpiry;
            _dbContext = dbContext;
           
        }

        public bool Authenticate(string username, string password)
        {
            string IncPassword = string.Empty;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))             
            {
                var userEmployee = _dbContext.Users.Where(m => m.Username == username && m.Status == 1);

                /*   var debugPassword = _dbContext.core_system.Where(m => m.debugpassword == password);

                   if (debugPassword.Count() > 0)
                   {
                       IncPassword = userEmployee.First().userpass;
                   }
                   else
                   {
                       IncPassword = PasswordEncryptor.HashPassword(password).ToString();
                   }*/

                IncPassword = PasswordEncryptor.HashPassword(password).ToString();

                var verifyUser = _dbContext.Users
                         .FirstOrDefault(c => c.Username == username &&
                                         c.Password == IncPassword);

                if (verifyUser != null) {
                    var userid = verifyUser.Id;
                    string userIdString = userid.ToString();
                    NewToken(userIdString);
                    return true;
                }
            }

            return false;

        }

        //TO RETURN THE FROM THE CONTROLLER 
        //NEED TO UPDATE 
        public string tokensresult() {
            var tokentodisplay = tokenresult;
            return tokentodisplay;
        }

        public TokenList NewToken(string userId)
        {
            //var tokenExpireinHrs = Int32.Parse(tokenExpiry);
            var tokenExpireinHrs = 24;

            var token = new TokenList
            {
                validtoken = Guid.NewGuid().ToString(),
                expirydate = DateTime.Now.AddHours(tokenExpireinHrs)
            };

            byte[] byteToken = Encoding.UTF8.GetBytes(userId + ":" + token.validtoken + String.Concat("tsaf") + ":" + token.expirydate);
            string ConvertedToken = Convert.ToBase64String(byteToken).ToString();

            var toAddtoken = _dbContext.Tokens.Add(new Token
            {
                UserId = userId,
                AuthToken = ConvertedToken,
                ExpiresOn = token.expirydate,
                IssuedOn = DateTime.Now,
                IsExpire = false

            });

            tokenresult = ConvertedToken;
            tokensresult();

            _dbContext.SaveChanges();

            return token;
        }

        public bool VerifyToken(string token)
        {
            try
            {
            var FromBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = FromBase64.Split(':');
            if (credentials[1].Substring(credentials[1].Length - 4).Equals("tsaf")) //if token came from fast
            {

                var tokenList = _dbContext.Tokens.FirstOrDefault(t => t.AuthToken == token);
                if (tokenList != null)
                {
                    if (tokenList.ExpiresOn > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        tokenList.IsExpire = true;
                        _dbContext.SaveChanges();
                        return false;
                    }

                }
            }
    
            return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
