using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Infrastructure.Persistence;

/// <summary>
/// Входящее сообщение transactional inbox.
/// </summary>
public class InboxMessage
{
    [Key] public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public bool Processed { get; set; }
}