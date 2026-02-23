# ⚡ Velocity Executor 

[![Framework](https://img.shields.io/badge/.NET-4.8-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet-framework/net48)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-Educational-green.svg)](#disclaimer)

**Velocity** is a high-performance, modern Roblox script executor designed for power users. Experience a seamless workflow with a sleek dark-themed UI, advanced tab management, and automated injection capabilities.

---

## ✨ Key Features

### 🖥️ Premium Interface
- **Modern Dark UI**: A carefully curated color palette designed for low eye strain.
- **Rounded Corners**: Native Windows 11-style rounded corners (compatible with Windows 10 via the Custom Region system).
- **Custom Tab System**: 
  - Dynamic tab management.
  - Smooth horizontal scrolling with custom navigation buttons.
  - Multi-tab support with close buttons (×) and easy tab creation (+).

### 📝 Advanced Editor
- **FastColoredTextBox**: Professional-grade text editor integration.
- **Lua Intelligence**: 
  - Syntax highlighting for Lua.
  - Smart Autocomplete for Roblox API.
  - Line numbers and code indentation.

### ⚙️ Automation & Control
- **Auto-Injection**: Instantly attaches to the Roblox process when detected.
- **Auto-Execute**: Automatically runs scripts placed in the `/AutoExec` directory upon injection.
- **Seamless Settings**: 
  - Always on Top toggle.
  - Persistent settings storage.
  - Dedicated settings overlay window.

---

## 🛠️ Technical Details

| Component | Technology |
| :--- | :--- |
| **Language** | C# (C-Sharp) |
| **Framework** | .NET Framework 4.8 |
| **UI System** | Custom WinForms (GDI+ & DWM API) |
| **Editor** | FastColoredTextBox |
| **Core Connection** | VelocityAPI Integration |

---

## 🚀 Getting Started

### 📋 Prerequisites
- **Windows 10/11**
- **.NET Framework 4.8 Runtime**
- **Visual Studio 2022** (if compiling from source)
- **VelocityAPI.dll** (Essential for core functionality)

### ⚙️ Installation & Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Kleshzz/Roblox-Executor.git
   ```

2. **Obtain VelocityAPI**:
   > ⚠️ **IMPORTANT**: `VelocityAPI.dll` is **NOT** included due to distribution policies. Please visit the official site: [realvelocity.xyz](https://realvelocity.xyz/#products) to download the latest API.

3. **Configure Project**:
   - Open `RobloxExecutor.sln` in Visual Studio.
   - Set the build platform to **x64**.
   - Add a reference to `VelocityAPI.dll` in the **RobloxExecutor** project.
   - Restore NuGet packages (FastColoredTextBox).

4. **Build**:
   - Build the solution (**Ctrl+Shift+B**) and locate the output in `/bin/x64/Debug`.

---

## 📂 Project Structure

- `Core/`: Core logic, Settings management, and Logger.
- `UI/`: WinForms UI implementation and custom controls.
- `UI/Controls/`: 
  - `LuaAutocomplete.cs`: Roblox-specific autocompletion logic.
  - `LuaStyle.cs`: Syntax highlighting configuration.
  - `ToggleSwitch.cs`: Sleek custom toggle components.
- `AutoExec/`: Directory for scripts that should run automatically.

---

## ⚖️ Disclaimer

**Educational Purpose Only**. This software is intended for research and educational use. We do not encourage or condone the use of this tool for violating the terms of service of any third-party software. Use at your own discretion.

---

<p align="center">
  Developed with ❤️ by <a href="https://github.com/Kleshzz">Kleshzz</a>
</p>
