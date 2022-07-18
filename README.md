# PS3-Save-Decrypter
A save decrypter for PS3.  
Download [here](./build/GSecPs3Decrypter.exe).  

- games.conf is downloaded from public url
- if database doesn't contains info for the game, there might be more comprehesive versions with it.  
(reading from file is disabled due to issues, see todo)

## Changelog:

- ReadConfigFromtext2, auto-releases.  
better games.conf parsing code.  

- fix game search algo
match by TitleID.Substring(0,9) or Title  
optimize: only per dir load - new SavMan and key search ; make ps3SaveManager public   
refactor Form1.cs  
switch output to console, add logging.  

- universality fix (1)  
get SecureFileID from games.conf (2) based on titleID from SFO   
Param.SFO -> TitleID-> games.conf -> SecureFileID  
(1) restore and fix cut feature  
(2) downloads if not presented  
minor: add .gitignore

## TODO:
- refactor xDownloadAldosGameConfig: read from file, not url; use bigger db; 