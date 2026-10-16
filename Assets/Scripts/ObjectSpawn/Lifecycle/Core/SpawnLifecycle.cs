public class SpawnLifecycle
{

    private readonly SpawnLifetime spawnLifetime;
    public SpawnState CurrentState { get; private set; }
    public SpawnLifecycle(float lifetimeSeconds = 20f)
    {
        this.spawnLifetime = new SpawnLifetime(lifetimeSeconds);
        CurrentState = SpawnState.NoObjects;
    }
    public void NotifySpawnSucceeded()
    {
        // If the spawn succeeded
        if (CurrentState != SpawnState.NoObjects)
        {
            return;
        }

        spawnLifetime.StartLifetime();
        CurrentState = SpawnState.Visible;
    }
    // 수명이 끝난 경우에만 Remove 명령을 반환하고 NoObjects로 전환, 그렇지 않으면 None 반환
    public SpawnCommand Tick(float deltaTime)
    {
        if (CurrentState == SpawnState.NoObjects)
        {
            return SpawnCommand.None;
        }

        bool lifetimeExpired = spawnLifetime.Tick(deltaTime);

        if (!lifetimeExpired)
        {
            return SpawnCommand.None;
        }

        CurrentState = SpawnState.NoObjects;
        return SpawnCommand.Remove;
    }

    public SpawnCommand OnPhotoOpened()
    {
        if (CurrentState != SpawnState.Visible)
        {
            return SpawnCommand.None;
        }

        CurrentState = SpawnState.HiddenForPhoto;
        return SpawnCommand.Hide;
    }
    public SpawnCommand OnPhotoClosed()
    {
        if (CurrentState != SpawnState.HiddenForPhoto)
        {
            return SpawnCommand.None;
        }

        CurrentState = SpawnState.Visible;
        return SpawnCommand.Show;
    }
}
