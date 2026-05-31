import { useEffect, useMemo, useState } from "react";
import axiosClient from "../api/axiosClient";

type RecurringPayment = {
  id: number;
  name: string;
  amount: number;
  dayOfMonth: number;
  categoryId?: number;
};

function RecurringPage() {
  const [payments, setPayments] = useState<RecurringPayment[]>([]);
  const [name, setName] = useState("");
  const [amount, setAmount] = useState("");
  const [dayOfMonth, setDayOfMonth] = useState("1");
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState("");

  const loadPayments = async () => {
    try {
      setIsLoading(true);
      const response = await axiosClient.get("/RecurringTransactions");
      setPayments(response.data);
    } catch (error) {
      console.error(error);
      setMessage("Could not load recurring payments.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadPayments();
  }, []);

  const totalMonthly = useMemo(() => {
    return payments.reduce((sum, payment) => sum + Number(payment.amount), 0);
  }, [payments]);

  const yearlyCost = totalMonthly * 12;

  const addPayment = async () => {
    if (!name.trim() || !amount || !dayOfMonth) {
      setMessage("Please complete all fields.");
      return;
    }

    try {
      setMessage("");

     await axiosClient.post("/RecurringTransactions", {
        name,
        amount: Number(amount),
        dayOfMonth: Number(dayOfMonth),
      });

      setName("");
      setAmount("");
      setDayOfMonth("1");

      await loadPayments();
    } catch (error) {
      console.error(error);
      setMessage("Could not add recurring payment.");
    }
  };

  const updatePayment = async (
    payment: RecurringPayment,
    field: keyof RecurringPayment,
    value: string
  ) => {
    const updatedPayment = {
      ...payment,
      [field]:
        field === "amount" || field === "dayOfMonth" ? Number(value) : value,
    };

    setPayments((current) =>
      current.map((item) => (item.id === payment.id ? updatedPayment : item))
    );

    try {
      await axiosClient.put(`/RecurringTransactions/${payment.id}`, {
        name: updatedPayment.name,
        amount: Number(updatedPayment.amount),
        dayOfMonth: Number(updatedPayment.dayOfMonth),
        
      });
    } catch (error) {
      console.error(error);
      setMessage("Could not update payment.");
      await loadPayments();
    }
  };

  const deletePayment = async (id: number) => {
    try {
      await axiosClient.delete(`/RecurringTransactions/${id}`);
      setPayments((current) => current.filter((payment) => payment.id !== id));
    } catch (error) {
      console.error(error);
      setMessage("Could not delete payment.");
    }
  };

  return (
    <div className="recurring-page">
      <section className="recurring-hero">
        <div>
          <span className="recurring-eyebrow">Fixed monthly costs</span>
          <h1>Recurring Payments</h1>
          <p>
            Add rent, subscriptions, phone bills or any repeated payment. These
            costs are saved to your account and used to understand your real
            monthly spending.
          </p>
        </div>

        <div className="recurring-quote">
          <strong>Small payments add up.</strong>
          <span>Know your fixed costs before planning the rest.</span>
        </div>
      </section>

      <section className="recurring-summary-grid">
        <div className="recurring-summary-card">
          <span>Total monthly</span>
          <strong>{totalMonthly.toFixed(0)} lei</strong>
        </div>

        <div className="recurring-summary-card">
          <span>Total yearly</span>
          <strong>{yearlyCost.toFixed(0)} lei</strong>
        </div>

        <div className="recurring-summary-card">
          <span>Active payments</span>
          <strong>{payments.length}</strong>
        </div>
      </section>

      <section className="recurring-form-card">
        <h2>Add recurring payment</h2>

        {message && <p className="form-error">{message}</p>}

        <div className="recurring-form-grid no-category">
          <input
            type="text"
            placeholder="What do you pay for?"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />

          <input
            type="number"
            placeholder="Amount"
            min="0"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
          />

          <input
            type="number"
            placeholder="Day of month"
            min="1"
            max="31"
            value={dayOfMonth}
            onChange={(e) => setDayOfMonth(e.target.value)}
          />

          <button type="button" onClick={addPayment}>
            Add payment
          </button>
        </div>
      </section>

      <section className="recurring-list">
        {isLoading ? (
          <div className="recurring-empty">
            <h3>Loading payments...</h3>
          </div>
        ) : payments.length === 0 ? (
          <div className="recurring-empty">
            <h3>No recurring payments yet</h3>
            <p>Add your first monthly payment to start tracking fixed costs.</p>
          </div>
        ) : (
          payments.map((payment) => (
            <article className="recurring-card" key={payment.id}>
              <div className="recurring-card-main">
                <div>
                  <h3>{payment.name}</h3>
                  <p>Charged every month on day {payment.dayOfMonth}</p>
                </div>

                <strong>{payment.amount} lei</strong>
              </div>

              <div className="recurring-edit-grid no-category">
                <input
                  type="text"
                  value={payment.name}
                  onChange={(e) =>
                    updatePayment(payment, "name", e.target.value)
                  }
                />

                <input
                  type="number"
                  min="0"
                  value={payment.amount}
                  onChange={(e) =>
                    updatePayment(payment, "amount", e.target.value)
                  }
                />

                <input
                  type="number"
                  min="1"
                  max="31"
                  value={payment.dayOfMonth}
                  onChange={(e) =>
                    updatePayment(payment, "dayOfMonth", e.target.value)
                  }
                />

                <button
                  type="button"
                  className="delete-payment-btn"
                  onClick={() => deletePayment(payment.id)}
                >
                  Delete
                </button>
              </div>
            </article>
          ))
        )}
      </section>
    </div>
  );
}

export default RecurringPage;