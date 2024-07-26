//-----------------------------------------------------------------------
// <copyright file="AdminPlaywrightTest.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.E2ETests.Common;

using AliasServerDb;

/// <summary>
/// Base class for Playwright E2E tests that run against Admin webapp.
/// </summary>
public class AdminPlaywrightTest : PlaywrightTest
{
    private static readonly int _basePort = 5700;
    private static int _currentPort = _basePort;

    /// <summary>
    /// For starting the Admin project in-memory.
    /// </summary>
    private readonly WebApplicationAdminFactoryFixture<AliasVault.Admin.Program> _webAppFactory = new();

    /// <summary>
    /// Gets the db context for the Admin project.
    /// </summary>
    protected AliasServerDbContext DbContext => _webAppFactory.GetDbContext();

    /// <summary>
    /// Tear down the Playwright test which runs after all tests are done in the class.
    /// </summary>
    /// <returns>Async task.</returns>
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Page.CloseAsync();
        await Context.CloseAsync();
        await Browser.CloseAsync();

        await _webAppFactory.DisposeAsync();
    }

    /// <summary>
    /// Setup the Playwright test environment.
    /// </summary>
    /// <returns>Async task.</returns>
    protected override async Task SetupEnvironment()
    {
        // Set the base port for the test starting at 5600. Increase the port by 2 for each test running
        // in parallel to avoid port conflicts.
        int appPort;
        lock (Lock)
        {
            appPort = Interlocked.Increment(ref _currentPort);
        }

        AppBaseUrl = "http://localhost:" + appPort + "/";

        // Start Admin project in-memory.
        _webAppFactory.HostUrl = "http://localhost:" + appPort;
        _webAppFactory.CreateDefaultClient();

        await SetupPlaywrightBrowserAndContext();
        Page = await Context.NewPageAsync();
        InputHelper = new(Page);

        // Check that we get redirected to /user/login when accessing the root URL and not authenticated.
        await Page.GotoAsync(AppBaseUrl);
        await WaitForUrlAsync("user/login**");

        // These credentials are based on the static environment variables set by WebApplicationAdminFactoryFixture.cs
        // for this test.
        TestUserUsername = "admin";
        TestUserPassword = "password";
        await LoginAsAdmin();
    }

    /// <summary>
    /// Login to the Admin webapp as the default admin user.
    /// </summary>
    /// <returns>Async task.</returns>
    protected async Task LoginAsAdmin()
    {
        // Check that we are on the login page.
        await WaitForUrlAsync("user/login**");

        // Enter login credentials.
        await InputHelper.FillInputFields(new Dictionary<string, string>
        {
            { "username", TestUserUsername },
            { "password", TestUserPassword },
        });

        var submitButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        await submitButton.ClickAsync();

        // Wait for the dashboard to load.
        await WaitForUrlAsync("**", "Welcome");

        var pageContent = await Page.TextContentAsync("body");
        Assert.That(pageContent, Does.Contain("Welcome to the AliasVault admin portal"), "No entry page content visible after logging in to admin app.");
    }
}
