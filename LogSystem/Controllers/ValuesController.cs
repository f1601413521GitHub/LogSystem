using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LogSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [Route("[action]")]
        [HttpGet]
        public string Index()
        {
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            _logger.LogTrace($"This trace log from {controllerName}");
            _logger.LogDebug($"This debug log from {controllerName}");
            _logger.LogInformation($"This information log from {controllerName}");
            _logger.LogWarning($"This warning log from {controllerName}");
            _logger.LogError($"This error log from {controllerName}");
            _logger.LogCritical($"This critical log from {controllerName}");
            return $"this {controllerName}, {actionName}";
        }

        [Route("[action]")]
        [HttpPost]
        public string Error()
        {
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            throw new Exception($"this {controllerName}, {actionName}");
        }

        [Route("[action]")]
        [HttpPost]
        public void TestNLogWriteToDb()
        {
            try
            {
                int zero = 0;
                int result = 5 / zero;
            }
            catch (DivideByZeroException ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "databaseLogger" logger from our NLog.config file
                //Logger logger = LogManager.GetLogger("databaseLogger");

                //// add custom message and pass in the exception
                _logger.LogError(ex, "Whoops!");
                throw new Exception(ex.Message, ex);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public TestModel TestModelBinding([FromBody]TestModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            else if (model.Id.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(model.Id));
            }
            else if (model.Amount == 12345)
            {
                string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
                string actionName = ControllerContext.RouteData.Values["action"].ToString();
                throw new Exception($"this {controllerName}, {actionName}");
            }
            else
            {
                model = new TestModel()
                {
                    Id = Guid.NewGuid(),
                    CreateOnDate = DateTime.Now,
                    ProductName = "ABC",
                    Amount = 12345m
                };
                string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            }

            return model;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            int max = 5;
            if (id > max)
            {
                //throw new Exception($"ERROR: id={id} > {max}, timestamp: {((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()}");
                throw new Exception($"ERROR: id={id} > {max}, currentTime: {DateTime.Now.ToString("HH:mm:ss.fff")}");
            }
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
