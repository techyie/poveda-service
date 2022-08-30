using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Text;
using MyCampusV2.Common;
using MyCampusV2.IServices;
using Microsoft.Extensions.Options;
using MyCampusV2.Common.ViewModels.UserViewModel;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Helpers.Encryption;
using System.Threading.Tasks;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.Helpers;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        protected readonly IAuditTrailService _audit;

        public TokenController(IOptions<AppSettings> appSettings, IUserService userService, IAuditTrailService audit)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _audit = audit;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel login)
        {
            IActionResult response = Unauthorized();
            if (!login.Username.Equals("admin"))
            {
                var validateActiveDir = _userService.ValidateAdAccount(login.Username, login.Password, AdSettingsStat.Domain, AdSettingsStat.Url);

                if (!validateActiveDir.ValidLogin)
                    return BadRequest(new { Constants.LOGIN_INVALID_AD });

                Logger _logging = new Logger();

                _logging.Activity("AD Email: " + validateActiveDir.mail.ToString());

                var person = _userService.VerifyEmail(validateActiveDir.mail);

                if (person == null)
                    return BadRequest(new { Constants.LOGIN_INVALID_EMAIL });

                var user = _userService.VerifyUserAccountRole(person.Person_ID);

                if (user == null)
                {
                    try
                    {
                        personEntity personemployee = _userService.GetPersonCampusId(person.Person_ID);

                        userEntity nuser = new userEntity();
                        nuser.Person_ID = personemployee.Person_ID;
                        nuser.Person_Type = personemployee.Person_Type;
                        //nuser.PersonEntity.CampusEntity.Campus_ID = person.PositionEntity.DepartmentEntity.Campus_ID;
                        nuser.User_Name = login.Username;
                        nuser.User_Password = Encryption.Encrypt(login.Password);
                        nuser.Role_ID = 2;
                        nuser.Date_Time_Added = DateTime.Now;
                        nuser.Added_By = 1;
                        nuser.Last_Updated = DateTime.Now;
                        nuser.Updated_By = 1;
                        nuser.IsActive = true;
                        nuser.ToDisplay = true;

                        //userRoleEntity nuserrole = new userRoleEntity();
                        //nuserrole.User_ID = nuser.User_ID;
                        //nuserrole.Role_ID = 2;

                        await _userService.AddUserWithDefaultRole(nuser);

                        await _userService.UpdateRoleUser(nuser.User_ID, nuser.Role_ID);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Constants.LOGIN_BAD_REQUEST });
                    }
                }
                else
                {
                    try
                    {
                        personEntity personemployee = _userService.GetPersonCampusId(person.Person_ID);

                        userEntity euser = new userEntity();
                        euser.Person_ID = personemployee.Person_ID;
                        euser.Person_Type = personemployee.Person_Type;
                        //euser.PersonEntity.CampusEntity.Campus_ID = person.PositionEntity.DepartmentEntity.Campus_ID;
                        euser.User_Name = login.Username;
                        euser.User_Password = Encryption.Encrypt(login.Password);
                        euser.Role_ID = user.Role_ID;
                        euser.Date_Time_Added = DateTime.Now;
                        euser.Added_By = 1;
                        euser.Last_Updated = DateTime.Now;
                        euser.Updated_By = 1;
                        euser.IsActive = true;
                        euser.ToDisplay = true;

                        await _userService.UpdateUser(euser, user.User_ID);

                        await _userService.UpdateRoleUser(euser.User_ID, euser.Role_ID);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { Constants.LOGIN_BAD_REQUEST });
                    }
                }

                user = await _userService.AuthenticateAccount(login.Username);

                if (user == null)
                    return BadRequest(new { message = "Invalid Account!" });

                //var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.UserRoleEntity.Role_ID, Campus = user.PersonEntity.CampusEntity.Campus_ID });
                //var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.Role_ID, Campus = user.PersonEntity.CampusEntity.Campus_ID });
                var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.Role_ID, Person_ID = user.Person_ID });

                _audit.Audit(user.User_ID, (int)Form.User_List, string.Format("User: {0}, successfully logged in to the system.", user.User_Name));

                response = Ok(new { access_token = tokenString });

            }
            else
            {
                var user = await _userService.AuthenticateAdmin(login.Username, Encryption.Encrypt(login.Password));

                if (user != null)
                {
                    //var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.UserRoleEntity.Role_ID, Campus = user.PersonEntity.Campus_ID });
                    //var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.Role_ID, Campus = user.PersonEntity.Campus_ID });
                    var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.Role_ID, Person_ID = user.Person_ID });

                    _audit.Audit(user.User_ID, (int)Form.User_List, string.Format("User: {0}, successfully logged in to the system.", user.User_Name));

                    response = Ok(new { access_token = tokenString });
                }
                else
                {
                    return BadRequest(new { Constants.LOGIN_INCORRECT_INPUT });
                }
            }

            return response;
        }

        //[AllowAnonymous]
        //[HttpPost("auth/login")]
        //public IActionResult CreateToken([FromBody]LoginModel login)
        //{
        //    IActionResult response = Unauthorized();

        //    if (!login.Username.Equals("admin"))
        //    {
        //        if(login.Username.IndexOf("@") <= 0)
        //            return BadRequest(new { message = "Invalid Email Account!" });

        //        var validateActiveDir = _userService.ValidateAdAccount(login.Username, login.Password, AdSettingsStat.Domain, AdSettingsStat.Url);

        //        if (!validateActiveDir.ValidLogin)
        //            return BadRequest(new { message = "Invalid Active Directory Account!" });

        //        var person = _userService.VerifyEmail(login.Username);

        //        if (person == null)
        //            return BadRequest(new { message = "Invalid Email Account!" });

        //        var user = _userService.VerifyUserAccountRole(person.Person_ID);

        //        if (user == null)
        //        {
        //            try
        //            {
        //                PersonEntity_employee personemployee = _userService.GetPersonCampusId(person.Person_ID);

        //                tbl_user nuser = new tbl_user();
        //                nuser.Person_ID = personemployee.Person_ID;
        //                nuser.Campus_ID = person.PersonEntity_employee.PositionEntity.DepartmentEntity.Campus_ID;
        //                nuser.User_Name = login.Username;
        //                nuser.User_Password = Encryption.Encrypt(login.Password);
        //                nuser.Date_Time_Added = DateTime.Now;
        //                nuser.Added_By = 1;
        //                nuser.Last_Updated = DateTime.Now;
        //                nuser.Updated_By = 1;
        //                nuser.IsActive = true;

        //                userRoleEntity nuserrole = new userRoleEntity();
        //                nuserrole.User_ID = nuser.User_ID;
        //                nuserrole.Role_ID = 2;

        //                _userService.AddUserWithDefaultRole(nuser, nuserrole);
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest(new { message = ex.Message });
        //            }
        //        }

        //        user = _userService.AuthenticateAccount(login.Username).Result;

        //        var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.userRoleEntitys.Role_ID, Campus = user.Campus_ID });

        //        _audit.Audit(user.User_ID, 0, string.Format("User {0} logged in. ", user.User_Name));

        //        response = Ok(new { access_token = tokenString });
        //    }
        //    else
        //    {
        //        var user = _userService.Authenticate(login.Username, Encryption.Encrypt(login.Password));

        //        if (user != null)
        //        {
        //            var tokenString = BuildToken(new TokenModel { Email = user.PersonEntity.Email_Address, First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, UserID = user.User_ID, UserRole = user.userRoleEntitys.Role_ID, Campus = user.Campus_ID });

        //            _audit.Audit(user.User_ID, 0, string.Format("User {0} logged in. ", user.User_Name));

        //            response = Ok(new { access_token = tokenString });
        //        }
        //        else
        //        {
        //            return BadRequest(new { message = "Username or password is incorrect" });
        //        }
        //    }
        //    return response;
        //}

        private string BuildToken(TokenModel user)
        {
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.PrimarySid, user.Person_ID.ToString()),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = ClaimTypes.Upn;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_appSettings.Issuer,
              _appSettings.Audience,
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*
        [HttpPost]
        public IActionResult Refresh(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = GetRefreshToken(username); //retrieve the refresh token from a data store
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();
            DeleteRefreshToken(username, refreshToken);
            SaveRefreshToken(username, newRefreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        */
    }
}