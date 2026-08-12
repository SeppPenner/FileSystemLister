# Project rules for Claude

## What this is

FileSystemLister is a small Windows Forms desktop application. The user picks a folder and an
output file, the program walks the folder recursively and writes the name of every file it finds
into that output file, one name per line. A check box switches the output to bulletin board code,
then every line gets a `[*]` prefix and the whole list is wrapped in `[list]` and `[/list]`. The UI
is bilingual (German, English) and switchable at runtime through a combo box. Distribution happens
as an Inno Setup installer, not as a NuGet package.

One solution `src/FileSystemLister.sln` with exactly one project,
`src/FileSystemLister/FileSystemLister.csproj`. There is no test project, no example project and no
second project of any kind.

Layout inside `src/FileSystemLister`:

- `Program.cs`: entry point, `[STAThread]`, `Application.EnableVisualStyles()`,
  `Application.SetCompatibleTextRenderingDefault(false)`, `Application.Run(new Main())`. This is
  the old style startup, not the `ApplicationConfiguration.Initialize()` one that newer templates
  generate.
- `Main.cs`: the whole application logic. Folder dialog, save file dialog, the `BackgroundWorker`
  that does the scan, the recursive enumeration, the result file and the language handling.
- `Main.Designer.cs` plus `Main.resx`: Windows Forms designer output. `Main.resx` is about 250 kB
  because the window icon is embedded in it. Designer code is generated, it does not follow the
  hand written conventions below, do not reformat it by hand.
- `UiThreadInvoke/UiThreadInvokeClass.cs`: one extension method `UiThreadInvoke` on `Control` that
  marshals an `Action` to the UI thread when `InvokeRequired` says so. The background scan reads
  every control value through it.
- `GlobalUsings.cs`: all usings of the project.
- `languages/de-DE.xml` and `languages/en-US.xml`: the UI texts, 11 keys per file.
- `FileSystemLister.ico`: application and installer icon. `License.txt`: shipped next to the
  executable.

Translation comes from the NuGet package
[HaemmerElectronics.SeppPenner.Language](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.Language/)
(assembly and namespace `Languages`, source in the sibling repository `CSharpLanguageManager`).
Its runtime contract is convention based and this project depends on it:

- `LanguageManager` loads every `*.xml` from a `languages` directory beside the executing assembly.
- Each file deserializes into `Identifier`, `Name` and `Words/Word/Key` plus `Value`. The
  identifier must be a culture name that `CultureInfo` understands (`de-DE`, `en-US`).
- `GetWord` returns `null` for an unknown key, it does not throw and it does not fall back to
  another language. A new UI text therefore has to be added to **both** language files, otherwise
  one language silently shows an empty string.
- The two XML files are copied to the output directory with `CopyToOutputDirectory=Always`, the
  same holds for `License.txt`. Removing that is what makes the shipped program start without any
  texts.

## Build

```powershell
dotnet build src/FileSystemLister.sln
```

- Single target framework `net9.0-windows`, `WinExe`, `UseWindowsForms`, `RuntimeIdentifiers`
  `win-x64`. This is a Windows only application, unlike the library it references.
- All build properties live directly in `src/FileSystemLister/FileSystemLister.csproj`. There is no
  `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.8-1` for the first
  commit after tag `1.0.7`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/FileSystemLister.sln --source https://api.nuget.org/v3/index.json`.
- `Setup/build-setup-files.bat` deletes all `bin` and `obj` folders below `src`, then runs
  `dotnet publish -c Release -o bin/publish` and removes the `*.pdb` files from the publish output.
  The batch file does **not** run the Inno Setup compiler, that is a separate manual step.
- **There are no tests in this repository.** Never claim a test run happened. Verification means a
  clean build, and where behaviour changed, starting the built executable, pointing it at a folder
  and comparing the written result file.

## Code conventions

Follow the surrounding code, it is consistent throughout the hand written files:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace `FileSystemLister`.
- XML doc comments on every type and every member, private members and event handlers included, no
  exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into `src/FileSystemLister/GlobalUsings.cs`, inside the existing
  `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig requires
  usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`), which
  global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- `Main.cs` is deliberately split into small single purpose private methods (`Initialize`,
  `InitializeCaption`, `SearchDirectory`, `SaveFileNames`, `SaveFileName`, ...). Keep new logic in
  that shape instead of growing one big handler.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **Only file names are written, never paths.** `SaveFileName` stores `Path.GetFileName(file)`, so
  two files with the same name in different folders produce two identical lines and the result file
  does not say where anything lives. That is what the program is for, it feeds forum posts.
