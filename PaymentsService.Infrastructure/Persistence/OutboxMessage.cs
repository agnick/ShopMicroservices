using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Infrastructure.Persistence;

/// <summary>
/// Исходящее сообщение transactional outbox.
/// </summary>
public class OutboxMessage
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public bool Processed { get; set; }
}