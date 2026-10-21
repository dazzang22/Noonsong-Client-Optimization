# Friends! Noonsong · 프렌즈 눈송

> 캠퍼스 건물 구역에서 AR 마스코트를 만나고 수집하는 GPS 기반 모바일 게임

![프렌즈 눈송 게임 화면](https://github.com/user-attachments/assets/cf67caca-2726-469d-8398-0f3abd7253f0)

건물 구역에 들어가면 해당 건물의 캐릭터가 AR 공간에 등장합니다. 플레이어가 화면의 캐릭터를 수집하면 발견 상태가 바뀌고 도감 UI에 반영됩니다. 이 README는 그 흐름을 **건물 데이터 연결 → 생성 객체 식별 → 수집 → 수명 관리** 순서로 설명합니다.

| 항목 | 내용 |
|---|---|
| 플랫폼 | iOS · Android |
| 엔진 | Unity 2022.3.10f1 · C# · AR Foundation · Mapbox |
| 팀 | 17명 중 클라이언트 3명 |
| 개발 / 운영 | 2024.01–2025.05 / 2025.05.01–05.21 |
| 저장소 | 팀 프로젝트를 바탕으로 한 개인 개선 fork |

## 제가 맡은 범위

| 구분 | 책임 |
|---|---|
| 기존 팀 시스템 | GPS 구역 판정과 Controller 활성화, 서버 저장·불러오기, 전역 단일 Spawn 정책의 시작 |
| 직접 구현 · 2024–2025 | Controller 이름을 건물 키로 사용하는 규칙, 건물별 Entry 후보 선택과 랜덤 Spawn, `GameObject`–`NoonsongEntry` 매핑, AR 수집과 클라이언트 발견 상태·도감 UI 갱신 |
| 후속 개선 · 2026 | 건물별 Dictionary 캐시, AR 탐지 배열·호출 주기 최적화, Controller별 객체 소유권과 `SpawnLifecycle` 상태 계약 |

후속 생명주기 작업에는 Codex의 구현·테스트 보조가 있었습니다. **20초 생성 주기를 보존하고, 객체가 먼저 사라져도 수명은 계속 진행한다는 정책**은 제가 정한 설계 기준입니다. 아래의 13개 테스트는 순수 로직 테스트 구성이고, 최신 Unity Test Runner 통과 여부는 별도로 재확인해야 합니다.

## 1. 건물 신호를 캐릭터 데이터로 연결

```text
기존 위치 시스템 → 활성 ScriptActivationController
                 → Controller.gameObject.name = 건물 키
                 → NoonsongEntry.buildingName 조회
                 → 후보 중 하나 선택 → Prefab 생성
```

`NoonsongEntry.buildingName`은 기존 데이터의 건물 식별 필드입니다. 별도 ID를 만드는 대신 부모 Controller의 GameObject 이름과 같은 규칙으로 관리했습니다. 위치 판정 자체를 제가 구현했다고 주장하지 않습니다.

현재 키를 읽는 코드는 [`PlayerObjectSpawn.GetFilteredNoonsongEntries()`](Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs)에 있습니다.

```csharp
var activationController = GetComponentInParent<ScriptActivationController>();
string buildingName = activationController != null
    ? activationController.gameObject.name
    : null;
```

**Before — Spawn 시마다 전체 Entry 순회.** [`0c87cdff` 이전 코드](https://github.com/dazzang22/Noonsong-Client-Optimization/blob/aa180b0ecba120218c550d549a18e423280ea9b9/Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs)는 조회할 때마다 후보 List를 만들고 전체 Entry를 비교했습니다.

```csharp
List<NoonsongEntry> filteredEntries = new List<NoonsongEntry>();
NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();
// 테스트 모드 분기 생략
foreach (var entry in entries)
{
    if (entry.buildingName == buildingName)
        filteredEntries.Add(entry);
}
```

**After — 초기화 시 건물별 캐시.** 현재 [`BuildEntryCache()`](Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs)는 Entry를 건물명으로 사전 분류하고, Spawn 후보 조회에는 `TryGetValue()`를 사용합니다.

```csharp
if (!entriesByBuilding.TryGetValue(entry.buildingName, out var list))
{
    list = new List<NoonsongEntry>();
    entriesByBuilding[entry.buildingName] = list;
}
list.Add(entry);
```

```csharp
if (entriesByBuilding != null &&
    entriesByBuilding.TryGetValue(buildingName, out var entries))
    return entries;
return EmptyEntries;
```

이 변경의 목적은 **반복적인 전체 목록 탐색과 임시 List 생성 예방**입니다. 실행 시간이나 FPS 향상은 측정하지 않았습니다. 캐시는 초기화 시점에 만들므로, 실행 중 Entry 데이터가 바뀐다면 별도 갱신이 필요합니다. 구현 흐름과 이력은 [상세 문서](docs/building-collection-pipeline.md)에 정리했습니다.

## 2. Spawn한 객체에서 원본 Entry 되찾기

생성된 캐릭터의 외형만으로는 수집 시 어느 도감 Entry인지 안정적으로 알 수 없습니다. [`SpawnedObject`](Assets/Scripts/CollectionScene/SpawnObject.cs)는 **생성된 `GameObject`와 원본 `NoonsongEntry`를 한 쌍**으로 보관합니다.

```csharp
ownedSpawnedObject = new SpawnedObject(
    instance,
    spawnedObject.NoonsongEntry);
```

[`ARObjectCatch`](Assets/Scripts/CollectionScene/ARObjectCatch.cs)는 화면에서 식별한 `currentTarget`으로 Manager의 Spawn 목록에서 같은 객체를 찾아 Entry를 복원합니다.

```csharp
return playerObjectSpawnManager.SpawnedObjects.Find(
    obj => obj.GameObject == target);
```

수집이 확정되면 [`NoonsongManager.DiscoverItem()`](Assets/Scripts/CollectionScene/NoonsongManager.cs)이 클라이언트 발견 상태를 바꾸고 도감 UI를 갱신합니다.

```csharp
entry.isDiscovered = true;
PopulateNoonsong();
```

이 메서드의 직접 책임을 서버 저장으로 확대해 설명하지 않습니다. 일반 캐릭터는 `NoonsongEntry == null`인 별도 경로로 처리합니다.

## 3. 전역 제거에서 소유 객체의 생명주기로

기존 구현에서 각 건물의 `PlayerObjectSpawn`은 20초 타이머를 진행했지만, 만료 시 Manager의 **전역 제거 함수**를 호출했습니다. Manager는 호출한 Controller와 무관하게 건물별 Spawn 목록 전체를 순회했습니다. 전역 최대 1개 Spawn 제한이 있었으므로, 여러 Controller의 객체가 동시에 생성돼 실제 오삭제가 났다고 주장하지 않습니다. 문제는 **수명 판단 주체와 제거 범위가 맞지 않는 구조적 위험**입니다.

**Before — Controller 타이머 → Manager 전역 제거** ([리팩터링 전 호출부](https://github.com/dazzang22/Noonsong-Client-Optimization/blob/01948f81d918597a01a5804f598c594047bed7d7/Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs), [전역 순회](https://github.com/dazzang22/Noonsong-Client-Optimization/blob/01948f81d918597a01a5804f598c594047bed7d7/Assets/Scripts/ObjectSpawn/PlayerObjectSpawnManager.cs))

```csharp
if (timer >= changeInterval)
{
    if (PlayerObjectSpawnManager.Instance != null)
    {
        PlayerObjectSpawnManager.Instance.RemoveSpawnedObjects();
    }
    // 조우 UI·재생성 분기 생략
    timer = 0f;
}
```

```csharp
foreach (var kvp in _spawnedObjectsPerBuilding)
{
    foreach (var obj in kvp.Value)
    {
        if (obj?.GameObject != null)
        {
            obj.GameObject.SetActive(false);
            GameObject.Destroy(obj.GameObject);
        }
    }
}
```

**After — 상태 판단과 Unity 실행 분리.** 순수 C# [`SpawnLifecycle`](Assets/Scripts/ObjectSpawn/Lifecycle/Core/SpawnLifecycle.cs)은 상태와 20초 수명을 계산해 `SpawnCommand`를 반환합니다. [`PlayerObjectSpawn`](Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs)은 자신의 `ownedSpawnedObject`에 그 명령을 실행하고, Manager의 전역 목록을 갱신합니다.

```csharp
ExecuteSpawnCommand(spawnLifecycle.Tick(Time.deltaTime));
// ...
case SpawnCommand.Remove:
    RemoveOwnedSpawnedObject();
    break;
```

조기 수집 등으로 실제 객체가 먼저 사라지면 `WaitingForLifetimeEnd`로 옮겨 **최초 생성 시점부터 20초가 끝날 때까지** 기다립니다. `NoObjects`는 즉시 재생성을 뜻하지 않으며, 구역·전역 슬롯 등 다른 조건도 충족해야 다음 Spawn이 가능합니다. 구역 밖에서도 `Tick()`은 먼저 실행되어 기존 객체의 수명이 계속 진행됩니다. 조우 중인 객체는 만료 때 화면에 남길 수 있지만 필드 슬롯과 소유 참조는 정리합니다.

테스트 파일은 [`SpawnLifetimeTests` 2개](Assets/Tests/Tests/Editmode/Test%20Assembly%20Definition/Noonsong.SpawnLifecycle.EditModeTests/SpawnLifetimeTests.cs)와 [`SpawnLifecycleTests` 11개](Assets/Tests/Tests/Editmode/Test%20Assembly%20Definition/Noonsong.SpawnLifecycle.EditModeTests/SpawnLifecycleTests.cs)입니다. 수명 계산·상태 전이·명령 반환을 검증하며 실제 Unity `GameObject` 생성·제거 통합 테스트는 아닙니다. [설계·이력](docs/spawn-lifecycle-follow-up.md)

## 4. 라이브 증상에서 측정 가능한 비용 찾기

운영 중 장시간 플레이의 발열과 AR 캐릭터 미생성이 보고됐고 직접 증상도 경험했습니다. 두 현상의 원인을 단정하지 않고, 화면 변화와 무관하게 반복되는 AR 탐지를 조사했습니다.

**Before — 매 프레임 실행, 호출마다 검사점 배열 생성.** 아래 발췌는 [최적화 전 실제 코드](https://github.com/dazzang22/Noonsong-Client-Optimization/blob/c18069d47db6e7798fec9a9e720ff0255be41d51/Assets/Scripts/CollectionScene/ARObjectCatch.cs)입니다.

```csharp
void Update()
{
    CheckForObjectInView();
}

// CheckForObjectInView() 내부의 배열 생성
Vector3[] checkPoints = new Vector3[]
{
    objectPosition,
    objectPosition + new Vector3(boundingRadius, 0, 0),
    objectPosition - new Vector3(boundingRadius, 0, 0),
    objectPosition + new Vector3(0, boundingRadius, 0),
    objectPosition - new Vector3(0, boundingRadius, 0)
};
```

**After — 인스턴스 배열 재사용, 간격을 둔 탐지.** 현재 [`ARObjectCatch`](Assets/Scripts/CollectionScene/ARObjectCatch.cs)는 `Vector3[5]`를 인스턴스 필드에 두고 검사 때 값을 덮어씁니다. 일반 실행 조건은 재사용 배열과 `WaitForSeconds(detectInterval)`을 사용하는 Coroutine입니다. 같은 파일의 A/B/C 분기는 비교 측정을 위한 코드입니다.

```csharp
private readonly Vector3[] checkPoints = new Vector3[5];
// 검사 시 points[0]~points[4]를 현재 위치로 갱신
```

| Unity Editor Play Mode 측정 | A · 매 호출 배열 생성 | B · 배열 재사용 | C · 배열 재사용 + 간격 |
|---|---:|---:|---:|
| 탐지 Marker GC Alloc | 92 B/call | 0 B/call | — |
| 탐지 호출 빈도 | 중앙값 95.47 calls/sec | 매 프레임 유지 | 중앙값 8.21 calls/sec |

A→B는 배열 생성 방식, B→C는 실행 방식을 바꾼 비교입니다. A/C 호출 빈도는 조건별 3회 측정의 중앙값으로 **91.4% 감소**했습니다. `calls/sec = Marker 총 호출 수 ÷ (Frame Count × Mean Frame Time)`으로 산출했습니다. `92 B`는 선택한 탐지 Marker의 **호출당** GC Alloc이며 배열 하나의 크기나 앱 전체 GC가 아닙니다. 이 결과로 CPU·발열·AR 미생성 개선을 주장하지 않습니다. [측정 방법과 해석 범위](docs/runtime-optimization.md)

## 코드와 검증 자료

- [건물별 Spawn과 Entry 연결](Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs) · [AR 화면 탐지와 수집](Assets/Scripts/CollectionScene/ARObjectCatch.cs)
- [건물별 데이터 연결 문서](docs/building-collection-pipeline.md) · [런타임 측정 문서](docs/runtime-optimization.md) · [생명주기 설계 문서](docs/spawn-lifecycle-follow-up.md)
- 본문의 2026 후속 개선은 원 프로젝트 구현과 구분했습니다. 측정된 호출·할당 수치 외에 FPS, 기기 온도, 라이브 장애 해결 수치는 제시하지 않습니다.
