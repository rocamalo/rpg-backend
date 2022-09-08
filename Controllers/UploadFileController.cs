using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Services.UserService;
using System.Net.Http.Headers;

namespace rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : Controller
    {

        private readonly IUserService _userService;
        public UploadFileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(int id)
        {
            try
            {
                var response = new ServiceResponse<string>();
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    response = await _userService.UpdateImageAsync(id, dbPath);
                    if (response.Success)
                    {
                        return Ok(new { dbPath });
                    } else
                    {
                        response.Success = false;
                        return BadRequest(response);
                        Console.WriteLine("Error at uploading image");
                    }
                    
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
