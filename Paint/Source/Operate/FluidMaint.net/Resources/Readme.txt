Notes on the PW4 Fluid maintenance screen
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
PLC Comm
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
"ManualFuncHotlink" (Zone hotlinks)
Default logix setup: Z1ManualFuncHotlink = gd_Z1FlowData[0], DINT[50]
Required length = 10 + number of robot arms in the zone
Indexing from mDeclares.vb
    Friend Enum ePLCMANUAL_ZONE_LINK
        TestTimeWd = 0			'Flow test total time(ms)
        TestTimeRemainingWd = 1	'Time remaining in active flow test(ms)
        CurrentColorWd = 2		'Current manual color
        CurrentColorChangeCycleWd = 3	'Not currently used
        RobotsSelectedWd = 4			'Robots selected for manual functions, arm1 = bit 1
        RobotsReadyWd = 5		'Robots ready for manual functions - bit 0 = zone, bit 1 = arm 1...
        BoothReadyBit = 0		
        Robot1Bit = 1
        ManualStatusWd = 6	'Calibration status word
        AutoCalActivBit = 0		'Paint/autocal active
        ScaleCalActivBit = 1	'scale cal active
        DQCalActivBit = 2		'DQ cal active
        FlowTestStatusBit = 4	'flow test active
        BeakerEnabBit = 5	'PLC enables the beaker mode button
        BeakerActiveBit = 6	'Beaker mode active
		'Device screen 
        BBZoneWord = 9		'Zone bingo board dword
        BBRobotWord = 9		'Add ARM number to this for bingo board word
        BBRobotStartWord = 10
Bingo board data is at word 9 for the zone, 10 for arm 1, 11 for arm 2.  That's per arm, not per controller

"FluidMaintHotLink_Rx":  Robot hotlinks
Default logix setup: Z1FluidMaintHotLink_R1(Z1FluidMaintHotLink_R2,...), INT[10]
The robot hot links aren't being used right now.
They could be required for CC times or estat and bell speed feedback depending on 
the robot version and electrical config.
        
"SelectRobotWord" (GUI writes to PLC)
Default logix setup: Z1SelectRobotWord = gn_Z1RobotsSelected, INT
bit 1 = arm 1, bit 2 = arm 2
    
"BeakerModeSelected" (GUI writes to PLC)
Default logix setup: Z1BeakerModeSelected =gd_Z1GUIOperate.4, BOOL
The GUI writes a 1 to this bit to enable beaker mode, 0 to turn it off.
The PLC should turn this bit off when it disables beaker mode on it's own.  like when it goes to auto or teach.

"StartCalibration" (GUI writes to PLC)
Default logix setup: Z1StartCalibration = gd_Z1GUIOperate.5, BOOL
"StartScaleCal"
Default logix setup: Z1StartCalibration = gd_Z1GUIOperate.6, BOOL
"StartDQCal"
Default logix setup: Z1StartCalibration = gd_Z1GUIOperate.7, BOOL
These start paint cal, scale cal, and DQ autocal
The GUI sets them once and then forgets them.
The cal status bits in"ManualFuncHotlink" are used to disable the buttons while it's running a calibration and
the robot status is updated with scattered access.
The "DQ screen active" bit isn't there any more.  If app disable is required for one equipment, The PLC will need to do the app disable and delay before starting DQ cal.

"SelectEngUnits" (GUI writes to PLC)
Default logix setup: Z1SelectEngUnits=gd_Z1GUIOperate.3, BOOL
Not really needed anymore, the robot handles it directly.  It's also not completely done here.
The screen writes to a bitmapped number, bit 0 = FF eng units, bit 1 = AA eng units... 
The default PLC setup is only a bit.  If it is needed at some point, most of it's there.  It'll 
just need to be tweaked for whatever science project needs it.

"StartFlowTest" (GUI writes to PLC)
Default logix setup: Z1StartFlowTest=gd_Z1GUIOperate.1, BOOL
The GUI writes a 1 to this bit to start the manual flow test.  All the flow parameters are sent to the robots directly.
The time is sent in "ManualFlowTestParam"
The GUI watches the flow test time in the zone hotlink to determine when it is done.

"AbortFlowTest" (GUI writes to PLC)
Default logix setup: Z1StartFlowTest=gd_Z1GUIOperate.2, BOOL

"ManualFlowTestParam" (GUI writes to PLC)
Default logix setup: Z1ManualFlowTestParam = gd_Z1ManualFlowParam[0], DINT[5]
Sends flow test parameters to the PLC.  Only really needs the time, maybe estats.
Word 0 = flow test time in milliseconds
Word 1 = paint flow rate
Word 2 = Atom air or bell speed
Word 3 = fan air or chaping air
Word 4 = Estats

