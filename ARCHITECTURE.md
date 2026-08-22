# JRC Ligo Campaign Gateway Architecture

## Status
CURRENT documentation index for the Sytel <-> Ligo/Meta campaign middleware.

## System Boundary
`Jrc.LigoCampaignGateway` mediates campaign/message operations between Sytel Omnichannel and Ligo Messaging / Meta WhatsApp Business Platform. It is middleware/integration infrastructure, not a conversational-agent repository.

## Core Structure
- C# / .NET 10 solution.
- Clean Architecture under `src/`.
- Automated tests under `tests/`.
- Deployment assets under `deploy/`.
- API/provider contracts under `docs/contracts/`.
- Architecture decisions under `docs/architecture/`.
- Operational procedures under `docs/runbooks/`.

## Key ADRs
- `docs/architecture/ADR-000-runtime-version.md`
- `docs/architecture/ADR-001-net10-lts.md`
- `docs/architecture/ADR-002-media-lease-and-multipart.md`
- `docs/architecture/ADR-003-idempotency-and-state-machine.md`
- `docs/architecture/ADR-004-token-provider-and-callbacks.md`

Read the relevant ADR before changing those contracts.

## Architectural Invariants
- Preserve Clean Architecture boundaries.
- Idempotency/state transitions must prevent duplicate external side effects.
- Retry must be bounded and compatible with idempotency.
- Provider acceptance and final delivery are distinct states.
- Media upload/lease and callbacks follow their documented lifecycle/contracts.
- Production sends are not test primitives.
- Secrets and sensitive message/customer data are not committed.

## Source of Truth
- GitHub: CURRENT code, ADRs, contracts and versioned technical docs.
- Google Drive: shared corporate docs, vendor manuals, evidence, baselines and runbooks where applicable.
- Jira: backlog, owner, priority, status and acceptance.
- Official provider documentation: current external API behavior when contracts need validation.
- ChatGPT Project: active context and analysis.

## Change Policy
Contract or architectural changes require design/ADR, isolated branch, `dotnet test` or applicable verification, diff review and Pull Request. `CURRENT`, `HML`, `PRODUCAO`, `BASELINE_VALIDADO`, `EXPERIMENTAL` and `LEGADO` are distinct states.
