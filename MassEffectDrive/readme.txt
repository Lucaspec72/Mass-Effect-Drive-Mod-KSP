Mass Effect Drive (MEdrive) Mod for KSP
Made by Lucaspec72
Alpha version 2
Released 05/07/2023


--------------------


Hi, thank you for trying out the alpha version for the MEdrive mod.
This is still very much a work in progress, but should be in a mostly playable state.
If you notice any bug or have suggestions, contact me on discord (Lucaspec72 331059604072955904).

Note : The mod currently uses modified variants of Stock Parts as placeholders until proper parts are added.

What's the mod, and how does it work :


This mod adds the Mass Effect Drive, a device able to warp it's vessel's mass by a percentage using a mysterious ressource called Element Zero, one that can only be found in
trace quantities on the far reaches of Eeloo.

To use the mass effect drive, you'll need three things : Element Zero, Electric Charge and Flux Capacitors. Element Zero is costly and difficult to obtain, but on
the upside isn't consumed by the drive. Element Zero serves as catalyst for the operation of the drive, determining the Electric Charge to Warped Mass ratio. To simplify, the
more Element Zero, the better.

When The Mass Effect Drive is running, it'll generate 'Flux' that can only be Discharged around a planet's magnetic field. It must
therefore be stored in high-capacity Capacitors.

The Mass Effect Drives automatically Discharge the vessel's Flux Capacitors when in low-orbit of a planet, and the length of this process is determined by how close you are to said planet.

To prevent accidently tearing the fabric of spacetime, the drive will automatically shut itself off when the vessel is altered.

Changelog :

Alpha 2 :

-Renamed Static Charge to Flux
-Fixed issues with Flux Discharging not working
-Changed Time.fixedDeltaTime to TimeWarp.fixedDeltaTime to fix timewarp not properly setting ressources.
-Made every fixedUpdate code of the vesselModule only run when Highlogic is set to LoadedSceneIsFlight to avoid error spam in console.

Alpha 1.1 hotfix :

-Fixed Error with drive cost that caused it to be negative when empty.
-Gets placeholder models directly from the squad folder instead of having copies in the mod folder to be EULA compliant.

Alpha 1 :

-First Release