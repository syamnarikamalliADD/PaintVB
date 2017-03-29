This folder contains the Master Help Files for each of the previously documented applicator 
types. "Master Help File.doc" contains instructions for configuring this help file for your
Project.

If your project includes PaintTool or DispenseTool help, copy the whole PaintTool or 
DispenseTool help folder for the 'Tool version you are running

	MCRA3PT5102901E - PaintTool Version 5.10
	MCRA3PN5210901E - PaintTool Version 5.20
	MCRA3PT5301011E - PaintTool Version 5.30
	MCRA3DT5301011E - DispenseTool Version 5.30

to your \paint\help folder. If PAINTworks is installed somewhere other than C:\Paint, 
you will need to modify the setting for "frnasource" in 
..\paint\help\MCRA3*********E\startup.bat. 

For Paint/DispenseTool versions prior to 5.30, before running PAINTworks with 'Tool 
Help, run ..\paint\help\MCRA3*********E\ASTAware\server.exe manually by double clicking on 
it. The serach engine that this file starts will map a drive X: on the GUI and you must 
tell it "OK" manually the first time you run it. 

The registry entry HKEY_LOCAL_MACHINE\SOFTWARE\FANUC\PAINTworks\PToolHelpFolder must be 
set to the name of the help folder. 
(ex. for PaintTool V5.30, PToolHelpFolder = "MCRA3PT5301011E")


Screens that could use some work:
Password, Scheduler haven't been updated for the v4 screens.  They're pretty close, though.
Looks like file copy needs a little work
DMON Viewer needs work for v4
BSD needs work
