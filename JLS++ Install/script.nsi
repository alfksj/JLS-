;##########################################################
; define Settings
;##########################################################
;----------------------------------------------------------
; Program name, version, etc.
;----------------------------------------------------------
!define PRODUCT_NAME "JLS++"
!define PRODUCT_VERSION "1.1.0"
!define PRODUCT_PUBLISHER "Alfksj"
#!define PRODUCT_WEBSITE ""
!define EXEFILE_NAME "JLS++"
!define EXEFULL_NAME "${EXEFILE_NAME}.exe"
!define EXEFILE_DIR "$PROGRAMFILES\${PRODUCT_NAME}"
!define OUTFILE_NAME "${PRODUCT_NAME}_${PRODUCT_VERSION}_installer.exe"
;----------------------------------------------------------
; Registry keys
;----------------------------------------------------------
!define REG_ROOT_KEY "HKLM"
!define REG_UNROOT_KEY "HKLM"
!define REG_APPDIR_KEY "Software\Microsoft\Windows\CurrentVersion\App Path\${EXEFULL_NAME}"
!define REG_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
;##########################################################
; MUI Settings
;##########################################################
;----------------------------------------------------------
; Request application privileges for Windows Vista
;----------------------------------------------------------
RequestExecutionLevel admin
;----------------------------------------------------------
; include
;----------------------------------------------------------
!include "MUI.nsh"
!include x64.nsh
;----------------------------------------------------------
; Icon
;----------------------------------------------------------
!define MUI_ICON "icon.ico"
!define MUI_UNICON "icon.ico"
;----------------------------------------------------------
; installer or uninstaller exiting message
;----------------------------------------------------------
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
;----------------------------------------------------------
; finishpage noAutoClose
;----------------------------------------------------------
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE
;##########################################################
; MUI Pages
;##########################################################
;----------------------------------------------------------
; Page Design
;----------------------------------------------------------
; installer or uninstaller Header image (175x53)
#define MUI_HEADERIMAGE
#!define MUI_HEADERIMAGE_BITMAP "img\header_inst.bmp" ; 175x53
#!define MUI_HEADERIMAGE_UNBITMAP "img\header_uninst.bmp" ; 175x53
#!define MUI_HEADERIMAGE_BITMAP_NOSTRETCH
#!define MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH
!define MUI_BGCOLOR FFFFFF
; installer Welcome & Finish page image (191x290)
; !define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
; !define MUI_WELCOMEFINISHPAGE_BITMAP "img\welcome_inst.bmp"
; uninstaller Welcome & Finish page image (191x290)
; !define MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH
; !define MUI_UNWELCOMEFINISHPAGE_BITMAP "img\welcome_uninst.bmp"
;----------------------------------------------------------
; Installer page
;----------------------------------------------------------
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "LICENSE"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
;----------------------------------------------------------
; Uninstaller pages
;----------------------------------------------------------
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;----------------------------------------------------------
; Language Files
;----------------------------------------------------------
!insertmacro MUI_LANGUAGE "English"
;##########################################################
; NSIS Settings
;##########################################################
;----------------------------------------------------------
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${OUTFILE_NAME}"
InstallDir "${EXEFILE_DIR}"
ShowInstDetails show
ShowUninstDetails show
SetOverWrite on ; on|off|try|ifnewer|ifdiff|lastused
;----------------------------------------------------------
InstallDirRegKey ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "Install_Dir"
;##########################################################
; SECTION
;##########################################################
;----------------------------------------------------------
Section "!JLS++ (Required)"
DetailPrint "Extracting package..."
SetDetailsPrint listonly
SetOutPath "$INSTDIR"
File "bin\*"
SetOutPath "$INSTDIR\en-US"
File "bin\en-US\*"
SetOutPath "$INSTDIR\ko-KR"
File "bin\ko-KR\*"
SetOutPath "$INSTDIR\Resources"
File "bin\Resources\*"
SetOutPath "$INSTDIR\x64"
File "bin\x64\*"
SetOutPath "$INSTDIR\x86"
File "bin\x86\*"
DetailPrint "Writing Registry"
; registry - installation path
WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "Install_Dir" "$INSTDIR"
WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "" "$INSTDIR\${EXEFULL_NAME}"
; registry - uninstall info
WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayName" "$(^Name)"
WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${EXEFULL_NAME}"
WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
;WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEBSITE}"
WriteRegStr ${REG_ROOT_KEY} "${REG_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
; create Uninstaller
WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd
Section "!Auto start on booting"
DetailPrint "Writing Registry(Auto start)"
; Bit differences
${If} ${RunningX64}
    SetRegView 64
${Else}
    SetRegView 32
${EndIf}
WriteRegStr ${REG_ROOT_KEY} "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "JLS++" "$INSTDIR\JLS++.exe -nowindow"
SectionEnd
;----------------------------------------------------------
Section Uninstall
; Kill JLS++ and chromedriver.exe
DetailPrint "Killing processes"
Exec "taskkill /f /im JLS++.exe"
Exec "taskkill /f /im chromedriver.exe"
DetailPrint "Deleting files"
RMDir /r "$INSTDIR"
DetailPrint "Deleting registry"
DeleteRegKey ${REG_ROOT_KEY} "${REG_APPDIR_KEY}"
DeleteRegKey ${REG_UNROOT_KEY} "${REG_UNINST_KEY}"
${If} ${RunningX64}
    SetRegView 64
${Else}
    SetRegView 32
${EndIf}
DeleteRegValue ${REG_ROOT_KEY} "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "JLS++"
SectionEnd