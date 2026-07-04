using JustAnotherWednesday;

namespace JustAnotherWednesday.Data;

public static class RoleData
{
    public record FinancialProfile(decimal Debt, decimal Savings, decimal MonthlyIncome);

    public static readonly Dictionary<Role, FinancialProfile> Profiles = new()
    {
        { Role.Striver, new FinancialProfile(20000, 15000, 4000) },
        { Role.Landlord, new FinancialProfile(600000, 40000, 8000) },
        { Role.Retiree, new FinancialProfile(0, 50000, 3000) },
        { Role.Farmer, new FinancialProfile(120000, 5000, 2000) },
        { Role.GigWorker, new FinancialProfile(8000, 2000, 2500) },
        { Role.ShopOwner, new FinancialProfile(50000, 25000, 6000) }
    };
}