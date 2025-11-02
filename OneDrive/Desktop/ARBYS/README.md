<div align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./GitHub_README_Assets/logo_dark.svg" />
    <source media="(prefers-color-scheme: light)" srcset="./GitHub_README_Assets/logo_light.svg" />
    <img alt="Redline Arbitrage Logo" src="./GitHub_README_Assets/logo_light.svg" width="160" />
  </picture>
  
  <br/>
  
  <img src="./GitHub_README_Assets/readme_banner.png" alt="Redline Arbitrage Banner" />
  
  <p>
    <a href="https://github.com/Snapwave333/Redline_Arb/actions/workflows/ci.yml">
      <img alt="CI" src="https://img.shields.io/github/actions/workflow/status/Snapwave333/Redline_Arb/ci.yml?label=CI&logo=githubactions&labelColor=%230D0D0F&color=%23FF0033" />
    </a>
    <img alt="Python" src="https://img.shields.io/badge/Python-3.10%2B-%23FF0033?logo=python&labelColor=%230D0D0F&color=%23FF0033" />
    <img alt="GUI" src="https://img.shields.io/badge/GUI-PyQt6-%23FF0033?logo=qt&labelColor=%230D0D0F&color=%23FF0033" />
    <img alt="Web" src="https://img.shields.io/badge/Web-React%2BTS-%23FF0033?logo=react&labelColor=%230D0D0F&color=%23FF0033" />
    <img alt="Mobile" src="https://img.shields.io/badge/Mobile-PWA-%23FF0033?logo=pwa&labelColor=%230D0D0F&color=%23FF0033" />
    <img alt="License" src="https://img.shields.io/badge/License-MIT-%23FF0033?labelColor=%230D0D0F&color=%23FF0033" />
    <img alt="Style" src="https://img.shields.io/badge/Style-Rajdhani%20%2B%20Orbitron-%23FF0033?labelColor=%230D0D0F&color=%23FF0033" />
  </p>
</div>

---

# 🚀 Redline Arbitrage

Precision sports-arbitrage engine with a modern PyQt6 interface, tuned for performance and reliability. Built for rapid market scanning, robust account health monitoring, and one-click execution workflows. Brand-focused presentation using the Redline palette: `#FF0033` (accent), `#0D0D0F` (charcoal), `#FFFFFF` (white).

<details>
<summary><strong>🧭 Table of Contents</strong></summary>

