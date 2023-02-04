﻿using Respawn;

namespace WebTesting.ProductTests;

public sealed class RespawnFixture : IAsyncLifetime
{
    public Respawner Respawn { get; private set; } = null!;

    public async Task ResetAsync() => await Respawn.ResetAsync(TestSettings.ConnectionString)
        .ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        Respawn = await Respawner.CreateAsync(TestSettings.ConnectionString)
            .ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}