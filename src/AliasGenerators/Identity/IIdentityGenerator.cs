//-----------------------------------------------------------------------
// <copyright file="IIdentityGenerator.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasGenerators.Identity;

/// <summary>
/// IdentityGenerator interface.
/// </summary>
public interface IIdentityGenerator
{
    /// <summary>
    /// Generates a random identity.
    /// </summary>
    /// <returns>Identity model object which contains the random identity.</returns>
    Task<Models.Identity> GenerateRandomIdentityAsync();
}
