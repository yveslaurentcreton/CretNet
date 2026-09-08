# CretNet engineering contract

CretNet owns shared platform behavior and generic Blazor controls. Consumer
applications own their API transports, persistence and business-specific UI.

- Read [the UI contract](docs/docs/cn-ui-contract.md) before changing Cn controls.
- Preserve host compatibility and existing date/time interaction.
- Control labels and generic wording belong to CretNet resources; support English
  and Dutch and preserve resource fallbacks. Hosts own application resources.
- Ship meaningful tests with control changes; render interaction changes in a browser.
- Keep feature work on a separate branch; do not push or release without owner authorization.
- Semantic-release remains the version authority. Never hand-edit a release version or tag.
- During the HCMT adoption, feature validation is manual (`workflow_dispatch`);
  automatic pushes build/release only main. Restore push branch filters after the
  owner ends this temporary budget-saving period. See the workflow and UI contract.