"CCCycleSelected" (GUI writes to PLC)
Default logix setup: Z1CCCycleSelected = gn_Z1CCCycleSelected, INT
Color change cycle to run - start whenever it's > 0.  The PLC should set it back to 0

"CCSharedValveReqWord" (GUI writes to PLC)
Default logix setup: Z1CCSharedValveReqWord = gn_Z1CCSharedValveReqWord,INT
"CCValveReqWord1"
Default logix setup: Z1CCValveReqWord1 = gn_Z1CCValveReqWord1,INT
"CCValveReqWord2"
Default logix setup: Z1CCValveReqWord2 = gn_Z1CCValveReqWord2,INT
Manual valve bits from the cartoon to the PLC
"CCValveReqWord2" is only required for the newer systems with two group outputs.
It sends one bit as a time to toggle the current status.  I'm not sure why.

"CurrentManualColorWord" - (Gui reads from PLC)
Default logix setup: Z1CurrentManualColorWord=gd_Z1FlowData[2], DINT
GUI reads this when a robot is selected

"SetManualColorWord" - (GUI writes to PLC)
Default logix setup: gn_Z1ManualColor, INT
GUI writes the manual color number when a color is selected

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Scattered access
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
All the parameter feedback should be good.  some of the databases will need an update

There's an index enum type in mDeclares.vb,     Friend Enum eSAIndex
The order isn't important, just make sure anything you add is before the "Max" item.
[Robots.vb]subIndexScatteredAccess searches through the data array for all the labels.
It's set up to do this without regard to options, so it'll deal with missing items by
ignoring them.  It's arm specific, so each arm will get it's own index array into the
data array.

All of the display of scattered access data is in [frmMain.vb]subUpdateScatteredAccess
It's divided into sections to support the detailed view, which can replace the fluid
diagram cartoon for cal status feedback, or details on one of the parameters in a fluid
test.
All the "Viewselect" stuff is about that detail panel or the cartoon.
bSelected is true for the robot selected in cboRobot.  Wven when one of the detail view 
for all robots is selected, that robot has all the stuff in the flow test box and the 
color change times.
All the major feedback items should be covered, databases may need updates to match the
constants in mDeclares.vb, starting with gsSA_FLOW_RATE.
A couple items have been added for the cal status functions:
     'DQ Calibration variables
    Friend Const gsSA_DQ_CAL_ACTIVE As String = "DQ Cal Active"
    Friend Const gsSA_DQ_OUTPUT As String = "DQ Output"
    Friend Const gsSA_DQ_CAL_STATUS As String = "DQ Cal Status"
    'IC Calibration variables
    Friend Const gsSA_IC_CAL_ACTIVE As String = "IC Cal Active"
    Friend Const gsSA_IC_CAL_STATUS As String = "IC Cal Status"
Accustat and accuflow will need a little work if we do them again, too.
[frmMain.vb]subUpdateScatteredAccess, in the section labeled 'Cal status selected
there's a case statement that has the IC cal, a case would be needed for accustat and accuflow.
For accustat it should be able to deel with autocal and scale cal with the same code.

Scattered access is no longer used to write the flow test parameters to the 
robot, so these items can be removed from the database:
    'Manual Flow Test
    Friend Const gsSA_FT_FLOW_RATE As String = "FT Flow Rate"
    Friend Const gsSA_FT_AABELL_SPEED As String = "FT AA/Bell Speed"
    Friend Const gsSA_FT_FASHAPING_AIR_ As String = "FT FA/Shaping Air"
    Friend Const gsSA_FT_ESTAT As String = "FT Estat"
    Friend Const gsSA_FT_FLOW_UNITS As String = "FT Flow units"
    Friend Const gsSA_FT_BELL_UNITS As String = "FT Bell units"
    Friend Const gsSA_FT_SHAPING_UNITS As String = "FT Shaping units"
    Friend Const gsSA_FT_ESTAT_UNITS As String = "FT Estat units"

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Color change types, clsApplicator
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
It's not using the separate applicator and CC types any more.  They were pretty much redundant
and I think they caused confusion.  Now there's one Enum eColorChangeType in mDeclares.vb
The applicator data is organized in ColorChange.vb
clsApplicators is a collection of clsApplicator, similar to how it was done with arms and controllers.
clsApplicator holds parameter data for an applicator.

The color change type data table has a few items added for the applicator class:
All the database items that hold display strings are now used to read from robots.resx 
Applicator name, and long name.
The short version is loaded into the applicator type cbo for now.  So it everything with a
matching short name will be selected together.  It'll be something to mess with on mixed zones.
offset and scale are stored for DQ, KV and uA feedback
Booleans are stored to enable bell speed and DQ feedback.
EnableBellFeatures enables the bell speed range limits and the beaker mode button.

