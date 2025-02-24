# Pen Recombobulator
I bought a new pen for my Huion tablet. It works but has 3 distinct issues:
- The pressure is inverted, so the pen will always draw, even when hovering
- Y-tilt is flipped
- The first 3 pressure readings are bogus (around 0.5% → 32% → 0.5%), causing random strokes when the pen first comes into range.
  
I made this plugin to fix each of these issues separately. If your tablet is being equally stubborn, you might want to give it a try! :D

**How it works**:

- You can invert the pressure and flip the x/y tilt.
- For fixing the initial readings, it compares the first few pressure values against a neutral baseline and if there's an unreasonable spike, forces them to that level, so you don't get unwanted strokes at the start.

**Building**

`dotnet build --configuration Release PenRecombobulator.csproj`
