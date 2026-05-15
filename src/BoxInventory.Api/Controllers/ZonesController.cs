using BoxInventory.Application.DTOs;
using BoxInventory.Application.Zones.Commands.CreateZone;
using BoxInventory.Application.Zones.Queries.GetAllZones;
using BoxInventory.Application.Zones.Queries.GetZoneById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxInventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ZonesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ZonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene todas las zonas registradas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ZoneDto>>> GetAll(CancellationToken cancellationToken)
    {
        var zones = await _mediator.Send(new GetAllZonesQuery(), cancellationToken);
        return Ok(zones);
    }

    /// <summary>Obtiene una zona por su identificador único.</summary>
    /// <param name="id">Identificador único de la zona.</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ZoneDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var zone = await _mediator.Send(new GetZoneByIdQuery(id), cancellationToken);
        return Ok(zone);
    }

    /// <summary>Crea una nueva zona.</summary>
    /// <param name="command">Nombre de la zona.</param>
    [HttpPost]
    public async Task<ActionResult<ZoneDto>> Create([FromBody] CreateZoneCommand command, CancellationToken cancellationToken)
    {
        var zone = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), zone);
    }
}
