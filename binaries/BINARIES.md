# GRIF Binaries

GRIF was created in C# .NET 8 on the Visual Studio 2022 Community (free) edition using Windows 10 64-bit.

To compile GRIF, open the "GRIF.sln" file in VS2022 and select "Build". It will create a debug version in "GRIF\bin\Debug\net8.0\grif.exe" plus other supporting files. This version will run on any Windows 64-bit computer with .NET 8 or higher installed. All together it is approximately 240 kilobytes in size.

To create the various binaries as self-contained single-file executables which include the necessary .NET 8 libraries and supporting files, select the "GRIF" project in the Solution Explorer, then select the menu items "Build", "Publish Selection". Under the dropdown, there should be six (at this time) publish profiles:

- _local.pubxml
- linux-arm.pubxml
- linux-arm64.pubxml
- linux-x64.pubxml
- win-x64.pubxml
- win-x86.pubxml

These are the publish profiles found in the GRIF/Properties/PublishProfiles in the GRIF source and GitHub site.

The first "_local.pubxml" publishes to my local "C:\Users\Scott\EXE", so feel free to change that. Great for testing! The others all publish to subfolders in the GRIF project, under "binaries".

Publish using each of the other profiles, and new folders under "binaries" should be created, each containing a "grif" or "grif.exe" executable for the specified target. Each should be approximately 8-14 megabytes in size due to the .NET 8 libraries included.

The batch file "_package.bat" in the "binaries" folder has the final step. It requires "7-zip" to be installed. It will copy the supporting files into each folder and compress them into ".zip" and ".tgz" files. These are each approximately 4-6 megabytes in size. These are the ones in the Releases section on GitHub.

Change the version number/date at the top of "_package.bat". It will create the ".zip" and ".tgz" files with that version/date in the filename and delete all others first.

These versions have been tested on the following systems and worked at that time.

- linux-arm: Raspberry Pi 32-bit
- linux-arm64: Raspberry Pi 64-bit
- linux-x64: Steam Deck in Desktop mode (with a keyboard)
- win-x64: Windows 10 64-bit
- win-x86: Tested on Windows 10 64-bit but should work on Windows x86 32-bit
