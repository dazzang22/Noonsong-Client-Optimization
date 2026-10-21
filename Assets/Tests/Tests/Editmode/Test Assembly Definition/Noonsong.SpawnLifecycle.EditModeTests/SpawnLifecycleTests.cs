using NUnit.Framework;
public class SpawnLifecycleTests
{
    [Test]
    // 처음에 객체가 없고, 실제 스폰 성공 통지를 받으면 보이는 상태로 전환
    public void NotifySpawnSucceeded_WhenNoObject_BecomesVisible()
    {
        var lifecycle = new SpawnLifecycle();
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));

        lifecycle.NotifySpawnSucceeded();

        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.Visible));
    }
    [Test]
    public void Tick_WhenLifetimeExpires_ReturnsRemoveCommandAndBecomesNoObjects()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();

        SpawnCommand firstCommand = lifecycle.Tick(0.5f);
        SpawnCommand secondCommand = lifecycle.Tick(0.5f);
        SpawnCommand thirdCommand = lifecycle.Tick(0.1f);

        // 첫 번째 Tick 호출에서는 수명이 아직 만료되지 않았으므로 Remove 명령이 반환되지 않아야 함
        // 두 번째 Tick 호출에서는 수명이 만료되어 Remove 명령이 반환되고 상태가 NoObjects로 전환되어야 함
        // 세 번째 Tick 호출에서는 이미 NoObjects 상태이므로 Remove 명령이 반환되지 않아야 함
        Assert.That(firstCommand, Is.EqualTo(SpawnCommand.None));
        Assert.That(secondCommand, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(thirdCommand, Is.EqualTo(SpawnCommand.None));
    }
    [Test]
    // 객체가 보이는 상태에서 사진이 열리면 Hide 명령이 반환되고 상태가 HiddenForPhoto로 전환
    public void OnPhotoOpened_WhenVisible_ReturnsHideCommandAndBecomesHiddenForPhoto()
    {
        var lifecycle = new SpawnLifecycle();
        lifecycle.NotifySpawnSucceeded();

        SpawnCommand command = lifecycle.OnPhotoOpened();

        Assert.That(command, Is.EqualTo(SpawnCommand.Hide));
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.HiddenForPhoto));
    }
    [Test]
    // 사진모드에서 숨겨도 20초 후에 Remove 명령이 반환되고 NoObjects 상태로 전환
    public void Tick_WhenHiddenForPhotoAndLifetimeExpires_ReturnsRemoveCommandAndBecomesNoObjects()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();
        lifecycle.OnPhotoOpened();

        SpawnCommand command = lifecycle.Tick(1f);

        Assert.That(command, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));
    }
    [Test]
    // 사진을 열고 0.5초 후 사진을 닫고 0.5초 후에 Remove 명령이 반환되고 NoObjects 상태로 전환
    public void Tick_WhenHiddenForPhotoAndPhotoClosedAndLifetimeExpires_ReturnsRemoveCommandAndBecomesNoObjects()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();
        SpawnCommand hideCommand = lifecycle.OnPhotoOpened();
        SpawnCommand tickcommand = lifecycle.Tick(0.5f);
        SpawnCommand finalCommand = lifecycle.OnPhotoClosed();
        SpawnCommand finalTickCommand = lifecycle.Tick(0.5f);

        Assert.That(hideCommand, Is.EqualTo(SpawnCommand.Hide));
        Assert.That(tickcommand, Is.EqualTo(SpawnCommand.None));
        Assert.That(finalCommand, Is.EqualTo(SpawnCommand.Show));
        Assert.That(finalTickCommand, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));
    }
    [Test]
    // 중복 NotifySpawnSucceeded 호출 시 상태가 변경되지 않고 수명은 계속 진행되어야 함
    public void NotifySpawnSucceeded_WhenAlreadyVisible_DoesNotChangeState()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();
        lifecycle.Tick(0.5f); // 일부 수명 경과
        lifecycle.NotifySpawnSucceeded(); // 중복 호출

        SpawnCommand command = lifecycle.Tick(0.5f); // 수명 만료
        Assert.That(command, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));
    }
    [Test]
    // 사진 오픈 후 수명만료되면 Remove 명령이 반환되고 NoObjects 상태로 전환되어야 함
    public void OnPhotoClosed_AfterLifetimeExpired_DoesNotShowExpiredObject()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();
        lifecycle.OnPhotoOpened(); // 사진 모드로 전환
        SpawnCommand expiredCommand = lifecycle.Tick(1f); // 수명 만료
        SpawnCommand closedCommand = lifecycle.OnPhotoClosed(); // 사진 모드 닫기

        Assert.That(expiredCommand, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));
        Assert.That(closedCommand, Is.EqualTo(SpawnCommand.None)); // 이미 NoObjects 상태이므로 Show 명령이 반환되지 않아야 함
    }
    [Test]
    // 수명이 만료된 후 NotifySpawnSucceeded 호출 시 새로운 수명이 시작되어야 함
    public void NotifySpawnSucceeded_AfterRemoval_StartsNewLifetime()
    {
        var lifecycle = new SpawnLifecycle(1f);

        lifecycle.NotifySpawnSucceeded();
        Assert.That(
            lifecycle.Tick(1f),
            Is.EqualTo(SpawnCommand.Remove)
        );

        lifecycle.NotifySpawnSucceeded();

        Assert.That(lifecycle.CurrentState, Is.EqualTo(SpawnState.Visible));
        Assert.That(
            lifecycle.Tick(0.5f),
            Is.EqualTo(SpawnCommand.None)
        );
        Assert.That(
            lifecycle.Tick(0.5f),
            Is.EqualTo(SpawnCommand.Remove)
        );
    }
    [Test]
    // 객체가 보이는 상태에서 외부에서 제거되면 WaitingForLifetimeEnd 상태로 전환되고, 수명이 끝나면 NoObjects 상태로 전환
    public void NotifyObjectRemovedExternally_WhenVisible_WaitsForLifetimeEnd()
    {
        var lifecycle = new SpawnLifecycle(1f);

        lifecycle.NotifySpawnSucceeded();
        lifecycle.NotifyObjectRemovedExternally();

        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.WaitingForLifetimeEnd)
        );

        Assert.That(
            lifecycle.Tick(0.5f),
            Is.EqualTo(SpawnCommand.None)
        );
        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.WaitingForLifetimeEnd)
        );

        Assert.That(
            lifecycle.Tick(0.5f),
            Is.EqualTo(SpawnCommand.None)
        );
        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.NoObjects)
        );
    }
    [Test]
    // 수집으로 슬롯이 해제되어도 수명이 끝나기 전까지는 Remove 명령이 반환되지 않아야 함
    public void NotifySpawnSucceeded_WhenWaitingForLifetimeEnd_DoesNotStartNewLifetime()
    {
        var lifecycle = new SpawnLifecycle(1f);
        lifecycle.NotifySpawnSucceeded();
        lifecycle.NotifyObjectRemovedExternally();

        lifecycle.NotifySpawnSucceeded(); // 수집으로 슬롯이 해제되어도 수명이 끝나기 전까지는 Remove 명령이 반환되지 않아야 함
        Assert.That(
             lifecycle.CurrentState,
             Is.EqualTo(SpawnState.WaitingForLifetimeEnd)
         );

        Assert.That(
            lifecycle.Tick(0.5f),
            Is.EqualTo(SpawnCommand.None)
        );
        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.WaitingForLifetimeEnd)
        );

        lifecycle.Tick(0.5f); // 수명이 끝나면 NoObjects 상태로 전환
        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.NoObjects)
        );
        lifecycle.NotifySpawnSucceeded(); // 새로운 수명 시작
        Assert.That(
            lifecycle.CurrentState,
            Is.EqualTo(SpawnState.Visible)
        );
    }
    [Test]
    // A/B 테스트: 수집으로 슬롯이 해제되어도 수명이 끝나기 전까지는 Remove 명령이 반환되지 않아야 함
    public void Tick_WhenNonOwnerHasNoObject_DoesNotRemoveOwnersObject()
    {
        var ownerLifecycle = new SpawnLifecycle(1f);
        var nonOwnerLifecycle = new SpawnLifecycle(1f);

        // A 건물만 실제 스폰에 성공
        ownerLifecycle.NotifySpawnSucceeded();

        // B 건물의 Tick은 제거할 자기 객체가 없으므로 아무 명령도 내리지 않음
        SpawnCommand nonOwnerCommand = nonOwnerLifecycle.Tick(1f);

        // A 건물의 수명 만료만 A 객체 제거 명령을 만듦
        SpawnCommand ownerCommand = ownerLifecycle.Tick(1f);

        Assert.That(nonOwnerCommand, Is.EqualTo(SpawnCommand.None));
        Assert.That(nonOwnerLifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));

        Assert.That(ownerCommand, Is.EqualTo(SpawnCommand.Remove));
        Assert.That(ownerLifecycle.CurrentState, Is.EqualTo(SpawnState.NoObjects));
    }
}