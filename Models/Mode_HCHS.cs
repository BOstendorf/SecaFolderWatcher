
using System;
using System.IO;

namespace SecaFolderWatcher;
public static class Mode_HCHS
{
    public static void TryDisableNAKO()
    {
        Logger.LogInformation("Disable NAKO Mode");
        Logger.logPrefix = "HCHS";
        FileInfo disableNAKO_Executable = SettingsReader.GetFilePathOf(SettingsReader.settingID_disableNAKO);
        if (ProcessRunner.RunExecutableFile(disableNAKO_Executable) != 0)
        {
            throw new SystemException("The disable NAKO script didn't execute correctly. Can't continue... ");
        }
    }
}
