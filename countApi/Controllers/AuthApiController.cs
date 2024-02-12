using countApi.Models;
using elacoreapi.Filter;
using FastNetCoreLibrary;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Text;
using static countApi.Controllers.AuthApiController;
using static countApi.Controllers.RegistrationController;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Linq;

/*entity.HasOne(d => d.UserBranchAccesses1).WithMany(p => p.branch1)
               .HasPrincipalKey(p => p.BranchCode)
               .HasForeignKey(d => d.Code)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UserBranchAccess_Branch");*/

/*
   entity.ToTable("UserMenu").HasKey(x => new { x.MenuId, x.UserId });

    [Key]
    [Column (Order = 1)]
    public int MenuId { get; set; }

    [Key]
    [Column(Order = 2)] 
    public long UserId { get; set; }*/

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class AuthApiController : ControllerBase
    {
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthApiController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }


        public class userMenu
        {
            public dynamic? selectedBranch { get; set; }
            public dynamic? userBranch { get; set; }
            public dynamic? allBranch { get; set; }
            public dynamic? userMenus { get; set; }
            public dynamic? allMenu { get; set; }

        }
        public partial class UserMenuSuper
        {
            public int MenuId { get; set; }

            public int UserId { get; set; }

            public byte Add { get; set; }

            public byte Edit { get; set; }

            public byte Delete { get; set; }
        }

        [HttpPost("drawerNavigation")]
        public ActionResult drawerNavigation(
        [FromForm] int id,
        [FromForm] String? branchCode
        )
        {

            try
            {
                userMenu usermenus = new userMenu();
                var tempUserMenu = _dbContext.UserMenus.Where(x => x.UserId == id && x.BranchCode == branchCode).ToList();

                var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) && x.BranchCodeNavigation.IsDelete == 0 && x.Status == 1).Select(x => new
                {
                    x.BranchCodeNavigation,
                    x.IsDefault

                }).ToList();

                for (int index = 0; index < userBranch.Count; index++)
                {
                    if (userBranch[index].IsDefault == 1)
                    {
                        usermenus.selectedBranch = userBranch[index];
                        if (branchCode == null)
                        {
                             tempUserMenu = _dbContext.UserMenus.Where(x => x.UserId == id && x.BranchCode == userBranch[index].BranchCodeNavigation.Code).ToList();
                        }
                    }

                }


                if (branchCode != null)
                {
                     tempUserMenu = _dbContext.UserMenus.Where(x => x.UserId == id && x.BranchCode == branchCode).ToList();
                }

                if (_dbContext.Users.Where(x=>x.Id.Equals(id)).FirstOrDefault().Type == "Super")
                {
                     tempUserMenu = _dbContext.UserMenus.Where(x => x.UserId == id).ToList();
                }

                var tempMenu = _dbContext.Menus.OrderBy(x => x.Sort).ToList();

                var allBranch = _dbContext.Branches.Where(x => x.Status == 1 && x.IsDelete == 0).ToList();

                usermenus.allBranch = allBranch;
                usermenus.userBranch = userBranch;
                usermenus.userMenus = tempUserMenu;
                usermenus.allMenu = tempMenu;

                return Ok(usermenus);


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




        [HttpPost("profileSaveChanges")]
        public ActionResult profileSaveChanges(
            [FromForm] Registration registration,
            [FromForm] String? EnteredPassword)
        {

            try
            {
                var selectedUser = _dbContext.Users.FirstOrDefault(x => x.Id == registration.Id);

                EnteredPassword = PasswordEncryptor.HashPassword(EnteredPassword);

                if (selectedUser != null && EnteredPassword == selectedUser.Password)
                {
                    selectedUser.Username = registration.Username;
                    selectedUser.Lastname = registration.Lastname.ToUpper();
                    selectedUser.Firstname = registration.Firstname.ToUpper();
                    selectedUser.Middlename = registration.Middlename.ToUpper();
                    selectedUser.Nickname = registration.Nickname.ToUpper();
                    selectedUser.EmailAddress = registration.EmailAddress;
                    selectedUser.ContactNumber = registration.ContactNumber;

                    _dbContext.SaveChanges();
                    return Ok(selectedUser);
                }
                else
                {
                    return StatusCode(202, "Invalid password!");
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

        [HttpPost("profileChangePassword")]
        public ActionResult profileChangePassword(
                [FromForm] String oldPass,
                [FromForm] String newPass,
                [FromForm] int id)
        {
            try
            {

                var selectedUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);

                oldPass = PasswordEncryptor.HashPassword(oldPass);

                if (selectedUser != null && oldPass == selectedUser.Password)
                {
                    newPass = PasswordEncryptor.HashPassword(newPass);

                    selectedUser.Password = newPass;
                    _dbContext.SaveChanges();
                    return Ok("Successfull change!");
                }
                else
                {
                    return StatusCode(202,"Invalid!");
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


        [HttpPost("verificationList")]
        public ActionResult verificationList([FromForm] string? search, [FromForm] String branchCode, [FromForm] String type)
        {

            try
            {
                if (search == null)
                {
                    dynamic list;

                    if (type == "Super")
                    {
                         list = _dbContext.Users.Where(x => x.IsVerified == 0 && x.Status == 1 && x.IsDeactivated == 0).Select(x => new {
                             x.Id,
                             x.Firstname,
                             x.Lastname,
                             x.Middlename,
                             x.EmailAddress,
                             x.UserBranchAccessUsers.FirstOrDefault().BranchCodeNavigation.Name}).ToList();
                    }
                    else
                    {
                        list = _dbContext.UserBranchAccesses.Where(x => x.User.IsVerified == 0 && x.User.Status == 1 && x.User.IsDeactivated == 0 && x.Status == 3 && x.BranchCode == branchCode).Select(x => new {
                            x.User,
                            x.BranchCodeNavigation.Name
                        }).ToList();
                    }

                    if (list.Count == 0)
                    { 
                        return Ok("No Data");
                    }

                    return Ok(list);
                }
                else
                {
                    var list = _dbContext.UserBranchAccesses.Where(x => x.User.IsVerified == 0 && x.User.Status == 1 && x.User.IsDeactivated == 0 && x.Status == 3 && x.BranchCode == branchCode && x.User.EmailAddress.Contains(search)).Select(x => x.User).ToList();

                    if (list.Count == 0)
                    {
                        return Ok("No Data");
                    }

                    return Ok(list);
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

        [HttpPost("declineVerification")]
        public ActionResult declineVerification([FromForm] int selectedUserId)
        {
            try
            {
                var selectedUser = _dbContext.Users.FirstOrDefault(x => x.Id.Equals(selectedUserId));

                selectedUser.IsVerified = 2;

                _dbContext.SaveChanges();
                return Ok("Declined successfully!");

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

        public class branchAccess
        {

            public int UserId { get; set; }

            public String BranchCode { get; set; }

            public byte IsDefault { get; set; }

            public int? CreatedBy { get; set; }

        }

        [HttpPost("verifyUser")]
        public ActionResult verifyUser(
        [FromForm] branchAccess branchAccess,
        [FromForm] int verifiedby,
        [FromForm] String type)
        {

            try
            {
                var userDetails = _dbContext.Users.Where(x => x.Id.Equals(branchAccess.UserId)).FirstOrDefault();
                userDetails.Type = type;


                userDetails.IsVerified = 1;
                userDetails.VerifiedBy = verifiedby;
                userDetails.VerifiedDate = DateTime.Now;

                var checker = _dbContext.UserBranchAccesses.Where(x => x.Status == 3 && x.UserId == userDetails.Id).FirstOrDefault();
                checker.Status = 1;
                checker.IsDefault = 1;
                checker.ModifiedByUserId = verifiedby;
                checker.ModifiedDate = DateTime.Now;

                var shiftChecker = _dbContext.Shifts.Where(x => x.BranchCode == checker.BranchCode && x.Status == 1).FirstOrDefault();

                if (shiftChecker != null)
                {
                    _dbContext.UserShifts.Add(new UserShift
                    {
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = verifiedby,
                        ShiftId = shiftChecker.Id,
                        UserId = checker.UserId
                    });
                }

                /*_dbContext.UserBranchAccesses.Add(new UserBranchAccess
                {

                    UserId = branchAccess.UserId,
                    BranchCode = branchAccess.BranchCode,
                    Status = 1,
                    IsDefault = branchAccess.IsDefault,
                    CreatedByUserId = branchAccess.CreatedBy,
                    CreatedDate = DateTime.Now,
                   


                });*/
                _dbContext.SaveChanges();
                return Ok("Verified successfully!");
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

        [HttpPost("branches")]
        public ActionResult branches([FromForm] int userId)
        {

            try
            {

                if (_dbContext.Users.Where(x => x.Id.Equals(userId)).Select(x => x.Type).FirstOrDefault() == "Super")
                {

                    return Ok(_dbContext.Branches.Where(x => x.Status == 1 && x.IsDelete == 0).ToList());
                }

                return Ok(_dbContext.UserBranchAccesses.Where(x => x.Status == 1 && x.BranchCodeNavigation.Status == 1 && x.BranchCodeNavigation.IsDelete == 0 && x.UserId.Equals(userId)).
                    Select(x => x.BranchCodeNavigation).
                    ToList());
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

        public partial class SelectedBranch
        {
            public int? Id { get; set; }
            public string? Code { get; set; }
            public string? Alias { get; set; }
            public string? Name { get; set; }
            public string? ContactId { get; set; }
            public string? ContactName { get; set; }
            public string? ContactNumber { get; set; }
            public string? ContactEmail { get; set; }
            public string? Address { get; set; }
            public string? Area { get; set; }
            public string? Region { get; set; }
            public string? MapReference { get; set; }
            public int? GroupId { get; set; }
            public int? CompanyId { get; set; }
            public int? Status { get; set; }

        }

        [HttpPost("branchesUserAccess")]
        public ActionResult branchesUserAccess([FromForm] string? username)
        {

            try
            {
                List<SelectedBranch> selectedBranch = new List<SelectedBranch>();

                if (username == null)
                {
                    return Ok("");
                }

                var user = _dbContext.Users.Where(x => x.Username.Equals(username) || x.EmailAddress.Equals(username))
                    .Select(x => new
                    {
                        x.Id,
                        x.Type
                    })
                    .FirstOrDefault();

                if (user == null)
                {
                    return Ok("");
                }
                else if (user.Type == "Super")
                {
                    return Ok("Super Admin");
                }

                var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(user.Id)).Select(x => x.BranchCode).ToList();

                for (int index = 0; index < userBranch.Count; index++)
                {
                    SelectedBranch branches = new SelectedBranch();

                    var selectedUserBranch = _dbContext.Branches.Where(b => b.Id.Equals(userBranch[index]))
                        .Select(b => new
                        {
                            b.Id,
                            b.Code,
                            b.Alias,
                            b.Name,
                        })
                        .FirstOrDefault();

                    if (selectedUserBranch == null)
                    {
                        return Ok("");
                    }

                    branches.Code = selectedUserBranch.Code;
                    branches.Name = selectedUserBranch.Name;
                    branches.Alias = selectedUserBranch.Alias;
                    branches.Id = selectedUserBranch.Id;

                    selectedBranch.Add(branches);
                }


                return Ok(selectedBranch);
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

        public class userisDefaultBranch
        {
            public dynamic? selectedBranch { get; set; }
            public dynamic? userBranch { get; set; }

        }

        void reloadDefault(int id)
        {



        }

        [HttpPost("isDefaultBranch")]
        public ActionResult isDefaultBranch([FromForm] String? branchCode, [FromForm] int id)
        {

            try
            {


                var setDefaultZ = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) && x.IsDefault == 1).FirstOrDefault();

                if (setDefaultZ != null)
                {
                    setDefaultZ.IsDefault = 0;
                }



                var setDefaultD = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) && x.BranchCode == branchCode).FirstOrDefault();

                if(setDefaultD == null)
                {
                    return StatusCode(202,"Please select branch");
                }

                setDefaultD.IsDefault = 1;

                _dbContext.SaveChanges();


                return Ok();

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

        [HttpPost("isDefaultBranchSecondRun")]
        public ActionResult isDefaultBranchSecondRun([FromForm] int id)
        {

            try
            {

                userisDefaultBranch userisDefaultBranch = new userisDefaultBranch();

                var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) && x.Status == 1).Select(x => new
                {
                    x.BranchCodeNavigation,
                    x.IsDefault

                }).ToList();

                userisDefaultBranch.userBranch = userBranch;

                var defaultUserBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) && x.IsDefault == 1 && x.Status == 1).Select(x => new
                {
                    x.BranchCodeNavigation,
                    x.IsDefault
                }).FirstOrDefault();

                userisDefaultBranch.selectedBranch = defaultUserBranch;


                return Ok(userisDefaultBranch);

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



        [HttpPost("branchPage")]
        public ActionResult branchPage([FromForm] int id, [FromForm] string? search)
        {

            try
            {



                var whatType = _dbContext.Users.Where(x => x.Id == id).Select(x => x.Type).FirstOrDefault();

                if (whatType == "Super")
                {

                    var selectedUserBranch = _dbContext.Branches.Where(x =>
                    x.IsDelete == 0 &&
                    x.Group.Status == "A" &&
                    x.Company.Status.Equals(1) &&
                    x.Group.IsDelete.Equals(0) &&
                    x.Company.IsDelete.Equals(0) &&
                    search == null ?
                    (x.Code.Contains("") || x.Name.Contains("") || x.Alias.Contains("")) :
                    (x.Code.Contains(search) || x.Name.Contains(search) || x.Alias.Contains(search))
                    ).OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                    if (selectedUserBranch.Count == 0)
                    {
                        return Ok("No Data");
                    }

                    return Ok(selectedUserBranch);

                }
                else
                {

                    var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) &&
                    x.BranchCodeNavigation.IsDelete.Equals(0) &&
                    x.BranchCodeNavigation.Group.Status == "A" &&
                    x.BranchCodeNavigation.Company.Status.Equals(1) &&
                    x.BranchCodeNavigation.Group.IsDelete.Equals(0) &&
                    x.BranchCodeNavigation.Company.IsDelete.Equals(0) &&
                    (search == null ?
                    (x.BranchCodeNavigation.Code.Contains("") || x.BranchCodeNavigation.Name.Contains("") || x.BranchCodeNavigation.Alias.Contains("")) :
                    (x.BranchCodeNavigation.Code.Contains(search) || x.BranchCodeNavigation.Name.Contains(search) || x.BranchCodeNavigation.Alias.Contains(search)))
                    ).Select(x => x.BranchCodeNavigation).Distinct().OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                    if (userBranch.Count == 0)
                    {
                        return Ok("No Data");

                    }
                    return Ok(userBranch);

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

        public class CompanyGroup
        {
            public dynamic Group { get; set; }
            public dynamic Company { get; set; }
            public dynamic Settings { get; set; }

            public dynamic branchCurrentSettings { get; set; }

        }

        [HttpPost("companyGroup")]
        public ActionResult companyGroup([FromForm] String? branchCode)
        {

            try
            {
                CompanyGroup companyGroup = new CompanyGroup();

                companyGroup.Group = _dbContext.Groups.Where( x => x.Status == "A" && x.IsDelete == 0).ToList();
                companyGroup.Company = _dbContext.Companies.Where(x => x.Status == 1 && x.IsDelete == 0).ToList();
                companyGroup.Settings = _dbContext.Settings.Where(x => x.Status == 1 && x.IsDelete == 0).ToList();

                if (branchCode != null)
                {
                    companyGroup.branchCurrentSettings = _dbContext.BranchSettings.Where(x => x.Status == 1 && x.BranchCode == branchCode).Select(x=> new { x.SettingId,x.Value }).ToList();
                }

                return Ok(companyGroup);

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

        [HttpPost("addBranch")]
        public ActionResult addBranch([FromForm] Branch branch, [FromForm] String jsonBranchSettings)
        {

            try
            {

                if (_dbContext.Branches.Where(x => x.Code == branch.Code).Count() > 0)
                {
                    return StatusCode(208, "Branch Already Exist!");
                }

                String[] tempback = branch.MapReference.Split('@');

                if (tempback.Length > 1 && tempback[1].Contains(','))
                {
                    String[] temp = tempback[1].Split(',');

                    branch.Longitude = temp[1];
                    branch.Latitude = temp[0];
                }
                branch.Status = 1;
                branch.CreatedDate = DateTime.Now;

                List<BranchSetting> branchSetting = JsonConvert.DeserializeObject<List<BranchSetting>>(jsonBranchSettings)!;

                Shift shift = new Shift();

                shift.CreatedByUserId = branch.CreatedByUserId;
                shift.Name = "Default";
                shift.BranchCode = branch.Code;
                shift.Status = 1;
                shift.CreatedDate = DateTime.Now;

                _dbContext.Shifts.Add(shift);
                _dbContext.Branches.Add(branch);
                _dbContext.SaveChanges();

                for (var index = 0; index < branchSetting.Count; index++)
                {
                    branchSetting[index].BranchCode = branch.Code;
                    _dbContext.BranchSettings.Add(branchSetting[index]);
                }
                 _dbContext.SaveChanges();

                if (_dbContext.Users.Where(x => x.Id.Equals(branch.CreatedByUserId)).Select(x => x.Type).FirstOrDefault() != "Super")
                {
                    UserBranchAccess userBranchAccess = new UserBranchAccess();

                    userBranchAccess.BranchCode = branch.Code;
                    userBranchAccess.UserId = branch.CreatedByUserId.Value;
                    userBranchAccess.Status = 1;
                    userBranchAccess.IsDefault = 0;
                    userBranchAccess.CreatedByUserId = branch.CreatedByUserId.Value;
                    userBranchAccess.CreatedDate = DateTime.Now;

                    _dbContext.UserBranchAccesses.Add(userBranchAccess);
                    _dbContext.SaveChanges();
                }




                return Ok("Successfully added!");

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

        [HttpPost("editBranch")]
        public ActionResult editBranch([FromForm] Branch branch, [FromForm] String jsonBranchSettings)
        {

            try
            {
                var content = _dbContext.Branches.Where(x => x.Code == branch.Code).FirstOrDefault();

                if (content != null && content.Id != branch.Id)
                {
                    return StatusCode(208, "Branch Already Exist!");
                }
                List<String> branchSetting = JsonConvert.DeserializeObject<List<String>>(jsonBranchSettings)!;
                var selectedBranch = _dbContext.Branches.Where(x => x.Id == branch.Id).FirstOrDefault();

                String[] tempback = branch.MapReference.Split('@');

                if (tempback.Length > 1 && tempback[1].Contains(','))
                {
                    String[] temp = tempback[1].Split(',');

                    branch.Longitude = temp[1];
                    branch.Latitude = temp[0];
                }

                branch.CreatedDate = DateTime.Now;

                selectedBranch.Name = branch.Name;
                selectedBranch.Alias = branch.Alias;
                selectedBranch.Code = branch.Code;
                selectedBranch.ContactId = branch.ContactId;
                selectedBranch.ContactName = branch.ContactName;
                selectedBranch.ContactNumber = branch.ContactNumber;
                selectedBranch.ContactEmail = branch.ContactEmail;
                selectedBranch.Address = branch.Address;
                selectedBranch.Area = branch.Area;
                selectedBranch.Region = branch.Region;
                selectedBranch.MapReference = branch.MapReference;
                selectedBranch.GroupId = branch.GroupId;
                selectedBranch.CompanyId = branch.CompanyId;
                selectedBranch.Latitude = branch.Latitude;
                selectedBranch.Longitude = branch.Longitude;

                var bsettings = _dbContext.BranchSettings.Where( x => x.BranchCode == selectedBranch.Code && x.Status == 1).ToList();

                for (var index = 0; index < branchSetting.Count; index++)
                {

                    bsettings[index].Value = branchSetting[index];
                }

                _dbContext.SaveChanges();

                return Ok("Modified successfully!");

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


        [HttpPost("branchesStatus")]
        public ActionResult branchesStatus(
            [FromForm] int userId,
            [FromForm] String branchCode,
            [FromForm] byte branchStatus,
            [FromForm] byte branchIsDelete)
        {

            try
            {

                var selectedBranch = _dbContext.Branches.Where(x => x.Code == branchCode).FirstOrDefault();
                var selectedUserAccessBranch = _dbContext.UserBranchAccesses.Where(x => x.BranchCode == branchCode && x.UserId == 1).FirstOrDefault();

                selectedBranch.Status = branchStatus;
                selectedUserAccessBranch.Status = branchStatus;
                selectedBranch.IsDelete = branchIsDelete;
                selectedBranch.ModifiedDate = DateTime.Now;
                selectedBranch.ModifiedByUserId = userId;

                _dbContext.SaveChanges();

                if (branchIsDelete == 1)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return Ok("Modified Successfully");
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


        [HttpPost("userPage")]
        public ActionResult userPage([FromForm] String? branchCode, [FromForm] int id, [FromForm] String? search, [FromForm] int page)
        {

            try
            {

                if (_dbContext.Users.Where(x => x.Id.Equals(id)).Select(x => x.Type).FirstOrDefault() == "Super")
                {
                  

                    var allUsers = _dbContext.Users.Where(x =>
                    x.IsDeactivated == 0 &&
                    x.Type != "Super" &&
                    x.IsVerified == 1 &&
                    (search == null ? x.IsDeactivated == 0 : (x.Firstname + x.Middlename + x.Lastname).Contains(search))
                    ).OrderBy(x => x.Firstname).OrderByDescending(x => x.Status).Skip(page * 10).Take(10).ToList();



                    return Ok(allUsers);

                }

                var users = _dbContext.UserBranchAccesses.Where(x =>
                x.User.Status == 1 &&
                x.Status ==1 &&
                x.BranchCode == branchCode &&
                x.User.IsDeactivated == 0 &&
                x.User.Id != id &&
                x.User.Type != "Super" &&
                (search == null ? x.User.IsDeactivated == 0 : (x.User.Firstname + x.User.Middlename + x.User.Lastname).Contains(search))).Select(x => new
                {
                    x.User,
                    x.Status,
                }).OrderBy(x => x.User.Firstname).OrderByDescending(x => x.Status).Skip(page * 10).Take(10).ToList();
                //.DistinctBy(x => x.User.Id)
                return Ok(users);

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

        public class userCreateAccountClass
        {
            public byte id { get; set; }
            public String type { get; set; }
            public String branchCode { get; set; }
        }

        [HttpPost("userCreateAccount")]
        public ActionResult userCreateAccount([FromForm] Registration registration, [FromForm] userCreateAccountClass userCreateAccountClass)
        {

            try
            {
                var userChecker = _dbContext.Users.Where(x => x.Username.Equals(registration.Username) && x.IsVerified != 2).FirstOrDefault();

                if (userChecker != null)
                {
                    return StatusCode(208, "Already exist");
                }
                if (registration.Middlename == null)
                {
                    registration.Middlename = "N/A";
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

                user.Type = userCreateAccountClass.type;
                user.Status = 1;
                user.IsVerified = 1;
                user.VerifiedBy = userCreateAccountClass.id;
                user.VerifiedDate = DateTime.Now;
                user.CreatedDate = DateTime.Now;
                user.CreatedByUserId = userCreateAccountClass.id;

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                UserBranchAccess userBranchAccess = new UserBranchAccess();

                userBranchAccess.UserId = user.Id;
                userBranchAccess.BranchCode = userCreateAccountClass.branchCode;
                userBranchAccess.IsDefault = 1;
                userBranchAccess.Status = 1;
                userBranchAccess.CreatedByUserId = userCreateAccountClass.id;
                userBranchAccess.CreatedDate = DateTime.Now;

                // UserShift userShift = new UserShift();

                var shiftChecker = _dbContext.Shifts.Where(x => x.BranchCode == userCreateAccountClass.branchCode && x.Status == 1).FirstOrDefault();

                if (shiftChecker !=  null)
                {
                    _dbContext.UserShifts.Add(new UserShift
                    {
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = userCreateAccountClass.id,
                        ShiftId = shiftChecker.Id,
                        UserId = user.Id
                    });
                }

              


                _dbContext.UserBranchAccesses.Add(userBranchAccess);
                _dbContext.SaveChanges();

                return Ok("Successfully Created!");

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


       /* [HttpPost("userEditAccount")]
        public ActionResult userEditAccount(
            [FromForm] Registration registration,
            [FromForm] int userId,
            [FromForm] String branchId,
            [FromForm] int currentBranchId
            )
        {

            try
            {

                if (currentBranchId != branchId)
                {
                    var isExistBranch = _dbContext.UserBranchAccesses.Where(x => 
                    x.UserId.Equals(registration.Id) && 
                    x.BranchCode == branchId).
                    Select(x => x.BranchCodeNavigation.Name).FirstOrDefault();

                    if (isExistBranch != null)
                    {

                        return StatusCode(202, "User already exist in " + isExistBranch);
                    }
                }

                var selectedUser = _dbContext.Users.Where(x => x.Id.Equals(registration.Id)).FirstOrDefault();

                if (selectedUser.Middlename == null)
                {
                    selectedUser.Middlename = "N/A";
                }
                else
                {
                    selectedUser.Middlename = registration.Middlename.ToUpper();
                }

                selectedUser.Lastname = registration.Lastname.ToUpper();
                selectedUser.Firstname = registration.Firstname.ToUpper();
                selectedUser.Nickname = registration.Nickname.ToUpper();
                selectedUser.EmailAddress = registration.EmailAddress;
                selectedUser.ContactNumber = registration.ContactNumber;

                selectedUser.ModifiedBy = userId;
                selectedUser.ModifiedDate = DateTime.Now;

                var switchBranches = _dbContext.UserBranchAccesses.Where(x => x.BranchId.Equals(currentBranchId) && x.UserId.Equals(registration.Id)).FirstOrDefault();

                switchBranches.Status = 0;
                switchBranches.IsDefault = 0;
                switchBranches.ModifiedBy = userId;
                switchBranches.ModifiedDate = DateTime.Now;

                UserBranchAccess userBranchAccess = new UserBranchAccess();

                userBranchAccess.BranchId = branchId;
                userBranchAccess.UserId = registration.Id.Value;
                userBranchAccess.Status = 1;
                userBranchAccess.CreatedBy = userId;
                userBranchAccess.ModifiedBy = userId;
                userBranchAccess.CreatedDate = DateTime.Now;
                userBranchAccess.ModifiedDate = DateTime.Now;

                _dbContext.UserBranchAccesses.Add(userBranchAccess);

                _dbContext.SaveChanges();


                return Ok("Saved Successfully!");

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

        }*/



        [HttpPost("changeAccountBranchStatus")]
        public ActionResult changeAccountBranchStatus(
                [FromForm] String? branchCode,
                [FromForm] int id,
                [FromForm] int userId,
                [FromForm] byte status
            )
        {

            try
            {
                var user = _dbContext.Users.Where(x => x.Id.Equals(userId)).FirstOrDefault();

                if (user.Type == "Super")
                {

                    var selectedUser = _dbContext.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();
                    var selectedUserBranchAccess = _dbContext.UserBranchAccesses.Where(x => x.User.Id.Equals(id)).ToList();

                    selectedUserBranchAccess.ForEach(x => x.Status = 0);

                    selectedUser.Status = status;
                    selectedUser.ModifiedByUserId = userId;
                    selectedUser.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();
                    return Ok("Modified Successfully!");
                }

                var userAccount = _dbContext.UserBranchAccesses.Where(x => x.UserId == id && x.BranchCode == branchCode).FirstOrDefault();

                userAccount.IsDefault = 0;
                userAccount.Status = status;
                    
                userAccount.ModifiedByUserId = userId;
                userAccount.ModifiedDate = DateTime.Now;

                _dbContext.SaveChanges();

                return Ok("Modified Successfully!");

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

        [HttpPost("deactivateUser")]
        public ActionResult deactivateUser([FromForm] int selectedUserId, [FromForm] int userId, [FromForm] String reason)
        {

            try
            {

                var toDeactivateUser = _dbContext.Users.Where(x => x.Id == selectedUserId).FirstOrDefault();
                toDeactivateUser.IsDeactivated = 1;
                toDeactivateUser.Status = 0;
                toDeactivateUser.DeactivateReason = reason;
                toDeactivateUser.ModifiedDate = DateTime.Now;
                toDeactivateUser.ModifiedByUserId = userId;

                var toDeactivateUserBranchAccess = _dbContext.UserBranchAccesses.Where(x => x.UserId == selectedUserId).ToList();

                if (toDeactivateUserBranchAccess != null)
                {
                    for (int index = 0; index < toDeactivateUserBranchAccess.Count; index++)
                    {
                        toDeactivateUserBranchAccess[index].Status = 0;
                        toDeactivateUserBranchAccess[index].ModifiedByUserId = userId;
                        toDeactivateUserBranchAccess[index].ModifiedDate = DateTime.Now;
                    }
                }

                var toDeactivateUserShift = _dbContext.UserShifts.Where(x => x.UserId == userId).ToList();
                if (toDeactivateUserShift != null)
                {
                    for (int index = 0; index < toDeactivateUserShift.Count; index++)
                    {
                        toDeactivateUserShift[index].Status = 0;
                    }
                }


                   _dbContext.SaveChanges();

                return Ok("successfully Deactivated");

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


        public partial class selectedUserBranchesUserPageClass
        {
            public dynamic UserBranch { get; set; }
            public dynamic SelectedUserBranch { get; set; }
        }

        [HttpPost("selectedUserBranchesUserPage")]
        public ActionResult selectedUserBranchesUserPage(
                [FromForm] int userId,
                [FromForm] int selectedUserId

            )
        {

            try
            {
                if (_dbContext.Users.Where(x => x.Id.Equals(userId)).Select(x => x.Type).FirstOrDefault() == "Super")
                {
                    selectedUserBranchesUserPageClass selectedUserBranchesUserPageClass = new selectedUserBranchesUserPageClass();

                    var userBranch = _dbContext.Branches.Where(x => x.Status == 1 && x.IsDelete == 0).ToList();

                    var selectedUserBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(selectedUserId) && x.Status == 1).Select(x => x.BranchCodeNavigation).ToList();

                    selectedUserBranchesUserPageClass.UserBranch = userBranch;
                    selectedUserBranchesUserPageClass.SelectedUserBranch = selectedUserBranch;

                    return Ok(selectedUserBranchesUserPageClass);
                }
                else
                {
                    selectedUserBranchesUserPageClass selectedUserBranchesUserPageClass = new selectedUserBranchesUserPageClass();

                    var userBranch = _dbContext.UserBranchAccesses.Where(x => x.Status == 1 && x.UserId.Equals(userId)).Select(x => x.BranchCodeNavigation).ToList();

                    var selectedUserBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(selectedUserId) && x.Status == 1).Select(x => x.BranchCodeNavigation).ToList();

                    selectedUserBranchesUserPageClass.UserBranch = userBranch;
                    selectedUserBranchesUserPageClass.SelectedUserBranch = selectedUserBranch;

                    return Ok(selectedUserBranchesUserPageClass);
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

        public partial class editUserShiftClass
        {
            public dynamic userAccess { get; set; }
            public dynamic newUserShift { get; set; }
        }

        [HttpPost("editUserShift")]
        public ActionResult editUserShift(
              [FromForm] int userId,
              [FromForm] int selectedUserId

          )
        {

            try
            {
                if (_dbContext.Users.Where(x => x.Id.Equals(userId)).Select(x => x.Type).FirstOrDefault() == "Super")
                {
   

                    //var userBranch = _dbContext.Branches.Where(x => x.Status == 1 && x.IsDelete == 0).ToList();

                    var allShift = _dbContext.Shifts.Where(x =>  x.Status == 1 && x.Status == 1 && x.IsDelete == 0
                    ).Select(x => new
                    {
                        x.BranchCodeNavigation,
                        x.Id,
                        x.Name,
                        x.Status
                       
                    }).ToList();

                
                   var newUserShift = _dbContext.UserShifts.Where(x =>
                  x.UserId.Equals(selectedUserId) &&
                  x.Status == 1 &&
                  x.Shift.IsDelete == 0 &&
                  x.Shift.Status == 1 ).
                  Select(x => x.Shift).ToList();

                    editUserShiftClass editUserShiftClass = new editUserShiftClass();

                    editUserShiftClass.newUserShift = newUserShift;
                    editUserShiftClass.userAccess = allShift;

                    return Ok(editUserShiftClass);

                }
                else
                {
                    var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(userId) && x.Status == 1).Select(x => x.BranchCodeNavigation).ToList();

                    var allShift = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(userId) && x.Status == 1 && x.BranchCodeNavigation.Status == 1).Select(x => new { 
                    
                        x.BranchCodeNavigation,
                        x.BranchCodeNavigation.Shifts

                    }).ToList();

                    editUserShiftClass editUserShiftClass = new editUserShiftClass();

                    editUserShiftClass.userAccess = allShift;

                    List<Shift> newUserShift = new List<Shift>();
                  

                    for (int index = 0; index < userBranch.Count; index++)
                    {
                        var userShift = _dbContext.UserShifts.Where(x =>
                  x.UserId.Equals(selectedUserId) &&
                  x.Status == 1 &&
                  x.Shift.IsDelete == 0 &&
                  x.Shift.Status == 1 &&
                  x.Shift.BranchCode.Equals(userBranch[index].Code)).
                  Select(x => x.Shift).ToList();

                        for (int index1 = 0; index1 < userShift.Count; index1++)
                        {
                            if (userShift[index1].BranchCode.Equals(userBranch[index].Code))
                            {
                                newUserShift.Add(userShift[index1]);


                            }
                        }

                    }

                 

                    editUserShiftClass.newUserShift = newUserShift;
                    

                    return Ok(editUserShiftClass);
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


        [HttpPost("editUserShiftSearch")]
        public ActionResult editUserShiftSearch(
           [FromForm] int userId,
           [FromForm] String? search

       )
        {

            try
            {
                if (_dbContext.Users.Where(x => x.Id.Equals(userId)).Select(x => x.Type).FirstOrDefault() == "Super")
                {

                    var allShift = _dbContext.Shifts.Where(x => x.Status == 1 && x.Status == 1 && x.IsDelete == 0 &&
                     x.Name.Contains(search??"")
                     ).Select(x => new
                     {
                         x.BranchCodeNavigation,
                         x.Id,
                         x.Name,
                         x.Status

                     }).ToList();

                    return Ok(allShift);
                }
                else
                {

                    var allShift = _dbContext.Shifts.Where(x => 
                    x.BranchCodeNavigation.UserBranchAccesses1.UserId.Equals(userId) &&
                    x.BranchCodeNavigation.UserBranchAccesses1.Status == 1 &&
                    x.BranchCodeNavigation.Status == 1 &&
                    x.BranchCodeNavigation.IsDelete == 0 &&
                    x.Name.Contains(search??"")
                    
                    )
                        .Select(x => new
                        {
                            x.BranchCodeNavigation,
                            x.Id,
                            x.Name,
                            x.Status

                        }).ToList();


                    return Ok(allShift);
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



        [HttpPost("editAccount")]
        public ActionResult editAccount(
                [FromForm] Registration registration,
                [FromForm] int userId,
                [FromForm] String jsonBranchCodes,
                [FromForm] String jsonShifts,
                [FromForm] String type
        )
        {
            try
            {
                var branchCodes = JsonConvert.DeserializeObject<List<String>>(jsonBranchCodes);
                var shifts = JsonConvert.DeserializeObject<List<int>>(jsonShifts);


                var selectedUser = _dbContext.Users.Where(x => x.Id.Equals(long.Parse(registration.Id.ToString()))).FirstOrDefault();

                if (selectedUser.Middlename == null)
                {
                    selectedUser.Middlename = "N/A";
                }
                else
                {
                    selectedUser.Middlename = registration.Middlename.ToUpper();
                }

                selectedUser.Lastname = registration.Lastname.ToUpper();
                selectedUser.Firstname = registration.Firstname.ToUpper();
                selectedUser.Nickname = registration.Nickname.ToUpper();
                selectedUser.EmailAddress = registration.EmailAddress;
                selectedUser.ContactNumber = registration.ContactNumber;
                selectedUser.Type = type;

                selectedUser.ModifiedByUserId = userId;
                selectedUser.ModifiedDate = DateTime.Now;


                var alreadyExistBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(long.Parse(registration.Id.ToString()))).ToList();

                for (int index = 0; index < alreadyExistBranch.Count; index++)
                {
                    alreadyExistBranch[index].Status = 0;
                }
                for (int index = 0; index < branchCodes.Count; index++)
                {
                    var branch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(long.Parse(registration.Id.ToString())) && x.BranchCode == branchCodes[index]).FirstOrDefault();
                    if (branch == null)
                    {
                        UserBranchAccess userBranchAccess = new UserBranchAccess();

                        userBranchAccess.UserId = registration.Id.Value;
                        userBranchAccess.BranchCode = branchCodes[index];
                        userBranchAccess.Status = 1;
                        userBranchAccess.CreatedByUserId = userId;
                        userBranchAccess.CreatedDate = DateTime.Now;
                        userBranchAccess.ModifiedByUserId = userId;
                        userBranchAccess.ModifiedDate = DateTime.Now;
                        _dbContext.UserBranchAccesses.Add(userBranchAccess);

                    }
                    else
                    {
                        branch.Status = 1;
                        branch.ModifiedByUserId = userId;
                        branch.ModifiedDate = DateTime.Now;
                    }
                }

                
                var alreadyExistShift = _dbContext.UserShifts.Where(x => x.UserId.Equals(long.Parse(registration.Id.ToString()))).ToList();

                for (int index = 0; index < alreadyExistShift.Count; index++)
                {
                    alreadyExistShift[index].Status = 0;
                }

                for (int index = 0; index < shifts.Count; index++)
                {
                    var newShift = _dbContext.UserShifts.Where(x => x.UserId.Equals(long.Parse(registration.Id.ToString())) && x.ShiftId == shifts[index]).FirstOrDefault();

                    if (newShift == null)
                    {
                        UserShift userShift = new UserShift();

                        userShift.UserId = registration.Id.Value;
                        userShift.ShiftId = shifts[index];
                        userShift.Status = 1;
                        userShift.CreatedByUserId = userId;
                        userShift.CreatedDate = DateTime.Now;
                        userShift.ModifiedByUserId = userId;
                        userShift.ModifiedDate = DateTime.Now;
                        _dbContext.UserShifts.Add(userShift);
                    }
                    else
                    {
                        newShift.Status = 1;
                        newShift.ModifiedByUserId = userId;
                        newShift.ModifiedDate = DateTime.Now;
                    }
                }

                _dbContext.SaveChanges();

                return Ok("Saved succesfully!");


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
