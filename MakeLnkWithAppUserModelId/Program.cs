using MakeLnkWithAppUserModelId.ShellHelpers;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MakeLnkWithAppUserModelId
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3 || args[1].IndexOfAny(Path.GetInvalidPathChars()) >= 0 ||
                !File.Exists(args[1]) || args[2].IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                Console.Error.WriteLine(@"
Usage: {0} <aumid> <exe_path> <shortcut_name> [arguments]

Generates <shortcut_name> pointing to <exe_path> (with optional arguments) with the AppUserModeId set to <aumid>.
If <shortcut_name> is not a path, '%APPDATA%\Microsoft\Windows\Start Menu\Programs' is prepended.
Uses code from https://code.msdn.microsoft.com/windowsdesktop/sending-toast-notifications-71e230a2/.
                ".Trim(), System.AppDomain.CurrentDomain.FriendlyName);
                System.Environment.Exit(1);
            }

            var appId = args[0];
            var exePath = args[1];
            var lnkPath = args[2];

            if (!lnkPath.Contains("\\"))
                lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\" + lnkPath;

            if (!lnkPath.ToLower().EndsWith(".lnk"))
                lnkPath += ".lnk";

            InstallShortcut(appId, exePath, lnkPath, arguments: args.Length > 3 ? string.Join(" ", args.Skip(3)) : "");

            Console.WriteLine("ok, shortcut saved to {0}", lnkPath);
        }

        private static void InstallShortcut(string appIdStr, string exePath, string shortcutPath, string arguments = "")
        {
            if (File.Exists(shortcutPath))
                Console.Error.WriteLine("warning: overwriting existing shortcut");
            // Find the path to the current executable
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(arguments));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(appIdStr))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }

    }
}
