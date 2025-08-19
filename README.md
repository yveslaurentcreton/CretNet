# CretNet

Modern .NET building blocks and a ready-to-use Blazor application platform.

This repository now has two pillars:

1. Core libraries you can use anywhere in .NET
2. CretNet Platform for quickly building cohesive Blazor web apps with pre-made controls, services, and Fluxor-based patterns

Target framework: .NET 9 only.

Documentation site: <https://dotnet.creton.dev>

## 1. Core libraries

| Name | NuGet | Description |
| --- | --- | --- |
| **CretNet** | [![NuGet](https://img.shields.io/nuget/v/CretNet.svg?logo=nuget)](https://www.nuget.org/packages/CretNet) | Small, practical helpers and extensions (collections, enums, LINQ). |
| **CretNet.Blazor** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Blazor.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Blazor) | Blazor-focused helpers/utilities. |
| **CretNet.FluentValidation.DependencyInjection** | [![NuGet](https://img.shields.io/nuget/v/CretNet.FluentValidation.DependencyInjection.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.FluentValidation.DependencyInjection) | FluentValidation registration helpers for Microsoft.Extensions.DependencyInjection. |

## 2. CretNet Platform (Blazor app accelerator)

The Platform is a set of cohesive projects that work together so you can bootstrap production-grade Blazor apps quickly. It leans on Fluxor for state management and Microsoft Fluent UI components for a consistent UX.

Highlights:

| Name | NuGet | Description |
| --- | --- | --- |
| **CretNet.Platform** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform) | Base abstractions like `IEntity<TId>`, DI helpers (e.g., `AddDecoratedSingleton`). |
| **CretNet.Platform.Data** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Data.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Data) | Repository pattern (`Repository<TEntity,TId>`) built on EF Core and Ardalis.Specification. |
| **CretNet.Platform.Data.Abstractions** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Data.Abstractions.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Data.Abstractions) | Default/search specs (`IEntityDefaultSpecification<T>`, `IEntitySearchSpecification<T>`). |
| **CretNet.Platform.Fluxor** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Fluxor.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Fluxor) | Helpers for Fluxor: `ICnpAction`, `ICnpEntityAction<T>`, `DispatcherExtensions.DispatchAsync(...)`. |
| **CretNet.Platform.Fluxor.Generators** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Fluxor.Generators.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Fluxor.Generators) | Source generators for actions/entities (reduce boilerplate). |
| **CretNet.Platform.Blazor** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Blazor.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Blazor) | Fluent UI components: grids/selects/dialogs; inputs; layout; filters; dynamic renderers; notifications. |
| **CretNet.Platform.Blazor.Server** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Blazor.Server.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Blazor.Server) | Server-hosting specifics for the Blazor Platform. |
| **CretNet.Platform.WebApi.Utilities** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.WebApi.Utilities.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.WebApi.Utilities) | Minimal API helpers (e.g., `MapPing()` health endpoint). |
| **CretNet.Platform.Storage** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Storage.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Storage) | Storage abstraction (`IStorageService`). |
| **CretNet.Platform.Storage.Sharepoint** | [![NuGet](https://img.shields.io/nuget/v/CretNet.Platform.Storage.Sharepoint.svg?logo=nuget)](https://www.nuget.org/packages/CretNet.Platform.Storage.Sharepoint) | SharePoint implementation/services & DI. |

## Showcase application

Applications that showcases the CretNet Platform:

- [CretCollect](https://github.com/yveslaurentcreton/CretCollect)

## Contributing

Contributions are welcome. We follow GitHub Flow with semantic releases:

- Branch types: `feature/*` and `bugfix/*`
- Submit a Pull Request to `main` when ready
- Use clear, conventional commit messages (semantic commits)

Report issues and feature requests here: <https://github.com/yveslaurentcreton/CretNet/issues>

Localization: English and Dutch are available out of the box; contributions for additional languages are welcome.

## License

Licensed under the [MIT License](LICENSE).
