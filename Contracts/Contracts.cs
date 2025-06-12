namespace Contracts;

/// <summary>Задача на оплату заказа (Order → Payments).</summary>
public record PaymentTask(Guid OrderId, Guid UserId, decimal Amount);

/// <summary>Событие о результате оплаты (Payments → Order).</summary>
public record OrderPaymentProcessed(Guid OrderId, Guid UserId, bool Success);