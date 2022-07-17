# PS3-Save-Decrypter
A save decrypter for PS3

## Configuration

- if games.conf is not presented, it is downloaded from public url
- if database doesn't contains info for the game ("0 Files Decrypted"), there might be more comprehesive versions with it

## Changelog:

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
- refactor: games.conf parsing code (ReadConfigFromtext)
- find larger games.conf.  
- add logging to DownloadAldosGameConfig.  