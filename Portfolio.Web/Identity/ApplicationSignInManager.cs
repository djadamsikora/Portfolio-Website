using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Web.Data;
using System;
using System.Threading.Tasks;

namespace Portfolio.Web.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<SignInManager<ApplicationUser>> _logger;

        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            ApplicationDbContext dbContext,
            IAuthenticationSchemeProvider schemeProvider,
            IUserConfirmation<ApplicationUser> userConfirmation
            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, userConfirmation)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextAccessor = contextAccessor;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var BaseResult = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            if( BaseResult.Succeeded )
            {
                // we succeeded, update last sign in
                ApplicationUser user = await _userManager.FindByNameAsync(userName);

                if( user != null )
                {
                    user.UserLastLogin = DateTime.Now.ToUniversalTime();

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e, "Error Updating User Last Login");
                    }
                }
            }

            return BaseResult;
        }
    }
}
