//-----------------------------------------------------------------------
// <copyright file="ServerSettingsService.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.Shared.Server.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AliasServerDb;
using AliasVault.Shared.Server.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Server settings service.
/// </summary>
/// <param name="dbContextFactory">IDbContextFactory instance.</param>
public class ServerSettingsService(IDbContextFactory<AliasServerDbContext> dbContextFactory)
{
    private readonly Dictionary<string, string?> _cache = new();

    /// <summary>
    /// Gets the setting async.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The setting.</returns>
    public async Task<string?> GetSettingAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var setting = await dbContext.ServerSettings.FirstOrDefaultAsync(x => x.Key == key);

        _cache[key] = setting?.Value;
        return setting?.Value;
    }

    /// <summary>
    /// Sets the setting async.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A task.</returns>
    public async Task SetSettingAsync(string key, string? value)
    {
        // First check if the value is already cached and matches
        if (_cache.TryGetValue(key, out var cachedValue) && cachedValue == value)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var setting = await dbContext.ServerSettings.FirstOrDefaultAsync(x => x.Key == key);
        var now = DateTime.UtcNow;

        // If setting exists and value hasn't changed, return early
        if (setting?.Value == value)
        {
            // Update cache to match database
            _cache[key] = value;
            return;
        }

        if (setting == null)
        {
            setting = new ServerSetting
            {
                Key = key,
                Value = value,
                CreatedAt = now,
                UpdatedAt = now,
            };
            dbContext.ServerSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync();
        _cache[key] = value;
    }

    /// <summary>
    /// Gets all settings async.
    /// </summary>
    /// <returns>The settings.</returns>
    public async Task<ServerSettingsModel> GetAllSettingsAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var settings = await dbContext.ServerSettings.ToDictionaryAsync(x => x.Key, x => x.Value);

        return new ServerSettingsModel
        {
            GeneralLogRetentionDays = int.TryParse(settings.GetValueOrDefault("GeneralLogRetentionDays"), out var generalDays) ? generalDays : 30,
            AuthLogRetentionDays = int.TryParse(settings.GetValueOrDefault("AuthLogRetentionDays"), out var authDays) ? authDays : 90,
            EmailRetentionDays = int.TryParse(settings.GetValueOrDefault("EmailRetentionDays"), out var emailDays) ? emailDays : 30,
            MaxEmailsPerUser = int.TryParse(settings.GetValueOrDefault("MaxEmailsPerUser"), out var maxEmails) ? maxEmails : 100,
            MaintenanceTime = TimeOnly.TryParse(
                settings.GetValueOrDefault("MaintenanceTime") ?? "00:00",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var time) ? time : new TimeOnly(0, 0),
            TaskRunnerDays = settings.GetValueOrDefault("TaskRunnerDays")?.Split(',').Select(int.Parse).ToList() ?? new List<int> { 1, 2, 3, 4, 5, 6, 7 },
        };
    }

    /// <summary>
    /// Saves the settings async.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>A task.</returns>
    public async Task SaveSettingsAsync(ServerSettingsModel model)
    {
        await SetSettingAsync("GeneralLogRetentionDays", model.GeneralLogRetentionDays.ToString());
        await SetSettingAsync("AuthLogRetentionDays", model.AuthLogRetentionDays.ToString());
        await SetSettingAsync("EmailRetentionDays", model.EmailRetentionDays.ToString());
        await SetSettingAsync("MaxEmailsPerUser", model.MaxEmailsPerUser.ToString());
        await SetSettingAsync("MaintenanceTime", model.MaintenanceTime.ToString("HH:mm", CultureInfo.InvariantCulture));
        await SetSettingAsync("TaskRunnerDays", string.Join(",", model.TaskRunnerDays));
    }
}
