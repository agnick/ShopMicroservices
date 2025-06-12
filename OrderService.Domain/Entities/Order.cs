using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

/// <summary>
/// Заказ пользователя магазина.
/// </summary>
public class Order
{
    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Сумма заказа.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Описание заказа.
    /// </summary>
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Текущий статус заказа.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.New;

    /// <summary>
    /// Номер версии строки для оптимистической блокировки.
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}