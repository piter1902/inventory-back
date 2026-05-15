using BoxInventory.Application.Boxes.Commands.CreateBox;
using BoxInventory.Application.Boxes.Commands.DeleteBox;
using BoxInventory.Application.Boxes.Commands.UpdateBox;
using BoxInventory.Application.Boxes.Queries.GetAllBoxes;
using BoxInventory.Application.Boxes.Queries.GetBoxById;
using BoxInventory.Application.Boxes.Queries.Search;
using BoxInventory.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxInventory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BoxesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoxesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene todas las cajas registradas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<BoxDto>>> GetAll(CancellationToken cancellationToken)
    {
        var boxes = await _mediator.Send(new GetAllBoxesQuery(), cancellationToken);
        return Ok(boxes);
    }

    /// <summary>Obtiene una caja por su identificador único.</summary>
    /// <param name="id">Identificador único de la caja.</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<BoxDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var box = await _mediator.Send(new GetBoxByIdQuery(id), cancellationToken);
        return Ok(box);
    }

    /// <summary>Busca cajas e items por nombre. El resultado se segmenta en cajas e items, cada item incluye el nombre de su caja.</summary>
    /// <param name="query">Término de búsqueda.</param>
    [HttpGet("search")]
    public async Task<ActionResult<SearchResultDto>> Search([FromQuery] string query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchQuery(query), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva caja. El identificador y el QR se generan automáticamente. Puede incluir items iniciales.</summary>
    [HttpPost]
    public async Task<ActionResult<BoxDto>> Create([FromBody] CreateBoxCommand command, CancellationToken cancellationToken)
    {
        var box = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = box.Id }, box);
    }

    /// <summary>Reemplaza completamente una caja: nombre, imagen e items.</summary>
    /// <param name="id">Identificador único de la caja.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<BoxDto>> Update(string id, [FromBody] UpdateBoxRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateBoxCommand(id, request.Name, request.Description, request.ImageBase64, request.ZoneId, request.Items);
        var box = await _mediator.Send(command, cancellationToken);
        return Ok(box);
    }

    /// <summary>Elimina una caja y todo su contenido.</summary>
    /// <param name="id">Identificador único de la caja a eliminar.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteBoxCommand(id), cancellationToken);
        return NoContent();
    }
}

public record UpdateBoxRequest(string? Name, string? Description, string? ImageBase64, string? ZoneId, List<UpdateItemRequest>? Items);
