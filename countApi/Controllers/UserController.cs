using Azure;
using countApi.Models;
using elacoreapi.Filter;
using EpeNetWebAPI.TokenAuthentication;
using FastNetCoreLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        bool withToken = true;

        Responses response = new Responses();
        Data data = new Data();
       
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager tokenManager;
        public UserController( IConfiguration configuration, ITokenManager tokenManager, CountdbContext dbContext)
        {
            _configuration = configuration;
            this.tokenManager = tokenManager;
            _dbContext = dbContext;

        }

        //[TokenAuthenticationFilter]           
        [HttpGet]
        [Route("api/operators/{UserId}")]
        public async Task<IActionResult> GetUserData([FromRoute] int UserId)
        {
            try
            {
                var result = _dbContext.Users.Where(u => u.Id == UserId).FirstOrDefault();

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Get([FromForm] Login login)
        {
            try
            {
                string loginname = login.email;
                string password = login.password;
                string IncPassword = string.Empty;

                /*var getUserAlias = _dbContext.Users
                    .Where(a => a.Nickname == loginname);

                if (getUserAlias.Count() > 0)
                {
                    loginname = getUserAlias.First().Username;
                }*/

                //CHECK THE DEBUG PASSWORD
                /*  var userDebugPassword = _dbContext.CoreSystems.Where(d => d.Debugpassword == password);
                  if (userDebugPassword.Count() > 0)
                  {
                      var userdebugpass = _dbContext.CoreVUsers.Where(u => u.Username == loginname).FirstOrDefault();
                      IncPassword = userdebugpass.Userpass;
                  }
                  else
                  {
                      IncPassword = PasswordEncryptor.HashPassword(password);
                  }*/

                IncPassword = PasswordEncryptor.HashPassword(password);
                //var qUserProfile = _dbContext.core_users.Where(m => m.username == loginname & m.userpass == IncPassword);
                var qUserProfile = _dbContext.Users.Where(u => 
                (u.Username == loginname ||u.EmailAddress == loginname) &&
                u.IsDeactivated == 0 &&
                u.Password == IncPassword).
                    Select(x => new {

                        x.Username,
                        x.Password,
                        x.Id,
                        x.Lastname,
                        x.Firstname,
                        x.Middlename,
                        x.Nickname,
                        x.EmailAddress,
                        x.ContactNumber,
                        x.Status,
                        x.Type
                    });


                if (qUserProfile.Count() > 0)
                {

                    if (qUserProfile.FirstOrDefault().Status == 0 && qUserProfile.FirstOrDefault().Type == "USER")
                    {
                        /* response.message = "This account is Inactive, contact your admin and try again.";
                         response.status = "FAILURE";*/
                        return StatusCode(202, "This account is Inactive, contact your admin and try again.");
                    }
                    var checker = _dbContext.Users.Where(u =>
                  (u.Username == loginname || u.EmailAddress == loginname) &&
                  u.IsDeactivated == 0 &&
                  u.Password == IncPassword).FirstOrDefault();
                    if (checker.IsVerified == 0)
                    {
                        return StatusCode(202, "Account is not yet verified, contact your admin and try again.");
                    }
                    else if(checker.IsVerified == 2)
                    {
                        return StatusCode(202, "Not verified, contact your admin and try again.");
                    }

                    data.Username = qUserProfile.FirstOrDefault().Username;
                    data.Id = int.Parse(qUserProfile.FirstOrDefault().Id.ToString());
                    data.Lastname = qUserProfile.FirstOrDefault().Lastname;
                    data.Firstname = qUserProfile.FirstOrDefault().Firstname;
                    data.Middlename = qUserProfile.FirstOrDefault().Middlename;
                    data.Nickname = qUserProfile.FirstOrDefault().Nickname;
                    data.EmailAddress = qUserProfile.FirstOrDefault().EmailAddress;
                    data.ContactNumber = qUserProfile.FirstOrDefault().ContactNumber;
                    data.Status = qUserProfile.FirstOrDefault().Status;
                    data.Type = qUserProfile.FirstOrDefault().Type;

                    response.stringParam1 = TokenGenerator.Encrypt(loginname) + ":" + TokenGenerator.Encrypt(password);

                    if (withToken == true)
                    {
                        var strngpram = response.stringParam1.ToString();
                        var credentials = strngpram.Split(':');

                        var username1 = TokenGenerator.Decrypt(credentials[0]);
                        var password1 = TokenGenerator.Decrypt(credentials[1]);

                        if (tokenManager.Authenticate(username1, password1))
                        {
                            var tokenresult = tokenManager.tokensresult();

                            Response.Headers.Add("token", tokenresult);
                            data.Token = tokenresult;

                            /*   return StatusCode(200, new { tokenresult });*/
                            //return Ok(new { TokenList = tokenManager.NewToken() });
                        }
                        else
                        {
                            ModelState.AddModelError("Unauthorized", "You are not authorized");
                            return Unauthorized("You are not authorized");
                        }
                    }

                    response.objParam1 = data;
                    response.stringParam2 = PasswordEncryptor.GetMd5Hash(qUserProfile.FirstOrDefault().Id.ToString()).ToString();
                    response.status = "SUCCESS";





                    return StatusCode(200, response);
                }
                else
                {
                    /* response.message = "Wrong email or password!";
                     response.status = "FAILURE";*/
                    return StatusCode(202, "Wrong email or password!");
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }
        public class Login
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        public class Responses
        {
            public string status { get; set; }
            public string message { get; set; }
            public string stringParam1 { get; set; }
            public string stringParam2 { get; set; }
            public object objParam1 { get; set; }
            public object objParam2 { get; set; }
            public object objMenuList { get; set; }

        }
        public class Data
        {
            public string Username { get; set; }
            public int Id { get; set; }
            public string Lastname { get; set; }
            public string Firstname { get; set; }
            public string Middlename { get; set; }
            public string Nickname { get; set; }
            public string EmailAddress { get; set; }
            public string ContactNumber { get; set; }
            public int Status { get; set; }
            public string Type { get; set; }
            public string Token { get; set; }
        }

    }
}
