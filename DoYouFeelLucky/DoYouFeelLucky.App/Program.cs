using DoYouFeelLucky.App;
using DoYouFeelLucky.App.Commands;
using DoYouFeelLucky.App.Configuration;
using DoYouFeelLucky.App.Interfaces;
using DoYouFeelLucky.App.Services;
using DoYouFeelLucky.Wallet.Interfaces;
using DoYouFeelLucky.Wallet.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {

        services.Configure<GameSettings>(
            context.Configuration.GetSection("GameSettings"));

        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<IRngService, RngService>();
        services.AddSingleton<IGameService, GameService>();

        var walletId = Guid.NewGuid();
        services.AddSingleton(sp => new CommandParser(
            sp.GetRequiredService<IWalletService>(),
            sp.GetRequiredService<IGameService>(),
            sp.GetRequiredService<IOptions<GameSettings>>(),
            walletId));

        services.AddSingleton<ConsoleGameRunner>();
    })
    .Build();

var runner = host.Services.GetRequiredService<ConsoleGameRunner>();
await runner.RunAsync();