- **Errors during the scan are swallowed on purpose.** The loops in `SaveFileNames` and
  `SearchDirectories` catch every exception per file and per directory and continue, which is what
  makes a scan over a folder with locked or protected subfolders finish at all.
- The button captions and the check box text are set **only** in the `OnLanguageChanged` handler,
  the designer assigns German literals that are never shown. `Initialize` therefore has a required
  order: `InitializeLanguageManager` subscribes to the event, and only the following
  `LoadLanguagesToCombo` with `SelectedIndex = 0` fires it for the first time. Swapping those two
  calls leaves the buttons on the designer literals.
- `SetCurrentLanguage("de-DE")` in `InitializeLanguageManager` runs before the event is subscribed,
  so it changes no caption. The language actually shown is whichever file ends up first in the
  combo box, which is `de-DE.xml` only because of the alphabetical file order.
- The window title is `Application.ProductName` plus `Application.ProductVersion`, and
  `ProductVersion` is the GitVersion informational version. On an untagged commit the title reads
  something like `FileSystemLister 1.0.8-1+Branch.master.Sha.bd94620...`. Only a tagged build shows
  a clean version.
- **The icon exists twice.** `src/FileSystemLister.ico` and `src/FileSystemLister/FileSystemLister.ico`
  are both tracked. Only the second one is used, by `ApplicationIcon` and by `SetupIconFile` in the
  Inno Setup script. The copy one level up is dead weight, but it is tracked history.
- `.gitignore` excludes `*.exe` and `[Bb]in`, yet `Setup/FileSystemLister-Setup.exe` is tracked. It
  was added with `git add -f` and has to be updated the same way for every release.
- `Setup/FileSystemLister-Setup.iss` is UTF-8 **with** BOM and has to stay that way. Inno Setup 6
  reads a script as UTF-8 only when the BOM is there, without it the file is interpreted in the
  system ANSI codepage and `Hämmer Electronics` becomes `HÃ¤mmer Electronics` in the installer.
  Editors that save "UTF-8 without BOM" by default silently break this.
- `README.md` is spelled in capitals here, the sibling repositories use `Readme.md`. Links from the
  outside point at the capitalized name.
- The form has `ClientSize` 713 x 201 but `MinimumSize` 674 x 240, so the window grows a little on
  the first show. Cosmetic, do not chase it.

## Releasing

The tag comes **before** the installer build, never after. GitVersion derives the assembly version
from the tag, so an installer compiled on an untagged commit contains an executable that reports
something like `1.0.8-4+Branch.master.Sha...` in its window title instead of a clean `1.0.8`.

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.8.0 (2026-08-11)** : Short description.`
3. Bump `MyAppVersion` in `Setup/FileSystemLister-Setup.iss` to the same version, four parts.
4. Commit that, then tag the commit with the plain version number, no `v` prefix (`1.0.7`,
   `1.0.6`, ...). The existing tags are lightweight tags, create new ones the same way.
5. Run `Setup/build-setup-files.bat`, it publishes the tagged commit to
   `src/FileSystemLister/bin/publish`.
6. Compile `Setup/FileSystemLister-Setup.iss` with Inno Setup, it writes
   `Setup/FileSystemLister-Setup.exe`.
7. Commit that file with `git add -f`, then push the commits and the tag. This last commit sits
   after the tag, the same way `Updated setup.` sits after tag `1.0.7`.

Never run the publish or the installer build unless explicitly asked to release.

There is no CI configuration in this repository, no `.github` folder and no publish pipeline. The
AppVeyor badge in `README.md` points to a build that is configured outside of the repository. There
is no `Updating.md` and no `HowToUse.md` here, the `README.md` with the two screenshots
(`Screenshot_DE.PNG`, `Screenshot_EN.PNG`) is the only user documentation.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies, the `de-DE.xml` values) always use real umlauts and ß,
  never ASCII transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and
  configuration keys stay unchanged where umlauts are technically undesirable.
