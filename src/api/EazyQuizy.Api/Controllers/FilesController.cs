using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace EazyQuizy.Api.Controllers;

[Authorize]
[Route("files")]
[ApiController]
public class FilesController : ControllerBase
{
	[HttpGet("{subfolder}/{fileName}")]
	public IActionResult GetFile(string subfolder, string fileName)
	{
		var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"files",subfolder, fileName);
		if (!Path.Exists(filePath)) return NotFound($"{fileName} not found");
		
		var provider = new FileExtensionContentTypeProvider();

		if (!provider.TryGetContentType(fileName, out var contentType))
		{
			contentType = "application/octet-stream";
		}
		return PhysicalFile(filePath,contentType);
	}
}