using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QRCodesController : ControllerBase
{
    private readonly IQRCodeService _qrCodeService;

    public QRCodesController(IQRCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.GetByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateQRCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.GenerateAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Resuelve un codigo QR - endpoint publico sin auth
    /// </summary>
    [HttpGet("resolve/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> Resolve(string code, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.ResolveAsync(code, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.DownloadQRImageAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return File(result.Data!, "image/svg+xml", $"qr-{id}.svg");
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.ToggleActiveAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(new { IsActive = result.Data });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return NoContent();
    }
}
