function LandingPage() {
  return (
    <div className="landing-page">
      <nav className="landing-nav">
        <div className="landing-logo">
          <div className="logo-icon">S</div>
          <span>SpendWise</span>
        </div>

        <div className="landing-nav-links">
          <a href="/login">Login</a>
          <a href="/register" className="landing-nav-button">
            Create account
          </a>
        </div>
      </nav>

      <main className="landing-content">
        <section className="landing-hero">
          <div className="landing-copy">
            <span className="landing-badge">Smart personal finance app</span>

            <h1>Manage your money with confidence.</h1>

            <p>
              Track expenses, plan budgets and scan products from one clean
              dashboard.
            </p>

            <div className="landing-actions">
              <a href="/register">Get started</a>
              <a href="/login" className="secondary">
                Login
              </a>
            </div>
          </div>

          <div className="landing-preview">
            <div className="preview-header">
              <span>Monthly overview</span>
              <strong>May 2026</strong>
            </div>

            <div className="preview-card">
              <span>Monthly income</span>
              <strong>6000 lei</strong>
            </div>

            <div className="preview-card">
              <span>Total expenses</span>
              <strong>1450 lei</strong>
            </div>

            <div className="preview-card green">
              <span>Remaining balance</span>
              <strong>4550 lei</strong>
            </div>
          </div>
        </section>

        <section className="landing-features">
          <div>
            <span>01</span>
            <h3>Track expenses</h3>
            <p>Organize transactions by category.</p>
          </div>

          <div>
            <span>02</span>
            <h3>Plan budgets</h3>
            <p>Create monthly spending limits.</p>
          </div>

          <div>
            <span>03</span>
            <h3>Scan products</h3>
            <p>Add products faster with barcode scanning.</p>
          </div>
        </section>
      </main>
    </div>
  );
}

export default LandingPage;