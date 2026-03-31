# GRIF Executables

GRIF version 2 was created in C# .NET 10 on the Visual Studio 2026 Community (free) edition using Windows 10 64-bit.

To compile GRIF, open the "grif.slnx" file in VS2026 and select "Build". It will create a debug version in "GRIF\bin\Debug\net10.0\grif.exe" plus other supporting files. This version will run on any Windows 64-bit computer with .NET 10 or higher installed. All together it is approximately 290 kilobytes in size.

To create the various executables as self-contained single-file executables which include the necessary .NET 10 libraries and supporting files, select the "GRIF" project in the Solution Explorer, then select the menu items "Build", "Publish Selection". Under the dropdown, there should be several publish profiles.

These are the publish profiles found in the GRIF/Properties/PublishProfiles in the GRIF source and GitHub site.

Publish using each of the profiles, and new folders under "executables" should be created, each containing a "grif" or "grif.exe" executable for the specified target. Each should be approximately 8-14 megabytes in size due to the .NET 10 libraries included.

The batch file "_package.bat" in the "executables" folder has the final step. It requires "7-zip" to be installed. It will copy the supporting files into each folder and compress them into ".zip" and ".tgz" files. These are each approximately 4-6 megabytes in size. These are the ones in the Releases section on GitHub.

Change the version number/date at the top of "_package.bat". It will create the ".zip" and ".tgz" files with that version/date in the filename and delete all others first.
