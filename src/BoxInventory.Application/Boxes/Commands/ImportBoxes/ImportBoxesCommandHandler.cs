using BoxInventory.Application.DTOs;
using BoxInventory.Domain.Entities;
using BoxInventory.Domain.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace BoxInventory.Application.Boxes.Commands.ImportBoxes;

public class ImportBoxesCommandHandler : IRequestHandler<ImportBoxesCommand, ImportBoxesResult>
{
    private readonly IExcelReaderService _excelReader;
    private readonly IBoxRepository _boxRepository;
    private readonly IZoneRepository _zoneRepository;

    public ImportBoxesCommandHandler(
        IExcelReaderService excelReader,
        IBoxRepository boxRepository,
        IZoneRepository zoneRepository)
    {
        _excelReader = excelReader;
        _boxRepository = boxRepository;
        _zoneRepository = zoneRepository;
    }

    public async Task<ImportBoxesResult> Handle(ImportBoxesCommand request, CancellationToken cancellationToken)
    {
        var bytes = Convert.FromBase64String(request.FileBase64);
        var builder = new BoxSheetBuilder();

        var boxes = _excelReader.Read(bytes, builder);

        var results = new List<BoxImportResult>();
        int boxIndex = 0;

        foreach (var sheetResult in builder.Results)
        {
            if (sheetResult.Error is not null)
            {
                results.Add(new BoxImportResult(sheetResult.SheetName, null, false, sheetResult.Error));
                continue;
            }

            try
            {
                var zoneId = await ResolveZoneId(sheetResult.ZoneName, cancellationToken);
                boxes[boxIndex].SetZone(zoneId);
                await _boxRepository.CreateAsync(boxes[boxIndex], cancellationToken);

                results.Add(new BoxImportResult(sheetResult.SheetName, boxes[boxIndex].Name, true, null));
                boxIndex++;
            }
            catch (Exception ex)
            {
                results.Add(new BoxImportResult(sheetResult.SheetName, boxes[boxIndex].Name, false, ex.Message));
                boxIndex++;
            }
        }

        return new ImportBoxesResult(
            builder.Results.Count,
            results.Count(r => r.Success),
            results.Count(r => !r.Success),
            results);
    }

    private async Task<ObjectId> ResolveZoneId(string? zoneName, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(zoneName))
        {
            var zone = await _zoneRepository.GetByNameAsync(zoneName, cancellationToken);
            if (zone is not null)
                return zone.Id;
        }

        var defaultZone = await _zoneRepository.GetByNameAsync(Zone.DefaultZoneName, cancellationToken)
            ?? throw new InvalidOperationException($"Default zone '{Zone.DefaultZoneName}' not found");

        return defaultZone.Id;
    }
}
