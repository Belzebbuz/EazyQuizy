using EazyQuizy.Core.Abstractions.Grains.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace EazyQuizy.Api.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController(IGrainFactory grainFactory) : ControllerBase
{
	[HttpGet("{grainId}/{fileName}")]
	public IActionResult GetFile(string grainId, string fileName)
	{
		var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"files",grainId, fileName);
		if (!Path.Exists(filePath)) return NotFound($"{fileName} not found");
		
		var provider = new FileExtensionContentTypeProvider();

		if (!provider.TryGetContentType(fileName, out var contentType))
		{
			contentType = "application/octet-stream";
		}
		return PhysicalFile(filePath,contentType);
	}
}