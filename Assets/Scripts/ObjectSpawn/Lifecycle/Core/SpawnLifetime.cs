public class SpawnLifetime
{
    private readonly float lifetimeSeconds = 20f;
    private float remainingLifetime;
    public bool IsRunning { get; private set; }
    public float RemainingLifetime => remainingLifetime;

    public SpawnLifetime(float lifetimeSeconds = 20f)
    {
        this.lifetimeSeconds = lifetimeSeconds;
        remainingLifetime = 0f;
        IsRunning = false;
    }
    public void StartLifetime()
    {
        remainingLifetime = lifetimeSeconds;
        IsRunning = true;
    }
    public bool Tick(float deltaTime)
    {
        if (!IsRunning)
        {
            return false;
        }

        remainingLifetime -= deltaTime;

        if (remainingLifetime <= 0f)
        {
            remainingLifetime = 0f;
            IsRunning = false;
            return true; // Lifetime : expired
        }
        return false;
    }
}