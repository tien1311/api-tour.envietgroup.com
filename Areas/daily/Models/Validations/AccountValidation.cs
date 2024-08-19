using api_tourgo.envietgroup.com.Areas.Authenticate.Models.Requests;
using FluentValidation;

namespace pi_tourgo.envietgroup.com.Areas.Authenticate.Validations
{

    public class AccountValidator : AbstractValidator<AccountRequest>
    {
        IConfiguration _configuration;
        public AccountValidator(IConfiguration configuration)
        {
            _configuration = configuration;
            //MasterMeta
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName Not Empty").Must((UserName) => IsUserNameValid(UserName)).WithMessage("Does not contain special characters");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password Not Empty").Must((Password) => IsPasswordValid(Password)).WithMessage("Does not contain special characters ");
        }
        public bool IsUserNameValid(string UserName)
        {
            
            return !UserName.Contains("'");
        }
        public bool IsPasswordValid(string Password)
        {
          
            return !Password.Contains("'");
        }
    }

}
