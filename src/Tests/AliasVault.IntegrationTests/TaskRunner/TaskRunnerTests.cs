//-----------------------------------------------------------------------
// <copyright file="TaskRunnerTests.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.IntegrationTests.TaskRunner;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Integration tests for TaskRunner service.
/// </summary>
[TestFixture]
public class TaskRunnerTests
{
    /// <summary>
    /// The test host instance.
    /// </summary>
    private IHost _testHost;

    /// <summary>
    /// The test host builder instance.
    /// </summary>
    private TestHostBuilder _testHostBuilder;

    /// <summary>
    /// Setup logic for every test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _testHostBuilder = new TestHostBuilder();
        _testHost = _testHostBuilder.Build();
    }

    /// <summary>
    /// Tear down logic for every test.
    /// </summary>
    /// <returns>Task.</returns>
    [TearDown]
    public async Task TearDown()
    {
        await _testHost.StopAsync();
        _testHost.Dispose();
    }

    /// <summary>
    /// Tests the EmailCleanup task.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task EmailCleanup()
    {
        // Arrange
        await InitializeWithTestData();

        // Assert
        var dbContext = _testHostBuilder.GetDbContext();
        var emails = await dbContext.Emails.ToListAsync();
        Assert.That(emails, Has.Count.EqualTo(50));
    }

    /// <summary>
    /// Tests the LogCleanup task.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task LogCleanup()
    {
        // Arrange
        await InitializeWithTestData();

        // Assert
        var dbContext = _testHostBuilder.GetDbContext();
        var generalLogs = await dbContext.Logs.ToListAsync();
        Assert.That(generalLogs, Has.Count.EqualTo(50), "Only recent general logs should remain");
    }

    /// <summary>
    /// Tests the LogCleanup task.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task AuthLogCleanup()
    {
        // Arrange
        await InitializeWithTestData();

        // Assert
        var dbContext = _testHostBuilder.GetDbContext();

        // Check auth logs
        var authLogs = await dbContext.AuthLogs.ToListAsync();
        Assert.That(authLogs, Has.Count.EqualTo(50), "Only recent auth logs should remain");
    }

     /// <summary>
    /// Tests that the TaskRunner does not run tasks before the maintenance time.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task MaintenanceTimeInFutureDoesNotRun()
    {
        // Seed database with generic test data.
        await SeedData.SeedDatabase(_testHostBuilder.GetDbContext());

        // Update maintenance time in database to future to ensure the task runner doesn't execute yet.

        // Get current time and set maintenance time to 2 hours in the future
        var now = DateTime.Now;
        var futureTime = now.AddHours(2);

        // Make sure we don't exceed midnight
        if (futureTime.Day != now.Day)
        {
            futureTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 5, DateTimeKind.Local);
        }

        // Update maintenance time in database
        var dbContext = _testHostBuilder.GetDbContext();
        var maintenanceTimeSetting = await dbContext.ServerSettings
            .FirstAsync(s => s.Key == "MaintenanceTime");
        maintenanceTimeSetting.Value = futureTime.ToString("HH:mm");
        await dbContext.SaveChangesAsync();

        // Get initial email count
        var initialEmailCount = await dbContext.Emails.CountAsync();

        // Start the service.
        await _testHost.StartAsync();

        // Verify email count hasn't changed (tasks haven't run)
        var currentEmailCount = await dbContext.Emails.CountAsync();
        Assert.That(currentEmailCount, Is.EqualTo(initialEmailCount), "Email count changed despite maintenance time being in the future. Check if TaskRunner is respecting the maintenance time setting.");
    }

    /// <summary>
    /// Tests that the TaskRunner does not run tasks when the current day is excluded.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task MaintenanceTimeExcludedDayDoesNotRun()
    {
        // Seed database with generic test data.
        await SeedData.SeedDatabase(_testHostBuilder.GetDbContext());

        // Get current day of week (1-7, Monday = 1, Sunday = 7)
        var currentDay = (int)DateTime.Now.DayOfWeek;
        if (currentDay == 0)
        {
            currentDay = 7; // Convert Sunday from 0 to 7
        }

        // Update maintenance settings in database to exclude current day
        var dbContext = _testHostBuilder.GetDbContext();

        // Set maintenance time to midnight
        var maintenanceTimeSetting = await dbContext.ServerSettings
            .FirstAsync(s => s.Key == "MaintenanceTime");
        maintenanceTimeSetting.Value = "00:00";

        // Set task runner days to all days except current day
        var taskRunnerDays = Enumerable.Range(1, 7)
            .Where(d => d != currentDay)
            .ToList();
        var taskRunnerDaysSetting = await dbContext.ServerSettings
            .FirstAsync(s => s.Key == "TaskRunnerDays");
        taskRunnerDaysSetting.Value = string.Join(",", taskRunnerDays);

        await dbContext.SaveChangesAsync();

        // Get initial email count
        var initialEmailCount = await dbContext.Emails.CountAsync();

        // Start the service
        await _testHost.StartAsync();

        // Verify email count hasn't changed (tasks haven't run)
        var currentEmailCount = await dbContext.Emails.CountAsync();
        Assert.That(currentEmailCount, Is.EqualTo(initialEmailCount), "Email count changed despite current day being excluded from maintenance days. Check if TaskRunner is respecting the task runner days setting.");
    }

    /// <summary>
    /// Initializes the test with test data.
    /// </summary>
    /// <returns>Task.</returns>
    protected async Task InitializeWithTestData()
    {
        await SeedData.SeedDatabase(_testHostBuilder.GetDbContext());
        await _testHost.StartAsync();
    }
}
