import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";
import {
  Apple,
  Bus,
  PartyPopper,
  ShieldAlert,
  Wallet,
  TrendingDown,
  PiggyBank,
} from "lucide-react";

type CategoryExpense = {
  name: string;
  budgetAmount: number;
  spentAmount: number;
};

type DashboardData = {
  monthlyIncome: number;
  totalExpenses: number;
  remainingBalance: number;
  categoryExpenses: CategoryExpense[];
};

const visibleCategories = ["Food", "Transport", "Unexpected", "Fun"];

const categoryIcons = {
  Food: Apple,
  Transport: Bus,
  Unexpected: ShieldAlert,
  Fun: PartyPopper,
};

function Dashboard() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        const response = await axiosClient.get("/Dashboard");
        setData(response.data);
      } catch {
        setError("Could not load dashboard data.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, []);

  if (loading) {
    return <div className="dashboard-page">Loading...</div>;
  }

  if (error || !data) {
    return (
      <div className="dashboard-page">
        {error || "No dashboard data found."}
      </div>
    );
  }

  const filteredCategories = data.categoryExpenses.filter((category) =>
    visibleCategories.includes(category.name)
  );

  return (
    <div className="dashboard-page">
      <section className="dashboard-hero">
        <div>
          <span className="dashboard-eyebrow">Monthly overview</span>
          <h1>SpendWise</h1>
          <p>Track your balance and see where your money goes this month.</p>
        </div>
      </section>

      <div className="dashboard-grid">
        <div className="stat-card">
          <div className="stat-icon income-icon">
            <Wallet />
          </div>
          <span>Monthly income</span>
          <strong>{data.monthlyIncome.toFixed(0)} lei</strong>
        </div>

        <div className="stat-card">
          <div className="stat-icon expense-icon">
            <TrendingDown />
          </div>
          <span>Total spent</span>
          <strong>{data.totalExpenses.toFixed(0)} lei</strong>
        </div>

        <div className="stat-card">
          <div className="stat-icon balance-icon">
            <PiggyBank />
          </div>
          <span>Remaining balance</span>
          <strong>{data.remainingBalance.toFixed(0)} lei</strong>
        </div>
      </div>

      <section className="dashboard-spending-section">
        <div className="dashboard-section-title">
          <div>
            <h2>Spending by category</h2>
            <p>Your spending compared with your monthly budget.</p>
          </div>
        </div>

        <div className="dashboard-category-grid">
          {filteredCategories.map((category) => {
            const left = category.budgetAmount - category.spentAmount;
            const usedPercentage =
              category.budgetAmount > 0
                ? (category.spentAmount / category.budgetAmount) * 100
                : 0;

            const isOver = left < 0;
            const Icon =
              categoryIcons[category.name as keyof typeof categoryIcons];

            return (
              <article
                className={
                  isOver
                    ? "dashboard-category-card over-budget"
                    : "dashboard-category-card"
                }
                key={category.name}
              >
                <div
                  className={`category-icon category-${category.name.toLowerCase()}`}
                >
                  {Icon && <Icon />}
                </div>

                <div className="dashboard-category-header">
                  <div>
                    <h3>{category.name}</h3>
                    <p>
                      Spent {category.spentAmount.toFixed(0)} lei from{" "}
                      {category.budgetAmount.toFixed(0)} lei
                    </p>
                  </div>

                  <strong className={isOver ? "danger-text" : "success-text"}>
                    {isOver
                      ? `${Math.abs(left).toFixed(0)} lei over`
                      : `${left.toFixed(0)} lei left`}
                  </strong>
                </div>

                <div className="dashboard-progress">
                  <div style={{ width: `${Math.min(usedPercentage, 100)}%` }} />
                </div>

                <span className="dashboard-card-note">
                  {usedPercentage.toFixed(0)}% used
                </span>
              </article>
            );
          })}
        </div>
      </section>
    </div>
  );
}

export default Dashboard;
