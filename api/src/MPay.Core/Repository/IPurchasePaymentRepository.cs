﻿using MPay.Core.Entities;

namespace MPay.Core.Repository;

public interface IPurchasePaymentRepository
{
    Task AddAsync(PurchasePayment purchasePayment);
}