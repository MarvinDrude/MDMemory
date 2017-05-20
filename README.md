# MDMemory
Simple small class that enables you to read and write memory of any process running.

Usage:
```C#

MDMemory mem = new MDMemory("csgo");
int mClient = (int)mem.GetModuleAddress("client.dll");
int someAddress = mClient + 0x000000FC;
int addressValue = mem.ReadInt32((IntPtr)someAddress);

```
