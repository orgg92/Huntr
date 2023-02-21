This program will scan your system for network interfaces and prompt a user to select which one to scan.

Based on the subnet mask, the network scan will run the whole range with the option to dump to a log file (default is project directory/Logs/log.txt). 

Following the network scan, the user can select a host and scan the ports listed in the provided text file (this file can be added to based on user requirements).

Both the network and port scanning are multithreaded optimizing the process.

Supplicant files - MacList.txt and PortList.txt providing contextual information.  

To do:  \
[x] Save log files  \
[-] Host specific tools (check for open ports etc)  \
[ ] Custom config  \
[ ] Enhance scanning tools   \
[ ] ?	

Branch note:	\
master: fully working features	\
beta: experimental changes	\
x.x.x: here be dragons	
