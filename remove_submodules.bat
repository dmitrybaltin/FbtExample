@echo off
echo Removing all Git submodules...

:: 1. Деинициализируем все субмодули
git submodule deinit -f --all

:: 2. Получаем список всех субмодулей
for /f "tokens=*" %%i in ('git config --file .gitmodules --get-regexp path ^| awk "{print $2}"') do (
    echo Removing submodule folder %%i from index...
    git rm -r --cached "%%i"
    echo Deleting folder %%i...
    rmdir /s /q "%%i"
)

:: 3. Удаляем .gitmodules
if exist .gitmodules (
    echo Deleting .gitmodules file...
    del /f /q .gitmodules
)

:: 4. Удаляем папку с данными субмодулей
if exist .git\modules (
    echo Deleting .git\modules folder...
    rmdir /s /q .git\modules
)

echo All submodules removed.
pause
