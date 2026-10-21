# Spawn Lifecycle Follow-up

이 문서는 2026년에 수행한 후속 개선을 설명합니다. 2024–2025 원 프로젝트의 개인 구현과 동일한 비중의 원성과로 제시하지 않습니다.

## Context

팀원이 도입한 전역 단일 Spawn 정책은 여러 Controller가 Manager의 공용 객체 목록을 사용했습니다. 후속 검토에서는 각 Controller의 생성 객체, 남은 수명, 사진 모드 상태, 제거 권한을 명시적으로 연결할 필요가 있다고 판단했습니다.

이 검토는 구조적 위험을 다룬 것입니다. 과거 라이브 Spawn 중단의 원인이 소유권 충돌이었다고 확정하지 않습니다.

## State Contract

`SpawnLifecycle`은 Unity 객체를 직접 조작하지 않고 상태에 따라 명령을 반환합니다.

```text
NoObjects
  └─ spawn succeeded → Visible

Visible
  ├─ photo opened → HiddenForPhoto
  ├─ removed externally → WaitingForLifetimeEnd
  └─ expired → NoObjects + Remove

HiddenForPhoto
  ├─ photo closed → Visible + Show
  └─ expired → NoObjects + Remove
```

`PlayerObjectSpawn`은 반환된 `Hide`, `Show`, `Remove` 명령을 실행합니다. 상태 판단과 `GameObject.SetActive/Destroy`를 분리해 순수 EditMode 테스트가 가능하도록 구성했습니다.

## Ownership

- Controller는 자신이 생성한 `ownedSpawnedObject`를 보관합니다.
- Manager는 전역 Spawn 가능 여부와 `slotOwner`를 관리합니다.
- 만료 시 Controller는 자신이 소유한 객체만 제거합니다.
- 조우 중인 객체는 화면에서 즉시 제거하지 않더라도 필드 슬롯과 registry를 정리합니다.

## Tests

프로젝트 자체 lifecycle test는 총 13개입니다.

- `SpawnLifetimeTests`: 2
- `SpawnLifecycleTests`: 11

검증 범위에는 정확한 1회 만료, 사진 모드 전환, 외부 제거, 중복 알림, 만료 후 재진입, 다른 Controller 객체의 비소유 제거 방지가 포함됩니다.

## AI Usage

상태 전이 구현과 테스트 확장에는 AI 도구를 사용했습니다. 직접 수행하고 현재 설명 가능한 범위는 다음과 같습니다.

- 공용 참조와 제거 책임이 결합된 위험 식별
- Controller 단위 소유권과 Manager 단위 슬롯 책임이 필요한 이유 검토
- AI가 제안한 상태·명령 구조를 기존 게임 규칙과 대조
- 재현할 실패 상황과 기대 동작 정리
- 생성 코드의 동작을 게임 규칙과 대조하고 순수 로직 테스트 구성을 확인. 최신 Unity Test Runner 실행 결과는 재확인 필요

## Related History

- [`01948f81`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/01948f81) — lifetime tracking
- [`6cd8ef7f`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/6cd8ef7f) — state machine
- [`173d783a`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/173d783a) — overlap contract
- [`0135959e`](https://github.com/dazzang22/Noonsong-Client-Optimization/commit/0135959e) — encounter-aware lifecycle