Applicator parameter DB table name - points to the table to read parameter details:
        Dim sParmName As String             'Parameter name - tag to read from resource file
        Dim sUnits As String                'Setpoint units - tag to read from resource file
        Dim nParmNum As Integer             'Parm number 
bUseCounts = true for cal table type parameters, false for motors, estat steps.
        Dim bUseCounts As Boolean           'Enable counts for manual flow test, cal table
bForceInteger = force integer values on the engineering units,(estat steps)
        Dim bForceInteger As Boolean        'Force integer setpoint (estat steps)
The full ranges are stored for both engineering units and analog (or binary) counts.
It will also try to read these from a robot.  If the robot is offline it uses the database.
        Dim fMinEngUnits As Single          'Max setpoint in engineering units
        Dim fMaxEngUnits As Single          'Min setpoint in engineering units
        Dim nMinCount As Integer            'Min counts
        Dim nMaxCount As Integer            'Max counts
Bell enabled ranges - mainly the minimum for shaping air and bell speed, just
from the database.  
        Dim fBellEnabMinEngUnits As Single  'Max setpoint with bell enabled
        Dim fBellEnabMaxEngUnits As Single  'Min setpoint with bell enabled
        Dim nBellEnabMinCount As Integer    'Min counts with bell enabled
        Dim nBellEnabMaxCount As Integer    'Max counts with bell enabled

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Color change cartoons
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
clsColorChangeCartoon is a base class that gets inherited by each color change type.
The object in frmMain is declared as clsColorChangeCartoon, but it's assigned with a new
statement to a cc type specific class in [frmMain.vb]subChangeCCType
The CC types made so far are:
clsVersabellCartoon - normal 1K verabell
clsVersabell2plus2PlusCartoon - 1K versabell plus with 32 group valve support (PaintTool 7.3)
clsGunCartoon - simple gun type
clsAccustatCartoon - it's partly here, but it's not working.  Don't copy it when making a new one.

The inheritance allows frmMain to access the 1 object without caring which type it is at the moment.
If you need to create a new type, copy one of the working classes and change the name.
Make sure you start with the right number of group valves.  
For the traditional setup (4 shared, 16 group) copy clsVersabellCartoon.
For the NGIP setup (4 shared, 32 group) copy clsVersabell2plus2PlusCartoon
Much of the code will stay the same.
In the " Declares " region you'll need to change the type of muctrlToon to a new usercontrol type
for the drawing - more on that later.
All the type specific stuff will be in subInitialize.
The simple parts - there's a reference to the CC type enum constant and the user control type.
All the labels are assigned here.  You don't need to create whole new lists in the resx file if you're 
only adding a couple valves.  See [clsVersabell2plus2PlusCartoon]subInitialize for an example.  It uses 
the standard versabell valves and only changes what it needs to.
Extra labels like outlet pressure are put in a list and passed to the user control.

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Color change cartoons user controls
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
The user controls - User controls are being used for the actual cartoon.  
Take the one closest to what you need and copy it.  As the class, the number of group valves is the biggest difference.
uctrlValve is used to create most of the valves.  It has most of the images already in there.
you can change the valve type in the design view and get it to line up correctly without having to 
search for valve images.
Most of the code can stay the same after you update the design view.
You'll need to customize uctrl{classname}_Load() and UpdateCartoons() to turn valve data into colorful pictures.
Any labels you add won't require code in the control, but they'll need to be addressed in the class and frmMain.vb

In the class subInitialize routine, it calls SetAdditionalData() with an array of strings
item 0 is a label name, item 1 is the label text...
The same routine is called from frmMain, but just for the value labels, not the label labels.

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Color change types in frmMain.vb
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Much of the code that deals with the color change cartoon won't need changes for new types because
of the inheritance.  
The objects using the inherited classes:
    CC type class from ColorChange.vb This'll get assigned a type-specific class inherited from clsColorChangeCartoon
	Private WithEvents mCCToon As clsColorChangeCartoon = Nothing
    'This'll get assigned to a cctype specific user-control that has the actual drawing.
    Friend uctrlCartoon As UserControl = Nothing
They get assigned to the cc type-specific class in subChangeCCType - there's a "Select Case mRobot.ColorChangeType" in there.
To add a new type, add copy an existing case and replace the type names for the class and usercontrol.

subUpdateScatteredAccess - There are several CC type case statements.  The "Else" cases should handle most configs.
Most of it is handled with applicator class details.  Valve control should be covered by the inheritance.
The case statement under "With mCCToon" near the end of the routine adds extra labels for the cartoon.

subSelectView has some case statements for labels, but most of it is covered by clsApplicator

subSetApplicatorLabels has a couple of CCType cases, but most of it is handled by the 
applicator class.  Special calibration or label needs can be handled here.



