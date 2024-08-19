using api_tourgo.envietgroup.com.Areas.Authenticate.Models;
using api_tourgo.envietgroup.com.Areas.Authenticate.Models.Requests;
using api_tourgo.envietgroup.com.Areas.Authenticate.Services.Abstraction;
using api_tourgo.envietgroup.com.Shared.Interfaces;
using api_tourgo.envietgroup.com.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pi_tourgo.envietgroup.com.Areas.Authenticate.Validations;
using SecuringWebApiUsingApiKey.Attributes;
using System.Net;

namespace api_tourgo.envietgroup.com.Areas.Authenticate.Controllers
{
    [ApiKey]
    [Authorize]
    [Route("daily/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        IAccountService _accModel;
        IConfiguration _configuration;
        public AccountController(IAccountService accModel, IConfiguration configuration)
        {
            _accModel = accModel;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AccountRequest request)
        {
            IReturnObject returnObject = new ReturnObject();
            var validator = new AccountValidator(_configuration);
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                Task<AccountResponse> account = _accModel.Authenticate(request.UserName, request.Password);
                string token = string.Empty;
                try
                {
                    returnObject.result = account.Result;
                    token = account.Result.GetToken();
                    account.Result.Token = token;
                }
                catch (Exception)
                {
                    AccountResponse reponseAuthenticate = new AccountResponse();
                    returnObject.status = HttpStatusCode.BadRequest;
                    returnObject.message = "Fail";
                    reponseAuthenticate.Message = "Incorrect Username or Password !";
                    returnObject.result = reponseAuthenticate;
                }

                Response.Headers.Add("Token", token);
            }
            else
            {
                AccountResponse reponseAuthenticate = new AccountResponse();
                returnObject.status = HttpStatusCode.BadRequest;
                returnObject.message = "Fail";
                reponseAuthenticate.Message = validationResult.Errors[0].ToString();
                returnObject.result = reponseAuthenticate;

            }

            return Ok(returnObject);
        }

    }
}
