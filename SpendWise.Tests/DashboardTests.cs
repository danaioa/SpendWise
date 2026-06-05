using Xunit;

namespace SpendWise.Tests;

public class DashboardTests
{
    [Fact]
    public void RemainingBalance_ShouldBeIncomeMinusExpenses()
    {
        decimal monthlyIncome = 5000;
        decimal totalExpenses = 3200;

        decimal remainingBalance = monthlyIncome - totalExpenses;

        Assert.Equal(1800, remainingBalance);
    }

    [Fact]
    public void BudgetAmount_ShouldBeCalculatedFromPercentage()
    {
        decimal monthlyIncome = 5000;
        decimal percentage = 20;

        decimal budgetAmount = monthlyIncome * percentage / 100;

        Assert.Equal(1000, budgetAmount);
    }

    [Fact]
    public void TotalBudgetPercentages_ShouldNotExceed100()
    {
        decimal food = 40;
        decimal transport = 20;
        decimal shopping = 30;
        decimal fun = 10;

        decimal total = food + transport + shopping+fun;

        Assert.True(total == 100);
    }

    [Fact]
        public void BudgetPercentage_ShouldBeInvalid_WhenGreaterThan100()
        {
            decimal percentage = 120;

            bool isValid = percentage >= 0 && percentage <= 100;

            Assert.False(isValid);
        }


        [Fact]
        public void TotalExpenses_ShouldBeSumOfTransactions()
        {
            decimal rent = 1200;
            decimal food = 800;
            decimal transport = 300;

            decimal totalExpenses = rent + food + transport;

            Assert.Equal(2300, totalExpenses);
        }
}