﻿This program will scan your system for network interfaces and prompt a user to select which one to scan.

Based on the subnet mask, the network scan will run the whole range with the option to dump to a log file (default is project directory/Logs/log.txt). 

Following the network scan, the user can select a host and scan the ports listed in the provided text file (this file can be added to based on user requirements).

Both the network and port scanning are multithreaded optimizing the process.

Supplicant files - MacList.txt and PortList.txt providing contextual information.  \
Both MacList and PortList are in the format of "00-00-00|Example" (OUI|Vendor) and "1234|Example" (Port#|Service Info) for ports.

Config file should be created on run as filepath/Common/Config/config.ini.

LOG_FILE_PATH - Location of the destination for the log file \
MAC_LIST_PATH - Location of the destination for the list of MAC addresses and vendors \
PORT_LIST_PATH - Location of the common ports list \
FULL_PORT_SCAN - This will enable full scanning from the list of ports or the list found in the CUSTOM_PORTS setting. Must be true or false
CUSTOM_PORTS= A space or comma separated list of integers eg. 1 2 80 8008 \


To do:  \
[x] Save log files  \
[-] Custom config  \
[-] Host specific tools (check for open ports etc)  \
[ ] Enhance scanning tools   \
[ ] ?	

Branch note:	\
master: fully working features	\
beta: experimental changes	\
x.x.x: here be dragons	

Credit to giuliocomi on Github for the arpscanner class (https://github.com/giuliocomi/arp-scanner)
