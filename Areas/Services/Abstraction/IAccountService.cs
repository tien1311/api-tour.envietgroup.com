using api_tourgo.envietgroup.com.Areas.Authenticate.Models;
using api_tourgo.envietgroup.com.Shared.Interfaces;


namespace api_tourgo.envietgroup.com.Areas.Authenticate.Services.Abstraction
{
    public interface IAccountService : IBaseService
    {
        Task<AccountResponse> Authenticate(string UserName, string PassWord);
    }
}
