using AutoMapper;
using E_commerce.DTOS;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, IMapper mapper, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            List<User> userList = _userService.GetAll();
            List<UserSendDto> convertedUserDtos = _mapper.Map<List<UserSendDto>>(userList);
            return Ok(convertedUserDtos);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetUser(Guid id)
        {
            try
            {
                var user = _userService.GetById(id);
                return Ok(user);

            }
            catch(UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
            catch (InvalidUserException iue)
            {
                return BadRequest(new { error = $"{iue.Message},{iue.StatusCode}" });
            }
        }

        [HttpGet("name/{name}")]
        public IActionResult GetUserByName(string name)
        {
            try
            {
                var user = _userService.GetByName(name);
                return Ok(user);
            }
            catch (InvalidUserException iue)
            {
                return BadRequest(new { error = $"{iue.Message},{iue.StatusCode}" });
            }
            catch (UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }
        }

         [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterUser([FromForm] UserSignUpDto userSignedDto)
        {
            try
            {
                if (userSignedDto.ImageFile == null)
                {
                    userSignedDto.FilePath = "images/DefaultImageBackend.jpg";
                }
                else if ( userSignedDto.ImageFile.Length == 0)
                {
                    return BadRequest("Please upload user profile picture");
                }
                else { 
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userSignedDto.ImageFile.FileName)}";
                var imagePath = Path.Combine("wwwroot", "images", fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await userSignedDto.ImageFile.CopyToAsync(fileStream);
                }
                    
                    userSignedDto.FilePath = $"images/{fileName}";
                }
                User user = _mapper.Map<User>(userSignedDto);
                user.UserId = Guid.NewGuid();
                _userService.Add(user);
                return CreatedAtAction(nameof(GetAllUser), new { id = user.UserId }, new { userId = user.UserId, userName=user.Name} );

            }
            catch (InvalidUserException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }
            catch (PhoneNumberAlreadyExistException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }
            catch (EmailAlreadyExistException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }

        }

        [HttpPost("admin/add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAUser([FromForm] PostUserDto postUserDto)
        {
            try
            {
                // Check if the user with the same email or phone number already exists
                var existingUser = _userService.GetUserByEmail(postUserDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { error = "Email address already exists." });
                }

                var existingPhoneUser = _userService.GetByPhoneNumber(postUserDto.PhoneNumber);
                if (existingPhoneUser != null)
                {
                    return BadRequest(new { error = "Phone number already exists." });
                }

                // Handle file upload if present
                if (postUserDto.ImageFile != null)
                {
                    if (postUserDto.ImageFile.Length == 0)
                    {
                        return BadRequest("Invalid profile picture.");
                    }

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(postUserDto.ImageFile.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "images", fileName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await postUserDto.ImageFile.CopyToAsync(fileStream);
                    }

                    postUserDto.FilePath = $"images/{fileName}";
                }

                // Map DTO to User model
                User user = _mapper.Map<User>(postUserDto);
                user.UserId = Guid.NewGuid();  // Assign new UserId

                _userService.Add(user);

                // Return created user info
                return CreatedAtAction(nameof(GetAllUser), new { id = user.UserId }, new { userId = user.UserId, userName = user.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto userUpdateDto)
        {
            try
            {
                User getUser= _userService.GetById(userUpdateDto.UserId);
                if(userUpdateDto.FilePath != getUser.FilePath)
                {
                    if(userUpdateDto.FilePath == null)
                    {
                        userUpdateDto.FilePath = "images/DefaultImageBackend.jpg";

                    }
                    else if(userUpdateDto.ImageFile.Length==0)
                    {
                        return BadRequest("Invalid Profile Pictures");
                    }
                    else
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userUpdateDto.ImageFile.FileName)}";
                        var imagePath = Path.Combine("wwwroot", "images", fileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await userUpdateDto.ImageFile.CopyToAsync(fileStream);
                        }

                        userUpdateDto.FilePath = $"images/{fileName}";
                        getUser.FilePath = $"images/{fileName}";
                    }
                
                }
                
                User updatedUser = _mapper.Map<User>(userUpdateDto);
                updatedUser.Password=getUser.Password;
                 
                var updated=_userService.Update(updatedUser);
                var sendUpdated=_mapper.Map<UserSendDto>(updated);
                return Ok(sendUpdated);

            }
            catch(InvalidUserException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }
            catch (PhoneNumberAlreadyExistException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }
            catch (EmailAlreadyExistException ex)
            {
                return BadRequest(new { error = $"{ex.Message}, {ex.StatusCode}" });
            }
            catch (UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }

        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            try
            {
                _userService.Delete(id);
                return Ok(new {message="User Deleted"});
            }
            catch (UserNotFoundException unfe) { 
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }


        }

        [HttpPost("login")]
        public  async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var recaptchaResponse = userLoginDto.Recaptcha;
                var isValidRecaptcha = await ValidateRecaptcha(recaptchaResponse);

                if (!isValidRecaptcha)
                {
                    return BadRequest(new { error = "Invalid reCAPTCHA." });
                }

                User getUser = _userService.GetByName(userLoginDto.Name);
                if (getUser == null)
                {
                    return BadRequest(new { error = "User not found." });
                }
                if (BCrypt.Net.BCrypt.Verify(userLoginDto.Password, getUser.Password))
                {
                    var token = CreateToken(getUser);
                    Response.Headers.Add("Jwt", JsonConvert.SerializeObject(token));
                    UserSendDto userSendDto = _mapper.Map<UserSendDto>(getUser);
                    return Ok(userSendDto);
                }
                else
                {
                    return BadRequest(new { error = $"Username and Password Does not Match" });
                }

            }
            catch (InvalidUserException iue)
            {
                return BadRequest(new { error = $"{iue.Message}, {iue.StatusCode}" });
            }
            catch (UserNotFoundException unfe)
            {
                return NotFound(new { error = $"{unfe.Message},{unfe.StatusCode}" });
            }

        }


        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            var secretKey = "6Ld3RDYqAAAAAMJiiIJmqICRLmnn9hWQodudY-VW";
            var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}", null);
            var result = await response.Content.ReadAsStringAsync();
            dynamic resultObj = JsonConvert.DeserializeObject(result);
            return resultObj.success == "true";
        }

        private string CreateToken(User getUser)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, getUser.Name),
                new Claim(ClaimTypes.Role, getUser.IsAdmin ? "Admin" : "User")
            };

            var t = Encoding.UTF32.GetBytes(_configuration.GetSection("AppSettings:Key").Value);
            var key = new SymmetricSecurityKey(t);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


    }
}
