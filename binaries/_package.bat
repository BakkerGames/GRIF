@echo off

set VER=105

del grif-linux-arm-*.tgz
del grif-linux-arm64-*.tgz
del grif-linux-x64-*.tgz
del grif-win-x64-*.zip
del grif-win-x86-*.zip

copy /y ..\LICENSE.md linux-arm
copy /y ..\LICENSE.md linux-arm64
copy /y ..\LICENSE.md linux-x64
copy /y ..\LICENSE.md win-x64
copy /y ..\LICENSE.md win-x86

copy /y ..\README.md linux-arm
copy /y ..\README.md linux-arm64
copy /y ..\README.md linux-x64
copy /y ..\README.md win-x64
copy /y ..\README.md win-x86

copy /y ..\GRIF\DETAILS.md linux-arm
copy /y ..\GRIF\DETAILS.md linux-arm64
copy /y ..\GRIF\DETAILS.md linux-x64
copy /y ..\GRIF\DETAILS.md win-x64
copy /y ..\GRIF\DETAILS.md win-x86

copy /y ..\DAGS\SYNTAX.md linux-arm
copy /y ..\DAGS\SYNTAX.md linux-arm64
copy /y ..\DAGS\SYNTAX.md linux-x64
copy /y ..\DAGS\SYNTAX.md win-x64
copy /y ..\DAGS\SYNTAX.md win-x86

copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-arm
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-arm64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif linux-x64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif win-x64
copy /y ..\CloakOfDarkness\CloakOfDarkness.grif win-x86

copy /y ..\TicTacToe\TicTacToe.grif linux-arm
copy /y ..\TicTacToe\TicTacToe.grif linux-arm64
copy /y ..\TicTacToe\TicTacToe.grif linux-x64
copy /y ..\TicTacToe\TicTacToe.grif win-x64
copy /y ..\TicTacToe\TicTacToe.grif win-x86

7z.exe a -ttar -so -an linux-arm | 7z.exe a -si grif-linux-arm-%VER%.tgz
7z.exe a -ttar -so -an linux-arm64 | 7z.exe a -si grif-linux-arm64-%VER%.tgz
7z.exe a -ttar -so -an linux-x64 | 7z.exe a -si grif-linux-x64-%VER%.tgz
7z.exe a -tzip grif-win-x64-%VER%.zip win-x64
7z.exe a -tzip grif-win-x86-%VER%.zip win-x86

pause