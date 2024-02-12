using countApi.Mailer;
using countApi.Models;
using FastNetCoreLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly CoredbMailContext _cOREDB_MAILContext;
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;

        public RegistrationController(CoredbMailContext mailContext, CountdbContext context, IConfiguration configuration)
        {
            _cOREDB_MAILContext = mailContext;
            _dbContext = context;
            _configuration = configuration;
        }

        public partial class Registration
        {
            public int? Id { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Lastname { get; set; }
            public string? Firstname { get; set; }
            public string? Middlename { get; set; }
            public string? Nickname { get; set; }
            public string? EmailAddress { get; set; }
            public string? ContactNumber { get; set; }

        }

        [HttpPost("registrationBranch")]
        public ActionResult registrationBranch()
        {
            try
            {

                var checkerBranch = _dbContext.Branches.Where(x => x.Status == 1).ToList();

                if (checkerBranch == null)
                {
                    return StatusCode(202,"No Data");
                }

                return Ok(checkerBranch);

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

        [HttpPost("registration")]
        public ActionResult registration([FromForm] Registration registration, [FromForm] String tempBranch)
        {
            try
            {
                if (registration.Middlename == null)
                {
                    registration.Middlename = "N/A";
                }

                var userChecker = _dbContext.Users.Where(x => x.Username.Equals(registration.Username) && x.IsVerified != 2).FirstOrDefault();

                if (userChecker != null)
                {
                    return StatusCode(208, "Already exist or not verified yet!");
                }

                registration.Password = PasswordEncryptor.HashPassword(registration.Password);

                User user = new User();

                user.Username = registration.Username;
                user.Password = registration.Password;
                user.Lastname = registration.Lastname.ToUpper();
                user.Firstname = registration.Firstname.ToUpper();
                user.Middlename = registration.Middlename.ToUpper();
                user.Nickname = registration.Nickname.ToUpper();
                user.EmailAddress = registration.EmailAddress;
                user.ContactNumber = registration.ContactNumber;

                user.Status = 1;
                user.IsRegistered = 1;
                user.IsVerified = 0;
                user.CreatedDate = DateTime.Now;

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                var setCreatedBy = _dbContext.Users.Where(x =>
                x.Username == user.Username &&
                x.Password == user.Password &&
                x.EmailAddress == user.EmailAddress
                ).FirstOrDefault();

                setCreatedBy.CreatedByUserId = int.Parse(user.Id.ToString());


                UserBranchAccess userBranchAccess = new UserBranchAccess();

                userBranchAccess.UserId = user.Id;
                userBranchAccess.BranchCode = tempBranch;
                userBranchAccess.Status = 3;
                userBranchAccess.CreatedByUserId = user.Id;
                userBranchAccess.CreatedDate = DateTime.Now;

                _dbContext.UserBranchAccesses.Add(userBranchAccess);

                _dbContext.SaveChanges();


                return Ok("Successfully Registered!");

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

        [HttpPost("email")]
        public ActionResult email([FromForm]String email)
        {
            try
            {
                var checker = _dbContext.Users.Where(x => x.EmailAddress == email).Select(x => x.EmailAddress).FirstOrDefault();

                if (checker == null)
                {
                    return StatusCode(202,"Email doesn't exist");
                }

                Random generator = new Random();
                String code = generator.Next(0, 1000000).ToString("D6");

                CoreMailer coreEmail = new CoreMailer
                {

                    SenderDisplayEmail = "no-reply@fastgroup.biz",
                    SenderDisplayName = "COUNT BUDDY V2",
                    Recipient = email,
                    MailSubject = "Count Buddy Forgot password",
                    MailBody = "<p style=\"font-size:20px;\"> Forgot password </p>" +
                "<p style=\"font-size:50px;\">" + code + "</p>",
                    MailStatus = "q",
                    MailFormat = "HTML",
                    CreatedBy = "COUNT BUDDY V2",
                    Created = DateTime.Now
                };

                UserForgotPassword userForgotPassword = new UserForgotPassword();

                userForgotPassword.Email = email;
                userForgotPassword.Code = code;
                userForgotPassword.IsUsed = 0;
                userForgotPassword.CreatedDate = DateTime.Now;

                _dbContext.UserForgotPasswords.Add(userForgotPassword);
                _dbContext.SaveChanges();

                _cOREDB_MAILContext.CoreMailers.Add(coreEmail);
                 _cOREDB_MAILContext.SaveChanges();

             




                return Ok("Sent Successfully");

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

        [HttpPost("verificationEmail")]
        public ActionResult verificationEmail([FromForm] String code, [FromForm] String email)
        {
            try
            {
                var checker = _dbContext.UserForgotPasswords.Where(x => x.Email == email && x.Code == code && x.IsUsed == 0).FirstOrDefault();

                if(checker == null)
                {
                    return StatusCode(202, "Invalid verification code");
                }

                checker.IsUsed = 1;
                _dbContext.SaveChanges();
                return Ok("Successfull!");

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

        [HttpPost("ForgotChangePassword")]
        public ActionResult ForgotChangePassword([FromForm] String email, [FromForm] String newPassword)
        {
            try
            {
                var checker = _dbContext.Users.Where(x => x.EmailAddress == email && x.IsVerified == 1 && x.Status == 1).FirstOrDefault();

                checker.Password = PasswordEncryptor.HashPassword(newPassword);

                _dbContext.SaveChanges();

                return Ok("Successfull!");

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



    }
}
