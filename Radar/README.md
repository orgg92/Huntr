The program scans your system and returns a complete list of interfaces.

The prompt asks the user to select which network to scan in which you must type one of the corresponding integer values.

The program will then proceed to scan the network for hosts, also using ARP to obtain the MAC address and device info for each host on the network.

The number of hosts is determined from the subnet, a larger subnet could take some time to complete a full scan - though your typical network will probably be a /24 network.

To do:
- Save log files
- Custom config
- Enhance scanning tools 
- Host specific tools (check for open ports etc)

Branch note:
master: fully working features
beta: experimental changes
x.x.x: here be dragons
