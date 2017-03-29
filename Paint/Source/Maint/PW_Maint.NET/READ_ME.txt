Version 4.1.4.2 has been modified to read the robot controller
auto backup timeout value in ms from PLC tag 
Z<ZoneNumber>AutoBackupTimeout. This way the values in the GUI 
and the PLC will always agree and can be modied in one place 
(the PLC data table).

The value in this PLC register must also be used in the PLC 
program to load the presets of the auto backup timeout timers for 
each robot controller.