using api_tourgo.envietgroup.com.Areas.Authenticate.Models;
using api_tourgo.envietgroup.com.Areas.Authenticate.Services.Abstraction;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_tourgo.envietgroup.com.Areas.Authenticate.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private string Server;
        private const string keySha256 = "6e5cace4233fd50f5c44f361384781e7f672c78ebcdef37678e65706c9a0a78b";

        public AccountService(IConfiguration configuration)
        {
             _configuration = configuration;
             Server = _configuration.GetConnectionString("SQL_EV_2_MAIN");
        }
        public async Task<AccountResponse> Authenticate(string UserName, string Password)
        {
            AccountResponse result = new AccountResponse();
            try
            {
                string sql = " select * from ApiCompanyName where UserName=@UserName and Password=@Password";
                using (var conn = new SqlConnection(Server))
                {
                    var param = new
                    {
                        UserName = UserName,
                        Password = Password
                    };
                    result = await conn.QueryFirstAsync<AccountResponse>(sql, param, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                }
                if (result != null)
                {
                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(keySha256);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                         {
                            new Claim(ClaimTypes.Name, UserName),
                            new Claim("username", UserName),
                            new Claim("AgentCode", result.AgentCode)

                        }),
                        Expires = DateTime.UtcNow.AddHours(3),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    result.SetToken(tokenHandler.WriteToken(token));
                    result.Expires = DateTime.Now.AddHours(3);

                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

    }
}
