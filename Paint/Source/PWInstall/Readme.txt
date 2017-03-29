Here's an attempt at an installation program.
use PWInstall.config to configure the setup.
Put the executable and PWInstall.config in the root of the install DVD

PWInstall.config has some setup lines followed by a bunch of commands
It's tab delimited, so this line:
	PaintFolderSource,.\Paint\
sets the copy from path relative to the location PWInstall is run from.
! marks comments.

It uses the following setup items:
PaintFolderSource,path - source Paint (or Sealer) folder.  
PaintFolderDest,path - Where it's getting installed.
Robots,RC_1,RC_2 - List of controller names so it can build the folder structure.
DuplicatePaintFolder,path - option to build a 2nd paint folder.
RobotBackupFolders,Master Backups,Robot Image Backups,Temp Backups - It'll make these folders with each robot subfolder.

And then the commands:
extract,zipfile - extracts a zip file to the paint folder,
copyfile,pathFrom,pathTo - simple copy.  If there's no PathTo it uses the paint folder
copyfolder,pathFrom,pathTo - folder copy.  If there's no PathTo it uses the paint folder
run,command, description - it  shells out whatever is after the comma.
vbs,path, description - runs a script
msi,path, description - runs an installation program
regimport,path,description - import registry
