# DoYouFeelLucky 🎰

A console-based player wallet simulation built in .NET 10, I created as a technical assessment.

---

## How to Run

1. Clone the repo
2. Open `DoYouFeelLucky.sln` in Visual Studio 2026
3. Set `DoYouFeelLucky.App` as the startup project
4. Hit run

Commands you can use:
```
deposit <amount>     e.g. deposit 10
withdraw <amount>    e.g. withdraw 5
bet <amount>         e.g. bet 10  (must be between $1 and $10)
exit
```

---

## Project Structure

```
DoYouFeelLucky.Common    → shared base classes used across projects
DoYouFeelLucky.Wallet    → the wallet "microservice" - deposit, withdraw, balance
DoYouFeelLucky.App       → the game layer that consumes the wallet
DoYouFeelLucky.UnitTests → xUnit tests
```

I deliberately separated the wallet from the game because in a real casino platform these would be independent microservices. The wallet doesn't know anything about bets or wins — it just moves money in and out. The game layer handles the rules and calls the wallet when it needs to debit or credit.

---

## Architecture & Decisions

### N-tier within a microservice-style split
The solution is split to reflect how I'd expect a real casino backend to be structured — a standalone wallet service and a game service that consumes it. Within each project the layering is straightforward: interfaces, services, models.

### Command pattern for input handling
Each user input maps to a command object (`DepositCommand`, `BetCommand` etc). The `CommandParser` turns raw strings into commands, and the `ConsoleGameRunner` just executes them. This keeps the game loop clean and makes adding new commands easy.

### Result pattern instead of exceptions
Services return result objects (`WalletOperationResult`, `BetResult`) with `Success`, `NewBalance`, and `ErrorMessage`. No exceptions for business rule failures — just explicit results the caller can act on.

### RNG abstraction
`IRngService` wraps `Random` so the game logic can be tested deterministically. In tests we mock it to force specific outcomes (always lose, always win, etc).

### Configuration over hardcoding
Bet limits and probabilities live in `appsettings.json` via `IOptions<GameSettings>`. In a real system this would come from a centralised config service per game type, but this approach demonstrates the concept without overengineering.

### Decimal for money, always
`double` and `float` can't represent all decimal values exactly in binary which causes rounding errors. For financial calculations `decimal` is fitting.

### Distributed systems thinking
Even though this is a console app, `Transaction` has `CorrelationId` (ties related wallet operations to one game round) and `IdempotencyKey` (prevents duplicate processing on retries). `TransactionStatus` tracks `Pending → Completed / Failed` lifecycle. In production these would matter a lot.

---

## Game Rules

- Bets accepted between $1 and $10
- 50% chance of losing
- 40% chance of winning up to x2 the bet
- 10% chance of winning between x2 and x10 the bet
- Balance formula: `new balance = old balance - bet amount + win amount`

---

## What I'd Do Differently in Production

- Persist transactions to a database — the in-memory wallet is an explicit compromise for this task
- Move `GameSettings` to a centralised config service, configurable per game and player tier
- Add proper distributed tracing with the `CorrelationId`
- Implement actual idempotency checks against a persistent store
- Work in integer cents to avoid floating point entirely
- Add more comprehensive test coverage including integration tests
