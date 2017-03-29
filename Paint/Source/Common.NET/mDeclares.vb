' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: mDeclares
'
' Description: Global Declarations Enumerations and Structures
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    11/06/2008 AM      Added ASCII support logging it on to a Text field on the ProdLog.mdb    4.00.00.01
'                           gsPROD_PLANTSTYLE_ASCII As String = "[Plant Style ASCII]"
'                           gsPROD_PLANTCOLOR_ASCII As String = "[Plant Color ASCII]"
'                           gsPROD_PLANTOPTION_ASCII As String = "[Plant Option ASCII]"
'    05/29/09   MSW     switch to VB 2008
'                       Add constants for names in Color Change Type dataset 
'                       Add constants for names in applicator parameter dataset 
'                       Updated scattered access tags
'		            	Removed Enum eApplicatorType
'	    	    	    Updated Enum ePLCMANUAL_ZONE_LINK
'              			Added Enum eSAIndex
'    11/04/09   MSW     Add not none as color change type option
'    11/10/09	MSW	    add versa2WB, honda cc types
'    11/23/09   BTK     added declarations for shaping air 2.
'    02/15/10   BTK     Added color change type for Versabell2 32 valves
'    02/17/10   BTK     Added ITW estat type and other.
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	  4.01.00.01
'    11/11/11   MSW		  Add constants for DMON schedman screen in vb.net	              4.01.01.00
'    01/03/12   MSW     Changes for speed - move applicator tables to XML                 4.01.01.01
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm   4.01.01.02
'    02/15/12   MSW     Import/Export Updates                                             4.01.01.03
'    02/20/12   RJO     Interprocess Communication constants Updates                      4.01.01.04
'    03/20/12   RJO     Interprocess Communication and Help constants Updates. Moved      4.01.02.00
'                       ePrivilege enum here for new .NET Password.
'    03/28/12   MSW     Add constants for scattered access                                4.01.03.00
'    04/11/12   RJO     Add constants for sealer applications                             4.01.03.01
'    06/07/12   MSW     Simplify help links for some tabbed screens                       4.01.03.02
'    12/13/12   MSW     Update gs_HELP_MAIN for singleinstance help screen                4.01.03.03
'    04/16/13   MSW     Add some constants for mitsubishi                                 4.01.05.00
'                       Standalone ChangeLog form
'                       Consolidate eParamType declarations from frmChangeLog & frmAll
'    05/20/13   RJO     Added Help constants for Sealer Scedule and Volume screens.	      4.01.05.01
'    05/28/13   MSW     Update constants for vision logger                                4.01.05.02
'    07/08/13   MSW     Add style ID in XML, paramsetup updates, robot type updates       4.01.05.03
'    07/15/13   RJO     Added Help constant for Sealer Bulk Supply screen.	              4.01.05.04
'    08/20/13   MSW     Updates for sealer status screens, manual cycle                   4.01.05.05
'    08/30/13   MSW     Updates for sealer manual cycle                                   4.01.05.06
'    09/30/13   MSW     add clsVersabell3dualWBCartoon, help constant changes             4.01.05.07
'    10/11/13   MSW     Add sealer status charts to paramsetup                            4.01.06.00
'    10/16/13   MSW     More work on ParamSetup for sealer status pages                   4.01.06.01
'    10/25/13   BTK     Added Tricoat By Style                                            4.01.06.02
'    12/03/13   MSW     Add Materials to robot setup                                      4.01.06.04
'    01/06/14   MSW     Add sealer help constants for fairfax                             4.01.06.05
'    02/13/14   RJO     Add Color Change Status to BSD Process screen                     4.01.06.05
'    11/11/13   BTK     Added iPackHV controller and shifted other.						  4.01.07.00
'    02/26/14   MSW     Added dispenser cartoons to ParamSetup screen.			          4.01.07.01
'    04/08/14   MSW     man cycle screen change from fairfax.         			          4.01.07.02
'    03/24/14   DJM     Add HasManualCycles to support sealer applications
'    04/22/14   MSW     ZDT                                                               4.01.07.03
'    06/09/14   RJO     Added P35_Opener = 19 to eArmType.                                4.01.07.04
'    07/30/14   MSW     Oakville CustController param setup changes                       4.01.07.05
'                       Dual can updates
'    10/08/14   RJO     Added VERSABELL3P1000 = 28 to eColorChangeType.                   4.01.07.06
'    10/24/14   MSW     Add help path for semi auto reports                               4.01.07.07
'    12/15/14   MSW     Allow direct access to read items in clsScatteredAccessItems      4.01.07.08
'    01/19/15   MSW     Honda Canada updates for Honda dualcan                            4.01.07.09
'    02/12/15   MSW     Found an uncredited fix to the maintenance menu                   4.01.07.10
'                       help link in flint
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Public Module mDeclares

