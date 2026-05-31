import { useEffect, useMemo, useState } from "react";
import axiosClient from "../api/axiosClient";

type BudgetCategory = {
  id: number;
  name: string;
  description: string;
  percentage: number;
};

type RecurringPayment = {
  id: number;
  name: string;
  amount: number;
  dayOfMonth: number;
};

const monthlyIncome = 6000;

const getBudgetDescription = (name: string) => {
  if (name === "Food") return "Groceries, meals and daily food expenses";
  if (name === "Transport") return "Fuel, bus, taxi or public transport";
  if (name === "Savings") return "Money kept for your future goals";
  if (name === "Unexpected") return "Emergency money for unplanned costs";
  if (name === "Fun") return "Entertainment, hobbies and going out";

  return "Monthly budget category";
};

function BudgetsPage() {
  const [budgets, setBudgets] = useState<BudgetCategory[]>([]);
  const [recurringPayments, setRecurringPayments] = useState<RecurringPayment[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const loadBudgets = async () => {
    try {
      setIsLoading(true);

      const response = await axiosClient.get("/Budgets");

      const budgetsFromApi = response.data
        .filter((budget: any) => budget.name !== "Bills & Subscriptions")
        .map((budget: any) => ({
          id: budget.id,
          name: budget.name,
          percentage: Number(budget.percentage),
          description: getBudgetDescription(budget.name),
        }));

      setBudgets(budgetsFromApi);
    } catch (error) {
      console.error("Could not load budgets", error);
    } finally {
      setIsLoading(false);
    }
  };

  const loadRecurringPayments = async () => {
    try {
      const response = await axiosClient.get("/RecurringTransactions");
      setRecurringPayments(response.data);
    } catch (error) {
      console.error("Could not load recurring payments", error);
    }
  };

  useEffect(() => {
    loadBudgets();
    loadRecurringPayments();
  }, []);

  const recurringTotal = useMemo(() => {
    return recurringPayments.reduce(
      (sum, payment) => sum + Number(payment.amount),
      0
    );
  }, [recurringPayments]);

  const availableForBudgets = monthlyIncome - recurringTotal;

  const recurringPercentage =
    monthlyIncome > 0 ? (recurringTotal / monthlyIncome) * 100 : 0;

  const totalPercentage = useMemo(() => {
    return budgets.reduce((sum, budget) => sum + Number(budget.percentage), 0);
  }, [budgets]);

  const allocatedAmount = (availableForBudgets * totalPercentage) / 100;
  const remainingAmount = availableForBudgets - allocatedAmount;
  const remainingPercentage = 100 - totalPercentage;

  const updateBudgetPercentage = async (id: number, value: string) => {
    const numericValue = Number(value);

    if (Number.isNaN(numericValue)) {
      return;
    }

    const safeValue = Math.max(0, Math.min(100, numericValue));

    setBudgets((currentBudgets) =>
      currentBudgets.map((budget) =>
        budget.id === id ? { ...budget, percentage: safeValue } : budget
      )
    );

    try {
      await axiosClient.put(`/Budgets/${id}`, {
        percentage: safeValue,
      });

      await loadBudgets();
    } catch (error) {
      console.error("Could not update budget", error);
    }
  };

  const resetBudgets = async () => {
    await loadBudgets();
  };

  return (
    <div className="budgets-page">
      <section className="budgets-hero">
        <div>
          <span className="budgets-eyebrow">Monthly planning</span>
          <h1>Plan what remains after fixed costs.</h1>
          <p>
            Your recurring payments are removed first. The rest of your money is
            then split between food, transport, savings, unexpected costs and fun.
          </p>
        </div>

        <div className="budgets-hero-card">
          <span>Available for budgets</span>
          <strong>{availableForBudgets.toFixed(0)} lei</strong>
          <p>
            Income {monthlyIncome} lei • Recurring {recurringTotal.toFixed(0)} lei
          </p>
        </div>
      </section>

      <section className="budget-summary-grid">
        <div className="budget-summary-card">
          <span>Recurring payments</span>
          <strong>{recurringTotal.toFixed(0)} lei</strong>
          <p>{recurringPercentage.toFixed(1)}% of monthly income</p>
        </div>

        <div className="budget-summary-card">
          <span>Allocated from remaining</span>
          <strong>{totalPercentage.toFixed(0)}%</strong>
          <p>{allocatedAmount.toFixed(0)} lei planned</p>
        </div>

        <div className="budget-summary-card">
          <span>Left after budget plan</span>
          <strong className={remainingAmount < 0 ? "danger-text" : "success-text"}>
            {remainingAmount.toFixed(0)} lei
          </strong>
          <p>{remainingPercentage.toFixed(0)}% of available money</p>
        </div>
      </section>

      {totalPercentage > 100 && (
        <div className="budget-warning">
          Your budget plan uses more than the money available after recurring
          payments. Reduce one or more categories.
        </div>
      )}

      <div className="budget-toolbar">
        <div>
          <h2>Budget categories</h2>
          <p>
            Bills & Subscriptions is calculated automatically from recurring
            payments. The other categories are saved in your account.
          </p>
        </div>

        <button type="button" onClick={resetBudgets}>
          Reload plan
        </button>
      </div>

      <section className="budget-category-grid">
        <article className="budget-plan-card budget-auto-card">
          <div className="budget-plan-top">
            <div>
              <h3>Bills & Subscriptions</h3>
              <p>Automatically calculated from your recurring payments.</p>
            </div>

            <strong>{recurringTotal.toFixed(0)} lei</strong>
          </div>

          <div className="budget-progress">
            <div style={{ width: `${Math.min(recurringPercentage, 100)}%` }} />
          </div>

          <div className="budget-edit-row">
            <label>Auto percentage</label>
            <div className="budget-input-group">
              <input type="number" value={recurringPercentage.toFixed(1)} disabled />
              <span>%</span>
            </div>
          </div>
        </article>

        {isLoading ? (
          <div className="recurring-empty">
            <h3>Loading budgets...</h3>
          </div>
        ) : (
          budgets.map((budget) => {
            const amount = (availableForBudgets * budget.percentage) / 100;

            return (
              <article className="budget-plan-card" key={budget.id}>
                <div className="budget-plan-top">
                  <div>
                    <h3>{budget.name}</h3>
                    <p>{budget.description}</p>
                  </div>

                  <strong>{amount.toFixed(0)} lei</strong>
                </div>

                <div className="budget-progress">
                  <div style={{ width: `${Math.min(budget.percentage, 100)}%` }} />
                </div>

                <div className="budget-edit-row">
                  <label htmlFor={`budget-${budget.id}`}>Percentage</label>

                  <div className="budget-input-group">
                    <input
                      id={`budget-${budget.id}`}
                      type="number"
                      min="0"
                      max="100"
                      value={budget.percentage}
                      onChange={(e) =>
                        updateBudgetPercentage(budget.id, e.target.value)
                      }
                    />
                    <span>%</span>
                  </div>
                </div>
              </article>
            );
          })
        )}
      </section>
    </div>
  );
}

export default BudgetsPage;