using BoxInventory.Application.DTOs;
using MediatR;

namespace BoxInventory.Application.Boxes.Commands.ImportBoxes;

public record ImportBoxesCommand(string FileBase64) : IRequest<ImportBoxesResult>;
