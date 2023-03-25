﻿namespace MPay.Core.Repository;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
    Task<Purchase> GetAsync(string id);
}