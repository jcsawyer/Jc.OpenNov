# Jc.OpenNov

Library for reading data from NFC Novo Nordisk insulin pens in .NET.

C# implementation derived from [lcacheux](https://github.com/lcacheux)'s Kotlin [nov-open-reader](https://github.com/lcacheux/nov-open-reader/tree/main) project - big thanks!

## Table of Contents

- [Introduction](#introduction)
- [Usage](#usage)
- [Barebones (MAUI/etc.)](#barebones-mauietc)
- [Android](#android)
- [iOS (coming soon)](#ios)

## Introduction

Jc.OpenNov is a library designed to facilitate the reading of data from NFC Novo Nordisk insulin pens.

### Components

- _Jc.OpenNov:_ Core library containing data structure and protocol implementation.
- _Jc.OpenNov.Nfc.Android:_ Android implementation of NFC communication using Jc.OpenNov.
- _Jc.OpenNov.Avalonia:_ Avalonia implementation of NFC communication using Jc.OpenNov.
- _Jc.OpenNov.Avalonia.Android:_ Android implementation of NFC communication using Jc.OpenNov.Avalonia.

### Sample Screenshots

| Android | iOS |
| ------- | --- |
| <img alt="Android" src="img/android.JPG" width="250" /> | | 

## Usage

### Barebones (MAUI/etc.)

To use Jc.OpenNov, you need to install the NuGet package:

```bash
dotnet add package Jc.OpenNov
```

Followed by adding the Android/iOS `Jc.OpenNov.Nfc.xxx` package to your project:

```bash
dotnet add package Jc.OpenNov.Nfc.Android
```

### Android

Install the following NuGet packages to their respective projects:

```bash
dotnet add package Jc.OpenNov.Avalonia
dotnet add package Jc.OpenNov.Avalonia.Android
```

Add the following to your AndroidManifest.xml:

```xml
<uses-permission android:name="android.permission.NFC" />
<uses-feature android:name="android.hardware.nfc" android:required="true" />
```

In your `MainActivity`, add to your `AppBuilder` like so:

```csharp
protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
{
    return base.CustomizeAppBuilder(builder)
        // ...
        .UseOpenNov(this);
}
```

To start listening for NFC tags, call:

```csharp
OpenNov.Current.MonitorNfc(/* you may pass in an optional stop condition */);
```

Likewise, to stop listening for NFC tags, call:

```csharp
OpenNov.Current.StopNfc();
```

To know when an NFC tag is detected and data is obtained, subscribe to the event handlers:

```csharp
public MainViewModel()
{
    StarNfcCommand = ReactiveCommand.Create(StartNfc);
    StopNfcCommand = ReactiveCommand.Create(StopNfc);

    Avalonia.OpenNov.Current.OnDataRead += OnDataRead;
    Avalonia.OpenNov.Current.OnTagDetected += OnTagDetected;
    Avalonia.OpenNov.Current.OnError += OnError;
}

~MainViewModel()
{
    Avalonia.OpenNov.Current.OnDataRead -= OnDataRead;
    Avalonia.OpenNov.Current.OnTagDetected -= OnTagDetected;
    Avalonia.OpenNov.Current.OnError -= OnError;
}

private void OnDataRead(object? sender, Data.PenResult e)
{
    if (e is Data.PenResult.Success success)
    {
        Serial = success.Data.Serial;
    }
}

private void OnTagDetected(object? sender, ITag? e)
{
    var bytes = e?.GetId();
    if (bytes is null)
    {
        TagId = "Tag not found";
        return;
    }

    TagId = Convert.ToHexString(bytes);
}

private void OnError(object? sender, Exception e)
{
    Error = e.Message;
}
```

### iOS

// Coming soon
