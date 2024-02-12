using countApi.Models;
using elacoreapi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace countApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class TransactionController : ControllerBase
    {
        private readonly CountdbContext _dbContext;
        private readonly IConfiguration _configuration;
        public TransactionController(CountdbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public class transactionPageClass
        {
            public dynamic userShift { get; set; }

            public dynamic customWidget { get; set; }

            public dynamic branchSettings { get; set; }
        }

        [HttpPost("transactionPage")]
        public ActionResult transactionPage(
       [FromForm] int userId,
       [FromForm] String? branchCode
       )
        {

            try
            {
                transactionPageClass transactionPageClass = new transactionPageClass();

                var userShift = _dbContext.UserShifts.Where(x => 
                x.UserId.Equals(userId) &&
                x.Shift.BranchCode == branchCode &&
                x.Status == 1 &&
                x.Shift.Status == 1).Select(x => new
                {
                    x.Shift,
                    x.Id
                }).ToList();

                var customWidget = _dbContext.Objects.Where(x => x.BranchCode == branchCode && x.Status == 1 && x.IsDelete == 0).OrderBy(x=>x.Sort).ToList();
                var branchSettings = _dbContext.BranchSettings.Where(x =>x.BranchCode == branchCode && x.Status.Equals(1)).Select(x => new { x.Value, x.SettingId }).ToList();

                transactionPageClass.userShift = userShift;
                transactionPageClass.customWidget = customWidget;
                transactionPageClass.branchSettings = branchSettings;

                return Ok(transactionPageClass);

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

        [HttpPost("itemCodeScanDesc")]
        public ActionResult itemCodeScanDesc(
     [FromForm] string? code,
     [FromForm] String? branchCode
     )
        {

            try
            {
                var settings = _dbContext.VBranchSettings.Where(x => x.BranchCode == branchCode).Select(x => new { x.SettingId, x.Value }).ToList();

                if (settings.Where(x => x.SettingId == 1).FirstOrDefault().Value.ToUpper() == "YES")
                {
                    var checker = _dbContext.Items.Where(x => x.BranchCode == branchCode && x.Code == code && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                    if (checker != null)
                    {
                        return Ok(checker.Description);
                    }
                    else
                    {
                        return Ok("Not Maintained");
                    }

                }
                else
                {
                    return Ok("");
                }

               /* if (settings.Where(x => x.SettingId == 2).FirstOrDefault().Value.ToUpper() == "YES")
                {
                    var checker = _dbContext.Locations.Where(x => x.BranchCode == transaction.BranchCode && x.Code == transaction.LocationCode && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                    if (checker == null)
                    {
                        return StatusCode(208, "Location does not exist!");
                    }

                }*/

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

        [HttpPost("locationCodeScanDesc")]
        public ActionResult locationCodeScanDesc(
   [FromForm] string? code,
   [FromForm] String? branchCode
   )
        {

            try
            {
                var settings = _dbContext.VBranchSettings.Where(x => x.BranchCode == branchCode).Select(x => new { x.SettingId, x.Value }).ToList();

                if (settings.Where(x => x.SettingId == 2).FirstOrDefault().Value.ToUpper() == "YES")
                {
                    var checker = _dbContext.Locations.Where(x => x.BranchCode == branchCode && x.Code == code && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                    if (checker != null)
                    {
                        return Ok(checker.Description);
                    }
                    else
                    {
                        return Ok("Not Maintained");
                    }

                }
                else
                {
                    return Ok("");
                }

                /* if (settings.Where(x => x.SettingId == 2).FirstOrDefault().Value.ToUpper() == "YES")
                 {
                     var checker = _dbContext.Locations.Where(x => x.BranchCode == transaction.BranchCode && x.Code == transaction.LocationCode && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                     if (checker == null)
                     {
                         return StatusCode(208, "Location does not exist!");
                     }

                 }*/

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



        [HttpPost("onlineTransaction")]
        public ActionResult onlineTransaction(
       [FromForm] Transaction transaction,
       [FromForm] String jsontransactionObjectValue
       )
        {

            try
            {
                var settings = _dbContext.VBranchSettings.Where(x => x.BranchCode == transaction.BranchCode).Select(x => new {x.SettingId,x.Value}).ToList();

                if (settings.Where(x=>x.SettingId == 1).FirstOrDefault().Value.ToUpper() == "YES")
                {
                    var checker = _dbContext.Items.Where(x => x.BranchCode == transaction.BranchCode && x.Code == transaction.ItemCode && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                    if(checker == null)
                    {
                        return StatusCode(208, "Item does not exist!");
                    }
                    
                }

                if (settings.Where(x => x.SettingId == 2).FirstOrDefault().Value.ToUpper() == "YES")
                {
                    var checker = _dbContext.Locations.Where(x => x.BranchCode == transaction.BranchCode && x.Code == transaction.LocationCode && x.Status == 1 && x.IsDelete == 0).FirstOrDefault();

                    if (checker == null)
                    {
                        return StatusCode(208, "Location does not exist!");
                    }

                }


                List<TransactionObjectValue> transactionObjectValue = JsonConvert.DeserializeObject<List<TransactionObjectValue>>(jsontransactionObjectValue)!;

                if (transaction.ScanDate == null)
                {
                    transaction.ScanDate = DateTime.Now;
                }

                transaction.CreatedDate = DateTime.Now;
                _dbContext.Transactions.Add(transaction);
                _dbContext.SaveChanges();

                for(int index = 0; index < transactionObjectValue.Count; index++)
                {
                    transactionObjectValue[index].CreatedDate = DateTime.Now;
                    transactionObjectValue[index].TransactionId = transaction.Id;
                    _dbContext.TransactionObjectValues.Add(transactionObjectValue[index]);  
                }

                _dbContext.SaveChanges();

                return Ok("Transaction successfull!");


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


        [HttpPost("forceTransaction")]
        public ActionResult forceTransaction(
[FromForm] Transaction transaction,
[FromForm] String jsontransactionObjectValue
)
        {

            try
            {


                List<TransactionObjectValue> transactionObjectValue = JsonConvert.DeserializeObject<List<TransactionObjectValue>>(jsontransactionObjectValue)!;

                if (transaction.ScanDate == null)
                {
                    transaction.ScanDate = DateTime.Now;
                }

                transaction.CreatedDate = DateTime.Now;
                _dbContext.Transactions.Add(transaction);
                _dbContext.SaveChanges();

                for (int index = 0; index < transactionObjectValue.Count; index++)
                {
                    transactionObjectValue[index].CreatedDate = DateTime.Now;
                    transactionObjectValue[index].TransactionId = transaction.Id;
                    _dbContext.TransactionObjectValues.Add(transactionObjectValue[index]);
                }

                _dbContext.SaveChanges();

                return Ok("Transaction successfull!");


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

        public class scanCodeClass
        {
            public dynamic Inventories { get; set; }

            public dynamic InventoryObjectValues { get; set; }

        }

        [HttpPost("scanCode")]
        public ActionResult scanCode(
            [FromForm] String? scanCode,
            [FromForm] String branchCode
            )
        {

            try
            {

                if(scanCode == null)
                {
                    return StatusCode(202, "Scan code is empty");
                }

                scanCodeClass scanCodeClass = new scanCodeClass();

                var scanned = _dbContext.Inventories.Where(x => x.ScanCode == scanCode && x.BranchCode == branchCode ).FirstOrDefault();

                if (scanned == null)
                {
                    return StatusCode(202, "Code didn't exist!");
                }

                var objvalue = _dbContext.InventoryObjectValues.Where(x => x.InventoryId == scanned.Id).ToList();

                scanCodeClass.Inventories = scanned;
                scanCodeClass.InventoryObjectValues = objvalue;

                return Ok(scanCodeClass);

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
