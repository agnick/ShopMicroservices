using System.ComponentModel.DataAnnotations;

namespace OrderService.Infrastructure.Persistence;

/// <summary>
/// Запись transactional outbox.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Идентификатор записи.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Имя типа сообщения.
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// Тело сообщения в формате JSON.
    /// </summary>
    public string Payload { get; set; } = default!;

    /// <summary>
    /// Отметка обработки сообщения.
    /// </summary>
    public bool Processed { get; set; }
}