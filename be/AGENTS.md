# Backend agent instructions

Read `../doc/.be/README.md`, `architecture.md`, `requirements.md`, and the relevant data/API/flow pages before editing.
Use .NET 10, four Clean Architecture projects, and PostgreSQL. Domain must not reference EF, Identity, or HTTP.
Never commit secrets, log connection strings, or put credentials in frontend variables.
Every backend change MUST update the affected files in `doc/.be/` and its changelog in the same change. Clearly distinguish implemented behavior from planned APIs.
Run relevant unit/integration tests. Never use the NAS database for automated destructive tests.
Do not automatically migrate or seed on API startup. Do not modify Docker volumes or unrelated frontend code.
Check ownership, archive, transactions, revision, and idempotency when implementing business mutations.
