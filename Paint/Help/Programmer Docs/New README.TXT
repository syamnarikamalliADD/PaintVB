Reorganized a bit with PW 4.1.3.0
FANUCManuals folder contains "R30iA GM Paint and Dispense eDoc"
If you change that, you'll need to change some links in Database\XML\Browser.XML in the OnlineManuals tags

The menu sections shouldn't need much work.

Help\Equipment Docs\ has some more organization now.  

Help\Equipment Docs\PaintToolDocs\
Index.htm is the main page in here.  It's using the Img*R30iA.htm files for 
image procedures.  I left the old files in ther, but there aren't any links to them.
Site Specific - Print the load procedure to SLP.pdf and print all the configs to Config.pdf 
and you won't need to adjust the links in the index file or Database\XML\Browser.XML.

Help\Equipment Docs\PC Docs\
PC Docs.htm - a new index page for this area
PC Setup Procedure.doc/pdf - update all the serial numbers and user names as needed.  You may 
need other adjustments to the procedure because the computers aren't always the same.
PC Image Procedures.doc/pdf - Procedures for clonezilla.  An old Norton Ghost procedure is 
still in the folder, but it's not linked anywhere.
The links all go to the pdf files, so keep PC Setup Procedure.pdf and PC Image Procedures.pdf 
in sync with the PW Install folder.  

The rest of the Equipment Doc in there are linked by Help\Equipment Docs\links.htm


Help\Welcome to PWIIIHelp.htm is the page it will open up to.  It links into the 
Equipment Doc pages and the main menu.  There also some more links in Database\XML\Browser.XML

The matching version of the product manuals are in \\hqdata\Works006\Archive_Me\PW3 Development\Paint\Help\FanucManuals
They should end up in \\Hqarch01\archive_works006\PW3 Development\Paint\Help\FanucManuals


FANUCManuals-NotGM has a version of the manuals that shipped on a non GM job and there's a Browser.XML to match.


10/1/13

With 4.1.6.0, The PWIIIs changed to PAINTworks, removed the III from file and folder names and links in code.  The last version before that is saved to Help_10_01_13.zip