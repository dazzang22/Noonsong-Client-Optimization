using NUnit.Framework;
public class SpawnLifetimeTests
{
    [Test]
    public void Tick_WhenLifetimeIsNotStarted_DoesNotExpire()
    {
        // SpawnLifetime 생성
        var spawnLifetime = new SpawnLifetime(1f); // 1초의 수명을 가진 SpawnLifetime 생성

        // Act
        bool expiredTick = spawnLifetime.Tick(0.5f); // Tick with 0.5 seconds

        // Assert
        Assert.IsFalse(expiredTick, "Ticking without starting the lifetime should not expire it.");
        Assert.IsFalse(spawnLifetime.IsRunning);
        Assert.That(spawnLifetime.RemainingLifetime, Is.EqualTo(0f), "Remaining lifetime should remain unchanged when not started.");
    }

    [Test]
    public void Tick_WhenLifetimeReachesZero_ExpiresExactlyOnce()
    {
        // SpawnLifetime 생성
        var spawnLifetime = new SpawnLifetime(1f); // 1초의 수명을 가진 SpawnLifetime 생성
        spawnLifetime.StartLifetime();

        // Act
        bool expiredFirstTick = spawnLifetime.Tick(0.5f); // Tick with 0.5 seconds
        bool expiredSecondTick = spawnLifetime.Tick(0.5f); // Tick with another 0.5 seconds
        bool expiredThirdTick = spawnLifetime.Tick(0.1f); // Tick with another 0.1 seconds

        // Assert
        Assert.IsFalse(expiredFirstTick, "The first tick should not expire the lifetime.");
        Assert.IsTrue(expiredSecondTick, "The second tick should expire the lifetime.");
        Assert.IsFalse(spawnLifetime.IsRunning);
        Assert.IsFalse(expiredThirdTick, "The third tick should not expire the lifetime.");
        Assert.That(spawnLifetime.RemainingLifetime, Is.EqualTo(0f), "Remaining lifetime should be zero after expiration.");
    }
}