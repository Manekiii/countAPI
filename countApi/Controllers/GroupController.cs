using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class GroupController : ControllerBase
    {
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public GroupController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        [HttpPost("groupPage")]
        public ActionResult companyPage([FromForm] String? search)
        {

            try
            {

                return Ok(_dbContext.Groups.Where(x => x.IsDelete == 0 && 
                search == null ? x.IsDelete == 0 : x.Name.Contains(search)).
                 OrderBy(x => x.Status == "I").ThenBy(x => x.Name).ToList());

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


        [HttpPost("groupSetStatus")]
        public ActionResult companySetStatus(
           [FromForm] int userId,
           [FromForm] int groupId,
           [FromForm] String status,
           [FromForm] byte isDelete

           )
        {

            try
            {
                var company = _dbContext.Groups.Where(x => x.Id.Equals(groupId)).FirstOrDefault();

                company.Status = status;
                company.ModifiedByUserId = userId;
                company.IsDelete = isDelete;
                company.ModifiedDate = DateTime.Now;

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


        [HttpPost("addGroup")]
        public ActionResult addGroup(
           [FromForm] int userId,
           [FromForm] Group? group
           )
        {

            try
            {
                var checker = _dbContext.Groups.Where(x => x.Code == group.Code).FirstOrDefault();

                if (checker != null)
                {
                    return StatusCode(202, "Already Exist!");
                }

                group.CreatedDate = DateTime.Now;
                group.CreatedByUserId = userId;
                _dbContext.Groups.Add(group);
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

        }

        [HttpPost("editGroup")]
        public ActionResult editGroup(
         [FromForm] int userId,
         [FromForm] Group? group
         )
        {

            try
            {
                var selectedGroup = _dbContext.Groups.Where(x => x.Id == group.Id).FirstOrDefault();
                var checker = _dbContext.Groups.Where(x => x.Code == group.Code).FirstOrDefault();

                if (checker != null && selectedGroup.Code != checker.Code)
                {
                    return Ok("Already Exist!");
                }

                selectedGroup.Code = group.Code;
                selectedGroup.Name = group.Name;
                selectedGroup.ContactId = group.ContactId;
                selectedGroup.ContactName = group.ContactName;
                selectedGroup.ContactNumber = group.ContactNumber;
                selectedGroup.ContactEmail = group.ContactEmail;

                selectedGroup.ModifiedByUserId = userId;
                selectedGroup.ModifiedDate = DateTime.Now;

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

        }


    }
}
