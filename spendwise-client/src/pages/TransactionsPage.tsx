import { useEffect, useMemo, useState } from "react";
import axiosClient from "../api/axiosClient";

type Transaction = {
  id: number;
  description: string;
  amount: number;
  date: string;
  categoryName: string;
  isExpense: boolean;
};

type RecurringPayment = {
  id: number;
  name: string;
  amount: number;
  dayOfMonth: number;
};

type TransactionRow = {
  id: string;
  date: string;
  description: string;
  categoryName: string;
  amount: number;
  isExpense: boolean;
  type: "transaction" | "recurring" | "salary";
};

function TransactionsPage() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [recurringPayments, setRecurringPayments] = useState<RecurringPayment[]>([]);
  const [monthlyIncome, setMonthlyIncome] = useState(0);
  const [selectedDate, setSelectedDate] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const loadData = async () => {
    try {
      setIsLoading(true);

      const transactionsResponse = await axiosClient.get("/Transactions", {
        params: selectedDate ? { date: selectedDate } : {},
      });

      const recurringResponse = await axiosClient.get("/RecurringTransactions");
      const dashboardResponse = await axiosClient.get("/Dashboard");

      setTransactions(transactionsResponse.data);
      setRecurringPayments(recurringResponse.data);
      setMonthlyIncome(Number(dashboardResponse.data.monthlyIncome));
    } catch (error) {
      console.error("Could not load transactions", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const rows: TransactionRow[] = useMemo(() => {
    const today = new Date();

    const salaryRow: TransactionRow = {
      id: "salary-monthly",
      date: new Date(today.getFullYear(), today.getMonth(), 1).toISOString(),
      description: "Monthly salary",
      categoryName: "Salary",
      amount: monthlyIncome,
      isExpense: false,
      type: "salary",
    };

    const normalRows: TransactionRow[] = transactions.map((transaction) => ({
      id: `transaction-${transaction.id}`,
      date: transaction.date,
      description: transaction.description,
      categoryName: transaction.categoryName,
      amount: Number(transaction.amount),
      isExpense: transaction.isExpense,
      type: "transaction",
    }));

    const recurringRows: TransactionRow[] = recurringPayments.map((payment) => ({
      id: `recurring-${payment.id}`,
      date: new Date(
        today.getFullYear(),
        today.getMonth(),
        payment.dayOfMonth
      ).toISOString(),
      description: payment.name,
      categoryName: "Recurring payment",
      amount: Number(payment.amount),
      isExpense: true,
      type: "recurring",
    }));

    const allRows =
      monthlyIncome > 0
        ? [salaryRow, ...normalRows, ...recurringRows]
        : [...normalRows, ...recurringRows];

    const filteredRows = selectedDate
      ? allRows.filter((row) => {
          const rowDate = new Date(row.date).toISOString().split("T")[0];
          return rowDate === selectedDate;
        })
      : allRows;

    return filteredRows.sort(
      (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
    );
  }, [transactions, recurringPayments, monthlyIncome, selectedDate]);

  const totalIncome = rows
    .filter((row) => !row.isExpense)
    .reduce((sum, row) => sum + Number(row.amount), 0);

  const totalExpenses = rows
    .filter((row) => row.isExpense)
    .reduce((sum, row) => sum + Number(row.amount), 0);

  const balance = totalIncome - totalExpenses;

  return (
    <div className="transactions-page">
      <section className="transactions-hero">
        <div>
          <span className="transactions-eyebrow">Money movement</span>
          <h1>Transactions</h1>
          <p>
            View your monthly salary, transactions and recurring payments in one
            place.
          </p>
        </div>
      </section>

      <section className="transactions-summary-grid">
        <div className="transactions-summary-card">
          <span>Income</span>
          <strong className="success-text">+{totalIncome.toFixed(0)} lei</strong>
        </div>

        <div className="transactions-summary-card">
          <span>Expenses</span>
          <strong className="danger-text">-{totalExpenses.toFixed(0)} lei</strong>
        </div>

        <div className="transactions-summary-card">
          <span>Balance</span>
          <strong>{balance.toFixed(0)} lei</strong>
        </div>
      </section>

      <section className="transactions-toolbar">
        <div>
          <h2>Your activity</h2>
          <p>Salary and recurring payments are included automatically.</p>
        </div>

        <div className="transactions-filter">
          <input
            type="date"
            value={selectedDate}
            onChange={(e) => setSelectedDate(e.target.value)}
          />

          <button type="button" onClick={loadData}>
            Filter
          </button>

          {selectedDate && (
            <button
              type="button"
              className="secondary-action"
              onClick={() => {
                setSelectedDate("");
                setTimeout(loadData, 0);
              }}
            >
              Clear
            </button>
          )}
        </div>
      </section>

      <section className="transactions-table-card">
        {isLoading ? (
          <p className="transactions-empty">Loading transactions...</p>
        ) : rows.length === 0 ? (
          <p className="transactions-empty">No transactions for this date.</p>
        ) : (
          <table className="transactions-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Description</th>
                <th>Category</th>
                <th>Type</th>
                <th>Amount</th>
              </tr>
            </thead>

            <tbody>
              {rows.map((row) => (
                <tr key={row.id}>
                  <td>{new Date(row.date).toLocaleDateString()}</td>
                  <td>{row.description}</td>
                  <td>{row.categoryName}</td>
                  <td>
                    <span
                      className={
                        row.type === "recurring"
                          ? "transaction-badge recurring"
                          : row.type === "salary"
                          ? "transaction-badge salary"
                          : "transaction-badge"
                      }
                    >
                      {row.type === "recurring"
                        ? "Recurring"
                        : row.type === "salary"
                        ? "Salary"
                        : "Normal"}
                    </span>
                  </td>
                  <td className={row.isExpense ? "amount-expense" : "amount-income"}>
                    {row.isExpense ? "-" : "+"}
                    {Number(row.amount).toFixed(0)} lei
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}

export default TransactionsPage;