# MaterialDesignThemes.Prism

This package allows better integration of [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) with [Prism library](https://github.com/PrismLibrary/Prism).

## Table of Contents

1. [Features](#features)
1. [Examples](#examples)

## Features

- Show Material Design dialogs with Prism DialogService

The **DialogService** from Prism allows showing dialogs from your view models respectfully of MVVM architecture, without knowing the parts (View/ViewModel) that compose your dialog.

Dialogs are registered within the container (on your application composition root) with a name, a View, and a ViewModel so that the rest of your application needs to only know the name of the dialog in order to show it.

When you use the DialogHost from Material Design in XAML to show a dialog you lose some of these useful features. This library aims to bring you the best of both worlds, letting you use the DialogService from Prism with a Material Design in XAML DialogHost

## Examples

### How to show a modal dialog

- Register the **MaterialDialogService** implementation of IDialogService into your Prism container of choice, within **RegisterTypes** method

```
containerRegistry.Register<IDialogService, MaterialDialogService>();
```

- Call a **ShowDialogHost** overload instead of a **ShowDialog** whenever you call the Prism’s dialog service, by passing as a last parameter the Material Design in XAML’s **DialogHost**

For example, substitute this:

```
_dialogService.ShowDialog("YourDialogName", new DialogParameters(), result =>
            {
                // Access your result parameters
            });
```

to this:

```
_dialogService.ShowDialogHost("YourDialogName", new DialogParameters(), result =>
            {
                // Access your result parameters
            }, "MainDialogHost");
```