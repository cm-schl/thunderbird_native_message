# Thunderbird Native Messaging Demo
Code to show how native messaging can be used in Thunderbird (https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Native_messaging).

This code was taken from a working addon written by me, but sensitive informations where deleted. Therefore the folder "localHandler" only contains the C# sourcecode of the program used to recieve the native messages.
Additionaly you have to insert some informations in the file manifest.json and localHandler/send_to_d3.json to use it.

The complete addon is used to save emails recieved or sent in Thunderbrid to the filesystem as .eml file. The "localHandler" in this specific addon saves the files always to a folder on the Windows Desktop of the acutal user. This folder is watched by a Document Management System that automatically starts an import when a new file is found. Using the addon the user can start the import to the DMS in one click.