#Region " Friend Constants "

    Friend Const gsMAINCLIENT As String = "WorksMainClient"

    'New table names for .NET
    Friend Const gsSETUP_TABLENAME As String = "Setup Info"
    Friend Const gsSTARTUP_TABLENAME As String = "Startup List"
    Friend Const gsZONE_TABLENAME As String = "Zone Info"

    'SQL tables have a common column to sort on
    Friend Const gsCOL_SORTORDER As String = "SortOrder"
    Friend Const gsCOL_ZONENAME As String = "Zone Name"

    'constants for names in zone dataset
    Friend Const gsZONE_DS_TABLENAME As String = "Zones"
    Friend Const gsZONE_NAME As String = "Zone Name"
    Friend Const gsZONE_COL_NUM As String = "Zone"
    Friend Const gsZONE_COL_IP As String = "IP Address"
    Friend Const gsZONE_COL_MAXSTY As String = "Max Styles"
    Friend Const gsZONE_COL_CONT As String = "Controllers"
    Friend Const gsZONE_COL_PLC As String = "PLCType"
    Friend Const gsZONE_COL_BYSTYLE As String = "PresetsByStyle"
    Friend Const gsZONE_COL_MAXCOL As String = "Max Colors"

    'constants for names in arms dataset
    Friend Const gsARM_DS_TABLENAME As String = "Arms"
    Friend Const gsARM_COL_DNAME As String = "Arm Display Name"
    Friend Const gsARM_COL_ROB_NUMBER As String = "Robot Number"
    Friend Const gsARM_COL_STATION_NUMBER As String = "Station Number" 'CBZ 7/30/13
    Friend Const gsARM_COL_CONT_NUMBER As String = "Arm Controller Number"
    Friend Const gsARM_COL_ARM_NUMBER As String = "Arm Number"
    Friend Const gsARM_COL_ARM_TYPE As String = "Arm Type"
    Friend Const gsARM_COL_CC_TYPE As String = "ColorChange Type"
    Friend Const gsARM_COL_DISPENSERS As String = "Dispensers" 'RJO 04/11/12
    Friend Const gsARM_COL_MATERIALS As String = "Materials" 'MSW 12/03/13
    Friend Const gsARM_COL_DISPENSER_TYPE As String = "DispenserType" 'RJO 04/11/12
    Friend Const gsARM_COL_ES_TYPE As String = "Estat Type"
    Friend Const gsARM_COL_ES_HOST As String = "Estat Hostname"
    Friend Const gsARM_COL_APTYPE As String = "Applicator Type"
    Friend Const gsARM_COL_ESNAME As String = "EstatName"
    Friend Const gsARM_COL_ISOPENER As String = "IsOpener"
    Friend Const gsARMCOL_SORTORDER As String = "Arm SortOrder"
    Friend Const gsARM_COL_PLC_TAG_PRFX As String = "PLC Tag Prefix"
    ' 12/10/09  MSW     Support PaintToolAlias property for 7.50 alarm screen changes
    Friend Const gsARM_PT_ALIAS As String = "PaintTool Alias"
    'constants for names in Controllers dataset
    Friend Const gsCONT_DS_TABLENAME As String = "Controllers"
    Friend Const gsCONT_COL_HNAME As String = "Hostname"
    Friend Const gsCONT_COL_DNAME As String = "Display Name"
    Friend Const gsCONT_COL_NUMBER As String = "Controller Number"
    Friend Const gsCONT_COL_TYPE As String = "Controller Type"
    Friend Const gsCONT_COL_IBE As String = "Internal Backup Enabled"
    Friend Const gsARM_COL_HASMANUALCYCLES As String = "HasManualCycles"
    Friend Const gsARM_COL_PIG_SYSTEMS As String = "PigSystems" 'NRU 161215 Piggable

    'constants for names in dt mask Type dataset
    Friend Const gsDTMASK_DS_TABLENAME As String = "Downtime Mask"
    Friend Const gsDTMASK_COL_ID As String = "ID"
    Friend Const gsDTMASK_Alarm As String = "Alarm"

    'constants for names in Color Change Type dataset
    Friend Const gsCCTYPE_XMLTABLE As String = "ColorChangeType"
    Friend Const gsCCTYPE_XMLNODE As String = "Applicator"
    Friend Const gsCCTYPE_COL_ID As String = "ID"
    Friend Const gsCCTYPE_COL_APPL_TABLE As String = "ApplicatorTable"
    Friend Const gsCCTYPE_DQ_OFFSET As String = "DQFeedbackOffset"
    Friend Const gsCCTYPE_DQ_SCALE As String = "DQFeedbackScale"
    Friend Const gsCCTYPE_KV_OFFSET As String = "KVFeedbackOffset"
    Friend Const gsCCTYPE_KV_SCALE As String = "KVFeedbackScale"
    Friend Const gsCCTYPE_UA_OFFSET As String = "uAFeedbackOffset"
    Friend Const gsCCTYPE_UA_SCALE As String = "uAFeedbackScale"
    Friend Const gsCCTYPE_ENABLE_BELL As String = "EnableBell"
    Friend Const gsCCTYPE_NUM_CYCLES As String = "NumberOfCycles"
    Friend Const gsCCTYPE_CYCLE_NAMES As String = "CycleNames"

    Friend Const gsCCTYPE_NUM_ACTIONS As String = "NumberOfActions"
    Friend Const gsCCTYPE_ACTION_NAMES As String = "ActionNames"
    Friend Const gsCCTYPE_NUM_EVENTS As String = "NumberOfEvents"
    Friend Const gsCCTYPE_EVENT_NAMES As String = "EventsNames"
    Friend Const gsCCTYPE_NUM_SHARED_VALVES As String = "NumShValves"
    Friend Const gsCCTYPE_SHARED_NAMES As String = "ShVlvNames"
    Friend Const gsCCTYPE_SHARED_NAMES_CAP As String = "ShVlvNames"
    Friend Const gsCCTYPE_NUM_GROUP_VALVES As String = "NumGpValves"
    Friend Const gsCCTYPE_GROUP_NAMES As String = "GpVlvNames"
    Friend Const gsCCTYPE_GROUP_NAMES_CAP As String = "GpVlvNames"
    Friend Const gsCCTYPE_ENABLE_DC As String = "EnableDontCare"

    'constants for names in Color Change cycle name dataset
    Friend Const gsCCNAME_DS_TABLENAME As String = "Color Change Cycles"
    Friend Const gsCCNAME_COL_ID As String = "ID"
    Friend Const gsCCNAME_COL_CYCLES As String = "Cycles"
    Friend Const gsCCNAME_COL_CYCLENAMEPRE As String = "Cycle "

    'constants for names in system colors dataset
    Friend Const gsSYSC_DS_TABLENAME As String = "SystemColors"
    Friend Const gsVALVE_DS_TABLENAME As String = "ColorValves"
    Friend Const gsSYSC_COL_ZONE As String = "Zone"
    Friend Const gsSYSC_COL_NUMBER As String = "Color Number"
    Friend Const gsSYSC_COL_CONT As String = "Controller"
    Friend Const gsSYSC_COL_PLANTNUM As String = "PlantNumber"
    Friend Const gsSYSC_COL_DESC As String = "Description"
    Friend Const gsSYSC_COL_FANUCNUM As String = "FanucNumber"
    Friend Const gsSYSC_COL_VALVENUM As String = "ValveNumber"
    Friend Const gsSYSC_COL_ENABLE As String = "Enable"
    Friend Const gsSYSC_COL_ARGB As String = "ARGB"
    ' 02/23/10  BTK     Added Tricoat Tab
    Friend Const gsSYSC_COL_TRICOATNUM As String = "TricoatNumber"
    Friend Const gsSYSC_COL_TWOCOATS As String = "TwoCoats"

    Friend Const gsSYSC_COL_H_VALVE As String = "HardenerValve"
    Friend Const gsSYSC_COL_R_RATIO As String = "ResinRatio"
    Friend Const gsSYSC_COL_H_RATIO As String = "HardenerRatio"
    Friend Const gsSYSC_COL_R_SOLV As String = "ResinSolvent"
    Friend Const gsSYSC_COL_H_SOLV As String = "HardenerSolvent"

    'constants for names in system styles dataset
    Friend Const gsSYSS_DS_TABLENAME As String = "System Styles"
    Friend Const gsSYSS_COL_ITEM As String = "Item"
    Friend Const gsSYSS_COL_ZONE As String = "Zone"
    Friend Const gsSYSS_COL_PLANTNUM As String = "PlantNumber"
    Friend Const gsSYSS_REGISTER As String = "Register"

    'Friend Const gsSYSS_COL_PLANTASC As String = "Plant Ascii"
    Friend Const gsSYSS_COL_FANUCNUM As String = "FanucNumber"
    Friend Const gsSYSS_COL_DESC As String = "Description"
    Friend Const gsSYSS_COL_ROBRQD As String = "RobotsRequired"
    Friend Const gsSYSS_COL_ENABLE As String = "Enable"
    Friend Const gsSYSS_COL_IMG_KEY As String = "ImageKey"
    Friend Const gsSYSS_COL_ENT_MUTE As String = "EntMuteStartCount" 'RJO 04/08/10
    Friend Const gsSYSS_COL_EXIT_MUTE As String = "ExitMuteStartCount" 'RJO 04/08/10
    Friend Const gsSYSS_COL_MUTE_LEN As String = "MuteLength" 'RJO 04/08/10
    Friend Const gsSYSS_COL_TWOCOATS As String = "TwoCoats" 'jbw 02/21/11
    Friend Const gsSYSS_COL_TRICOATENB As String = "TricoatByStyleRbtsReq" '10/25/13 BTK Tricoat By Style

    'BTK 02/02/10
    'constants for style ID names
    Friend Const gsSYSID_DS_TABLENAME As String = "StyleID"
    Friend Const gsSYSID_DS_ITEMNAME As String = "Item"
    Friend Const gsSYSID_COL_STYLEINDEX As String = "StyleIndex"
    Friend Const gsSYSID_COL_CONVEYOR_COUNT As String = "ConveyorCount"
    Friend Const gsSYSID_COL_PHOTOEYE_PATTERN As String = "PhotoeyePattern"
    Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN As String = "SunroofPhotoeyePattern"
    Friend Const gsSYSID_COL_PHOTOEYE_IGNORE As String = "PhotoeyeIgnore"
    Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE As String = "SunroofPhotoeyeIgnore"

    'geo added options .net p700 development
    'constants for names in system options dataset
    Friend Const gsSYSO_DS_TABLENAME As String = "SystemOptions"
    Friend Const gsSYSO_COL_ITEM As String = "Item"
    Friend Const gsSYSO_COL_ZONE As String = "Zone"
    Friend Const gsSYSO_COL_PLANTNUM As String = "PlantNumber"
    '  Friend Const gsSYSO_COL_PLANTASC As String = "Plant Ascii"
    Friend Const gsSYSO_COL_FANUCNUM As String = "FanucNumber"
    Friend Const gsSYSO_COL_DESC As String = "Description"
    Friend Const gsSYSO_COL_ROBRQD As String = "RobotsRequired"
    Friend Const gsSYSO_COL_ENABLE As String = "Enable"
    Friend Const gsSYSO_REGISTER As String = "Register"

    'repair panel const
    Friend Const gsSYSR_DS_TABLENAME As String = "System Repair Panels"
    Friend Const gsSYSR_COL_ITEM As String = "Item"
    Friend Const gsSYSR_COL_ZONE As String = "Zone"
    Friend Const gsSYSR_COL_PANEL As String = "Panel"
    Friend Const gsSYSR_COL_KEY As String = "ID"   'NEW ADDED KEY TO REPAIRS TABLE
    Friend Const gsSYSR_COL_STYLEINDEX As String = "StyleIndex"
    Friend Const gsSYSR_COL_DESC As String = "Description"
    Friend Const gsSYSR_COL_ROBRQD As String = "Robots Required"
    'constants for names in Days To Keep dataset
    Friend Const gsDTK_DS_TABLENAME As String = "Days To Keep"
    Friend Const gsDTK_COL_ALARM As String = "Alarm"
    Friend Const gsDTK_COL_CHANGE As String = "Change"
    Friend Const gsDTK_COL_PROD As String = "Production"
    Friend Const gsDTK_COL_VISION As String = "Vision"
    Friend Const gsDTK_COL_DIAG As String = "Diagnostic"
    Friend Const gsDTK_COL_DF As String = "DriveFullThreshold"
    Friend Const gsDTK_COL_DIAGARCH As String = "DiagnosticArchive"
    Friend Const gsDTK_COL_ROBBAK As String = "RobotBackups"

    'constants for names in Applicator Parameter Tables
    Friend Const gsAPPL_PARM_XMLTABLE As String = "ApplicatorParameterTables"
    Friend Const gsAPPL_PARM_XMLNODE As String = "Parameter"

    Friend Const gsAPPLPARM_PARMNAME As String = "Name"
    Friend Const gsAPPLPARM_PARMNAMECAP As String = "NameCap"
    Friend Const gsAPPLPARM_PARMTITLE As String = "Title"
    Friend Const gsAPPLPARM_PARMNUM As String = "Number"
    Friend Const gsAPPLPARM_UNITS As String = "Units"
    Friend Const gsAPPLPARM_USE_COUNTS As String = "UseCounts"
    Friend Const gsAPPLPARM_FORCE_INT As String = "ForceInteger"
    Friend Const gsAPPLPARM_MIN_CNT As String = "MinCounts"
    Friend Const gsAPPLPARM_MAX_CNT As String = "MaxCounts"
    Friend Const gsAPPLPARM_MIN_ENG As String = "MinEngUnits"
    Friend Const gsAPPLPARM_MAX_ENG As String = "MaxEngUnits"
    Friend Const gsAPPLPARM_MIN_CNT_BE As String = "MinCountsBell"
    Friend Const gsAPPLPARM_MAX_CNT_BE As String = "MaxCountsBell"
    Friend Const gsAPPLPARM_MIN_ENG_BE As String = "MinEngUnitsBell"
    Friend Const gsAPPLPARM_MAX_ENG_BE As String = "MaxEngUnitsBell"
    Friend Const gsAPPLPARM_CNTR_TYPE As String = "CntrType"
    Friend Const gsAPPLPARM_CAL_SOURCE As String = "CalSource"
    Friend Const gsAPPLPARM_OUT_TYPE As String = "OutType"

    'constants for names in change logging dataset
    ''09/18/07 comment out for sql server database
    ''Friend Const gsCHANGE_DS_TABLENAME As String = "[Data Change Log]"
    ''Friend Const gsCHANGE_ZONE As String = "Zone"
    ''Friend Const gsCHANGE_AREA As String = "Area"
    ''Friend Const gsCHANGE_DEVICE As String = "Device"
    ''Friend Const gsCHANGE_ITEMDESC As String = "ItemDescription"
    ''Friend Const gsCHANGE_OLDVAL As String = "OldValue"
    ''Friend Const gsCHANGE_NEWVAL As String = "NewValue"
    ''Friend Const gsCHANGE_PARAM As String = "Parameter"
    ''Friend Const gsCHANGE_TIME As String = "Time"
    Friend Const gsCHANGE_ITEM As String = "Item"
    Friend Const gsCHANGE_USER As String = "User"
    Friend Const gsCHANGE_STARTSER As String = "Start Serial"
    Friend Const gsCHANGE_SPECIFIC As String = "Specific"
    Friend Const gsCHANGE_PARM1 As String = "Parm1"

    '09/18/07 new for sql server database
    Friend Const gsCHANGE_DS_TABLENAME As String = "[Data Change Log]"
    Friend Const gsCHANGE_DATABASENAME As String = "Changelog"
    Friend Const gsCHANGE_ZONE As String = "Zone"
    Friend Const gsCHANGE_AREA As String = "Area"
    Friend Const gsCHANGE_PWUSER As String = "PWUser"
    Friend Const gsCHANGE_DEVICE As String = "Device"
    Friend Const gsCHANGE_DESC As String = "Description"
    Friend Const gsCHANGE_PARAM As String = "Parameter"
    Friend Const gsCHANGE_TIME As String = "Time"
    Friend Const gsCHANGE_ID As String = "ID"


    'constants for names in Alarm logging dataset
    Friend Const gsALARM_DS_TABLENAME As String = "[Alarmlog]"
    Friend Const gsALARM_DATABASENAME As String = "Alarm Log"
    Friend Const gsALARM_NUM As String = "[Alarm#]"
    Friend Const gsALARM_DEVICE As String = "[Device]"
    Friend Const gsALARM_DESC As String = "[Description]"
    Friend Const gsALARM_SEV As String = "[Severity]"
    Friend Const gsALARM_CAUSE As String = "[CauseMnemonic]"
    Friend Const gsALARM_START As String = "[Start Serial]"
    Friend Const gsALARM_END As String = "[End Serial]"
    Friend Const gsALARM_ZONE As String = "[Zone]"
    Friend Const gsALARM_PRODID As String = "[Prod ID]"
    Friend Const gsALARM_JOBID As String = "[Job ID]"
    Friend Const gsALARM_STYLE As String = "[Style Number]"
    Friend Const gsALARM_COLOR As String = "[Color Number]"
    Friend Const gsALARM_VALVE As String = "[Valve Number]"
    Friend Const gsALARM_JOBNAME As String = "[Job Name]"
    Friend Const gsALARM_PROCESS As String = "[Process]"
    Friend Const gsALARM_NODE As String = "[Node]"
    Friend Const gsALARM_CATEGORY As String = "[Category]"
    Friend Const gsALARM_DOWNTIME As String = "[Downtime Flag]"

    'constants for names in Production logging dataset
    Friend Const gsPROD_DS_TABLENAME As String = "[Prodlog]"
    Friend Const gsPROD_DATABASENAME As String = "Production Log"
    Friend Const gsPROD_ZONE As String = "[Zone]"
    Friend Const gsPROD_SEQNUMBER As String = "[Sequence Number]"
    Friend Const gsPROD_JOBID As String = "[Job ID]"
    Friend Const gsPROD_PRODID As String = "[Prod ID]"
    Friend Const gsPROD_DEVICE As String = "[Device]"
    Friend Const gsPROD_PLANTSTYLE_ASCII As String = "[Plant Style ASCII]"
    Friend Const gsPROD_PLANTSTYLE As String = "[Plant Style]"
    Friend Const gsPROD_PLANTCOLOR As String = "[Plant Color]"
    Friend Const gsPROD_PLANTCOLOR_ASCII As String = "[Plant Color ASCII]"
    Friend Const gsPROD_PLANTOPTION As String = "[Plant Option]"
    Friend Const gsPROD_PLANTOPTION_ASCII As String = "[Plant Option ASCII]"
    Friend Const gsPROD_PLANTREPAIR As String = "[Plant Repair]"
    Friend Const gsPROD_PURGESTATUS As String = "[Purge Status]"
    Friend Const gsPROD_ROBOTREPAIR As String = "[Robot Repair]"
    Friend Const gsPROD_COMPLETESTATUS As String = "[Completion Status]"
    Friend Const gsPROD_DEGRADESTATUS As String = "[Degrade Status]"
    Friend Const gsPROD_STARTSERIAL As String = "[Start Serial]"
    Friend Const gsPROD_CYCLETIME As String = "[Cycle Time]"
    Friend Const gsPROD_INDEXTIME As String = "[Index Time]"
    Friend Const gsPROD_VISIONTIME As String = "[Vision Time]"
    Friend Const gsPROD_PAINTTOTAL1 As String = "[Paint Total 1]"
    Friend Const gsPROD_PAINTTOTAL2 As String = "[Paint Total 2]"
    Friend Const gsPROD_MATLEARNED1 As String = "[Material Learned 1]"
    Friend Const gsPROD_MATLEARNED2 As String = "[Material Learned 2]"
    Friend Const gsPROD_MATTEMP1 As String = "[Material Temp 1]"
    Friend Const gsPROD_MATTEMP2 As String = "[Material Temp 2]"
    Friend Const gsPROD_ERRORSTRING As String = "[Error String]"
    Friend Const gsPROD_INITSTRING As String = "[Init String]"
    Friend Const gsGHOST_STATUS As String = "[Ghost]"
    Friend Const gsPROD_CCTIME As String = "[CC Time]"
    Friend Const gsPROD_ARM As String = "[Arm]"
    Friend Const gsPROD_CANISTERTPSU As String = "[Canister TPSU]"
    Friend Const gsPROD_SPARE16 As String = "[Spare 16]"
    Friend Const gsPROD_SPARE17 As String = "[Spare 17]"
    Friend Const gsRATIO As String = "[Ratio]"
    Friend Const gsPROD_SPARE26 As String = "[Spare 26]"

    'constants for names in Schedule datasets
    Friend Const gsSCHED_MAINT_DS_TABLENAME As String = "Backup"
    Friend Const gsSCHED_MAINT_ID As String = "ID"
    Friend Const gsSCHED_MAINT_DAY As String = "Day"
    Friend Const gsSCHED_MAINT_CLEAN_EN As String = "Cleanup Enabled"
    Friend Const gsSCHED_MAINT_BACK_EN As String = "Backup Enabled"
    Friend Const gsSCHED_MAINT_RBACK_EN As String = "Robot Backup Enabled"
    Friend Const gsSCHED_MAINT_START_HOUR As String = "Start Hour"
    Friend Const gsSCHED_MAINT_START_MIN As String = "Start Minute"
    Friend Const gsSCHED_MAINT_START_SEC As String = "Start Second"

    Friend Const gsSCHED_OPTN_DS_TABLENAME As String = "Options"
    Friend Const gsSCHED_OPTN_ID As String = "ID"
    Friend Const gsSCHED_OPTN_PATH As String = "Path"
    Friend Const gsSCHED_OPTN_DB As String = "Database"
    Friend Const gsSCHED_OPTN_ROB_MASTER As String = "Robot Master"
    Friend Const gsSCHED_OPTN_ROB_SCHED As String = "Robot Scheduled"
    Friend Const gsSCHED_OPTN_ROB_TEMP As String = "Robot Temp"
    Friend Const gsSCHED_OPTN_ROB_IMAGE As String = "Robot Image"
    Friend Const gsSCHED_OPTN_ADD_FOLD As String = "Additional Folders"
    Friend Const gsSCHED_OPTN_EXT_COP_FOLD As String = "Extra Copies"
    Friend Const gsSCHED_OPTN_NEW_FOLD As String = "NewFolder"
    Friend Const gsSCHED_OPTN_LOGFILE As String = "Logfile"
    Friend Const gsSCHED_OPTN_NOTIFY_ST As String = "Notify Start Of Shift"
    Friend Const gsSCHED_OPTN_NOTIFY_END As String = "Notify End Of Shift"

    Friend Const gsSCHED_SHIFT_DS_TABLENAME As String = "Schedule"
    Friend Const gsSCHED_SHIFT_ID As String = "ID"
    Friend Const gsSCHED_SHIFT_ZONE As String = "Zone"
    Friend Const gsSCHED_SHIFT_ITEM As String = "Item"
    Friend Const gsSCHED_SHIFT_START_HOUR As String = "Start Hour"
    Friend Const gsSCHED_SHIFT_START_MIN As String = "Start Minute"
    Friend Const gsSCHED_SHIFT_START_SEC As String = "Start Second"
    Friend Const gsSCHED_SHIFT_END_HOUR As String = "End Hour"
    Friend Const gsSCHED_SHIFT_END_MIN As String = "End Minute"
    Friend Const gsSCHED_SHIFT_END_SEC As String = "End Second"
    Friend Const gsSCHED_SHIFT_EN As String = "Enabled"

    Friend Const gsSCHED_FOLDER_DS_TABLENAME As String = "Additional Folders"
    Friend Const gsSCHED_FOLDER_ID As String = "ID"
    Friend Const gsSCHED_FOLDER_NAME As String = "Name"
    Friend Const gsSCHED_FOLDER_PATH As String = "Folder Path"
    Friend Const gsSCHED_FOLDER_INC_SUB As String = "Include Subfolders"

    Friend Const gsSCHED_EXT_COP_DS_TABLENAME As String = "Extra Copies"
    Friend Const gsSCHED_EXT_COP_ID As String = "ID"
    Friend Const gsSCHED_EXT_COP_FROM As String = "From"
    Friend Const gsSCHED_EXT_COP_TO As String = "To"
    Friend Const gsSCHED_EXT_COP_INC_SUB As String = "Include Subfolders"

    Friend Const gsSCHED_HIST_DS_TABLENAME As String = "Last Performed"
    Friend Const gsSCHED_HIST_ID As String = "ID"
    Friend Const gsSCHED_HIST_ITEM As String = "Item"
    Friend Const gsSCHED_HIST_LAST As String = "Last Performed"

    'tool bar button tags for button identification
    Friend Const gsCLOSE_BUTTON As String = "close"
    Friend Const gsPRINT_BUTTON As String = "print"
    Friend Const gsSAVE_BUTTON As String = "save"
    Friend Const gsUNDO_BUTTON As String = "undo"
    Friend Const gsCHANGE_BUTTON As String = "change"
    Friend Const gsSTATUS_BUTTON As String = "status"
    Friend Const gsCOPY_BUTTON As String = "copy"
    Friend Const gsMULTI_BUTTON As String = "multi"
    Friend Const gsRESTORE_BUTTON As String = "restore"

    Friend Const gsALARMLOG_DBNAME As String = "Alarms.Mdb"
    Friend Const gsALARM_LOG_DBNAME As String = "Alarm Log"   ' sql dbname, still using old one as well 5/9/08
    ''Friend Const gsCONFIG_DBNAME As String = "Config.Mdb"   ' using SQLServer
    Friend Const gsCONFIG_DBNAME As String = "Configuration"
    Friend Const gsPROCESS_DBNAME As String = "Process Data"
    Friend Const gsSCHEDULE_DBNAME As String = "Schedule"     ' using SQLServer
    Friend Const gsCHANGE_DBNAME As String = "Change.Mdb"
    Friend Const gsCOLCHANGE_DBNAME As String = "Mdb"
    Friend Const gsPRESET_DBNAME As String = "Presets.mdb"
    Friend Const gsPRODLOG_DBNAME As String = "Production Log"

    'constants for names in Vision logging dataset
    Friend Const gsVISN_TABLENAME As String = "[VisionLog]"
    Friend Const gsVISN_DATABASENAME As String = "Vision Log"


    'for multiview form
    Friend Const gsALL_INDEX As String = "Index"


    'Weekly Report DB constants 
    Friend Const gsWEEKLY_REPORT_DBNAME As String = "WeeklyReports"
    Friend Const gsWEEKLY_COL_ID As String = "ID"
    Friend Const gsWEEKLY_LIST_TBLNAME As String = "TableList"
    Friend Const gsWEEKLY_LIST_COL_DATE As String = "Date"
    Friend Const gsWEEKLY_LIST_COL_NAME As String = "TableName"
    Friend Const gsWEEKLY_LIST_COL_MTBF As String = "MTBF"
    Friend Const gsWEEKLY_LIST_COL_MTTR As String = "MTTR"
    Friend Const gsWEEKLY_LIST_COL_UPTIME As String = "Uptime"

    Friend Const gsWEEKLY_OPT_TBLNAME As String = "Options"
    Friend Const gsWEEKLY_OPT_COL_STOR_UNIT As String = "StorageLengthUnit"
    Friend Const gsWEEKLY_OPT_COL_STOR_LEN As String = "StorageLength"
    Friend Const gsWEEKLY_OPT_COL_CHART_UNIT As String = "ChartLengthUnit"
    Friend Const gsWEEKLY_OPT_COL_CHART_LEN As String = "ChartLength"

    'Param setup DB constants
    Friend Const gsPRMSET_DB_NAME As String = "ParamSetup"
    'Screen list table
    Friend Const gsPRMSET_SCRNTBL_NAME As String = "ScreenList"
    Friend Const gsPRMSET_COL_INDEX As String = "Index"
    Friend Const gsPRMSET_COL_TBLNAME As String = "TableName"
    Friend Const gsPRMSET_COL_SCRNNAME As String = "ScreenName"
    Friend Const gsPRMSET_COL_PWDNAME As String = "PasswordName"
    Friend Const gsPRMSET_COL_ICON As String = "Icon"
    'Table name suffixes
    Friend Const gsPRMSET_TAB_SFX As String = "Tabs"
    Friend Const gsPRMSET_ITEM_SFX As String = "Items"
    'Tab Table columns
    Friend Const gsPRMSET_COL_TABLABEL As String = "TabLabel"
    Friend Const gsPRMSET_COL_TABTAG As String = "TabTag"
    Friend Const gsPRMSET_COL_USEZONE As String = "UseZone"
    Friend Const gsPRMSET_COL_USECNTR As String = "UseController"
    Friend Const gsPRMSET_COL_USECUSTCNTR As String = "UseCustomController"
    Friend Const gsPRMSET_COL_CUSTCNTR As String = "CustomController"
    Friend Const gsPRMSET_COL_TAGCNTR As String = "UseTagbyController"
    Friend Const gsPRMSET_COL_USEARM As String = "UseArm"
    Friend Const gsPRMSET_COL_SHOWOPENERS As String = "ShowOpeners"
    Friend Const gsPRMSET_COL_USECOLOR As String = "UseColor"
    Friend Const gsPRMSET_COL_USEVALVE As String = "UseValve"
    Friend Const gsPRMSET_COL_USECUSTOMPARM As String = "UseCustomParm"
    Friend Const gsPRMSET_COL_CUSTOMPARM As String = "CustomParm"
    Friend Const gsPRMSET_COL_HELPFILE As String = "HelpFile"
    Friend Const gsPRMSET_COL_NOEDIT As String = "NoEdit"
    Friend Const gsPRMSET_COL_AUTOREFRESH As String = "AutoRefresh"
    Friend Const gsPRMSET_COL_SA As String = "ScatteredAccess"
    Friend Const gsPRMSET_COL_HIDE As String = "Hide"
    Friend Const gsPRMSET_COL_GRAPHS As String = "Graphs"
    Friend Const gsPRMSET_COL_GRAPH As String = "Graph"
    '    02/26/14   MSW     Added dispenser cartoons to ParamSetup screen.			          4.01.07.01
    Friend Const gsPRMSET_COL_TOON As String = "Toon"



    'Item table 
    Friend Const gsPRMSET_COL_LABEL As String = "Label"
    Friend Const gsPRMSET_COL_TAB As String = "Tab"
    Friend Const gsPRMSET_COL_DATASOURCE As String = "DataSource"
    Friend Const gsPRMSET_COL_DATATYPE As String = "Datatype"
    Friend Const gsPRMSET_COL_PLCTAG As String = "PLCTag"
    Friend Const gsPRMSET_COL_PROGNAME1 As String = "ProgName1"
    Friend Const gsPRMSET_COL_VARNAME1 As String = "VarName1"
    Friend Const gsPRMSET_COL_PROGNAME2 As String = "ProgName2"
    Friend Const gsPRMSET_COL_VARNAME2 As String = "VarName2"
    Friend Const gsPRMSET_COL_DBNAME As String = "DBName"
    Friend Const gsPRMSET_COL_DBTABLENAME As String = "DBTableName"
    Friend Const gsPRMSET_COL_DBROWNAME As String = "DBRowName"
    Friend Const gsPRMSET_COL_DBCOLUMNNAME As String = "DBColumnName"
    Friend Const gsPRMSET_COL_REGPATH As String = "RegPath"
    Friend Const gsPRMSET_COL_REGKEY As String = "RegKey"
    Friend Const gsPRMSET_COL_EDIT As String = "Edit"
    Friend Const gsPRMSET_COL_MIN As String = "Min"
    Friend Const gsPRMSET_COL_MAX As String = "Max"
    Friend Const gsPRMSET_COL_SCALE As String = "Scale"
    Friend Const gsPRMSET_COL_PRECISION As String = "Precision"
    Friend Const gsPRMSET_COL_USECBO As String = "UseCbo"
    Friend Const gsPRMSET_COL_CUSTOMCBO As String = "CustomCBO"
    Friend Const gsPRMSET_COL_CUSTOMCOLORS As String = "CustomColors"
    Friend Const gsPRMSET_COL_COLORRANGE As String = "ColorRange"
    Friend Const gsPRMSET_COL_FORMULA As String = "Formula"
    Friend Const gsPRMSET_COL_PAINTER_ONLY As String = "PainterOnly"
    Friend Const gsPRMSET_COL_DISPLAY As String = "Display"
    '    02/26/14   MSW     Added dispenser cartoons to ParamSetup screen.			          4.01.07.01
    Friend Const gsPRMSET_COL_BITMASK As String = "Bitmask"

    'to take the place of the applicator type,colorchangetype enum etc.
    Friend Const gsNONE As String = "None"
    Friend Const gsGUN As String = "Gun"
    Friend Const gsDP_GUN As String = "Dual Purge Gun"
    Friend Const gsBELL As String = "Bell"
    Friend Const gsACCUSTAT As String = "Accustat"
    Friend Const gsPOWDER As String = "Powder"
    Friend Const gsAQUABELL As String = "Aquabell"
    Friend Const gsSERVOBELL As String = "Servobell"
    Friend Const gsCHOP_GUN As String = "Chop Gun"
    Friend Const gsGEL_GUN As String = "Gel Gun"
    Friend Const gsVERSABELL As String = "Versabell"
    Friend Const gsVERSABELL2 As String = "Versabell 2"
    Friend Const gsESTATFB200 As String = "fb200"

    'XML File Names
    Friend Const gsSYS_COL_XMLFILE As String = "System Colors.xml"
    Friend Const gsSYS_COL_XMLSCHEMA As String = "System Colors.xsd"
    Friend Const gsROBOTS_XMLFILE As String = "Robot Setup.xml"
    Friend Const gsROBOTS_XMLSCHEMA As String = "Robot Setup.xsd"

    ' limit for description fields
    Friend Const gsMAX_DESC_LEN As Integer = 30

    Friend Const gsDMON_DELIMITER As String = "-"

    '*** these variables are used to lookup the value recieved from scattered access and
    'must match the data array recieved from scattered access oSA.Data(description,value)
    ' geo 3/24/09.  this supports the SA interop for use in .net for now.
    ' NOTE : do not include the 'EQ1: ' or 'EQ2: ' prefix in the declaration 
    '    12/15/14   MSW     Allow direct access to read items in clsScatteredAccessItems      4.01.07.08
    Friend Const gsSA_FAST_CLOCK As String = "Controller Fast Clock"
    Friend Const gsSA_FLOW_RATE As String = "Flow Rate"
    Friend Const gsSA_PAINT_AOUT As String = "Paint AOUT"
    Friend Const gsSA_REQUESTED_FLOW As String = "Requested Flow"
    Friend Const gsSA_FLOW_TOTAL As String = "Flow Total"
    Friend Const gsSA_REQUESTED_FLOW_PUMP_1 As String = "Requested Flow Pump 1"
    Friend Const gsSA_REQUESTED_FLOW_PUMP_2 As String = "Requested Flow Pump 2"
    Friend Const gsSA_RESIN_INLET_PRESS As String = "Resin Inlet Press"
    Friend Const gsSA_RESIN_OUTLET_PRESS As String = "Resin Outlet Press"
    Friend Const gsSA_HARDENER_INLET_PRESS As String = "Hardener Inlet Press"
    Friend Const gsSA_HARDENER_OUTLET_PRESS As String = "Hardener Outlet Press"
    Friend Const gsSA_CAN_POS As String = "Canister Position"
    Friend Const gsSA_CAN_TORQUE As String = "Canister Torque"
    Friend Const gsSA_SUBCAN_POS As String = "Sub Canister Position"
    Friend Const gsSA_SUBCAN_TORQUE As String = "Sub Canister Torque"
   
    Friend Const gsSA_PAINT_IN_CAN As String = "Paint In Can"
    Friend Const gsSA_REQUESTED_BELL_SPEED As String = "Requested Bell Speed"
    Friend Const gsSA_ACTUAL_TURBINE_SPEED As String = "Actual Turbine Speed"
    Friend Const gsSA_REQUESTED_SHAPING_AIR As String = "Requested Shaping Air"
    Friend Const gsSA_REQUESTED_SHAPING_AIR2 As String = "Requested Shaping Air 2"
    Friend Const gsSA_SHAPING_AIR_MANIFOLD_PRESS As String = "Shaping Air Manifold Press"
    Friend Const gsSA_ACTUAL_SHAPING_AIR As String = "Actual Shaping Air"
    Friend Const gsSA_ACTUAL_SHAPING_AIR2 As String = "Actual Shaping Air 2"
    Friend Const gsSA_ESTAT_PRESET_NUMBER As String = "Estat Preset Number"
    Friend Const gsSA_FLUID_PRESET_NUM As String = "Fluid Preset Number"
    Friend Const gsSA_ESTAT_REQUESTED_KV As String = "Estat Requested KV"
    Friend Const gsSA_ESTAT_ACTUAL_KV As String = "Estat Actual KV"
    Friend Const gsSA_ESTAT_ACTUAL_uA As String = "Estat Actual uA"
    Friend Const gsSA_CAN2_POS As String = "Canister 2 Position"
    Friend Const gsSA_CAN2_TORQUE As String = "Canister 2 Torque"
    Friend Const gsSA_PAINT_IN_CAN2 As String = "Paint In Can 2"
    Friend Const gsSA_FLOW_RATE_2 As String = "Requested Flow 2"

    Friend Const gsSA_CC_CYCLE_NAME As String = "Color Change Cycle Name"
    Friend Const gsSA_CC_CYCLE_NUMBER As String = "CC Cycle Number"
    Friend Const gsSA_CC_CYCLE_TIME As String = "Color Change Cycle Time"
    Friend Const gsSA_CC_STATUS As String = "Color Change Status" 'RJO 02/13/14
    Friend Const gsSA_CC_TIME_1 As String = "Color Change Cycle 1 Time"
    Friend Const gsSA_CC_TIME_2 As String = "Color Change Cycle 2 Time"
    Friend Const gsSA_CC_TIME_3 As String = "Color Change Cycle 3 Time"
    Friend Const gsSA_CC_TIME_4 As String = "Color Change Cycle 4 Time"
    Friend Const gsSA_CC_TIME_5 As String = "Color Change Cycle 5 Time"
    Friend Const gsSA_CC_TIME_6 As String = "Color Change Cycle 6 Time"
    Friend Const gsSA_CC_TIME_7 As String = "Color Change Cycle 7 Time"
    Friend Const gsSA_CC_TIME_8 As String = "Color Change Cycle 8 Time"
    Friend Const gsSA_CC_TIME_9 As String = "Color Change Cycle 9 Time"
    Friend Const gsSA_CC_TIME_10 As String = "Color Change Cycle 10 Time"
    Friend Const gsSA_CC_TIME_PRE As String = "Color Change Cycle "
    Friend Const gsSA_CC_TIME_POST As String = " Time"

    Friend Const gsSA_CURRENT_COLOR As String = "Current Color"
    Friend Const gsSA_CURRENT_COLOR_NAME As String = "Current Color Name"
    Friend Const gsSA_CURRENT_VALVE As String = "Current Valve"
    Friend Const gsSA_CURRENT_JOB As String = "Current Job"
    Friend Const gsSA_CURRENT_PATH As String = "Current Path"
    Friend Const gsSA_BELL_STARTUP_COMPLETE As String = "Bell Startup Complete"
    Friend Const gsSA_TRIGGER As String = "Trigger"
    Friend Const gsSA_SPARE_1 As String = "Spare 1"
    Friend Const gsSA_HARDENER As String = "Hardener"
    Friend Const gsSA_COLOR_ENABLE As String = "Color Enable"
    Friend Const gsSA_CC_VALVES As String = "CC Valves"
    Friend Const gsSA_CC_VALVES2 As String = "CC Valves2"
    Friend Const gsSA_ACA As String = "ACA"
    Friend Const gsSA_pACS As String = "pACS"
    Friend Const gsSA_ACVA As String = "ACVA"
    Friend Const gsSA_PUMP_1_RUNNING As String = "Pump 1 Running"
    Friend Const gsSA_PUMP_2_RUNNING As String = "Pump 2 Running"
    Friend Const gsSA_AT_HOME As String = "At Home"
    Friend Const gsSA_AT_CLEANIN As String = "At Clean In"
    Friend Const gsSA_AT_CLEANOUT As String = "At Clean Out"
    Friend Const gsSA_AT_BYPASS As String = "At Bypass"
    Friend Const gsSA_AT_PURGE As String = "At Purge"
    Friend Const gsSA_AT_MASTER1 As String = "At Master 1"
    Friend Const gsSA_AT_SPECIAL1 As String = "At Special 1"
    'DQ Calibration variables
    Friend Const gsSA_DQ_CAL_ACTIVE As String = "DQ Cal Active"
    Friend Const gsSA_DQ_OUTPUT As String = "DQ Output"
    Friend Const gsSA_DQ_CAL_STATUS As String = "DQ Cal Status"
    'DQ2 Calibration variables
    Friend Const gsSA_DQ2_CAL_ACTIVE As String = "DQ2 Cal Active"
    Friend Const gsSA_DQ2_OUTPUT As String = "DQ2 Output"
    Friend Const gsSA_DQ2_CAL_STATUS As String = "DQ2 Cal Status"
    'IC Calibration variables
    Friend Const gsSA_IC_CAL_ACTIVE As String = "IC Cal Active"
    Friend Const gsSA_IC_CAL_STATUS As String = "IC Cal Status"
    Friend Const gsSA_IC_CAL2_ACTIVE As String = "IC Cal 2 Active"
    Friend Const gsSA_IC_CAL2_STATUS As String = "IC Cal 2 Status"
    Friend Const gsSA_IC_CAN_SELECT As String = "Can Select"
    'AF Calibration variables
    Friend Const gsSA_AF_CAL_ACTIVE As String = "AF Cal Active"

    'IC Calibration variables
    Friend Const gsSA_IPC_CAL_ACTIVE As String = "IPC Cal Active"

    Friend Const gsSA_CONTROLLER_FAST_CLOCK As String = "Controller Fast Clock"

    'Honda WB "S-Unit" dock device constants
    Friend Const gsSA_SUNIT_POS As String = "Dock Position"
    Friend Const gsSA_SUNIT_FORCE As String = "Dock Force"
    Friend Const gsSA_SUNIT_CAL_STATUS As String = "Dock Cal Status"

    'Line Status variables 'RJO 04/21/11
    Friend Const gsSA_PUSHED_OUT As String = "Pushed Out"
    Friend Const gsSA_CLEANED_OUT As String = "Cleaned Out"
    Friend Const gsSA_FILLED As String = "Filled"

    Friend Const gsSA_PUSHED_OUT2 As String = "Pushed Out 2"
    Friend Const gsSA_CLEANED_OUT2 As String = "Cleaned Out 2"
    Friend Const gsSA_FILLED2 As String = "Filled 2"
 'CurColor
    Friend Const gsSA_CurColor As String = "Cur Color"
    'NextColor
    Friend Const gsSA_NextColor As String = "Next Color"
    'SubCurColor
    Friend Const gsSA_SubCurColor As String = "Sub Cur Color"
    'MainLineState
    Friend Const gsSA_MainLineState As String = "Main Line State"
    'SubLineState
    Friend Const gsSA_SubLineState As String = "Sub Line State"
    '    JCA
    Friend Const gsSA_JCA As String = "JCA"
    '    JCA
    Friend Const gsSA_JCR As String = "JCR"
    '    dock
    Friend Const gsSA_CLNEXT As String = "CLN-EXT"
    Friend Const gsSA_CLNRET As String = "CLN-RET"
    '    Grounding Unit
    Friend Const gsSA_GWP As String = "GWP"
    Friend Const gsSA_GWSP As String = "GWSP"
    Friend Const gsSA_GAIRP As String = "GAIRP"

    'PaintTool 7.50 preset configuration
    Friend Const gnMAX_ELM_NODE As Integer = 1000



    'Help constants
    'Browser screen treeview load constants
    Friend Const gs_HELP_MANUALS As String = "MANUALS"
    Friend Const gs_HELP_ROBOTS As String = "ROBOTS"
    Friend Const gs_HELP_PWHELP As String = "PWHELP"
    'help executable name
    Friend Const gs_HELP_EXE As String = "PWBrowser.exe"

    'help Screen link constants
    'PW4 Main
    Friend Const gs_HELP_MAIN As String = "Welcome to PWHelp.htm"
    Friend Const gs_HELP_MENU_CONFIG As String = "[1.0]PAINTWorksMainMenu\ConfigurationMenu.htm"
    Friend Const gs_HELP_MENU_PROCESS As String = "[1.0]PAINTWorksMainMenu\ProcessMenu.htm"
    Friend Const gs_HELP_MENU_VIEW As String = "[1.0]PAINTWorksMainMenu\ViewMenu.htm"
    Friend Const gs_HELP_MENU_OPERATE As String = "[1.0]PAINTWorksMainMenu\OperateMenu.htm"
    Friend Const gs_HELP_MENU_REPORTS As String = "[1.0]PAINTWorksMainMenu\ReportsMenu.htm"
    Friend Const gs_HELP_MENU_UTILITIES As String = "[1.0]PAINTWorksMainMenu\UtilitiesMenu.htm"
    Friend Const gs_HELP_MENU_MAINT As String = "[1.0]PAINTWorksMainMenu\MaintenanceMenu.htm"
    Friend Const gs_HELP_MENU_HELP As String = "[1.0]PAINTWorksMainMenu\HelpMenu.htm"
    'config
    Friend Const gs_HELP_JOBSETUP_STYLES As String = "[2.0]ConfigurationMenu\JobSetupStyles.htm"
    Friend Const gs_HELP_JOBSETUP_STYLEOPTION As String = "[2.0]ConfigurationMenu\JobSetupStyleOptions.htm"
    Friend Const gs_HELP_JOBSETUP_OPTIONS As String = "[2.0]ConfigurationMenu\JobSetupOptions.htm"
    Friend Const gs_HELP_JOBSETUP_REPAIR As String = "[2.0]ConfigurationMenu\JobSetupRepair.htm"
    Friend Const gs_HELP_JOBSETUP_DEGRADE As String = "[2.0]ConfigurationMenu\JobSetupDegrade.htm"
    Friend Const gs_HELP_JOBSETUP_INTRUSION As String = "[2.0]ConfigurationMenu\JobSetupIntrusion.htm"
    Friend Const gs_HELP_JOBSETUP_STYLEID As String = "[2.0]ConfigurationMenu\JobSetupStyleID.htm"
    Friend Const gs_HELP_JOBSETUP_STYLEIDSS As String = "[2.0]ConfigurationMenu\JobSetupStyleIDSS.htm"
    Friend Const gs_HELP_SYSCOLORS_VALVES As String = "[2.0]ConfigurationMenu\SystemColorsValves.htm"
    Friend Const gs_HELP_SYSCOLORS_ROBREQ As String = "[2.0]ConfigurationMenu\SystemColorsRobotsRequired.htm"
    Friend Const gs_HELP_SYSCOLORS_2K As String = "[2.0]ConfigurationMenu\SystemColors2K.htm"
    Friend Const gs_HELP_SYSCOLORS_TRICOAT As String = "[2.0]ConfigurationMenu\SystemColorsTricoat.htm"

    'process
    Friend Const gs_HELP_COLORCHANGE As String = "[2.1]ProcessMenu\ColorChange.htm"
    Friend Const gs_HELP_COLORCHANGE_MV As String = "[2.1]ProcessMenu\ColorChangeSubs\CCMV.htm"
    Friend Const gs_HELP_COLORCHANGE_COPY As String = "[2.1]ProcessMenu\ColorChangeSubs\CCCopy.htm"
    Friend Const gs_HELP_CALIBRATION As String = "[2.1]ProcessMenu\Calibration.htm"
    Friend Const gs_HELP_CALIBRATION_MV As String = "[2.1]ProcessMenu\CalibrationSubs\CalibrationMV.htm"
    Friend Const gs_HELP_CALIBRATION_COPY As String = "[2.1]ProcessMenu\CalibrationSubs\CalibrationCopy.htm"
    Friend Const gs_HELP_DMON_SCHEDMAN As String = "[2.1]ProcessMenu\DiagnosticsManager.htm"
    Friend Const gs_HELP_DMON_SCHEDMAN_SYSTEM As String = "[2.1]ProcessMenu\DiagnosticsManager_SystemSetup.htm"
    Friend Const gs_HELP_DMON_SCHEDMAN_SCHEDULES As String = "[2.1]ProcessMenu\DiagnosticsManager_ManageSchedules.htm"
    Friend Const gs_HELP_DMON_SCHEDMAN_ITEMS As String = "[2.1]ProcessMenu\DiagnosticsManager_ManageItems.htm"

    Friend Const gs_HELP_PRESETS As String = "[2.1]ProcessMenu\FluidPresets.htm"
    Friend Const gs_HELP_PRESETS_COPY As String = "[2.1]ProcessMenu\FluidPresetsSubs\FluidPresetsCopy.htm"
    Friend Const gs_HELP_PRESETS_MV As String = "[2.1]ProcessMenu\FluidPresetsSubs\FluidPresetsMultiView.htm"
    Friend Const gs_HELP_PRESETS_OVERRIDE As String = "[2.1]ProcessMenu\FluidPresetsSubs\FluidPresetsOverride.htm"
    Friend Const gs_HELP_CCPRESETS As String = "[2.1]ProcessMenu\CCPresets.htm"
    Friend Const gs_HELP_CCPRESETS_COPY As String = "[2.1]ProcessMenu\CCPresetsSubs\CCPresetsCopy.htm"
    Friend Const gs_HELP_CCPRESETS_MV As String = "[2.1]ProcessMenu\CCPresetsSubs\CCPresetsMultiView.htm"
    Friend Const gs_HELP_ESTATPRESETS As String = "[2.1]ProcessMenu\EstatPresets.htm"
    Friend Const gs_HELP_ESTATPRESETS_COPY As String = "[2.1]ProcessMenu\EstatPresetsSubs\EstatPresetsCopy.htm"
    Friend Const gs_HELP_ESTATPRESETS_MV As String = "[2.1]ProcessMenu\EstatPresetsSubs\EstatPresetsMultiView.htm"
    Friend Const gs_HELP_ESTATPRESETS_OVERRIDE As String = "[2.1]ProcessMenu\EstatPresetsSubs\EstatPresetsOverride.htm"
    Friend Const gs_HELP_ESTATPRESETS_STEPS As String = "[2.1]ProcessMenu\EstatPresetsSubs\EstatPresetsStep.htm"
    Friend Const gs_HELP_AREA_VOLS As String = "[2.1]ProcessMenu\AreaVolume.htm"
    Friend Const gs_HELP_LEARNED_VOLS As String = "[2.1]ProcessMenu\LearnedVolume.htm"
    Friend Const gs_HELP_SEAL_SCHED As String = "[2.1]ProcessMenu\Sealant Schedules.htm"
    Friend Const gs_HELP_SEALER_DIAG As String = "[2.1]ProcessMenu\SealerDiagnostics.htm"
    Friend Const gs_HELP_SEALER_DIAG_TAB2 As String = "[2.1]ProcessMenu\SealerDiagnosticsTab2.htm"
    Friend Const gs_HELP_SEALER_DIAG_TAB3 As String = "[2.1]ProcessMenu\SealerDiagnosticsTab3.htm"
    Friend Const gs_HELP_SEALER_DIAG_TAB4 As String = "[2.1]ProcessMenu\SealerDiagnosticsTab4.htm"
    Friend Const gs_HELP_SEALER_DIAG_TAB5 As String = "[2.1]ProcessMenu\SealerDiagnosticsTab5.htm"

    'View
    Friend Const gs_HELP_VIEW_BSD As String = "[2.2]ViewMenu\BoothStatusDisplay.htm"
    Friend Const gs_HELP_VIEW_CELLSTATUS As String = "[2.2]ViewMenu\CellStatusDisplay.htm"
    Friend Const gs_HELP_VIEW_BSD_QUEUE As String = "[2.2]ViewMenu\BSDSubs\BSDRobotProcessInd.htm"
    Friend Const gs_HELP_VIEW_BSD_PROCESS As String = "[2.2]ViewMenu\BSDSubs\BSDRobotProcessInd.htm"
    Friend Const gs_HELP_VIEW_CELLSTATUS_PROCESS As String = "[2.2]ViewMenu\CellStatusProccess.htm"
    Friend Const gs_HELP_VIEW_PLCIO As String = "[2.2]ViewMenu\PLCIOMonitor.htm"
    Friend Const gs_HELP_VIEW_WEB As String = "[2.2]ViewMenu\RobotWeb.htm"
    Friend Const gs_HELP_VIEW_DMON As String = "[2.2]ViewMenu\Diagnostics.htm"
    Friend Const gs_HELP_VIEW_DMON_CHART As String = "[2.2]ViewMenu\DiagnosticsGraphView.htm"
    Friend Const gs_HELP_VIEW_ROBOT_IO As String = "[2.2]ViewMenu\RobotIOMonitor.htm"
    Friend Const gs_HELP_VIEW_BULKSUPPLY As String = "[2.2]ViewMenu\BulkSupply.htm" 'RJO 07/15/13
    Friend Const gs_HELP_ZDT As String = "ZDT\ZDT_Help.pdf"
    'Operate
    Friend Const gs_HELP_FLUIDMAINT As String = "[2.3]OperateMenu\FluidMaintenance.htm"
    Friend Const gs_HELP_FLUIDMAINT_DEVICES As String = "[2.3]OperateMenu\FluidMaintenanceSubs\FluidMaintenanceDevices.htm"
    Friend Const gs_HELP_MANUAL_CYCLE As String = "[2.3]OperateMenu\ManualCycle.htm"
    Friend Const gs_HELP_MANUAL_CYCLE_RESPRAY As String = "[2.3]OperateMenu\ManualCycleRespray.htm"
    Friend Const gs_HELP_MANUAL_CYCLE_DEVICES As String = "[2.3]OperateMenu\ManualOperationsSubs\Devices.htm"
    Friend Const gs_HELP_GUNTEST As String = "[2.3]OperateMenu\Guntest.htm"
    Friend Const gs_HELP_GUNTEST_DEVICES As String = "[2.3]OperateMenu\GuntestSubs\Devices.htm"

    'Reports
    Friend Const gs_HELP_REPORTS_PRODUCTION As String = "[2.4]ReportsMenu\ProductionLog.htm"
    Friend Const gs_HELP_REPORTS_ALARM As String = "[2.4]ReportsMenu\Alarms.htm"
    Friend Const gs_HELP_REPORTS_DOWNTIME As String = "[2.4]ReportsMenu\DowntimeLog.htm"
    Friend Const gs_HELP_REPORTS_RMCHARTS As String = "[2.4]ReportsMenu\RMCharts.htm"
    Friend Const gs_HELP_REPORTS_RMCHARTSSEMIAUTO As String = "[2.4]ReportsMenu\RMChartsSemiAuto.htm"
    Friend Const gs_HELP_REPORTS_CHANGE As String = "[2.4]ReportsMenu\ChangeLog.htm"
    Friend Const gs_HELP_REPORTS_CHANGESUMMARY As String = "[2.4]ReportsMenu\ChangeLogSummary.htm"
    Friend Const gs_HELP_REPORTS_VISION As String = "[2.4]ReportsMenu\VisionLog.htm"

    'Utilities
    Friend Const gs_HELP_UTILITIES_SCHEDULER As String = "[2.5]UtilitiesMenu\Scheduler.htm"
    Friend Const gs_HELP_UTILITIES_ARCHIVE As String = "[2.5]UtilitiesMenu\Archive.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_ACCESS As String = "[2.5]UtilitiesMenu\ArchiveAccess.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_XML As String = "[2.5]UtilitiesMenu\ArchiveXML.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_IMAGE As String = "[2.5]UtilitiesMenu\ArchiveImage.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_NOTEPAD As String = "[2.5]UtilitiesMenu\ArchiveNotepad.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_ROBOTS As String = "[2.5]UtilitiesMenu\ArchiveRobots.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_APP As String = "[2.5]UtilitiesMenu\ArchiveApplication.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_DMON As String = "[2.5]UtilitiesMenu\ArchiveDMON.htm"
    'Friend Const gs_HELP_UTILITIES_ARCHIVE_DMON_ARCHIVE As String = "[2.5]UtilitiesMenu\ArchiveDMONArchive.htm"
    Friend Const gs_HELP_UTILITIES_CLOCK As String = "[2.5]UtilitiesMenu\DateTimeSetup.htm"
    Friend Const gs_HELP_UTILITIES_FILECOPY As String = "[2.5]UtilitiesMenu\FileCopy.htm"
    'Friend Const gs_HELP_UTILITIES_FILECOPY_SOURCE As String = "[2.5]UtilitiesMenu\FileCopySource.htm"
    'Friend Const gs_HELP_UTILITIES_FILECOPY_FILES As String = "[2.5]UtilitiesMenu\FileCopyFiles.htm"
    'Friend Const gs_HELP_UTILITIES_FILECOPY_DOCOPY As String = "[2.5]UtilitiesMenu\FileCopyDoCopy.htm"
    'Friend Const gs_HELP_UTILITIES_FILECOPY_DEST As String = "[2.5]UtilitiesMenu\FileCopyDest.htm"
    Friend Const gs_HELP_UTILITIES_FILEVIEWCOMPARE As String = "[2.5]UtilitiesMenu\FileViewCompare.htm"
    Friend Const gs_HELP_UTILITIES_NOTEPAD As String = "[2.5]UtilitiesMenu\Notepad.htm"
    Friend Const gs_HELP_UTILITIES_ROBOTVARIABLES As String = "[2.5]UtilitiesMenu\RobotVariables.htm"
    Friend Const gs_HELP_UTILITIES_VERSIONS As String = "[2.5]UtilitiesMenu\Versions.htm"
    Friend Const gs_HELP_UTILITIES_PWUSERMAINT As String = "[2.5]UtilitiesMenu\Passwords.htm" 'RJO 03/20/12
    'Maint
    Friend Const gs_HELP_MAINT_NET As String = "[2.6]MaintenanceMenu\NetworkConfiguration.htm"
    'alarms
    Friend Const gs_HELP_ALARMS As String = "[2.8]Alarms\Alarms.htm"


    Friend Const gs_HELP_SCATTEREDACCESS As String = "[2.9]ScatteredAccess\ScatteredAccess.htm"

    '********New program-to-program communication object******************************************
    Friend Const gs_COM_ID_PASSWORD As String = "password"
    Friend Const gs_COM_ID_PW_MAIN As String = "PW4_Main"
    Friend Const gs_COM_ID_PWUSERMAINT As String = "pwusermaint"
    Friend Const gs_COM_ID_ALARM_MAN As String = "AlarmMan"
    Friend Const gs_COM_ID_PROD_LOG As String = "Prodlogger"
    Friend Const gs_COM_ID_SCHEDMAN As String = "SchedMan"
    Friend Const gs_COM_ID_SCHEDULER As String = "Scheduler"
    Friend Const gs_COM_ID_MAINT As String = "PW_Maint"
    Friend Const gs_COM_ID_REPORTS As String = "reports"
    Friend Const gs_COM_ID_SA As String = "ScatteredAccess"
    '********************************************************************************************

    '    02/15/12   MSW     Import/Export Updates                                             4.01.01.03
    '********Zip Utility Commands******************************************
    Friend Const gs_ZIP_ALL As String = "ZIP_ALL"
    Friend Const gs_UNZIP_ALL As String = "UNZIP_ALL"
    Friend Const gs_ZIP_UTIL_FILENAME As String = "ZipUtil.exe"
    Friend Const gs_ZIP_UTIL_ERRLOG As String = "ZipUtil.txt"
    '********************************************************************************************

    '********Zip Utility Commands******************************************
    Friend Const gs_XLS_TO_XML As String = "XLSXML"
    Friend Const gs_XML_TO_XLS As String = "XMLXLS"
    Friend Const gs_XML_TO_XLS_PIC As String = "XMLXLSPIC"
    Friend Const gs_EXCEL_UTIL_FILENAME As String = "ExcelUtil.exe"
    Friend Const gs_EXCEL_UTIL_ERRLOG As String = "ExcelUtil.txt"
    '********************************************************************************************

    '********Standalone change log screen******************************************
    Friend Const gs_CHANGELOG_EXE As String = "ChangeLog.exe"

    '********Vision Logger and Hotedit logger need screen names managed manually for reports*****
    Friend Const gs_VISION_LOGGER_NAME As String = "VisionLogger"
    Friend Const gs_HOTEDIT_LOGGER_NAME As String = "HotEditLogger"

