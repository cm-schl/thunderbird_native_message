# Thunderbird Native Messaging Demo
Code to show how native messaging can be used in Thunderbird.

This code was taken from a working addon written by me, but sensitive informations where deleted. Therefore the folder "localHandler" only contains the C# sourcecode of the program used to recieve the native messages.
Additionaly you have to insert some informations in the file manifest.json and send_to_d3.json to use it. For details on how native messaging works please see this link: https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/Native_messaging

The complete addon is used to save emails recieved or sent in Thunderbrid to the filesystem as .eml file. The "localHandler" in this specific addon saves the files always to a folder on the Windows Desktop of the acutal user. This folder is watched by a Document Management System that automatically starts an import when a new file is found. Using the addon the user can start the import to the DMS in one click.

Please note: I'm publishing this because it was difficult for me to understand how native messaging can be realized with C#. I'm pretty sure that there are much better ways to write the code of this addon - my intention is to help beginners to get started and maybe learn by feedback of others.
