using System;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Contract for per-CurrencyType balances. Pure interface, no base class — not registered
    /// by ServiceRegistry.InstallDefaults(); a concrete project implements and registers its own.
    /// </summary>
    public interface ICurrencyService
    {
        int GetBalance(CurrencyType type);

        bool TrySpend(CurrencyType type, int amount);

        void AddCurrency(CurrencyType type, int amount);

        event Action<CurrencyType, int> OnBalanceChanged;
    }
}
