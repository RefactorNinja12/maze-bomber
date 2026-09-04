# 💣 BomberRoyal

**BomberRoyal** är ett 2D-spel inspirerat av klassiska *Bomberman*, utvecklat i **C#** med hjälp av **MonoGame Framework** (.NET 8).  
Projektet är byggt som en flerskiktslösning med separata projekt för:
- 🎮 **Spelklient renderering** (`BomberRoyal`)
- 🧠 **SpelLogik / logikbibliotek** (`BomberRoyal.Core`)
- 🧪 **Enhetstester** (`BomberRoyal.Tests`)


---

## ⚙️ Systemkrav

| Komponent | Krav |
|------------|------|
| **.NET SDK** | [.NET 8.0 eller högre](https://dotnet.microsoft.com/en-us/download) |
| **MonoGame** | v3.8 (installeras automatiskt via NuGet) |
| **Operativsystem** | Windows / Linux / macOS (DesktopGL) |
| **IDE (valfritt)** | Visual Studio 2022, JetBrains Rider eller VS Code |
| **Testverktyg** | xUnit + NSubstitute (NuGet-paket) |

> 🔹 Alla nödvändiga beroenden som MonoGame, xUnit, NSubstitute och Test SDK hämtas automatiskt via **NuGet** när du bygger projektet första gången så är bara bygg och starta.

---

## 🧩 Projektstruktur