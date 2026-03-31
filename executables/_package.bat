@echo off

set VER=20260331

del grif-linux-arm-*.tgz
del grif-linux-arm64-*.tgz
del grif-linux-x64-*.tgz
del grif-osx-arm64-*.zip
del grif-osx-x64-*.zip
del grif-win-arm64-*.zip
del grif-win-x64-*.zip
del grif-win-x86-*.zip

copy /y ..\LICENSE.txt linux-arm
copy /y ..\LICENSE.txt linux-arm64
copy /y ..\LICENSE.txt linux-x64
copy /y ..\LICENSE.txt osx-arm64
copy /y ..\LICENSE.txt osx-x64
copy /y ..\LICENSE.txt win-arm64
copy /y ..\LICENSE.txt win-x64
copy /y ..\LICENSE.txt win-x86

copy /y ..\README.md linux-arm
copy /y ..\README.md linux-arm64
copy /y ..\README.md linux-x64
copy /y ..\README.md osx-arm64
copy /y ..\README.md osx-x64
copy /y ..\README.md win-arm64
copy /y ..\README.md win-x64
copy /y ..\README.md win-x86

copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-arm
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-arm64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-x64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif osx-arm64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif osx-x64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif win-arm64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif win-x64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif win-x86

copy /y ..\TicTacToe\TicTacToe.grif linux-arm
copy /y ..\TicTacToe\TicTacToe.grif linux-arm64
copy /y ..\TicTacToe\TicTacToe.grif linux-x64
copy /y ..\TicTacToe\TicTacToe.grif osx-arm64
copy /y ..\TicTacToe\TicTacToe.grif osx-x64
copy /y ..\TicTacToe\TicTacToe.grif win-arm64
copy /y ..\TicTacToe\TicTacToe.grif win-x64
copy /y ..\TicTacToe\TicTacToe.grif win-x86

7z.exe a -ttar -so -an linux-arm | 7z.exe a -si grif-linux-arm-%VER%.tgz
7z.exe a -ttar -so -an linux-arm64 | 7z.exe a -si grif-linux-arm64-%VER%.tgz
7z.exe a -ttar -so -an linux-x64 | 7z.exe a -si grif-linux-x64-%VER%.tgz
7z.exe a -tzip grif-osx-arm64-%VER%.zip osx-arm64
7z.exe a -tzip grif-osx-x64-%VER%.zip osx-x64
7z.exe a -tzip grif-win-arm64-%VER%.zip win-arm64
7z.exe a -tzip grif-win-x64-%VER%.zip win-x64
7z.exe a -tzip grif-win-x86-%VER%.zip win-x86

pause