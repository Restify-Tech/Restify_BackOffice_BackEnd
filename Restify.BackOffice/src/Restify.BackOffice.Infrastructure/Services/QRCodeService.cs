using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    private readonly IQRCodeRepository _qrCodeRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<QRCodeService> _logger;

    public QRCodeService(
        IQRCodeRepository qrCodeRepository,
        ITableRepository tableRepository,
        ICurrentUserService currentUserService,
        ILogger<QRCodeService> logger)
    {
        _qrCodeRepository = qrCodeRepository;
        _tableRepository = tableRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<QRCodeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(id, cancellationToken);
        if (qrCode == null)
            return Result<QRCodeDto>.Failure("Codigo QR no encontrado");

        return Result<QRCodeDto>.Success(ToDto(qrCode));
    }

    public async Task<Result<IEnumerable<QRCodeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var qrCodes = await _qrCodeRepository.GetAllAsync(cancellationToken);
        return Result<IEnumerable<QRCodeDto>>.Success(qrCodes.Select(ToDto));
    }

    public async Task<Result<QRCodeDto>> GenerateAsync(GenerateQRCodeRequest request, CancellationToken cancellationToken = default)
    {
        var qrType = (QRCodeType)request.Type;

        if (qrType == QRCodeType.Table)
        {
            if (request.TableId == null)
                return Result<QRCodeDto>.Failure("Se requiere una mesa para codigos QR de tipo Mesa");

            var table = await _tableRepository.GetByIdAsync(request.TableId.Value, cancellationToken);
            if (table == null)
                return Result<QRCodeDto>.Failure("Mesa no encontrada");
        }

        var code = await GenerateUniqueCodeAsync(cancellationToken);
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var baseUrl = request.BaseUrl?.TrimEnd('/') ?? "https://app.restaurant.com";
        var url = $"{baseUrl}/qr/{code}";

        var qrCode = new QRCode
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TableId = request.TableId,
            Code = code,
            Url = url,
            Type = qrType,
            IsActive = true,
            ScanCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _qrCodeRepository.CreateAsync(qrCode, cancellationToken);

        _logger.LogInformation(
            "QR Code {QRCodeId} generated with code {Code} for type {Type}",
            created.Id, code, qrType);

        return Result<QRCodeDto>.Success(ToDto(created));
    }

    public async Task<Result<QRCodeResolveResponse>> ResolveAsync(string code, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetByCodeAsync(code, cancellationToken);
        if (qrCode == null)
            return Result<QRCodeResolveResponse>.Failure("Codigo QR no valido");

        if (!qrCode.IsActive)
            return Result<QRCodeResolveResponse>.Failure("Este codigo QR esta desactivado");

        // Update scan stats
        qrCode.LastScannedAt = DateTime.UtcNow;
        qrCode.ScanCount++;
        await _qrCodeRepository.UpdateAsync(qrCode, cancellationToken);

        // TODO: Get restaurant slug from tenant configuration
        var restaurantSlug = "restaurant";

        var response = new QRCodeResolveResponse(
            RestaurantSlug: restaurantSlug,
            TenantId: qrCode.TenantId,
            TableId: qrCode.TableId,
            TableNumber: qrCode.Table?.Number,
            QRCodeType: qrCode.Type.ToString());

        return Result<QRCodeResolveResponse>.Success(response);
    }

    public async Task<Result<byte[]>> DownloadQRImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(id, cancellationToken);
        if (qrCode == null)
            return Result<byte[]>.Failure("Codigo QR no encontrado");

        var svgContent = GenerateQRSvg(qrCode.Url, qrCode.Code);
        var bytes = System.Text.Encoding.UTF8.GetBytes(svgContent);

        return Result<byte[]>.Success(bytes);
    }

    public async Task<Result<bool>> ToggleActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(id, cancellationToken);
        if (qrCode == null)
            return Result<bool>.Failure("Codigo QR no encontrado");

        qrCode.IsActive = !qrCode.IsActive;
        qrCode.UpdatedAt = DateTime.UtcNow;
        await _qrCodeRepository.UpdateAsync(qrCode, cancellationToken);

        return Result<bool>.Success(qrCode.IsActive);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(id, cancellationToken);
        if (qrCode == null)
            return Result<bool>.Failure("Codigo QR no encontrado");

        await _qrCodeRepository.DeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    #region Private Methods

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        string code;
        do
        {
            code = GenerateShortCode();
        } while (await _qrCodeRepository.CodeExistsAsync(code, cancellationToken));

        return code;
    }

    private static string GenerateShortCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = Random.Shared;
        return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    private static string GenerateQRSvg(string url, string code)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 200 240"" width=""200"" height=""240"">
  <rect width=""200"" height=""240"" fill=""white""/>
  <rect x=""10"" y=""10"" width=""180"" height=""180"" fill=""none"" stroke=""black"" stroke-width=""2"" rx=""8""/>
  <rect x=""20"" y=""20"" width=""40"" height=""40"" fill=""black"" rx=""4""/>
  <rect x=""140"" y=""20"" width=""40"" height=""40"" fill=""black"" rx=""4""/>
  <rect x=""20"" y=""140"" width=""40"" height=""40"" fill=""black"" rx=""4""/>
  <rect x=""80"" y=""80"" width=""40"" height=""40"" fill=""black"" rx=""4""/>
  <text x=""100"" y=""215"" text-anchor=""middle"" font-family=""monospace"" font-size=""16"" font-weight=""bold"" fill=""black"">{code}</text>
  <text x=""100"" y=""235"" text-anchor=""middle"" font-family=""sans-serif"" font-size=""8"" fill=""gray"">Escanear para ver menu</text>
</svg>";
    }

    private static QRCodeDto ToDto(QRCode qrCode)
    {
        return new QRCodeDto(
            Id: qrCode.Id,
            TableId: qrCode.TableId,
            TableNumber: qrCode.Table?.Number,
            Code: qrCode.Code,
            Url: qrCode.Url,
            Type: qrCode.Type.ToString(),
            IsActive: qrCode.IsActive,
            LastScannedAt: qrCode.LastScannedAt,
            ScanCount: qrCode.ScanCount,
            CreatedAt: qrCode.CreatedAt);
    }

    #endregion
}
