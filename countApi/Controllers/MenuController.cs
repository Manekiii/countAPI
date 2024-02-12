using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static countApi.Controllers.AuthApiController;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class MenuController : ControllerBase
    {


        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public MenuController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public class menuPageClass
        {
            public dynamic branch { get; set; }

            public dynamic menus { get; set; }

        }

        [HttpPost("menuPage")]
        public ActionResult menuPage([FromForm] int id, [FromForm] string? search)
        {

            try
            {
                menuPageClass menuPageClass = new menuPageClass();

                var whatType = _dbContext.Users.Where(x => x.Id == id).Select(x => x.Type).FirstOrDefault();

                if (whatType == "Super")
                {

                    var selectedUserBranch = _dbContext.Branches.Where(x =>
                    x.IsDelete == 0 &&
                    x.Group.Status == "A" &&
                    x.Company.Status.Equals(1) &&
                    x.Group.IsDelete.Equals(0) &&
                    x.Company.IsDelete.Equals(0) &&
                    x.Status == 1 &&
                    search == null ?
                    (x.Code.Contains("") || x.Name.Contains("") || x.Alias.Contains("")) :
                    (x.Code.Contains(search) || x.Name.Contains(search) || x.Alias.Contains(search))
                    ).OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                    if (selectedUserBranch.Count == 0)
                    {
                        menuPageClass.branch = "No Data";
                        return Ok(menuPageClass);
                    }

                    var menus = _dbContext.Menus.Where(x => x.Status == 1 && x.IsDelete == 0 && x.ParentMenuId != 0).ToList();

                    menuPageClass.branch = selectedUserBranch;
                    menuPageClass.menus = menus;

                    return Ok(menuPageClass);

                }
                else
                {

                    var userBranch = _dbContext.UserBranchAccesses.Where(x => x.UserId.Equals(id) &&
                    x.BranchCodeNavigation.IsDelete.Equals(0) &&
                    x.BranchCodeNavigation.Group.Status == "A" &&
                    x.BranchCodeNavigation.Company.Status.Equals(1) &&
                    x.BranchCodeNavigation.Group.IsDelete.Equals(0) &&
                    x.BranchCodeNavigation.Company.IsDelete.Equals(0) &&
                    x.BranchCodeNavigation.Status == 1 &&
                    search == null ?
                    (x.BranchCodeNavigation.Code.Contains("") || x.BranchCodeNavigation.Name.Contains("") || x.BranchCodeNavigation.Alias.Contains("")) :
                    (x.BranchCodeNavigation.Code.Contains(search) || x.BranchCodeNavigation.Name.Contains(search) || x.BranchCodeNavigation.Alias.Contains(search))
                    ).Select(x => x.BranchCodeNavigation).Distinct().OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                    if (userBranch.Count == 0)
                    {
                        menuPageClass.branch = "No Data";
                        return Ok(userBranch);

                    }
                    var menus = _dbContext.Menus.Where(x => x.Status == 1 && x.IsDelete == 0 && x.ParentMenuId != 0).ToList();

                    menuPageClass.branch = userBranch;
                    menuPageClass.menus = menus;

                    return Ok(menuPageClass);

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


        [HttpPost("menuUserAccess")]
        public ActionResult menuUserAccess(
            [FromForm] int menuId,
            [FromForm] String branchCode,
            [FromForm] String? search,
            [FromForm] int page,
            [FromForm] int limit
            )
        {

            try
            {
                var users = _dbContext.UserMenus.Where(x => 
                x.MenuId.Equals(menuId) &&
                x.BranchCode == branchCode &&
                x.Status == 1 &&
                x.User.Type != "Super" &&
                (search == null? (x.User.Firstname + x.User.Middlename + x.User.Middlename).Contains(""): (x.User.Firstname + x.User.Middlename + x.User.Middlename).Contains(search))
                ).Select(x=> new
                {
                    x.User.Firstname,
                    x.User.Middlename,
                    x.User.Lastname,
                    x.UserId,
                    x.Add,
                    x.Edit,
                    x.Delete
                }).Skip(page * limit).Take(limit).ToList();

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

        [HttpPost("menuUser")]
        public ActionResult menuUser(
           [FromForm] String branchCode,
           [FromForm] String? search,
           [FromForm] int page,
           [FromForm] int limit
           )
        {

            try
            {
                var users = _dbContext.UserBranchAccesses.Where(x =>
                x.BranchCode == branchCode &&
                x.Status == 1 &&
                x.User.Type != "Super" &&
                (search == null ? (x.User.Firstname + x.User.Middlename + x.User.Middlename).Contains("") : (x.User.Firstname + x.User.Middlename + x.User.Middlename).Contains(search))
                ).Select(x => new
                {   
                    x.User.Id,
                    x.User.Firstname,
                    x.User.Middlename,
                    x.User.Lastname,
                }).Skip(page * limit).Take(limit).ToList();

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

        [HttpPost("addMenuUser")]
        public ActionResult addMenuUser(
          [FromForm] UserMenu? userMenu
          )
        {

            try
            {
                var checker = _dbContext.UserMenus.Where(x =>
                x.BranchCode == userMenu.BranchCode &&
                x.MenuId.Equals(userMenu.MenuId) &&
                x.UserId.Equals(userMenu.UserId) &&
                x.Status.Equals(1)).FirstOrDefault();

                if (checker != null)
                {
                    return Ok("Already Exist!");
                }
                userMenu.Status = 1;
                userMenu.CreatedDate = DateTime.Now;
                _dbContext.UserMenus.Add(userMenu);
                _dbContext.SaveChanges();

                return Ok("Created successfully!");

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

        [HttpPost("editMenuUser")]
        public ActionResult editMenuUser(
       [FromForm] UserMenu? userMenu
       )
        {

            try
            {
                var checker = _dbContext.UserMenus.Where(x =>
                x.BranchCode == userMenu.BranchCode &&
                x.MenuId.Equals(userMenu.MenuId) &&
                x.UserId.Equals(userMenu.UserId) &&
                x.Status.Equals(1)).FirstOrDefault();

                checker.CreatedDate = DateTime.Now;

                checker.Add = userMenu.Add;
                checker.Edit = userMenu.Edit;
                checker.Delete = userMenu.Delete;

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

        [HttpPost("statustMenuUser")]
        public ActionResult statustMenuUser(
       [FromForm] int userId,
       [FromForm] int selectedUserId,
       [FromForm] String branchCode,
       [FromForm] int menuId
       )
        {

            try
            {

                var checker = _dbContext.UserMenus.Where(x =>
               x.BranchCode == branchCode &&
               x.MenuId.Equals(menuId) &&
               x.UserId.Equals(selectedUserId) &&
               x.Status.Equals(1)).FirstOrDefault();

                checker.Status = 0;
                checker.ModifiedDate = DateTime.Now;
                checker.ModifiedByUserId = userId;

                _dbContext.SaveChanges();

                return Ok("Deleted successfully!");

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
