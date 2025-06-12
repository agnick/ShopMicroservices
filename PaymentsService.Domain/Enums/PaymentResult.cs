namespace PaymentsService.Domain.Enums;

/// <summary>
/// Результат проведения платежа за заказ.
/// </summary>
public enum PaymentResult
{
    /// <summary>
    /// Платёж выполнен успешно.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Недостаточно средств на счёте.
    /// </summary>
    NotEnoughFunds = 1,

    /// <summary>
    /// Счёт пользователя отсутствует.
    /// </summary>
    AccountNotFound = 2
}