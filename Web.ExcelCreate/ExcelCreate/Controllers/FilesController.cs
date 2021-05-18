using ExcelCreate.Hubs;
using ExcelCreate.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IHubContext<MyHub> _hubcontext;

        public FilesController(AppDbContext context, IHubContext<MyHub> hubcontext)
        {
            _context = context;
            _hubcontext = hubcontext;
        }
        [HttpPost("hey")]
        public async Task<IActionResult> Upload(IFormFile file ,int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await _context.UserFiles.FirstAsync(x=>x.Id==fileId);

            var filePath = userFile.FileName + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/", filePath);

            using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            await _context.SaveChangesAsync();
            //signaIR notification
            await _hubcontext.Clients.User(userFile.UserId).SendAsync("complatedFile");


            return Ok();


        }
    }
}
