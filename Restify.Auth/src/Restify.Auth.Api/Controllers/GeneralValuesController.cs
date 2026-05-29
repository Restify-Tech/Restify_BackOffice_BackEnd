using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[ApiController]
[Route("api/general-values")]
[Authorize]
public class GeneralValuesController : ControllerBase
{
    private readonly IGeneralValueService _service;

    public GeneralValuesController(IGeneralValueService service) => _service = service;

    /// <summary>Obtiene el arbol de valores generales, opcionalmente filtrado por categoria</summary>
    [HttpGet]
    public async Task<IActionResult> GetTree([FromQuery] string? category, CancellationToken ct)
        => Ok(await _service.GetTreeAsync(category, ct));

    /// <summary>Obtiene el valor de un parametro por su clave</summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
    {
        var value = await _service.GetAsync(key, ct: ct);
        return value is null ? NotFound() : Ok(new { key, value });
    }

    /// <summary>Crea un nuevo parametro de configuracion (solo SuperAdmin)</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGeneralValueRequest request, CancellationToken ct)
        => Ok(await _service.CreateAsync(request, ct));

    /// <summary>Actualiza el valor de un parametro existente (solo SuperAdmin)</summary>
    [HttpPatch("{key}/value")]
    public async Task<IActionResult> UpdateValue(string key, [FromBody] UpdateValueRequest request, CancellationToken ct)
    {
        await _service.UpdateValueAsync(key, request.Value, ct);
        return NoContent();
    }
}

public record UpdateValueRequest(string? Value);
