# Runtime Optimization Measurement

## Operational Signal

2025년 라이브 운영 중 장시간 플레이 환경에서 발열과 Spawn 중단이 보고됐습니다. 두 현상의 단일 원인을 확정하지 않고, 화면 변화가 없어도 매 프레임 호출되던 AR 객체 탐지 경로를 우선 측정 대상으로 정했습니다.

## Isolation Strategy

현재 [`ARObjectCatch.cs`](../Assets/Scripts/CollectionScene/ARObjectCatch.cs)의 benchmark variant는 두 변수를 나누어 비교합니다.

| Variant | Array | Frequency | Purpose |
|---|---|---|---|
| A | 매 호출 `new Vector3[5]` | 매 프레임 | 기존 반복 할당 재현 |
| B | `checkPoints` 재사용 | 매 프레임 | allocation 변화 분리 |
| C | `checkPoints` 재사용 | 0.1초 간격 | 호출 빈도 변화 분리 |

`ProfilerMarker("ARObjectCatch.CheckForObjectInView")`로 같은 탐지 구간을 표시했습니다.

```csharp
private void RunDetectionBenchmarkSample()
{
    using (DetectionMarker.Auto())
    {
        UpdateCurrentTarget();
    }
}
```

## Allocation Result

| Condition | GC Alloc |
|---|---:|
| A · 새 `Vector3[5]` 생성 | 92 B/call |
| B · 배열 재사용 | 0 B/call |

이 수치는 탐지 Marker에서 확인한 호출당 할당량입니다. 전체 프레임 또는 전체 애플리케이션의 GC Alloc을 의미하지 않습니다.

## Call-frequency Result

Profile Analyzer에서 각 조건을 300 frames씩 3회 비교했습니다.

| Metric | Before | After |
|---|---:|---:|
| Detection calls/sec · median | 95.5 | 8.2 |
| Reduction |  | 약 91.4% |

계산식은 `Marker Count ÷ 측정 시간`이며, 측정 시간은 `Frame Count × Mean frame time`으로 정리했습니다. Figma 포트폴리오의 `95.5 → 8.2 calls/sec`, `91.4% 감소` 표기와 일치합니다.

## Interpretation Boundary

- 배열 재사용으로 해당 Marker의 반복 할당이 제거된 것은 확인했습니다.
- 0.1초 주기로 탐지 호출이 줄어든 것은 확인했습니다.
- CPU 시간은 Profiler 표시 해상도 아래였으므로 정량 개선 수치로 제시하지 않습니다.
- 실제 기기 온도와 소비 전력은 측정하지 않았습니다.
- 이 실험만으로 장시간 Spawn 중단의 근본 원인을 확정하지 않습니다.
- `.pdata`는 Unity 전용 형식이므로 GitHub README에서 직접 열람하기 어렵습니다. 후속 정리 시 동일 화면의 PNG 또는 표 내보내기를 추가하는 편이 좋습니다.

## Related History

- [`0e72016c`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/0e72016c) — test spawn mode and AR detection optimization
- [`0c87cdff`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/0c87cdff) — Entry cache
- [`f468f5c4`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/f468f5c4) — target detection refactor
- [`85a53c41`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/85a53c41) — benchmark variants
- [`191447e7`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/191447e7) — detector instance logging
