Currently supports robot variable, PLC and DB items.
The structure for registry items is set up, but the read and write routines are not written yet.

PLC and robot tags run through a conversion routine frmMain.sConvertTags.
It gets applied to both program and variable names.
Multiversion decoding
'{#VERx.xx#}var_name;#VERx.xx#var_name ... #VERx.xx##NOT_USED#
'First version optional, if no version is listed it's a default
'If the first version is listed, lower versions get this item disabled
'Assumes versions are in order
'For a variable that isn't used above a final version, add the version and #NOT_USED#
version must be listed as 3 digits with a decimal point.
            
controller, arm, equipment tags:
'Simple tags - these are simple replacements.  
If there is a #Equip#, #Arm# or #Parm it'll automatically enable the arm combobox
If there's a #Controller# tag, but none of the others it'll add a controller combobox
#Equip# gets replaced by the equipment # within the controller
#Arm# gets replaced bythe arm # in the zone
#Controller# gets replaced bythe controller # in the zone
#ParmX#, X = 1...6, A little more complicated. For Eq1 it gets replaced it uses X, for Eq2 it's X+6


PW3(4) setup was added into ParamSetup for this version.  
The DB option writes to other databases instead of this table.  Any database this screen writes to needs a primary key.
I've only been trying it with a single row DB, but it should be able to operate with row numbers, or maybe with a select statement.

PLC read write - other than basic types, it can also do strings.  They will read and write to a PLC array.  It'll use the array length
from the tag to be the string length.  It's storing 1 character per array item, doesn't matter if it's SINT,INT, or DINT.  

Robot setup - 
It uses the multi-version stuff described above.  
Dual-arm and NGIP versions are in the DB, I haven't added older items yet.
I reorganized the tabs a bit and removed items we shouldn't be changing.
New tab layout:
Colorchange - common color change items by controller, not by equipment.
Arm Options - CC options and timing by arm
Common options - shell options by controller, not by arm
Estat and tracking tabs - stay about the same.

A couple items show up on multiple tabs.  
Gun on delay and gun off delay are in "Arm Options" for dual arm (6.3- 7.2)and "common options" for NGIP (7.30+)
You can uncheck the "display" selection for an item in the DB to make it go away, but
leave everything enabled in the standard DB



