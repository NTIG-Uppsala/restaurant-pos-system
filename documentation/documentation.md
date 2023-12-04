# Documentation

## Table of Content

- [Getting Started for Development](#getting-started-for-development)
- [Manage the installer](#manage-the-installer)
- [Add product buttons during development](#add-product-buttons-during-development)

## Getting Started for Development

### Installing Dotnet SDK

Download Dotnet SDK 6.0 from [Microsoft's official website](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.320-windows-x64-installer) and install it.

### Step 1 - Visual Studio 2022

Download Visual Studio 2022 Community from [Microsoft's official website](https://visualstudio.microsoft.com/vs/).

### Step 2 - Opening the project.

Open Visual Studio 2022 and press "Open a project or solution". Navigate to the project's folder and open the "PointOfSaleSystem.sln"

**Note: Build the solution before running the tests.**

## Manage the installer

### Install the extension

* In Visual Studio go to the extensions tab at the top of the window.
* Click `Manage Extensions` and search for `Microsoft Visual Studio Installer Project`, then download and follow its steps after closing Visual Studio.
* You may have to reload the POSS-Installer project to make changes to it.

### Setup the installer

* Build the PointOfSaleSystem project
* Build the POSS-Installer project

**Note: if it says (incompatible) reload the project with dependencies by right clicking the solution and press "Reload with dependencies"**

To find the installer you go to POSS-Installer folder then Debug or release depending on your Visual Studio settings, then run the POSS-Installer.msi

### Update installer versions

When the project has been updated you need to update the version of the installer, for the installer to be able to update the program.

* Click the POSS-Installer in Visual Studio and press F4
* The properties window will open
* Scroll down to the version tab and update the version

In the installer folder, place the new POSS-Installer.msi and setup.exe file

## Add product buttons during development

* Navigate to the folder where the .exe is located
    * Should be `PointOfSaleSystem/PointOfSaleSystem/bin/Debug/net6.0-windows`
* Open the items.json file which is created when the project is run and there is no previous items.json
* There new products can be added and old ones edited/deleted