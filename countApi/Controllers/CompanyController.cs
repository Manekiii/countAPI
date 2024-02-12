using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class CompanyController : ControllerBase
    {

        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public CompanyController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        [HttpPost("companyPage")]
        public ActionResult companyPage([FromForm] String? search)
        {

            try
            {

                return Ok(_dbContext.Companies.Where(x => x.IsDelete == 0 && 
                search == null? x.Name.Contains(""): x.Name.Contains(search!)).
                OrderBy(x => x.Status == 0).ThenBy(x => x.Name).ToList());

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

        [HttpPost("companySetStatus")]
        public ActionResult companySetStatus(
            [FromForm] int userId,
            [FromForm] int companyId,
            [FromForm] byte status,
            [FromForm] byte isDelete

            )
        {

            try
            {
                var company = _dbContext.Companies.Where(x => x.Id.Equals(companyId)).FirstOrDefault();

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



        [HttpPost("addCompany")]
        public ActionResult addCompany(
           [FromForm] int userId,
           [FromForm] Company? company
           )
        {

            try
            {
                var checker = _dbContext.Companies.Where(x => x.Code == company.Code).FirstOrDefault();

                if (checker != null)
                {
                    return StatusCode(202,"Already Exist!");
                }

                company.CreatedDate = DateTime.Now;
                company.CreatedByUserId = userId;
                company.Status = 1;
                _dbContext.Companies.Add(company);
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

        [HttpPost("editCompany")]
        public ActionResult editCompany(
          [FromForm] int userId,
          [FromForm] Company? company
          )
        {

            try
            {
                var selectedCompany = _dbContext.Companies.Where(x => x.Id == company.Id).FirstOrDefault();
                var checker = _dbContext.Companies.Where(x => x.Code == company.Code).FirstOrDefault();

                if (checker != null && selectedCompany.Code != checker.Code)
                {
                    return Ok("Already Exist!");
                }

                selectedCompany.Code = company.Code;
                selectedCompany.Name = company.Name;
                selectedCompany.ContactNumber = company.ContactNumber;

                selectedCompany.ModifiedByUserId = userId;
                selectedCompany.ModifiedDate = DateTime.Now;

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
