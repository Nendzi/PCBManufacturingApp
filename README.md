# PCBManufacturingApp

PCBManufacturingApp is a Windows desktop application for configuring and quoting printed circuit board (PCB) manufacturing options. It is built with WPF on .NET 8 and uses an MVVM-style structure with dependency injection for application services.

## Features

- PCB preferences form with material, solder mask, and thickness selection.
- Filtered option lists where available solder masks depend on the selected material, and available thicknesses depend on the selected solder mask.
- Price and delivery-time impact messages based on selected manufacturing options.
- Quote tab for selecting PCB width, length, and number of layers from the available inventory.
- 3D PCB layer preview rendered with WPF `Viewport3D`.
- Postal code validation with live error feedback.
- Order confirmation flow using a dialog service abstraction.

## Technology Stack

- C#
- .NET 8
- WPF
- XAML
- MVVM-style view models
- Microsoft.Extensions.DependencyInjection
- Microsoft.Xaml.Behaviors.Wpf

## Requirements

- Windows
- .NET 8 SDK
- Visual Studio 2022 or another IDE/editor that supports WPF projects

## Getting Started

Clone or open the repository, then restore and run the solution:

```powershell
dotnet restore PCBManufacturingApp.sln
dotnet build PCBManufacturingApp.sln
dotnet run --project PCBManufacturing\PCBManufacturing.csproj
```

You can also open `PCBManufacturingApp.sln` in Visual Studio and run the `PCBManufacturing` project.

## Project Structure

```text
PCBManufacturingApp/
+-- PCBManufacturingApp.sln
+-- README.md
+-- PCBManufacturing/
    +-- App.xaml
    +-- App.xaml.cs
    +-- MainWindow.xaml
    +-- MainWindow.xaml.cs
    +-- Commands/
    |   +-- BasicRelayCommand.cs
    +-- Controls/
    |   +-- DropdownListWithLabel.xaml
    +-- Converters/
    |   +-- BoolToVisibilityConverter.cs
    |   +-- DoubleToVisibilityConverter.cs
    |   +-- SelectedWidthToFontWeightConverter.cs
    +-- Models/
    |   +-- InventoryModel.cs
    |   +-- OptionItemModel.cs
    +-- Services/
    |   +-- IDialogService.cs
    |   +-- MessageBoxService.cs
    +-- Validators/
    |   +-- PostalCodeValidationRule.cs
    +-- ViewModels/
        +-- DropdownListWithLabelViewModel.cs
        +-- MainWindowViewModel.cs
```

## Architecture Notes

The application starts in `App.xaml.cs`, where services are registered with `Microsoft.Extensions.DependencyInjection`. `MainWindow` receives `MainWindowViewModel` through constructor injection and assigns it as the window data context.

`MainWindowViewModel` contains the main application state and behavior:

- preference option data,
- selected material, solder mask, and thickness,
- quote dimensions and layer count,
- postal code validation state,
- 3D PCB layer model generation,
- order confirmation command.

The current data source is in-memory inside `MainWindowViewModel`. A future database or API-backed implementation could replace this with dedicated repository or service classes.

## Main Workflow

1. Choose material, solder mask, and PCB thickness on the Preferences tab.
2. Enter a valid five-digit postal code.
3. Select width, length, and layer count on the Quote tab.
4. Review the generated purchase summary on the Order tab.
5. Confirm the request.

## Validation

Postal codes are validated through `INotifyDataErrorInfo` in `MainWindowViewModel`. A valid postal code must:

- contain only digits,
- be exactly five digits,
- not start with `0`.

The confirm button is enabled only when the quote selections are complete and the postal code is valid.

## Build Outputs

Compiled files are generated under:

```text
PCBManufacturing/bin/
PCBManufacturing/obj/
```

These directories are build artifacts and should not be edited manually.

## Testing

There is currently no automated test project in the solution. Suggested next steps are:

- add unit tests for `MainWindowViewModel`,
- test postal code validation rules,
- test option filtering for material, solder mask, and thickness combinations,
- test command enablement for valid and invalid quote states.
