This folder contains projects that build PLCComm.dll
Screens can link to PLCComm.dll instead of the form \Common.NET\frmPLC\ and the communication drivers.
When a screen is linked to PLCComm.dll, any of these PLCcomm dlls can be copied to the vbapps folder without rebuilding the screen.

Each of these projeccts uses the interface \Common.NET\frmPLC\infPLCForm.vb.  It's important to keep all the calls the same between the different PLCs.

The biggest change to get these built in a DLL is removal of clsZone.  It requires a small change to the code that uses it.

For a project done the old way:
Theres a link to \Common.NET\frmPLC\cls*.vb and a reference to the driver dll
Typical code:
            With rPLC
                .Zone = oZone
                sTag = sZonePrefix & "ManualFuncHotlink"
                .TagName = sTag
                sHotLinkData = .PLCData
            End With


For a project using the PLCComm.dll
	Build the appropriate PLCComm.dll for the PLC you're using.  Each PLCComm project puts the dll in the project folder.  Copy PLCComm.dll, PLCComm.pdb, and PLCComm.xml to vbapps.  This only needs to be done once for all the screens.

No link to PLC form.  No reference directly to the drivers, just reference \VBApps\PLCComm.dll

Add this to imports section of each vb file that uses the PLC
	Imports clsPLCComm = PLCComm.clsPLCComm
Typical code - change to pass in the zone name instead of a zone class:
            With rPLC
                .ZoneName = oZone.Name
                sTag = sZonePrefix & "ManualFuncHotlink"
                .TagName = sTag
                sHotLinkData = .PLCData
            End With

