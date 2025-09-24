using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.Repositories;

namespace NPPHASE.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpPost("AdminLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var Modeluser = await _userManager.FindByEmailAsync(model.UserName);
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                var tokenRequest = new TokenRequestViewModel();
                var tokenResponse = new AuthenticationResponse();
                TokenGenerator tokenGenerator = new TokenGenerator();
                var jwtToken = _configuration.GetValue<string>("JwtTokenKeysValue:JWT_SECURRITY_KEY");
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(Modeluser);
                    if (roles.Contains("Admin"))
                    {
                        tokenRequest.JwtKey = jwtToken;
                        tokenRequest.Email = Modeluser.Email;
                        tokenRequest.UserName = Modeluser.Name;
                        tokenRequest.Role = roles.FirstOrDefault();
                        tokenRequest.UserId = Modeluser.Id;
                        tokenResponse.Token = tokenGenerator.GenerateToken(tokenRequest);
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            Data = tokenResponse,
                            IsSuccess = true
                        });
                    }
                }
                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Incorrect UserName or Password"
                });
            }
            return new OkObjectResult(new ResponseMessageViewModel
            {
                IsSuccess = false,
                Message = "Model is not validate"
            });
        }

        [HttpPost("SuperAdminLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> SuperAdminLogin(AdminLoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var Modeluser = await _userManager.FindByEmailAsync(model.UserName);
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                var tokenRequest = new TokenRequestViewModel();
                var tokenResponse = new AuthenticationResponse();
                TokenGenerator tokenGenerator = new TokenGenerator();
                var jwtToken = _configuration.GetValue<string>("JwtTokenKeysValue:JWT_SECURRITY_KEY");
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(Modeluser);
                    if (roles.Contains("SuperAdmin"))
                    {
                        tokenRequest.JwtKey = jwtToken;
                        tokenRequest.Email = Modeluser.Email;
                        tokenRequest.UserName = Modeluser.Name;
                        tokenRequest.Role = roles.FirstOrDefault();
                        tokenRequest.UserId = Modeluser.Id;
                        tokenResponse.Token = tokenGenerator.GenerateToken(tokenRequest);
                        return new OkObjectResult(new ResponseMessageViewModel
                        {
                            Data = tokenResponse,
                            IsSuccess = true
                        });
                    }
                }

                return new OkObjectResult(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Incorrect UserName or Password"
                });


            }
            return new OkObjectResult(new ResponseMessageViewModel
            {
                IsSuccess = false,
                Message = "Model is not validate"
            });
        }

        [HttpPost("UpdateAdminCredentials")]
        public async Task<IActionResult> UpdateAdminCredentials(AdminLoginUserDetailsChangeViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Invalid model data"
                });
            }

            // Fetch TestAdmin detail
            var usersInRole = await _userManager.GetUsersInRoleAsync("ADMIN");
            var testAdminUser = usersInRole?.FirstOrDefault();

            if (testAdminUser == null)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "No TestAdmin user found."
                });
            }

            // Checkt the old and new email id
            if (testAdminUser.Email == model.NewUserName)
            {
                // Update password only
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(testAdminUser);
                var passwordUpdateResult = await _userManager.ResetPasswordAsync(testAdminUser, passwordResetToken, model.Password);

                if (!passwordUpdateResult.Succeeded)
                {
                    return BadRequest(new ResponseMessageViewModel
                    {
                        IsSuccess = false,
                        Message = "Failed to update password. Please enter a valid password."
                    });
                }

                return Ok(new ResponseMessageViewModel
                {
                    IsSuccess = true,
                    Message = "Password updated successfully."
                });
            }

            // Fetch the current user by email
            var oldAdminData = await _userManager.FindByEmailAsync(testAdminUser.Email);
            if (oldAdminData == null)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Admin user data not found."
                });
            }

            // Update email
            var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(oldAdminData, model.NewUserName);
            var emailUpdateResult = await _userManager.ChangeEmailAsync(oldAdminData, model.NewUserName, emailChangeToken);

            if (!emailUpdateResult.Succeeded)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Failed to update email."
                });
            }

            // Update username
            var userNameUpdateResult = await _userManager.SetUserNameAsync(oldAdminData, model.NewUserName);
            if (!userNameUpdateResult.Succeeded)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Failed to update username."
                });
            }

            // Update password
            var passwordResetTokenForUpdate = await _userManager.GeneratePasswordResetTokenAsync(oldAdminData);
            var passwordUpdateResultForUpdate = await _userManager.ResetPasswordAsync(oldAdminData, passwordResetTokenForUpdate, model.Password);

            if (!passwordUpdateResultForUpdate.Succeeded)
            {
                return BadRequest(new ResponseMessageViewModel
                {
                    IsSuccess = false,
                    Message = "Failed to update password. Please enter a valid password."
                });
            }

            return Ok(new ResponseMessageViewModel
            {
                IsSuccess = true,
                Message = "Email and password updated successfully."
            });
        }
        
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userId, string newPassword)
        {
            // Retrieve the user by ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // User not found
                return NotFound();
            }

            // Generate a reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                // Password updated successfully
                return Ok();
            }

            // Password update failed
            return BadRequest();
        }
    }
}
