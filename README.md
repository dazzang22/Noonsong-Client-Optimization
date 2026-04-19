# Friends_noonsong

> This repository is a fork of a team project.
> I was responsible for refactoring the AR object detection system and optimizing runtime
> performance.

Key Contributions:
- Refactored AR object detection flow
- Reduced detection cost from O(N) to O(1)
- Eliminated unnecessary allocations and Update-based polling

---

# ❄️ 프렌즈! 눈송 (GPS 기반 AR 수집 게임)
![ScreenRecording_04-14-202518-39-19_1-ezgif com-resize](https://github.com/user-attachments/assets/cf67caca-2726-469d-8398-0f3abd7253f0)
| 플랫폼 | 모바일 |
| --- | --- |
| ESD | Android, IOS |
| 장르 | 위치 기반 AR 게임 |
| 엔진 | Unity (2022.3.10f1) |
| 플레이 타임 | 7-10h |
| 기타 링크 | [Notion](https://friendsnoonsong.notion.site) |
> GPS 기반 AR 수집 게임으로,  
> 위치에 따라 생성된 캐릭터를 수집하고 도감을 완성하는 인터랙티브 게임입니다.


## 🩶 Overview

- **Platform**: Unity (AR Foundation)
- **Language**: C#
- **Role**: Client Developer (시스템 설계 및 구현)
- **Focus**: AR 인터랙션, 데이터 흐름 설계, UI 동기화, 라이브 환경 대응

## 🩶 Core Achievement

- `Spawn` → UI Sync 파이프라인 설계
- `AR Object` 탐지 구조 리팩토링
- `Update` 기반 로직을 `Coroutine` 구조로 최적화
- 탐색 및 데이터 처리 구조 개선
  
## 🩶 Key Implementation (Initial)
**1. `ScriptableObject` 기반 데이터 모델 설계**
- `Noonsong` / `Friends` / `Item` 등 게임 내 주요 엔트리 구조를 `ScriptableObject`로 설계  
- 게임 로직, UI, DB 동기화가 동일한 데이터 모델을 기준으로 동작하도록 구조 설계
<details>
<summary>Code</summary> 
    
~~~csharp
[CreateAssetMenu(fileName = "NewNoonsongEntry", menuName = "Noonsong Entry")]
public class NoonsongEntry : ScriptableObject
{
    public string noonsongName;
    public string university;
    public string description;
    public Sprite noonsongSprite;
    public bool isDiscovered;
    public GameObject prefab;
    public int requiredNoonsongs;
    public string buildingName;

    [Range(0, 100)]
    public int loveLevel = 0;
    public bool isFriend;
    public bool isBestFriend;
}
~~~
</details>

**2. 데이터 흐름 기반 UI 동기화 구조 설계**
- Spawn된 오브젝트와 `Entry` 데이터를 연결하여  
  Target → `Entry` → UI로 이어지는 데이터 파이프라인 구축  
- 수집 시 상태 변화가 도감 UI에 즉시 반영되도록 동기화 구조 설계  

**3. AR 기반 인터랙션 시스템 구현**
- Camera 기준으로 현재 상호작용 가능한 타겟을 판별하고 `currentTarget`으로 관리  
- 단순 `Raycast`로는 화면 내 노출 여부를 정확히 판단하기 어려워,  
  오브젝트의 바운더리 포인트를 기준으로 화면 내 존재 여부를 판정하는 로직 구현

<details>
<summary>Code</summary>
    
~~~csharp
bool isVisible = false;
foreach (Vector3 point in checkPoints)
{
    Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

    if (screenPoint.z > 0 &&
        screenPoint.x > -50 && screenPoint.x < Screen.width + 50 &&
        screenPoint.y > -50 && screenPoint.y < Screen.height + 50)
    {
        isVisible = true;
        break;
    }
}
~~~
</details>

→ 화면 중심 `Raycast`의 한계를 보완하여, 실제 사용자 시야 기준으로 상호작용 대상을 판별하도록 개선  

**4. 게임 시스템 구현 (재화 / 인벤토리 / 상점 / 관계 시스템)**

**5. 베타 테스트 기반 문제 분석 및 패치**

--- 

## 🩶 Core Problem 
기존 스폰 시스템은 랜덤 기반으로 동작하여,  
위치와 캐릭터 간의 연결 기준이 없었습니다.

또한, 스폰된 오브젝트와 도감(UI) 데이터가 분리되어  
수집 상태가 일관되게 관리되지 않는 문제가 있었습니다. 

## 🩶 Solution — Location-based Spawn Filtering
> 기존 랜덤 스폰 구조 위에,  
> 현재 진입한 건물 정보를 기준으로 캐릭터 후보군을 필터링하는 로직을 추가했습니다.

이를 통해  
위치 → 캐릭터 → 데이터로 이어지는 기준을 만들었습니다.

<details>
<summary>Code</summary>

```csharp
List<NoonsongEntry> GetFilteredNoonsongEntries()
{
    var activationController = GetComponentInParent<ScriptActivationController>();
    string buildingName = activationController != null ? activationController.gameObject.name : null;

    if (!string.IsNullOrEmpty(buildingName))
    {
        return GetNoonsongEntriesByBuildingName(buildingName);
    }

    return new List<NoonsongEntry>();
}

List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
{
    List<NoonsongEntry> filteredEntries = new List<NoonsongEntry>();
    NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();

    foreach (var entry in entries)
    {
        if (entry.buildingName == buildingName)
        {
            filteredEntries.Add(entry);
        }
    }

    return filteredEntries;
}
```
</details>

현재 진입한 건물 기준으로 캐릭터 후보군을 필터링했으며, 
필터링된 `Entry List`를 기존 스폰 로직에 연결하여 프리팹과 데이터가 함께 전달되도록 구성했습니다.

<details>
  <summary>Code</summary>
  
  ~~~csharp
if (filteredEntries.Count > 0)
{
    return new SpawnedObject(filteredEntries[randomIndex].prefab, filteredEntries[randomIndex]);
}
~~~
  
</details>

## 🩶 Optimization

### 1. 탐색 구조 개선
- 기존: `Update`에서 `Entry` 전체 순회 (O(N))
- 개선: `Dictionary` 기반 구조 → O(1)
→ 탐색 비용 대폭 감소
<details>
<summary>Before / After</summary>

### Before
~~~csharp
List<NoonsongEntry> filteredEntries = new List<NoonsongEntry>();
NoonsongEntry[] entries = noonsongEntryManager.GetNoonsongEntries();

foreach (var entry in entries)
{
    if (entry.buildingName == buildingName)
    {
        filteredEntries.Add(entry);
    }
}
~~~
### After
~~~csharp
List<NoonsongEntry> GetNoonsongEntriesByBuildingName(string buildingName)
{
    if (entriesByBuilding != null && entriesByBuilding.TryGetValue(buildingName, out var entries))
    {
        return entries;
    }
    return EmptyEntries;
}
~~~
</details>

### 2. GC Alloc 제거
- 기존: 매 프레임 `new` 연산 발생
- 개선: 배열 재사용 구조로 변경
→ 프레임 드랍 원인 제거
<details>
<summary>Before / After</summary>

### Before
~~~csharp
Vector3[] checkPoints = new Vector3[]
{
    objectPosition,
    objectPosition + new Vector3(boundingRadius, 0, 0),
    objectPosition - new Vector3(boundingRadius, 0, 0),
    objectPosition + new Vector3(0, boundingRadius, 0),
    objectPosition - new Vector3(0, boundingRadius, 0)
};
~~~
### After
~~~csharp
checkPoints[0] = objectPosition;
checkPoints[1] = objectPosition + rightOffset;
checkPoints[2] = objectPosition - rightOffset;
checkPoints[3] = objectPosition + upOffset;
checkPoints[4] = objectPosition - upOffset;
~~~
</details>

### 3. 실행 구조 개선
- 기존: `Update` 기반 탐지 로직
- 개선: `Coroutine` 기반 실행 주기 제어
→ 불필요한 연산 제거 및 CPU 안정화
<details>
<summary>Before / After</summary>

### Before
~~~csharp
void Update()
{
    DetectObject();
}
~~~
### After
~~~csharp
IEnumerator DetectRoutine()
{
    while (true)
    {
        DetectObject();
        yield return waitInterval;
    }
}
~~~
</details>

## 🩶 Result

- 탐색 연산: O(N) → O(1)
- GC Alloc 제거
- 불필요한 `Update` 호출 제거
- 프레임 드랍 제거 및 안정성 확보
- 위치 기반 스폰과 UI 상태가 일관되게 연결되는 구조 완성

## 🧪 Live Experience

- iOS / Android 크로스 플랫폼 환경에서 테스트 및 운영
- 100건 이상의 버그 및 사용자 피드백 대응
- 실제 사용자 로그 기반 문제 분석 및 원인 추적
- GPS 오차 및 네트워크 상태에 따른 예외 상황 대응
- 현장에서 직접 디버깅 및 이슈 재현 / 수정 / 패치 배포 경험

---
