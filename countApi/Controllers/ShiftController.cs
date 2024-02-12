using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class ShiftController : ControllerBase
    {
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public ShiftController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public partial class shiftPageShift
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public String? branchCode { get; set; }

            public byte Status { get; set; }

            public string branchName { get; set; }
        }

        [HttpPost("shiftPage")]
        public ActionResult shiftPage(
        [FromForm] int userId,
        [FromForm] String? branchCode,
        [FromForm] String? search,
        [FromForm] String? filter
        )
        {

            try
            {
                List<shiftPageShift> shiftPageShift = new List<shiftPageShift>();

                var isAdmin = _dbContext.Users.Where(x => x.Id.Equals(userId)).FirstOrDefault();

                if (isAdmin != null && isAdmin.Type == "Super")
                {
                    var shift = _dbContext.Shifts.Where(x =>
                    x.IsDelete == 0 &&
                    (filter == null ? x.BranchCode.Contains("") : x.BranchCode == filter) &&
                    (search == null ? x.Name.Contains("") : x.Name.Contains(search))).OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                    for (int index = 0; index < shift.Count; index++)
                    {
                        var branchName = _dbContext.Branches.Where(x => x.Code == shift[index].BranchCode).Select(x => x.Name).FirstOrDefault();

                        shiftPageShift.Add(new shiftPageShift
                        {
                            Id = shift[index].Id,
                            Name = shift[index].Name,
                            branchCode = shift[index].BranchCode,
                            Status = shift[index].Status,
                            branchName = branchName
                        });
                    }

                    return Ok(shiftPageShift);
                }

                var branchShift = _dbContext.Shifts.Where(x =>
                x.BranchCode == branchCode &&
                x.IsDelete.Equals(0) &&
                (filter == null ? x.BranchCode.Contains("") : x.BranchCode == filter) &&
                (search == null ? x.Name.Contains("") : x.Name.Contains(search))
                ).OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList();

                for (int index = 0; index < branchShift.Count; index++)
                {
                    var branchName = _dbContext.Branches.Where(x => x.Code.Equals(branchShift[index].BranchCode)).Select(x => x.Name).FirstOrDefault();

                    shiftPageShift.Add(new shiftPageShift
                    {
                        Id = branchShift[index].Id,
                        Name = branchShift[index].Name,
                        branchCode = branchShift[index].BranchCode,
                        Status = branchShift[index].Status,
                        branchName = branchName
                    });
                }

                return Ok(shiftPageShift);

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

        [HttpPost("branch")]
        public ActionResult branch(
        [FromForm] int userId
        )
        {

            try
            {

                var isAdmin = _dbContext.Users.Where(x => x.Id.Equals(userId)).FirstOrDefault();

                if (isAdmin.Type == "Super")
                {
                    return Ok(_dbContext.Branches.Where(x => x.Status == 1 && x.IsDelete == 0).ToList());
                }
                else
                {
                    return Ok(_dbContext.UserBranchAccesses.Where(x =>
                    x.Status == 1 &&
                    x.BranchCodeNavigation.IsDelete == 0 &&
                    x.BranchCodeNavigation.Status == 1 &&
                    x.UserId.Equals(userId)).Select(x => x.BranchCodeNavigation).ToList());
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

        [HttpPost("addShift")]
        public ActionResult addShift(
          [FromForm] Shift? shift
          )
        {

            try
            {

                var checker = _dbContext.Shifts.Where(x => x.Name == shift.Name && x.BranchCode == shift.BranchCode).FirstOrDefault();

                if (checker != null)
                {
                    return Ok("Already exist!");
                }

                shift.CreatedDate = DateTime.Now;
                _dbContext.Shifts.Add(shift);
                _dbContext.SaveChanges();

                return Ok("Added Successfully!");

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

        [HttpPost("editShift")]
        public ActionResult editShift(
     [FromForm] Shift? shift
     )
        {

            try
            {
                var selectedShift = _dbContext.Shifts.Where(x => x.Id.Equals(shift.Id)).FirstOrDefault();
                var checker = _dbContext.Shifts.Where(x => x.Name == selectedShift.Name).FirstOrDefault();

                if (selectedShift.Name != checker.Name && checker != null)
                {
                    return Ok("Already Exist!");
                }

                selectedShift.Name = shift.Name;
                selectedShift.ModifiedByUserId = shift.ModifiedByUserId;
                selectedShift.ModifiedDate = DateTime.Now;


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

        public partial class UserShiftPage
        {
            public dynamic userShift { get; set; }
            public dynamic user { get; set; }

        }

        [HttpPost("userShiftPage")]
        public ActionResult userShiftPage(
     [FromForm] String? branchCode,
     [FromForm] int shiftId
     )
        {

            try
            {

                UserShiftPage userShiftPage = new UserShiftPage();

                var userShift = _dbContext.UserShifts.Where(x => x.ShiftId.Equals(shiftId) &&
                x.User.IsDeactivated == 0 &&
                x.User.Status == 1
                ).Select(x => new
                {
                    x.User,
                    x.Status
                }).Skip(0 * 10).Take(10).ToList();

                userShiftPage.userShift = userShift;

                var user = _dbContext.UserBranchAccesses.Where(x =>
                x.BranchCode == branchCode &&
                x.User.IsDeactivated == 0 &&
                x.User.Status == 1 &&
                x.User.Type == "User"
                ).Select(x => x.User).Skip(0 * 10).Take(10).ToList();

                userShiftPage.user = user;

                return Ok(userShiftPage);

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


        [HttpPost("usersShift")]
        public ActionResult usersShift(
            [FromForm] String? search,
            [FromForm] int shiftId,
 [FromForm] int limit,
 [FromForm] int page,
 [FromForm] int? branchId
 )
        {

            try
            {
                var userShift = _dbContext.UserShifts.Where(x => x.ShiftId.Equals(shiftId) &&
                               x.User.IsDeactivated == 0 &&
                               x.User.Status == 1 &&
                               (search == null ? x.User.Firstname.Contains("") :
                               (x.User.Firstname + x.User.Middlename + x.User.Lastname).Contains(search))
                               ).Select(x => new
                               {
                                   x.User,
                                   x.Status
                               }).OrderBy(x => x.Status == 0).ThenBy(x => x.User.Firstname).Skip(page * 10).Take(10).ToList();


                return Ok(userShift);

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

        [HttpPost("users")]
        public ActionResult users(
            [FromForm] String? search,
 [FromForm] int page,
 [FromForm] int limit,
 [FromForm] String? branchCode
 )
        {

            try
            {

                var user = _dbContext.UserBranchAccesses.Where(x =>
                x.BranchCode == branchCode &&
                x.User.IsDeactivated == 0 &&
                x.User.Status == 1 &&
                x.User.Type == "User" &&
                (search == null ? x.User.Firstname.Contains("") :
                (x.User.Firstname + x.User.Middlename + x.User.Lastname).Contains(search))
                ).OrderBy(x => x.User.Firstname)
                .Select(x => x.User).Skip(page * limit).Take(limit).Distinct().ToList();


                return Ok(user);

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

        [HttpPost("addUsersShift")]
        public ActionResult addUsersShift(
            [FromForm] int userId,
            [FromForm] int shiftId,
            [FromForm] int createdByUserId
        )
        {

            try
            {

                var checker = _dbContext.UserShifts.Where(x => x.UserId.Equals(userId) && x.ShiftId.Equals(shiftId)).FirstOrDefault();

                if (checker != null)
                {
                    return Ok("Already exist!");
                }

                _dbContext.UserShifts.Add(new UserShift
                {
                    Status = 1,
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = createdByUserId,
                    ShiftId = shiftId,
                    UserId = userId
                });

                _dbContext.SaveChanges();


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

        [HttpPost("shiftStatus")]
        public ActionResult shiftStatus(
            [FromForm] int userId,
            [FromForm] int shiftId,
            [FromForm] byte status,
            [FromForm] byte isDelete
)
        {

            try
            {

                var shift = _dbContext.Shifts.Where(x => x.Id.Equals(shiftId)).FirstOrDefault();

                shift.Status = status;
                shift.ModifiedByUserId = userId;
                shift.ModifiedDate = DateTime.Now;
                shift.IsDelete = isDelete;

                _dbContext.SaveChanges();

                if (isDelete == 1)
                {
                    return Ok("Deleted Successfully!");
                }

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


        [HttpPost("userShiftStatus")]
        public ActionResult userShiftStatus(
    [FromForm] int userId,
    [FromForm] int shiftId,
    [FromForm] int selectedUserId,
    [FromForm] byte status
)
        {

            try
            {

                var shift = _dbContext.UserShifts.Where(x => x.UserId.Equals(selectedUserId) && x.ShiftId.Equals(shiftId)).FirstOrDefault();

                shift.Status = status;
                shift.ModifiedByUserId = userId;
                shift.ModifiedDate = DateTime.Now;

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


    }
}
