# 🔒 Strict Branching & Git Policy

## ⚠️ Core Rule
**DO NOT COMMIT, MERGE, OR TARGET THE `main` BRANCH DIRECTLY.**

The `main` branch is reserved for stable production releases. All active development, refactoring, and quality engineering in this project must take place exclusively against the **`v1.2`** branch.

---

## 📋 Git Workflow Guidelines

1. **Source Branch**: Always branch from `v1.2`:
   ```powershell
   git checkout v1.2
   git pull origin v1.2
   git checkout -b test/quality-assurance-suite
   ```

2. **Commit Standard**: Keep commits small, atomic, and conventional:
   - `test: Adicionar testes unitários para CreateCourseCommandHandler`
   - `test: Adicionar testes de validação com FluentValidation`
   - `test: Adicionar testes de arquitetura com NetArchTest`

3. **Pull Request Target**:
   When opening PRs with the GitHub CLI, explicitly pass `--base v1.2`:
   ```powershell
   gh pr create --base v1.2 --title "test: Suíte de Testes e Garantia de Qualidade" --body-file pr_body.md
   ```
