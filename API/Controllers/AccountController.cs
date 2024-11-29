using API.Dtos.Account;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            IEmailService emailService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new
                        {
                            message = "Invalid data",
                            errors = ModelState
                                .Values.SelectMany(x => x.Errors)
                                .Select(x => x.ErrorMessage),
                        }
                    );
                }

                // App user
                var appUser = new AppUser
                {
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                };

                // check email exists
                var userExists = await _userManager.FindByEmailAsync(registerDto.Email);

                if (userExists != null)
                {
                    return BadRequest(
                        new
                        {
                            message = "Email already exists",
                            errors = new { Email = "Email already exists" },
                        }
                    );
                }

                // create user
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                // check user creation success
                if (!createdUser.Succeeded)
                {
                    return BadRequest(
                        new
                        {
                            message = "User creation failed",
                            errors = createdUser.Errors.Select(x => x.Description),
                        }
                    );
                }
                else
                {
                    // add user to role
                    var roleResults = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResults.Succeeded)
                    {
                        // confirm code
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                        // dictionary of key value pairs
                        var queryParams = new Dictionary<string, string>
                        {
                            { "Email", registerDto.Email },
                            { "Token", code },
                        };

                        // get domain
                        var domain = Environment.GetEnvironmentVariable("DOMAIN");

                        // generate confirmation link
                        var confirmationLink = QueryHelpers.AddQueryString(
                            $"{domain}/api/account/confirm-email",
                            queryParams!
                        );

                        // send email
                        await _emailService.SendEmailAsync(
                            registerDto.Email,
                            "Confirm your email",
                            $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>"
                        );

                        return Ok(
                            new
                            {
                                message = "Please confirm your email address ",
                                errors = new string[] { },
                            }
                        );
                    }
                    else
                    {
                        return BadRequest(
                            new
                            {
                                message = "User creation failed",
                                errors = roleResults.Errors.Select(x => x.Description),
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", errors = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new
                        {
                            message = "Invalid data",
                            errors = ModelState
                                .Values.SelectMany(x => x.Errors)
                                .Select(x => x.ErrorMessage),
                        }
                    );
                }

                // check user exists
                var user = await _userManager.Users.FirstOrDefaultAsync(x =>
                    x.Email == loginDto.Email
                );

                if (user == null)
                {
                    return Unauthorized(
                        new
                        {
                            message = "Invalid credentials",
                            errors = new { Email = "Invalid credentials" },
                        }
                    );
                }

                // check password
                var passwordValid = await _signInManager.CheckPasswordSignInAsync(
                    user,
                    loginDto.Password,
                    false
                );

                if (!passwordValid.Succeeded)
                {
                    return Unauthorized(
                        new
                        {
                            message = "Invalid credentials",
                            errors = new { Password = "Invalid credentials" },
                        }
                    );
                }

                // generate token
                var token = _tokenService.CreateToken(user);

                return Ok(
                    new
                    {
                        message = "Login successful",
                        user = new NewUserDto { Email = user.Email, Token = token },
                        errors = new string[] { },
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", errors = ex.Message });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new
                        {
                            message = "Invalid data",
                            errors = ModelState
                                .Values.SelectMany(x => x.Errors)
                                .Select(x => x.ErrorMessage),
                        }
                    );
                }

                // check user exists
                var user = await _userManager.Users.FirstOrDefaultAsync(x =>
                    x.Email == confirmEmailDto.Email
                );

                if (user == null)
                {
                    return Unauthorized(
                        new
                        {
                            message = "Invalid credentials",
                            errors = new { Email = "Invalid credentials" },
                        }
                    );
                }

                // confirm email
                var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Email confirmed", errors = new string[] { } });
                }
                else
                {
                    return BadRequest(
                        new
                        {
                            message = "Email confirmation failed",
                            errors = result.Errors.Select(x => x.Description),
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", errors = ex.Message });
            }
        }
    }
}