- [About](#-about)
- [Features](#-features)
- [Installation](#-installation)
- [Usage](#-usage)
- [Screenshots](#-screenshots)
- [Tech Stack](#-tech-stack)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

</details>

## 🎯 About

**ARBYS (Redline Arbitrage) is a sports arbitrage betting toolkit exclusively focused on sports markets.** This project contains **NO DeFi, cryptocurrency, blockchain, or meme coin content**. All functionality is related to legitimate sports arbitrage betting across traditional sportsbooks.

Redline Arbitrage is an arbitrage research and execution toolkit featuring:
- A performant orchestration layer to aggregate odds and metadata from multiple sources.
- A polished PyQt6 desktop UI with DPI-aware scaling and theme styling.
- Clean architecture with testing, linting, and packaging pipelines included.

The application targets sports markets, providing rapid arbitrage detection, stake calculations, and health checks across accounts to mitigate risk while maintaining speed.

### ⚠️ Project Scope
- ✅ **Sports Arbitrage**: Football, basketball, tennis, soccer, and other sports
- ✅ **Bookmaker Integration**: Bet365, William Hill, and other licensed sportsbooks
- ✅ **Odds Aggregation**: SportRadar, SofaScore, OddsAPI, and sports data providers
- ❌ **No DeFi/Crypto**: This project does not contain any blockchain or cryptocurrency functionality
- ❌ **No Meme Coins**: No token discovery, DEX integrations, or Web3 features

## 🧰 Features

- ⚡ High-performance data orchestrator (async-ready variants available)
- 📈 Arbitrage calculation and optimized stake distribution
- 🧬 Account health scoring with a dedicated dashboard
- 🧩 Modular provider interfaces (TheOddsAPI, Sportradar, and more)
- 💾 Historical data storage utilities
- 🎛️ Theming (Rajdhani + Orbitron-inspired visual style) and carbon fiber UI accents
- 🪪 First-run onboarding, splash, and setup wizard
- 🧪 Comprehensive test suite (unit, perf, integration)
- 📦 Portable build and Windows installer scripts

## 🛠 Installation

Choose one of the following:

### Desktop Application

1) Windows Binary (recommended)
- Download the latest release `Redline_Arbitrage.exe` and run.

2) Portable ZIP
- Download `Redline_Arbitrage.zip`, extract, and run `Redline_Arbitrage.exe` inside the folder. Configuration files will be stored locally inside the `portable_data` directory.

3) From Source
- Requirements: Python 3.10+, Windows/macOS/Linux
- Clone the repo and install dependencies:

```bash
git clone https://github.com/Snapwave333/Redline_Arb.git
cd Redline_Arb
python -m venv .venv && .venv\Scripts\activate
pip install -r requirements.txt
python main.py
```

### 🌐 Mobile Web App (Browser-Based)

Run Redline Arbitrage directly in your browser with full offline functionality:

- **Requirements**: Node.js 18+, modern web browser
- **Platform**: iOS Safari, Android Chrome, desktop browsers
- **Storage**: All data stored locally in browser (IndexedDB)

```bash
cd mobile_web_app
npm install
npm run dev
# Open http://localhost:3000 in your browser
```

**Features**:
- ⚡ Full arbitrage detection and stake calculation
- 📱 Mobile-optimized responsive interface
- 🔒 Complete offline functionality
- 💾 Local data storage (no server required)
- 📲 PWA-ready for app-like experience

[📖 Mobile App Documentation](mobile_web_app/README.md)

#### 🧪 Testing the Mobile Web App

From `mobile_web_app/`:

```bash
npm install
npx playwright install

# Unit & component tests with coverage
npm run test

# Production build + preview server
npm run build
npm run preview   # http://localhost:4173/

# E2E tests against preview server
npm run test:e2e

# Performance benchmarks (tagged @perf)
npm run test:perf

# View Playwright HTML report
npx playwright show-report
```

Coverage output lives at `mobile_web_app/coverage/index.html`.

## ▶️ Usage

- Launch the app (`Redline_Arbitrage.exe` or `python main.py`).
- Complete the onboarding flow and configure your providers in the API Provider dialog.
- Use the main window to begin scanning markets and view arbitrage opportunities.
- The tachometer widget and account health indicators guide risk decisions.

Tips:
- Configure settings in `config/settings.py`.
- First run flags live in `config/first_run_flags.json`.
- For advanced orchestration, see `src/data_orchestrator_enhanced.py`.

## 📸 Screenshots

<div align="center">
  <img src="./preview_ui.png" alt="UI Preview" />
</div>

Social Preview (used on repo cards):

<div align="center">
  <img src="./GitHub_README_Assets/social_preview.png" alt="Social Preview" />
</div>

### Additional UI Previews

<div align="center">
  <table>
    <tr>
      <td><img src="./previews/main_window.png" alt="Main Window" width="420"/></td>
      <td><img src="./previews/setup_wizard.png" alt="Setup Wizard" width="420"/></td>
    </tr>
    <tr>
      <td><img src="./previews/firstday_slideshow.png" alt="First-Day Slideshow" width="420"/></td>
      <td><img src="./previews/tutorial_overlay.png" alt="Tutorial Overlay" width="420"/></td>
    </tr>
  </table>
  <br/>
  <sub>Previews generated via <code>scripts/gui_preview_capture.py</code></sub>
  <br/>
  <sub>Run a local viewer: <code>python -m http.server 8000</code> (from the <code>previews</code> folder)</sub>
</div>

## 🧱 Tech Stack

- Python 3.10+
- PyQt6 (GUI)
- AioHTTP, Requests (data fetching)
- NumPy (analysis)
- BeautifulSoup4 (scraping)
- PyInstaller (packaging), Inno Setup (installer)
- GitHub Actions CI

## 🗺️ Roadmap

- Multi-API expansion and failover improvements
- Enhanced performance benchmarks and profiling
- Advanced error handling and state management via structured datatables/structs
- Configurable lighting/theme presets for UI polish
- Additional scrapers and data providers

See `MULTI_API_ENHANCEMENT.md`, `PERFORMANCE_OPTIMIZATION.md`, and `PHASE2_STATUS.md` for ongoing work.

## 🤝 Contributing

We welcome issues and pull requests! To contribute:

```bash
python -m pip install nox
nox  # run linting, tests, and quality checks
```

Standards:
- Formatting: Black + Isort
- Linting: Ruff
- Types: MyPy (strict mode)
- Tests: PyTest with property/perf/integration suites

## ✅ License

MIT License. See `pyproject.toml` for project metadata.

## 📬 Contact

For support or requests, open an issue on GitHub. Security concerns can be reported via private issue.

---

<div align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./GitHub_README_Assets/logo_dark.svg" />
    <source media="(prefers-color-scheme: light)" srcset="./GitHub_README_Assets/logo_light.svg" />
    <img alt="Redline Arbitrage Logo" src="./GitHub_README_Assets/logo_light.svg" width="120" />
  </picture>
  <br/>
  <sub>Built with ❤️ — Palette: #FF0033 / #0D0D0F / #FFFFFF</sub>
</div>
