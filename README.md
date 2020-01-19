[![AppVeyor](https://img.shields.io/appveyor/ci/xleon/optimus.svg?style=for-the-badge)](https://ci.appveyor.com/project/xleon/optimus) 
[![NuGet](https://img.shields.io/nuget/v/optimustool.svg?style=for-the-badge)](https://www.nuget.org/packages/optimustool/) 
[![NuGet](https://img.shields.io/nuget/dt/optimustool.svg?style=for-the-badge)](https://www.nuget.org/packages/optimustool/)

# Optimus
Command line tool to track and optimise all the images under a git repo

# Install

Make sure you have [dotnet core](https://dotnet.microsoft.com/download) installed at your machine.

```console
dotnet tool install -g optimustool
```

# Update

Once installed, you can update to the latest version with:

```console
dotnet tool update -g optimustool
```

Please check releases at https://github.com/xleon/Optimus/releases

# Uninstall

```console
dotnet tool uninstall -g optimustool
```

# Use

From anywhere in your system:

```console
cd /your/project/path && optimustool
```

or
```console
optimustool /your/project/absolute/path
```

The first time you run the command a configuration .txt file will be created in your project root (`OptimusConfiguration.txt`). You must edit the file with your TinyPNG API key/s. 
You can use multiple keys. When 1 key reaches its monthly limits the next one will be used.  

```json
{
  "tinyPngApiKeys": [
    "api-key-1",
    "api-key-2"
  ],
  "fileExtensions": [
    ".jpg",
    ".jpeg",
    ".png"
  ]
}
```

The tool will scan all images (only files added to git) with the extensions specified in the configuration file and will start optimising each one with TinyPNG API.
Optimised files will be tracked in a text file at your project root called `OptimusFileTracker.txt`.

Both `OptimusConfiguration.txt` and `OptimusFileTracker.txt` should be added to git so that other developers can optimise only the new images added to the project.

Deleted or modified images from the file system will be untracked when the command starts, so you should not have to worry about them. 
