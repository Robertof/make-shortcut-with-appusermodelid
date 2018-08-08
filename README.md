# make-shortcut-with-appusermodelid

To send toast notifications on modern Windows versions, a shortcut (`.lnk` file) needs to be
created in `%APPDATA%\Microsoft\Windows\Start Menu\Programs` with the property
"AppUserModelId" set to a
[specific value](https://docs.microsoft.com/it-it/windows/desktop/shell/appids).
This isn't possible using standard Windows interfaces nor with PowerShell, and it is normally
expected to be done by MSI installers (which have specific configuration parameters to do so).
However, it may be useful to create such a shortcut without creating a full-fledged installer, and
this is why this tool was born.

A precompiled version of this tool can be found in the "Releases" page.

## Notice

Most of the code which deals with low-level link creation comes from
[an example published by Microsoft](https://code.msdn.microsoft.com/windowsdesktop/sending-toast-notifications-71e230a2/)
on this subject. All rights for the code being used belong to the respective owners.

## Usage

```
Usage: makelnk.exe <aumid> <exe_path> <shortcut_name> [arguments]

Generates <shortcut_name> pointing to <exe_path> (with optional arguments) with the AppUserModeId set to <aumid>.
If <shortcut_name> is not a path, '%APPDATA%\Microsoft\Windows\Start Menu\Programs' is prepended.
```
