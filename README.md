# Pen Recombobulator
I bought a new pen for my Huion tablet. It works but has 3 distinct issues:
- Inverted pressure: The pen will always draw, even when hovering
- Y-tilt is inverted
- First 3 pressure readings are BOGUS, causing sporadic strokes when pen is first detected
  
I made this plugin to fix each issue separately.

if your pen's being equally stubborn, you might want to try! :D

build this bad boy 
```
dotnet build --configuration Release PenRecombobulator.csproj 
```