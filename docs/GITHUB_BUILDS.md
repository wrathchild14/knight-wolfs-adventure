# Building on GitHub

The repository includes `.github/workflows/build.yml`. GitHub hosts the build machines; no server, personal access token, or repository secret is required for this workflow.

## First run

1. Push the commits from your local checkout. The current branch is `master`:
   ```sh
   git push origin master
   ```
2. Open https://github.com/wrathchild14/knight-wolfs-adventure/actions and select **Build and test**. A push starts the workflow automatically.
3. If GitHub reports that Actions is disabled, enable it under **Settings → Actions → General**. Repository or organization policies must permit `actions/checkout` and `actions/upload-artifact`.
4. Wait for both matrix jobs, Linux and Windows, to finish. Each job compiles the game, runs the mechanics tests and complete journey simulation, then packages the executable, assets and documentation. Linux additionally runs a graphical smoke test on a virtual display.
5. Open the run summary and download **knight-wolf-Linux** or **knight-wolf-Windows** under **Artifacts**. Sign in with an account that has repository read access. Artifacts are retained for 30 days.

The artifact download contains the platform archive. Extract that archive as well:

- **Linux:** extract `knight-wolf-linux-x64.tar.gz` and run `./knight_wolf` from the extracted directory. The tar archive preserves executable permissions. This build targets the GitHub Ubuntu runner's system libraries; build locally if your distribution is older.
- **Windows:** extract `knight-wolf-windows-x64.zip` and launch `knight_wolf.exe`. Keep the entire `assets` directory beside it. The Visual Studio build may require the Microsoft Visual C++ x64 Redistributable on a machine without it.

These are workflow artifacts, not automatically published GitHub Releases. No binaries need to be committed to the repository.

## Later runs

Pushes and pull requests start builds automatically. For a manual build, use **Actions → Build and test → Run workflow**, select the branch, and confirm. The workflow must first exist on the repository's default branch for the manual-run button to be available.

With the optional GitHub CLI, after signing in:

```sh
gh workflow run build.yml --ref master
gh run list --workflow build.yml
# Substitute the run ID shown by the previous command:
gh run watch RUN_ID
gh run download RUN_ID
```

## If a job fails

Open the failing job and expand the red step. Configuration errors usually concern dependency downloads or runner changes; test failures show the specific failed mechanic or chapter. Failed jobs do not upload a game package. Fix the cause locally and push another commit, or use **Re-run failed jobs** for a temporary service/network failure.

The workflow is configured in this checkout but has not yet been run remotely. Local validation is recorded in [VALIDATION.md](VALIDATION.md).

GitHub reference: [manually running workflows](https://docs.github.com/en/actions/how-tos/manage-workflow-runs/manually-run-a-workflow) and [downloading artifacts](https://docs.github.com/en/actions/how-tos/manage-workflow-runs/download-workflow-artifacts).
