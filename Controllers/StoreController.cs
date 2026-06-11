using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagementAppService;
using StoreManagementModels;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace WatsonsStoreManagementAPI.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreAppService _appservice;

        public StoreController()
        {
            _appservice = new StoreAppService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<StoreModels>> GetAllBranches()
        {
            var stores = _appservice.GetBranches();
            return Ok(stores);
        }

        [HttpGet("{id:int}")]
        public ActionResult<StoreModels> GetBranchesById(int id)
        {
            var store = _appservice.GetBranches().FirstOrDefault(s => s.BranchID == id);

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpPost]
        public IActionResult CreateBranch([FromBody] Models.StoreViewModel store)
        {
            if (store == null)
            {
                return BadRequest("Branch data is required.");
            }

            var newStore = new StoreManagementModels.StoreModels
            {
                BranchID = store.BranchID,
                BranchName = store.BranchName,
                BranchAddress = store.BranchAddress,
                BranchContact = store.BranchContact,
                BranchIncome = store.BranchIncome
            };

            var created = _appservice.AddBranch(
                newStore.BranchID,
                newStore.BranchName ?? string.Empty,
                newStore.BranchAddress ?? string.Empty,
                newStore.BranchContact ?? string.Empty,
                newStore.BranchIncome
);
            if (!created)
            {
                return Conflict("Branch could not be created.");
            }

            return CreatedAtAction(
                nameof(GetBranchesById),
                new { id = newStore.BranchID },
                newStore);
        }

        [HttpPatch("{id:int}")]
        public IActionResult UpdateBranch(int id, [FromBody] Models.StoreViewModel store)
        {
            if (store == null)
            {
                return BadRequest("Branch data is required.");
            }

            var existingBranch = _appservice.BranchExists(id);

            if (!existingBranch)
            {
                return NotFound();
            }

            var updatedBranch = new StoreManagementModels.StoreModels
            {
                BranchID = id,
                BranchName = store.BranchName,
                BranchAddress = store.BranchAddress,
                BranchContact = store.BranchContact,
                BranchIncome = store.BranchIncome
            };

            var updated = _appservice.UpdateBranch(
                updatedBranch.BranchID,
                updatedBranch.BranchName ?? string.Empty,
                updatedBranch.BranchAddress ?? string.Empty,
                updatedBranch.BranchContact ?? string.Empty,
                updatedBranch.BranchIncome
            );

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Branch could not be updated.");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBranch(int id)
        {
            var existingBranch = _appservice.GetBranches().FirstOrDefault(s => s.BranchID == id);

            if (existingBranch == null)
            {
                return NotFound();
            }

            _appservice.DeleteBranch(id);

            return NoContent();
        }


    }
}
