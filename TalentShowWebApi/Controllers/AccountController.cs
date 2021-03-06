﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using TalentShowWebApi.Models;
using TalentShowWebApi.Providers;
using TalentShowWebApi.Results;
using System.Linq;
using TalentShowDataStorage;
using TalentShow;
using TalentShow.Services;

namespace TalentShowWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            return GetUser(UserManager.FindById(User.Identity.GetUserId()));
        }

        // GET api/Account/UserInfo
        [Route("UsersInfo")]
        public IEnumerable<UserInfoViewModel> GetUsersInfo()
        {
            var users = new List<UserInfoViewModel>();
            var appUsers = new List<ApplicationUser>();

            foreach (var user in UserManager.Users)
            {
                appUsers.Add(user);
            }

            foreach (var user in appUsers)
            {
                users.Add(GetUser(user));
            }

            return users;
        }

        private UserInfoViewModel GetUser(ApplicationUser user)
        {
            var roles = UserManager.GetRoles(user.Id);
            var claims = new List<UserClaimViewModel>();

            foreach (var claim in user.Claims)
                claims.Add(new UserClaimViewModel() { Type = claim.ClaimType, Value = claim.ClaimValue });

            string firstName = "";
            string lastName = "";
            Organization affiliation = null;

            var personNameIdClaim = claims.FirstOrDefault(n => n.Type == "personNameId");

            if (personNameIdClaim != null)
            {
                var personNameId = Convert.ToInt32(personNameIdClaim.Value);
                var personName = new PersonNameRepo().Get(personNameId);

                firstName = personName.FirstName;
                lastName = personName.LastName;
            }

            var organizationIdClaim = claims.FirstOrDefault(n => n.Type == "organizationId");

            if (personNameIdClaim != null)
            {
                var organizationId = Convert.ToInt32(organizationIdClaim.Value);
                affiliation = new OrganizationRepo().Get(organizationId);
            }

            if (roles.Contains("Judge"))
            {
                var judges = new JudgeService(new JudgeRepo(), new ContestJudgeRepo()).GetAll().Where(j => j.UserId == user.Id);

                foreach (var judge in judges)
                    claims.Add(new UserClaimViewModel() { Type = "judgeId", Value = judge.Id.ToString() });
            }

            var contests = new ContestService(new ContestRepo(), new ShowContestRepo()).GetAll().Where(c => c.TimeKeeperId == user.Id);

            if (contests.Any())
                roles.Add("TimeKeeper");

            return new UserInfoViewModel
            {
                Id = user.Id,
                Email = user.UserName,
                FirstName = firstName,
                LastName = lastName,
                Affiliation = affiliation,
                Roles = roles,
                Claims = claims
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/UpdateUserInfo
        [Route("UpdateUserInfo")]
        public HttpResponseMessage UpdateUserInfo(UpdateUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            }

            var user = UserManager.FindById(model.Id);

            user.Email = model.Email;
            var personNameClaim = user.Claims.FirstOrDefault(n => n.ClaimType == "personNameId");

            if (personNameClaim != null)
            {
                int personNameId = Convert.ToInt32(personNameClaim.ClaimValue);
                new PersonNameRepo().Update(new PersonName(personNameId, model.FirstName, model.LastName));
            }
            else
            {
                var newPersonName = new PersonName(model.FirstName, model.LastName);
                new PersonNameRepo().Add(newPersonName);

                foreach (var claim in UserManager.GetClaims(user.Id).Where(c => c.Type == "personNameId"))
                    UserManager.RemoveClaim(user.Id, claim);

                UserManager.AddClaim(user.Id, new Claim("personNameId", newPersonName.Id.ToString()));
            }

            foreach (var claim in UserManager.GetClaims(user.Id).Where(c => c.Type == "organizationId"))
                UserManager.RemoveClaim(user.Id, claim);

            UserManager.AddClaim(user.Id, new Claim("organizationId", model.OrganizationId.ToString()));
            
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, GetUser(user));
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            }

            IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, GetUserInfo());
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await AddUserToRole(new UserRoleBindingModel()
            {
                UserId = user.Id,
                RoleName = "Judge" 
            });

            var personName = new PersonName(model.FirstName, model.LastName);
            new PersonNameRepo().Add(personName);

            await AddClaimToUser(new UserClaimBindingModel()
            {
                UserId = user.Id,
                Type = "personNameId",
                Value = personName.Id.ToString()
            });

            await AddClaimToUser(new UserClaimBindingModel()
            {
                UserId = user.Id,
                Type = "organizationId",
                Value = model.OrganizationId.ToString()
            });

            return Ok();
        }

        // POST api/Account/AddUserToRole
        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IHttpActionResult> AddUserToRole(UserRoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = model.UserId;
            string roleName = model.RoleName;

            ApplicationUser user = UserManager.FindById(userId);

            if (user == null)
            {
                return BadRequest("The user id does not exist: \"" + userId + "\"");
            }

            IdentityRole role = new IdentityRole(roleName);
     
            if (!AppRoleManager.RoleExists(roleName))
            {
                IdentityResult result = await AppRoleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }

            UserManager.AddToRole(user.Id, roleName);

            return Ok();
        }

        // DELETE api/Account/DeleteUserRole
        [HttpDelete]
        [Route("DeleteUserRole")]
        public IHttpActionResult DeleteUserRole(UserRoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = model.UserId;
            string roleName = model.RoleName;

            ApplicationUser user = UserManager.FindById(userId);

            if (user == null)
            {
                return BadRequest("The user id does not exist: \"" + userId + "\"");
            }

            UserManager.RemoveFromRole(user.Id, roleName);

            return Ok();
        }

        // POST api/Account/AddClaimToUser
        [HttpPost]
        [Route("AddClaimToUser")]
        public async Task<IHttpActionResult> AddClaimToUser(UserClaimBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = model.UserId;
            var claimType = model.Type;
            var claimValue = model.Value;

            ApplicationUser user = UserManager.FindById(userId);

            if (user == null)
            {
                return BadRequest("The user id does not exist: \"" + userId + "\"");
            }

            foreach (var claim in UserManager.GetClaims(user.Id).Where(c => c.Type == claimType))
                UserManager.RemoveClaim(user.Id, claim);

            var result = await UserManager.AddClaimAsync(user.Id, new Claim(claimType, claimValue));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // DELETE api/Account/DeleteUserClaim
        [HttpDelete]
        [Route("DeleteUserClaim")]
        public IHttpActionResult DeleteUserClaim(DeleteUserClaimBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = model.UserId;
            var claimType = model.Type;

            ApplicationUser user = UserManager.FindById(userId);

            if (user == null)
            {
                return BadRequest("The user id does not exist: \"" + userId + "\"");
            }

            foreach (var claim in UserManager.GetClaims(user.Id).Where(c => c.Type == claimType))
                UserManager.RemoveClaim(user.Id, claim);

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
