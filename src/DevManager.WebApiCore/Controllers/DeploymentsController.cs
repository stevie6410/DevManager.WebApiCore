using Microsoft.AspNetCore.Mvc;

namespace DevManager.WebApiCore.Controllers
{
    [Route("api/[controller]")]
    public class DeploymentsController : Controller
    {

        [HttpGet]
        public string Get()
        {
            return "Hello AspNetCore!";
        }

    }
}