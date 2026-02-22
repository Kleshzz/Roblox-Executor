# Velocity Executor

A powerful and modern Roblox script executor with a clean dark interface, auto-injection, and auto-execution features.

---

## 🚀 Features

- **Modern UI**: Full-form overlay settings with custom Toggle Switches.
- **Auto-Injection**: Automatically attaches to Roblox whenever the game is detected.
- **Auto-Execute**: Automatically runs all scripts from the `AutoExec` folder upon successful injection.
- **Persistence**: Remembers your settings across sessions.
- **Error Logging**: Detailed error tracking in the `Logs` directory.

---

## 📋 Requirements

- Windows 10/11
- .NET Framework 4.8
- **VelocityAPI.dll** (See Setup for details)
- Roblox (Windows Store or Browser version)

---

## ⚙️ Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Kleshzz/Roblox-Executor.git
   ```

2. **Download VelocityAPI.dll**:
   > ⚠️ **IMPORTANT**: Due to security and distribution policies, `VelocityAPI.dll` is **NOT** included in this repository. You must download it separately from the official velocity site(https://realvelocity.xyz/#products). 

3. **Install dependencies**:
   - Open the solution in **Visual Studio**.
   - Restore NuGet packages (FastColoredTextBox).
   - Right-click on **References** -> **Add Reference** -> **Browse** and select your downloaded `VelocityAPI.dll`.

4. **Build and Run**:
   - Set build configuration to `x64`.
   - Press **F5** to compile and launch.

---

## 🛠 Project Structure

- `Core/`: Business logic, settings, and logging.
- `UI/`: Main form and application windows.
- `UI/Controls/`: Custom components like ToggleSwitch and Lua Autocomplete.

---

## ⚖️ Disclaimer

This tool is for educational purposes only. Use it at your own risk. We are not responsible for any bans or issues related to your Roblox account.
