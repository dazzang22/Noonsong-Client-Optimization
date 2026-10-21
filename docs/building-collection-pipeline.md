# Building-to-Collection Pipeline

이 문서는 2024–2025년에 직접 구현한 건물별 Entry 필터링, 랜덤 Spawn, AR 수집 데이터 연결과 2026년 cache 리팩터링을 시점별로 구분합니다.

## Responsibility Boundary

기존 위치 시스템은 구역 진입을 판정하고 해당 `ScriptActivationController`를 활성화합니다. 위치 계산과 Controller 활성화는 이 사례의 구현 범위가 아닙니다.

직접 구현한 범위는 활성 Controller 신호 이후입니다.

```text
Active Controller
→ building key
→ matching NoonsongEntry candidates
→ weighted branch and random selection
→ GameObject + Entry
→ AR encounter
→ collection state and UI
```

## Original Implementation · 2024

[`86f0eb86`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/86f0eb86)에서 기존 `NoonsongEntry.buildingName` 필드를 활용하고, 활성 Controller 이름을 건물 키로 읽어 후보를 필터링했습니다. 당시 구현 파일은 `ARObjectSpawn.cs`였으며 현재 파일 구조와 다릅니다.

핵심 분기는 다음과 같습니다.

```csharp
SpawnedObject GetRandomPrefab()
{
    float probability = Random.Range(0f, 1f);
    if (probability < 0.6f)
    {
        List<NoonsongEntry> filteredEntries = GetFilteredNoonsongEntries();

        if (filteredEntries.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredEntries.Count);
            return new SpawnedObject(
                filteredEntries[randomIndex].prefab,
                filteredEntries[randomIndex]);
        }
        else
        {
            int randomIndex = Random.Range(0, generalNoonsong.Length);
            return new SpawnedObject(generalNoonsong[randomIndex], null);
        }
    }
    else
    {
        int randomIndex = Random.Range(0, generalNoonsong.Length);
        return new SpawnedObject(generalNoonsong[randomIndex], null);
    }
}
```

전공 캐릭터 분기에서도 건물 후보가 비어 있으면 일반 캐릭터를 선택해 Spawn 실패를 피했습니다. 이 코드는 역사적 구현을 설명하기 위한 발췌이며 현재 HEAD 코드가 아닙니다.

## Current Implementation · 2026 Refactor

현재 [`PlayerObjectSpawn.cs`](../Assets/Scripts/ObjectSpawn/PlayerObjectSpawn.cs)는 같은 규칙을 유지하면서 조회 구조를 변경했습니다.

### Cache construction

`BuildEntryCache()`는 전체 Entry를 한 번 순회합니다.

- `null` Entry 제외
- 비어 있는 `buildingName` 제외
- `TryGetValue()` 실패 시 건물별 List 생성
- 이후 같은 건물 Entry를 동일 List에 축적

조회 단계의 전체 순회와 임시 List 생성을 없애고, 존재하지 않는 키에는 공유 `EmptyEntries`를 반환합니다.

```csharp
private List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
{
    if (testModeSpawnInFrontOfCamera)
        return allEntriesCache;

    if (entriesByBuilding != null &&
        entriesByBuilding.TryGetValue(buildingName, out var entries))
    {
        return entries;
    }

    return EmptyEntries;
}
```

이 변경은 [`0c87cdff`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/0c87cdff)에서 확인할 수 있습니다.

### Runtime mapping

Spawn 결과는 `GameObject`만 등록하지 않고 원본 Entry와 함께 저장합니다.

```csharp
ownedSpawnedObject = new SpawnedObject(
    instance,
    spawnedObject.NoonsongEntry);

PlayerObjectSpawnManager.Instance.AddSpawnedObject(
    buildingName,
    ownedSpawnedObject);
```

`ARObjectCatch`는 화면에서 선택한 `GameObject`로 `SpawnedObject`를 찾고 Entry를 꺼냅니다. Entry가 `null`이면 일반 캐릭터 대화로, Entry가 있으면 해당 캐릭터의 조우 UI로 분기합니다. 수집 확정 시 `NoonsongManager.DiscoverItem(entry)`가 동일 Entry의 상태를 변경하고 UI를 다시 구성합니다.

## Team and Follow-up Context

- 전역 단일 Spawn 정책과 `PlayerObjectSpawnManager`의 시작은 팀원 구현입니다.
- 건물 이름 기반 필터링과 수집 연결은 개인 구현 및 유지보수 범위입니다.
- 현재 Manager의 `slotOwner`와 lifecycle state는 2026 후속 개선이며 원래 구현과 구분합니다.

## Trade-offs

- Controller 이름을 데이터 키로 사용하므로 Scene 이름과 `buildingName` 규칙이 일치해야 합니다.
- cache는 시작 시점의 Entry 목록을 기준으로 하며 런타임 추가·변경을 자동 반영하지 않습니다.
- 일반 캐릭터의 `NoonsongEntry == null`은 현재 의도된 표현이지만, 타입으로 구분하는 모델보다 null 분기에 의존합니다.
