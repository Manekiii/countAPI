using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Object = countApi.Models.Object;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class CustomWidgetController : ControllerBase
    {

        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public CustomWidgetController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        [HttpPost("customWidgetPage")]
        public ActionResult customWidgetPage([FromForm] int id, [FromForm] string? search)
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
                    x.Status == 1 &&
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
                    x.BranchCodeNavigation.Status == 1 &&
                    search == null ?
                    (x.BranchCodeNavigation.Code.Contains("") || x.BranchCodeNavigation.Name.Contains("") || x.BranchCodeNavigation.Alias.Contains("")) :
                    (x.BranchCodeNavigation.Code.Contains(search) || x.BranchCodeNavigation.Name.Contains(search) || x.BranchCodeNavigation.Alias.Contains(search))
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


        [HttpPost("branchWidget")]
        public ActionResult branchWidget(
            [FromForm] String branchCode)
        {

            try
            {

                var brachWidget = _dbContext.Objects.Where(x=> 
                x.BranchCode == branchCode &&
                x.IsDelete == 0
                ).OrderBy( x => x.Sort).ToList();

                if(brachWidget.Count == 0)
                {
                    return Ok("No Data");
                }

                return Ok(brachWidget);

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

        [HttpPost("addWidget")]
        public ActionResult addWidget(
         [FromForm] Object? objects)
        {

            try
            {

                var checker = _dbContext.Objects.Where( x=> x.Alias == objects.Alias && x.Name == objects.Name && x.BranchCode == objects.BranchCode && x.IsDelete == 0).FirstOrDefault();

                if(checker != null)
                {
                    return StatusCode(202,"Already created!");
                }

                var sorting = _dbContext.Objects.Where(x => x.BranchCode == objects.BranchCode && x.IsDelete == 0).ToList();


                objects.Sort = sorting.Count + 1;
                objects.CreatedDate = DateTime.Now;
                _dbContext.Objects.Add(objects);
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


        [HttpPost("editWidget")]
        public ActionResult editWidget(
      [FromForm] Object? objects)
        {

            try
            {

                var checker = _dbContext.Objects.Where(x => x.Alias == objects.Alias && x.IsDelete == 0).FirstOrDefault();

                if (checker != null && checker.Id != objects.Id)
                {
                    return StatusCode(202, "Already created!");
                }

                var selectedObjects = _dbContext.Objects.Where(x => x.Id.Equals(objects.Id)).FirstOrDefault();

                selectedObjects.Form = objects.Form;
                selectedObjects.Alias = objects.Alias;
                selectedObjects.Name = objects.Name;
                selectedObjects.Placeholder = objects.Placeholder;
                selectedObjects.InputType = objects.InputType;
                selectedObjects.Status = objects.Status;
                selectedObjects.IsRequired = objects.IsRequired;
                selectedObjects.ScanCapable = objects.ScanCapable;
                selectedObjects.ModifiedByUserId = objects.ModifiedByUserId;
                selectedObjects.ModifiedDate = DateTime.Now;

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



        [HttpPost("setStatusWidget")]
        public ActionResult setStatusWidget(
      [FromForm] byte status,
      [FromForm] int widgetId,
      [FromForm] int userId,
      [FromForm] byte isDelete)
        {

            try
            {
                var widget = _dbContext.Objects.Where(x => x.Id.Equals(widgetId)).FirstOrDefault();

                widget.ModifiedByUserId = userId;
                
                widget.ModifiedDate = DateTime.Now;

                if (isDelete == 1)
                {
                    widget.Status = 0;
                    widget.IsDelete = isDelete;
                }
                else
                {
                    widget.Status = status;
                }

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


        [HttpPost("positionWidget")]
        public ActionResult positionWidget(
    [FromForm] String branchCode,
    [FromForm] int oldIndex,
    [FromForm] int newIndex
    )
        {

            try
            {

                var items = _dbContext.Objects.Where(x => x.BranchCode == branchCode && x.IsDelete == 0).OrderBy(x=>x.Sort).ToList();

                
                var temp = items[oldIndex];
                items.RemoveAt(oldIndex);

                items.Insert(newIndex, temp);

                for (int index = 0; index < items.Count; index++)
                {
                    items[index].Sort = index + 1;
                }


                _dbContext.SaveChanges();

                return Ok("Position switched successfully!");

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
