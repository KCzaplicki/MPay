﻿using MPay.Core.Entities;

namespace MPay.Core.Policies.PurchasePaymentStatus;

internal class PurchasePaymentStatusInvalidCardPolicy : IPurchasePaymentStatusPolicy
{
    public int Priority => 10;

    public bool CanApply(PurchasePayment purchasePayment)
    {
        return purchasePayment.Status == default && purchasePayment.CardNumber.ToString().EndsWith("1");
    }

    public void Apply(PurchasePayment purchasePayment)
    {
        purchasePayment.Status = Entities.PurchasePaymentStatus.InvalidCard;
    }
}