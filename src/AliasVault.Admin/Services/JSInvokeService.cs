//-----------------------------------------------------------------------
// <copyright file="JSInvokeService.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.Admin.Services;

using Microsoft.JSInterop;

/// <summary>
/// Service for invoking JavaScript functions from C#.
/// </summary>
public class JsInvokeService(IJSRuntime js)
{
    /// <summary>
    /// Invoke a JavaScript function with retry and exponential backoff.
    /// </summary>
    /// <param name="functionName">The JS function name to call.</param>
    /// <param name="initialDelay">Initial delay before calling the function.</param>
    /// <param name="maxAttempts">Maximum attempts before giving up.</param>
    /// <param name="args">Arguments to pass on to the javascript function.</param>
    /// <returns>Async Task.</returns>
    public async Task RetryInvokeAsync(string functionName, TimeSpan initialDelay, int maxAttempts, params object[] args)
    {
        TimeSpan delay = initialDelay;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                bool isDefined = await js.InvokeAsync<bool>("isFunctionDefined", functionName);
                if (isDefined)
                {
                    await js.InvokeVoidAsync(functionName, args);
                    return; // Successfully called the JS function, exit the method
                }
            }
            catch
            {
                // Optionally log the exception
            }

            // Wait for the delay before the next attempt
            await Task.Delay(delay);

            // Exponential backoff: double the delay for the next attempt
            delay = TimeSpan.FromTicks(delay.Ticks * 2);
        }

        // Optionally log that the JS function could not be called after maxAttempts
    }
}
