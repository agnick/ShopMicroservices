namespace OrderService.Domain.Enums;

/// <summary>
/// Статус заказа.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Заказ создан, но ещё не оплачен.
    /// </summary>
    New = 0,

    /// <summary>
    /// Заказ успешно оплачен.
    /// </summary>
    Finished = 1,

    /// <summary>
    /// Оплата заказа не удалась.
    /// </summary>
    Cancelled = 2
}