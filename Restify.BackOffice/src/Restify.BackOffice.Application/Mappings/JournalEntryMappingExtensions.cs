using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Mappings;

public static class JournalEntryMappingExtensions
{
    public static JournalEntryDto ToDto(this JournalEntry entity)
    {
        var lines = entity.Lines?.Select(l => l.ToDto()).ToList() ?? new();

        return new JournalEntryDto
        {
            Id = entity.Id,
            EntryNumber = entity.EntryNumber,
            PeriodId = entity.PeriodId,
            PeriodName = entity.Period?.Name,
            Date = entity.Date,
            Description = entity.Description,
            Reference = entity.Reference,
            EntryType = entity.EntryType,
            SourceId = entity.SourceId,
            Status = entity.Status,
            PostedBy = entity.PostedBy,
            PostedAt = entity.PostedAt,
            ReversedBy = entity.ReversedBy,
            ReversedAt = entity.ReversedAt,
            Lines = lines,
            TotalDebit = lines.Sum(l => l.Debit),
            TotalCredit = lines.Sum(l => l.Credit),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static JournalEntryLineDto ToDto(this JournalEntryLine entity)
    {
        return new JournalEntryLineDto
        {
            Id = entity.Id,
            AccountId = entity.AccountId,
            AccountCode = entity.Account?.Code,
            AccountName = entity.Account?.Name,
            Description = entity.Description,
            Debit = entity.Debit,
            Credit = entity.Credit
        };
    }

    public static JournalEntry ToEntity(this CreateJournalEntryRequest request, Guid tenantId, string entryNumber)
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EntryNumber = entryNumber,
            PeriodId = request.PeriodId,
            Date = request.Date,
            Description = request.Description,
            Reference = request.Reference,
            EntryType = request.EntryType,
            SourceId = request.SourceId,
            Status = JournalEntryStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var lineRequest in request.Lines)
        {
            var line = new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                JournalEntryId = entry.Id,
                AccountId = lineRequest.AccountId,
                Description = lineRequest.Description,
                Debit = lineRequest.Debit,
                Credit = lineRequest.Credit,
                CreatedAt = DateTime.UtcNow
            };

            entry.Lines.Add(line);
        }

        return entry;
    }
}
