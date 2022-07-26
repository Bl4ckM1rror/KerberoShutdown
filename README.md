<p align="center">
  <img src="https://github.com/Bl4ckM1rror/KerberoShutdown/blob/master/logo.PNG"> <br>
</p>

# :skull: KerberoShutdown :zap:
KerberoShutdown is an Active Directory Recon tool that enumerates misconfigurations in the AD environment to identify possible attack vectors.

All you need to have is a machine joined on the Domain.

The aim of developing this tool is to help me learn more about Active Directory Security(ADS). 

[*] it is currently under development, if you find bugs let me know.

## Enumeration
* Finding all group members (also within Nested groups)
* Finding Unquoted Service paths
* Finding Writable Files
* Finding all UAC flags of a specific user
* Finding AS-REP Roastable accounts
* Finding possible DCSync accounts
* Finding Unconstrained Delegation accounts
* Finding Constrained Delegation accounts
* Finding Resource Based Constrained Delegation accounts
* Create Hidden Domain Admin account on Domain Controller (for Persistence purpose)

## TODO
* Scan for sAMAccountName spoofing

## Usage
```
PS C:\> .\KerberoShutdown.exe --help

    _  __         _                    _____ _           _      _
   | |/ /        | |                  / ____| |         | |    | |
   | ' / ___ _ __| |__   ___ _ __ ___| (___ | |__  _   _| |_ __| | _____      ___ __
   |  < / _ \ '__| '_ \ / _ \ '__/ _ \\___ \| '_ \| | | | __/ _` |/ _ \ \ /\ / / '_ \
   | . \  __/ |  | |_) |  __/ | | (_) |___) | | | | |_| | || (_| | (_) \ V  V /| | | |
   |_|\_\___|_|  |_.__/ \___|_|  \___/_____/|_| |_|\__,_|\__\__,_|\___/ \_/\_/ |_| |_|

                           v1.1 Powered by Bl4ckM1rror

  --GetWritableFiles  Enumerate all writable files on the target host
  --FindUnquotedsvc   Enumerate all unquoted services on the target host
  --GetAllMembers     Enumerate all users (also within nested groups)
  --GetUACFlags       Enumerate all UAC flags of a specific user
  --GetASREPRoastable Enumerate all AS-REP Roastable users
  --DCSync            Enumerate all possible DCSync accounts
  --UnconstrainDeleg  Enumerate all Unconstrained Delegation accounts
  --ConstrainDeleg    Enumerate all Constrained Delegation accounts
  --RBCD              Enumerate all Resource-based Constrained Delegation accounts
  --HiddenDA          Create Hidden Domain Admin account on Domain Controller
 
Example: 
         .\KerberoShutdown.exe --FindUnquotedsvc
         .\KerberoShutdown.exe --GetWritableFiles --root C:\ --fileFormat *.dll
         .\KerberoShutdown.exe --GetAllMembers --groupName <group_name> --domainName <domain_name>
         .\KerberoShutdown.exe --GetUACFlags --user <user_name>
         .\KerberoShutdown.exe --GetASREPRoastable
         .\KerberoShutdown.exe --DCSync
         .\KerberoShutdown.exe --UnconstrainDeleg
         .\KerberoShutdown.exe --ConstrainDeleg
         .\KerberoShutdown.exe --RBCD --domainName <domain_name>
         .\KerberoShutdown.exe --HiddenDA --da <fake_domain_admin> --password <password_value>
         
```

## Changelog

##### v 1.1:
    1. Added search for Unconstrained Delegation accounts
    2. Added search for Constrained Delegation accounts
    3. Added search for all UAC flags of a specific user
    4. Added search for Resource-based Constrained Delegation accounts
    5. Added feature for create hidden Domain Account
    6. Fixed generic bugs

##### v 1.0:
    1. Added search for AS-REP Roastable accounts
    2. Added search for Nested groups
    3. Added search for Unquoted Service paths
    4. Added search for Writable Files
    5. Added search for possible DCSync accounts