#End Region
#Region " Friend Enumerations "

    Public Enum eParamType
        None = 0
        Colors = 1
        Styles = 2
        Valves = 3
    End Enum

    Friend Enum eArmType
        None = 0
        P10_Opener = 1
        P15_Opener = 2
        P145 = 3
        P155 = 4
        P200 = 5
        P250 = 6
        P500 = 7
        S500 = 8
        P700iA = 9
        P700_Opener = 10
        P500_Opener = 11
        P20_Opener = 12
        P25_Opener = 13
        LeftArm = 14
        RightArm = 15
        LeftOpener = 16
        RightOpener = 17
        P1000 = 18
        P35_Opener = 19
    End Enum
    Friend Enum eLoadModes
        LoadFromPLC = 0    'read data from PLC
        LoadFromDB = 1     'read data from the database
        LoadFromRobot = 2
    End Enum
    Friend Enum eColorChangeType
        NOT_NONE = -2   '11/04/09   MSW     Add not none as color change type option
        NOT_SELECTED = -1
        NONE = 0
        SINGLE_PURGE = 1
        DUAL_PURGE = 2
        MODIFIED_DUAL_PURGE = 3
        GUN_2K = 4
        BELL_2K = 5
        ACCUSTAT = 6
        AQUABELL = 7
        SINGLE_PURGE_BELL = 8
        SERVOBELL = 9
        VERSABELL = 10
        VERSABELL2 = 11
        VERSABELL2_WB = 12
        VERSABELL2_2K = 13
        POWDER = 14
        VERSABELL_2K = 15
        VERSABELL2_2K_1PLUS1 = 16
        VERSABELL2_2K_MULTIRESIN = 17
        VERSABELL2_PLUS = 18
        VERSABELL2_PLUS_WB = 19
        HONDA_1K = 20
        HONDA_WB = 21
        VERSABELL2_32 = 22 'BTK 02/15/10 Added color change type for Versabell2 32 valves
        ACP2KGUNFLEX = 23 'HGB ACP 2k gun for the flex controller
        ACP2KBELLFLEX = 24 'HGB ACP 2k bell for the flex controller
        VERSABELL3_DUAL_WB = 25
        VERSABELL3_WB = 26
        VERSABELL3 = 27
        VERSABELL3P1000 = 28 'RJO P-1000 10/08/14
        HONDA_WB_DUAL = 29
        GUN_1K = 30 ' MAS 09122016 Adding a new type for Prime's gun NRU 160922 GUN_PUMP to GUN_1K
        GUN_2K_PIG = 31 'NVSP 12/05/2016 Changes to accomadate piggable
        GUN_2K_Mica = 32    'JZ 12062016 - Piggable stack only.
    End Enum
    Friend Enum eDispenserType '04/11/2012 RJO
        None = 0
        ISD = 1
        IPS = 2
        Bedliner = 3
        HemFlange = 4
        Generic = 5
    End Enum
    Friend Enum eEstatType
        None = 0
        GNM100 = 1
        GNM4002 = 2
        FB200 = 3
        MagicBus = 4
        Honda = 5
        ITW = 6 'BTK 02/17/10 Added ITW estat type and other.
        iPackHV = 7 'BTK 11/11/13 Added iPackHV controller and shifted other.
        Other = 8
    End Enum
    Friend Enum eCalParameters
        FirstParm = 1
        Lo_Range = 1
        Hi_Range = 2
        Adapt_Gain = 3
        Cal_Status = 4
        DQ_MAN_PRESS_WARN = 5  '7/14/08  add dq params
        DQ_MAN_PRESS_ALARM = 6
        DQ_CAL_STEP_DLY = 7
        DQ_OUT_TOL_TIMEOUT = 8
        DQ_GRACE_PERIOD = 9
        LastParm = 10
    End Enum
    Public Enum eSAProcessVarPos
        Flow = 0
        Total = 1
        RequestedFlow = 2
        RequestedAtom = 3
        RequestedFan = 4
        RequestedEStat = 5
        PaintPresetNo = 6
        EstatPresetNo = 7
        CycleTime = 8
        ColorChangeCycle = 9
        LastCCTime = 10
        TPR = 11
        TPRShouldBe = 12
        CanPosition = 13
        PaintInCan = 14
        CurrentColor = 15
        CurrentColorName = 16
        JobComment = 17
        PathComment = 18
        LastOne = 18
    End Enum
    Friend Enum ePLCMANUAL_ZONE_LINK
        TestTimeWd = 0          'Flow test total time
        TestTimeRemainingWd = 1 'Time remaining in active flow test
        CurrentColorWd = 2      'Current manual color
        CurrentColorChangeCycleWd = 3   'Not currently used
        RobotsSelectedWd = 4            'Robots selected for manual functions, arm1 = bit 1
        RobotsReadyWd = 5       'Robots ready for manual functions - bit 0 = zone, bit 1 = arm 1...
        BoothReadyBit = 0
        Robot1Bit = 1
        ManualStatusWd = 6  'Calibration status word
        AutoCalActivBit = 0     'Paint/autocal active
        ScaleCalActivBit = 1    'scale cal active
        DQCalActivBit = 2       'DQ cal active
        FlowTestStatusBit = 4   'flow test active
        BeakerEnabBit = 5   'PLC enables the beaker mode button
        BeakerActiveBit = 6 'Beaker mode active
        CCActiveBit = 7
        'Device screen 
        BBZoneWord = 9      'Zone bingo board dword
        BBRobotWord = 9     'Add ARM number to this for bingo board word
        BBRobotStartWord = 10
        BBRobotHomeBit = 7
        MitsBBZoneWord = 7      'Zone bingo board dword
        MitsBBRobotWord = 7     'Add ARM number to this for bingo board word
        MitsBBRobotStartWord = 8
        SealerBBZoneWord = 16
        SealerBBRobot1Word = 24
    End Enum
    Friend Enum ePLCMANUAL_ROBOT_LINK
        CurrentCCCycle = 0
        RobotsReadyWd = 1
        CurrentKV = 2
        CurrentUA = 3
        CurrentCCTime = 1
        '        Offset = 10
        '        Robot1StartWord = 10
    End Enum
    Public Enum ePLCType
        None = 0
        PLC5 = 1
        SLC500 = 2
        Logix5000 = 3
        Mitsubishi = 4
        QSeries = 4
        Siemens = 5
        S7_200 = 6
        S7_300 = 7
        S7_400 = 8
        GEF90_30 = 9
        GEF90_70 = 10
        GEFPac7 = 11
        'PLC3 = 101
        'GuardLogix = 102 '- use Logix5000
    End Enum
    Friend Enum eControllerType
        None = 0
        RJ2 = 1
        RJ3 = 2
        RJ3iB = 3
        RJ3iB_P500 = 4
        R30iA = 5
        R30iB = 6
    End Enum


    'for future use
    ''Friend Enum ePrivilegeID
    ''    None
    ''    Read
    ''    ReadLimited
    ''    ReadWrite
    ''    ReadWriteLimited
    ''End Enum
    Friend Enum eSAIndex
        FastClock
        ReqFlow
        ReqFlowP1
        ReqFlowP2
        ActFlow
        TotFlow
        PaintInCan
        CanisterPos
        SubCanisterPos
        ReqBSAA
        ActBSAA
        ReqSAFA
        ActSAFA
        ReqSAFA2
        ActSAFA2
        ReqES
        ActKV
        ActUA
        PresetNum
        EstatPresetNum
        CCCycleName
        CCCycleNumber
        'Don't split these up
        LastCCTime
        CCTimeCycle1
        CCTimeCycle2
        CCTimeCycle3
        CCTimeCycle4
        CCTimeCycle5
        CCTimeCycle6
        CCTimeCycle7
        CCTimeCycle8
        CCTimeCycle9
        CCTimeCycle10
        CCTimeCycle11
        CCTimeCycle12
        CCTimeCycle13
        CCTimeCycle14
        CCTimeCycle15
        CCTimeCycle16
        CCTimeCycle17
        CCTimeCycle18
        CCTimeCycle19
        CCTimeCycle20
        CCTimeCycle21
        CCTimeCycle22
        CCTimeCycle23
        CCTimeCycle24
        CCTimeCycle25
        'through here
        CurrentColor
        CurrentColorName
        CurrentValve
        CurrentJob
        CurrentPath
        BellStartupComplete
        ResinInletPressure
        ResinOutletPressure
        HardenerInletPressure
        HardenerOutletPressure
        Trigger_Shared0
        Shared1
        HE_Shared2
        CE_Shared3
        CCValves1
        CCValves2
        P1Running
        P2Running
        '    12/15/14   MSW     Allow direct access to read items in clsScatteredAccessItems      4.01.07.08
        'AtHome
        'AtCleanIn
        'AtCleanOut
        'AtBypass
        'AtPurge
        'AtMaster
        'AtSpecial1
        'AtSpecial2
        DQCalActive
        DQOutput
        DQCalStatus
        DQ2CalActive
        DQ2Output
        DQ2CalStatus
        ICCalActive
        ICCalStatus
        PaintAout
        AFCalActive
        CanisterTorque
        SubCanisterTorque
        SUnitPos
        SUnitTorque
        SUnitCalStatus
        IPCCalActive
        PushedOut 'RJO 04/21/11
        CleanedOut 'RJO 04/21/11
        Filled 'RJO 04/21/11
        PushedOut2
        CleanedOut2
        Filled2
        CCStatus 'RJO 02/13/14
        
        CurColor
        NextColor
        SubCurColor
        MainLineState
        SubLineState
        JCA
        JCR
		CLNEXT
		CLNRET
        Canister2Pos
        Canister2Torque
        PaintInCan2
        ReqFlow2
        ICCal2Active
        ICCal2Status
        ICCanSelect
        ACA
        pACS
        ACVA
        GWP
        GWSP
        GAIRP
        'Insert before "Max". it's used in array sizing
        Max
    End Enum

    Friend Enum eLineState 'RJO 04/21/11
        ArmNotSelected = -1
        CleanedOut = 0
        PushedOut = 1
        Filled = 2
        Unknown = 3
    End Enum

    Friend Enum ePrivilege As Integer 'RJO 03/09/12
        None = 0
        Edit = 1
        Copy = 2
        Delete = 3
        Administrate = 4
        Restore = 5
        Execute = 6
        Remote = 7
        Operate = 8
        Special = 9
    End Enum
#End Region

End Module
