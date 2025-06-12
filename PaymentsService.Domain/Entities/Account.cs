using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentsService.Domain.Entities;

/// <summary>
/// Счёт пользователя для хранения баланса.
/// </summary>
public class Account
{
    /// <summary>
    /// Идентификатор пользователя, одновременно первичный ключ счёта.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid UserId { get; set; }

    /// <summary>
    /// Текущий баланс счёта.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Номер версии строки для оптимистической блокировки.
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}