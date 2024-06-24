//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using AliasClientDb;
using AliasVault.WebApp;
using AliasVault.WebApp.Auth.Providers;
using AliasVault.WebApp.Auth.Services;
using AliasVault.WebApp.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using SqliteWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient("AliasVault.Api").AddHttpMessageHandler<AliasVaultApiHandlerService>();
builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("AliasVault.Api");
    if (builder.Configuration["ApiUrl"] is null)
    {
        throw new InvalidOperationException("The 'ApiUrl' configuration value is required.");
    }

    httpClient.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!);
    return httpClient;
});
builder.Services.AddTransient<AliasVaultApiHandlerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<AliasService>();
builder.Services.AddScoped<GlobalNotificationService>();
builder.Services.AddSingleton<ClipboardCopyService>();
builder.Services.AddSqliteWasmDbContextFactory<AliasClientDbContext>(
    opts => opts.UseSqlite("Data Source=:memory:;"));

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
