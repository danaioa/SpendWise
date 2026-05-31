import { useEffect, useMemo, useState } from "react";
import { PiggyBank, Wallet, CalendarCheck } from "lucide-react";
import axiosClient from "../api/axiosClient";



type MonthlySaving = {
  id: number;
  month: number;
  year: number;
  amount: number;
};

function SavingsPage() {
  const [remainingBalance, setRemainingBalance] = useState(0);
  const [savings, setSavings] = useState<MonthlySaving[]>([]);
  const [message, setMessage] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  const loadSavingsData = async () => {
    try {
      setIsLoading(true);

      const dashboardResponse = await axiosClient.get("/Dashboard");
      const savingsResponse = await axiosClient.get("/MonthlySavings");

      setRemainingBalance(Number(dashboardResponse.data.remainingBalance));
      setSavings(savingsResponse.data);
    } catch (error) {
      console.error(error);
      setMessage("Could not load savings data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadSavingsData();
  }, []);

  const totalSaved = useMemo(() => {
    return savings.reduce((sum, item) => sum + Number(item.amount), 0);
  }, [savings]);

  const saveCurrentMonth = async () => {
    try {
      setMessage("");

      await axiosClient.post("/MonthlySavings/close-month");

      setMessage("Current month balance was saved successfully.");
      await loadSavingsData();
    } catch (error: any) {
      console.error(error);
      setMessage(
        error.response?.data || "Could not save this month. It may already be saved."
      );
    }
  };

  const formatMonth = (month: number, year: number) => {
    return new Date(year, month - 1).toLocaleDateString("en-US", {
      month: "long",
      year: "numeric",
    });
  };

  if (isLoading) {
    return <div className="savings-page">Loading savings...</div>;
  }

  return (
    <div className="savings-page">
      <section className="savings-hero">
        <div>
          <span className="savings-eyebrow">Savings history</span>
          <h1>Money saved over time</h1>
          <p>
            Close each month and keep track of how much money remained after all
            expenses and recurring payments.
          </p>
        </div>
      </section>

      <section className="savings-summary-grid">
        <div className="savings-summary-card">
          <div className="stat-icon income-icon">
            <Wallet />
          </div>
          <span>Current month balance</span>
          <strong>{remainingBalance.toFixed(0)} lei</strong>
        </div>

        <div className="savings-summary-card">
          <div className="stat-icon balance-icon">
            <PiggyBank />
          </div>
          <span>Total saved</span>
          <strong>{totalSaved.toFixed(0)} lei</strong>
        </div>

        <div className="savings-summary-card">
          <div className="stat-icon income-icon">
            <CalendarCheck />
          </div>
          <span>Saved months</span>
          <strong>{savings.length}</strong>
        </div>
      </section>

      <section className="savings-action-card">
        <div>
          <h2>Close current month</h2>
          <p>
            Save the current remaining balance as this month&apos;s saved amount.
          </p>
        </div>

        <button type="button" onClick={saveCurrentMonth}>
          Save current month
        </button>
      </section>

      {message && <p className="savings-message">{message}</p>}

      <section className="savings-history-section">
        <div className="dashboard-section-title">
          <h2>Savings history</h2>
          <p>Your saved balance for each closed month.</p>
        </div>

        <div className="savings-history-grid">
          {savings.length === 0 ? (
            <div className="savings-history-card">
              <h3>No savings recorded yet</h3>
              <p>Save the current month to start building your history.</p>
            </div>
          ) : (
            savings.map((saving) => (
              <article className="savings-history-card" key={saving.id}>
                <span>{formatMonth(saving.month, saving.year)}</span>
                <strong>{Number(saving.amount).toFixed(0)} lei</strong>
                <p>Saved at month close</p>
              </article>
            ))
          )}
        </div>
      </section>
    </div>
  );
}

export default SavingsPage;