Option Explicit

Dim oNet, sUser, cInitial, startTime, sComputerName, sDomainName
Dim oShell, sKey, sSysRoot, sPW3Root, sPW_User, sUserProfile
Dim oFileSys

'**************************************************************
' Get account name
'**************************************************************
sPW_User = InputBox("Enter the PAINTworks User Account Name.", "PAINTworks User Account Setup", "PaintWorksUser")

If sPW_User = "" Then 
   Wscript.Echo "Action cancelled."
Else
   '**************************************************************
   ' Get current user name
   '**************************************************************
   Set oNet = CreateObject("WScript.Network")

   ' Get the user name. On Windows 9x, the use may not be logged 
   ' on when the script starts running; keep checking every 1/2 a 
   ' second until they are logged on.

   sUser = oNet.UserName
   startTime = Now

   Do While sUser = ""
      If DateDiff("s", startTime, Now) > 30 Then Wscript.Quit
      Wscript.Sleep 500
      sUser = oNet.UserName
   Loop

   If UCase(sUser) = UCase(sPW_User) Then

      '**************************************************************
      ' Get Computer Name and Domain Name
      '**************************************************************
      sComputerName = oNet.ComputerName
      sDomainName = oNet.UserDomain

      Set oShell = WScript.CreateObject("WScript.Shell")

      '**************************************************************
      ' Get environment variables
      '**************************************************************
      sSysRoot = oShell.ExpandEnvironmentStrings("%WINDIR%")
      sPW3Root = oShell.RegRead("HKLM\Software\FANUC\PAINTworks\PathName")
      sUserProfile = oShell.ExpandEnvironmentStrings("%USERPROFILE%")

      '**************************************************************
      ' Remove ALL Program Shortcuts from the account Start 
      ' Menu and store them away just in case.
      '**************************************************************
      Set oFileSys = CreateObject("Scripting.FileSystemObject")

      'With oFileSys
      '   If .FolderExists(sUserProfile & "\Start Menu\Programs") then
      '      If Not .FolderExists(sPW3Root & "Profile\" & sUser) then
      '         .CreateFolder(sPW3Root & "Profile\" & sUser)
      '      End If
      '      If Not .FolderExists(sPW3Root & "Profile\" & sUser & "\Start Menu") then
      '         .CreateFolder(sPW3Root & "Profile\" & sUser & "\Start Menu")
      '      End If
      '      .CopyFolder sUserProfile & "\Start Menu\Programs", sPW3Root & "Profile\" & sUser & "\Start Menu\"
      '      .DeleteFolder sUserProfile & "\Start Menu\Programs", True
      '      .CreateFolder(sUserProfile & "\Start Menu\Programs")
      '   End If

      '   '**************************************************************
      '   ' Add only the shortcuts we want to the Start Menu
      '   '**************************************************************
      '   If Not .FolderExists(sUserProfile & "\Start Menu\Programs") then
      '      .CreateFolder(sUserProfile & "\Start Menu\Programs")
      '   End If
      '   If .FolderExists(sPW3Root & "Profile\ShortCuts") Then
      '      .CopyFile sPW3Root & "Profile\ShortCuts\*.*", sUserProfile & "\Start Menu\Programs"
      '   End If
      'End With

      '**************************************************************
      ' Manipulate the Windows registry
      '**************************************************************

      With oShell
         '**************************************************************
         ' Account Auto Logon Setup
         '**************************************************************
         sKey = "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon\"
	
         .RegWrite sKey & "DefaultUserName", sUser, "REG_SZ"
         .RegWrite sKey & "DefaultPassword", "Paint", "REG_SZ"
         .RegWrite sKey & "DefaultDomainName", sDomainName, "REG_SZ"
         .RegWrite sKey & "AutoAdminLogon", "1", "REG_SZ"

         '**************************************************************
         ' Start Menu/Task Bar lockdown
         '**************************************************************
         sKey = "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\"

         '*** Windows 2000 Compatible ***
         '.RegWrite sKey & "NoStartMenuSubFolders", 1, "REG_DWORD"
         .RegWrite sKey & "NoWindowsUpdate", 1, "REG_DWORD"
         '.RegWrite sKey & "NoCommonGroups", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSMMyDocs", 1, "REG_DWORD"
         '.RegWrite sKey & "NoRecentDocsMenu", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSetFolders", 1, "REG_DWORD"
         '.RegWrite sKey & "NoNetworkConnections", 1, "REG_DWORD"
         '.RegWrite sKey & "NoFavoritesMenu", 1, "REG_DWORD"
         '.RegWrite sKey & "NoFind", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSMHelp", 1, "REG_DWORD"
         '.RegWrite sKey & "NoRun", 1, "REG_DWORD"
         '.RegWrite sKey & "ForceStartMenuLogOff", 1, "REG_DWORD"
         '.RegWrite sKey & "NoChangeStartMenu", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSetTaskbar", 1, "REG_DWORD"
         '.RegWrite sKey & "NoTrayContextMenu", 1, "REG_DWORD"
         '.RegWrite sKey & "NoRecentDocsHistory", 1, "REG_DWORD"

         '*** Windows XP Pro only ***
         '.RegWrite sKey & "NoSMMyPictures", 1, "REG_DWORD"
         '.RegWrite sKey & "NoStartMenuMyMusic", 1, "REG_DWORD"
         '.RegWrite sKey & "NoStartMenuNetworkPlaces", 1, "REG_DWORD"
         '.RegWrite sKey & "LockTaskbar", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSimpleStartMenu", 1, "REG_DWORD"
         '.RegWrite sKey & "NoSMBalloonTip", 1, "REG_DWORD"
         '.RegWrite sKey & "NoStartMenuPinnedList", 1, "REG_DWORD"
         '.RegWrite sKey & "NoStartMenuMFUprogramsList", 1, "REG_DWORD"
         '.RegWrite sKey & "NoStartMenuMorePrograms", 1, "REG_DWORD"
         '.RegWrite sKey & "NoTrayItemsDisplay", 1, "REG_DWORD"
         '.RegWrite sKey & "NoToolbarsOnTaskbar", 1, "REG_DWORD"

         '**************************************************************
         ' Desktop/Explorer Lockdown
         '**************************************************************
         '.RegWrite sKey & "NoDesktop", 1, "REG_DWORD"
         '.RegWrite sKey & "NoWelcomeTips", 1, "REG_DWORD"
         '.RegWrite sKey & "NoDriveTypeAutoRun", 255, "REG_DWORD"

         ' The LOCAL_MACHINE "NoDriveTypeAutoRun" setting overrides the
         ' CURRENT_USER setting so we'll write it also.
         sKey = "HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\"
         .RegWrite sKey & "NoDriveTypeAutoRun", 255, "REG_DWORD"

         '**************************************************************
         ' Ctrl + Alt + Del Lockdown
         '**************************************************************
      '   sKey = "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\System\"
      '   .RegWrite sKey & "DisableChangePassword", 1, "REG_DWORD"
      '   .RegWrite sKey & "DisableLockWorkstation", 1, "REG_DWORD"
      '   .RegWrite sKey & "DisableTaskMgr", 0, "REG_DWORD"

         '**************************************************************
         ' Auto Run PAINTworks at Login
         '**************************************************************
         sKey = "HKCU\Software\Microsoft\Windows NT\CurrentVersion\Winlogon\"

         .RegWrite sKey & "Shell", sPW3Root & "Vbapps\Pw4_main.exe", "REG_SZ"
 
      End With

      WScript.Echo sUser & " configuration is complete. Restart computer for changes to take effect."

   Else

      WScript.Echo sPW_User & " is not logged in. No changes have been made to the registry."

   End If

End If