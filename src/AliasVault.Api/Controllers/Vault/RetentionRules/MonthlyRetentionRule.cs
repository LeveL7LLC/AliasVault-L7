//-----------------------------------------------------------------------
// <copyright file="MonthlyRetentionRule.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.Api.Controllers.Vault.RetentionRules;

using AliasServerDb;

/// <summary>
/// Monthly retention rule that keeps the latest vault for each month.
/// </summary>
public class MonthlyRetentionRule : IRetentionRule
{
    /// <summary>
    /// Gets or sets amount of months to keep vault.
    /// </summary>
    public int MonthsToKeep { get; set; }

    /// <inheritdoc cref="IRetentionRule.ApplyRule"/>
    public IEnumerable<Vault> ApplyRule(List<Vault> vaults, DateTime now)
    {
        return vaults
            .Where(v => v.UpdatedAt >= now.Date.AddMonths(-MonthsToKeep))
            .GroupBy(v => new DateTime(v.UpdatedAt.Year, v.UpdatedAt.Month, 1))
            .Select(g => g.OrderByDescending(v => v.UpdatedAt).First());
    }
}
