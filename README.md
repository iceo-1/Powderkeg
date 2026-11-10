# Powderkeg

A clean, empty [MonoGame](https://monogame.net/) project for all supported desktop and mobile platforms, using the new MonoGame Content Builder.

It contains no game code beyond a minimal `Game1` class and no content - a blank canvas for building any game.

## Solution layout

| Folder | Purpose |
| --- | --- |
| `Source/` | Shared game code, compiled into every platform project. Start in `Game1.cs`. |
| `Resources/` | Your game's identity, icons and platform art, referenced by every platform. One copy of each asset, no duplicates per platform. |
| `Content/` | The Content Builder project. Put your raw assets under `Content/Assets/` and register them in `Content/Builder/Builder.cs`. Content is built automatically as part of each platform build. |
| `Platforms/DesktopGL/` | Cross-platform OpenGL desktop (Windows, Linux, macOS). |
| `Platforms/DesktopVK/` | Cross-platform Vulkan desktop (Windows, Linux, macOS). |
| `Platforms/WindowsDX/` | Windows DirectX 11 desktop. |
| `Platforms/WindowsDX12/` | Windows DirectX 12 desktop. |
| `Platforms/Android/` | Android mobile (`net10.0-android`). |
| `Platforms/iOS/` | iOS mobile (`net10.0-ios`, requires a Mac to build/deploy). |

Each folder under `Platforms/` contains only its entry point and platform-specific configuration; all game logic lives once in `Source/`.

## Naming your game

Everything that identifies your game lives in one file, [`Resources/Identity.props`](Resources/Identity.props). Every platform project imports it, so a value set there applies everywhere and no platform project file needs editing.

> [!TIP]
> You can override any of these by editing the `csproj`, manifest or `Info.plist` directly, if you prefer.

| Property | What it sets |
| --- | --- |
| `AssemblyName`, `AssemblyTitle` | The name of the built executable. |
| `ApplicationTitle` | The display name players see on the iOS home screen. |
| `ApplicationId` | Your unique app identity. Becomes the Android package name and the iOS bundle identifier. Change this before publishing to any store. |
| `ApplicationVersion` | Build number, incremented every store submission. Android `versionCode`, iOS `CFBundleVersion`. |
| `ApplicationDisplayVersion` | Public version string. Android `versionName`, iOS `CFBundleShortVersionString`. |

> [!NOTE]
> The Android name shown under the launcher icon is the one exception: it comes from `Resources/Android/Values/strings.xml`, because Android reads it from a string resource so it can be localised.

## Getting started

Creating your project from the command line/terminal is as simple as:

> [!TIP]
> Alternatively, you can use Visual Studio, VS Code or Rider with the .NET templates installed.
> See the [2D beginner tutorial](https://docs.monogame.net/articles/tutorials/building_2d_games/02_getting_started/index.html) for more information on getting your machine set up.

```sh
dotnet new monogame -n MyAwesomeGame --Platforms gl vk dx dx12
cd MyAwesomeGame
dotnet build
dotnet run --project Platforms/DesktopGL
```

> [!IMPORTANT]
> Build the solution once first. The Content Builder is a separate project that the platforms invoke rather than reference, so it needs one solution-wide restore before a single project will build on its own. The root folder holds only the solution, so a bare `dotnet build` builds every project. `dotnet run` still needs `--project` to choose a platform.

Or open the solution in Visual Studio, VS Code or Rider and pick a platform launch profile. The `.vscode` folder holds one build task and one launch profile per selected platform.

> [!TIP]
> The list of platforms supported by the template are as follows:
>
> - gl    - OpenGL Desktop
> - vk    - Vulkan Desktop
> - dx    - Windows Desktop DirectX
> - dx12  - Windows Desktop DirectX12
> - android - Android mobile
> - ios - iOS mobile
>
> run `dotnet new monogame --help` for more details on the parameters for the template.

## Adding content

1. Drop asset files into `Content/Assets/`.
2. Register them in `Content/Builder/Builder.cs` (see the commented examples in `Builder.GetContentCollection()`).
3. Build - the Content Builder runs automatically before compilation and the built content is bundled per platform.
4. Load assets in code with `Content.Load<T>("assetName")`.

## Replacing the placeholder art

All project artwork is located under `Resources/`, linked into the projects that need them. Replace the files in place and keep the names, and every platform that uses one picks up the change.

| Path | Used by |
| --- | --- |
| `Icon.png` | The 1024px master, also linked in as the iOS marketing icon. |
| `Icon.ico`, `Icon.bmp` | All four desktop platforms. |
| `Android/drawable-*/icon.png` | The Android launcher icon, one per screen density. |
| `Android/drawable-*/splash.png` | The Android splash screen. |
| `Android/Values/` | The resource entries that reference the Android art, plus the app name. |
| `iOS/AppIcon.xcassets/` | The remaining iOS app icon sizes. |
| `macOS/Icon.icns` | A macOS `.app` bundle, if you package one by hand. |

Each platform subfolder also holds that platform's manifest, where it has one:

- `iOS/Info.plist` sets supported orientations, status bar and device family for mobile devices.
- `macOS/Info.plist` describes a macOS `.app` bundle, for the DesktopGL and DesktopVK platforms only. It is a template: `macOS/MacBundle.targets` fills in the `$(...)` tokens from `Identity.props` and writes the result next to the binaries. Edit this file, not the generated copy.

## Notes

The MonoGame version is managed centrally in `Directory.Packages.props`, to make it easier to update the entire project at once. You can still override it per project if you need to.

