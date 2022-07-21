<p align="center">
  <img src="https://raw.githubusercontent.com/Bl4ckM1rror/KerberoShutdown/main/logo.PNG"> <br>
</p>

# KerberoShutdown
KerberoShutdown is a Beta Active Directory Recon tool that enumerates misconfigurations in the AD environment to identify possible attack vectors.

The aim of developing this tool is to help me learn more about Active Directory Security.

N.B.: it is currently under development, if you find bugs let me know.

## Enumeration
* Finding AS-REP Roastable users
* Finding Nested groups
* Finding Unquoted Service paths
* Finding Writable Files
* Finding possible DCSync accounts

## TODO
* Finding Unconstrained Delegation accounts
* Finding Constrained Delegation accounts
* Finding Resource Based Constrained Delegation accounts

## Usage
```
PS C:\> .\KerberoShutdown.exe --help

  _  __         _                    _____ _           _      _
 | |/ /        | |                  / ____| |         | |    | |
 | ' / ___ _ __| |__   ___ _ __ ___| (___ | |__  _   _| |_ __| | _____      ___ __
 |  < / _ \ '__| '_ \ / _ \ '__/ _ \\___ \| '_ \| | | | __/ _` |/ _ \ \ /\ / / '_ \
 | . \  __/ |  | |_) |  __/ | | (_) |___) | | | | |_| | || (_| | (_) \ V  V /| | | |
 |_|\_\___|_|  |_.__/ \___|_|  \___/_____/|_| |_|\__,_|\__\__,_|\___/ \_/\_/ |_| |_|

                           v1.0 Powered by Bl4ckM1rror

  --domainName        (Default: pentest.local) Domain to enumerate
  --Root              (Default: C:\) Root folder
  --groupName         (Default: Domain Admins) Local Group Name for Local Group Member Enumeration
  --GetWritableFiles  Enumerate all writable files on the target host
  --FindUnquotedsvc   Enumerate all unquoted services on the target host
  --GetASREPRoastable Enumerate all AS-REP Roastable users
  --GetAllMembers     Enumerate all users (also within nested groups)
  --DCSync            Enumerate all possible DCSync accounts
  --help              Display this help screen

Example: .\KerberoShutdown.exe --domainName domain.local

```

## Changelog

##### v 1.0:
    1. Added search for AS-REP Roastable users
    2. Added search for Nested groups
    3. Added search for Unquoted Service paths
    4. Added search for Writable Files
    5. Added search for possible DCSync accounts
