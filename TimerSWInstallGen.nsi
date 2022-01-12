; ------------------------------------------------------------
; Installer generation script for TimerSW
;
; This script is intended to be compiled using the NSIS compiler (MakeNSIS)
; and it must be placed in the directory containing the program files to be
; installed.
; ------------------------------------------------------------

!include "MUI2.nsh"
!include "logiclib.nsh"
!include "x64.nsh"

!define REG_UNINSTALL_KEY "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANY} ${APP_NAME}"
!define REG_INFO_KEY      "SOFTWARE\${COMPANY}\${APP_NAME}"

Unicode True
RequestExecutionLevel admin
SetCompressor /FINAL /SOLID lzma

; ------------------------------------------------------------
; Custom information

!define COMPANY "TMS"

!define APP_NAME "TimerSW"
!define APP_FILE "TimerSW.exe"

!define DISPLAY_VERSION "2022.0"

!define URL_INFO_ABOUT "https://github.com/TaylerMauk/TimerSW"
; ------------------------------------------------------------

; ------------------------------------------------------------
; Installer file name
OutFile "${APP_NAME} Setup.exe"

; Installer Name
Name "${APP_NAME}"

; Default isntallation directory
InstallDir "$PROGRAMFILES64\${COMPANY}\${APP_NAME}"

; Registry key to check for directory (so if you install again, it will overwrite the old one automatically)
InstallDirRegKey HKLM "${REG_INFO_KEY}" "InstallLocation"
; ------------------------------------------------------------

; ------------------------------------------------------------
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
; ------------------------------------------------------------

; ------------------------------------------------------------
Section "Core Components (Required)"
    SectionIn RO

    ${if} ${RunningX64}
        ${DisableX64FSRedirection}
        SetRegView 64
    ${EndIf}
    SetShellVarContext all
    SetOutPath "$INSTDIR"

    ; Extract files
    File /r /x *.nsi *.*

    ; Write the installation path into the registry
    WriteRegStr HKLM "${REG_INFO_KEY}" "InstallLocation" $INSTDIR

    ; Write uninstall info into the registry
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "DisplayName" "${APP_NAME}"
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "DisplayIcon" "$INSTDIR\${APP_FILE}"
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "DisplayVersion" "${DISPLAY_VERSION}"
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "Publisher" "${COMPANY}"
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "UninstallString" "$INSTDIR\uninstall.exe"
    WriteRegStr HKLM "${REG_UNINSTALL_KEY}" "UrlInfoAbout" "${URL_INFO_ABOUT}"
    WriteRegDWORD HKLM "${REG_UNINSTALL_KEY}" "NoModify" 1
    WriteRegDWORD HKLM "${REG_UNINSTALL_KEY}" "NoRepair" 1

    ; Create uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd
; ------------------------------------------------------------

; ------------------------------------------------------------
Section "Desktop Shortcut"
    SetShellVarContext all

    ; Create shortcut for Desktop
    CreateShortcut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_FILE}"
SectionEnd
; ------------------------------------------------------------

; ------------------------------------------------------------
Section "Start Menu Shortcut"
    SetShellVarContext all

    ; Create shortcut for Start Menu
    CreateDirectory "$SMPROGRAMS\${COMPANY}\${APP_NAME}"
    CreateShortcut "$SMPROGRAMS\${COMPANY}\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_FILE}"
SectionEnd
; ------------------------------------------------------------

; ------------------------------------------------------------
Section "Uninstall"
    ${if} ${RunningX64}
        ${DisableX64FSRedirection}
        SetRegView 64
    ${EndIf}
    SetShellVarContext all

    ; Remove registry keys
    DeleteRegKey HKLM "${REG_UNINSTALL_KEY}"
    DeleteRegKey HKLM "${REG_INFO_KEY}"
    DeleteRegKey /ifempty HKLM "SOFTWARE\${COMPANY}"

    ; Remove all files and directories from install location
    RMDir /r $INSTDIR

    ; Remove shortcuts, if any
    RMDir /r "$SMPROGRAMS\${COMPANY}\${APP_NAME}"
    Delete "$DESKTOP\${APP_NAME}.lnk"
SectionEnd
; ------------------------------------------------------------
