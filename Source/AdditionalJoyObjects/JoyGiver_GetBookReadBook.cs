﻿using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace AdditionalJoyObjects {

  public class JoyGiver_GetBookReadBook : JoyGiver_InteractBuildingSitAdjacent {
    protected override Job TryGivePlayJob(Pawn pawn, Thing t) {
      Thing chair = null;
      Thing bookcase = null;
      IntVec3 cell = t.Position;
      Room room = RoomQuery.RoomAt(cell);
      int radius = 0;

      for (int i = 0; i < 4; i++) {
        IntVec3 c = t.Position + GenAdj.CardinalDirections[i];
        if (!c.IsForbidden(pawn)) {
          Building edifice = c.GetEdifice();
          if (edifice != null && edifice.def.building.isSittable && pawn.CanReserve(edifice, 1)) {
            chair = edifice;
            break;
          }
        }
      }
      if (chair == null) {
        return null;
      }

      // A radius of 349 should give a 10-tile circular radius
      // (number of tiles in the radial pattern)
      while (radius < 349) {
        // Get the tiles within the radius range, starting from the middle
        IntVec3 radCell = cell + GenRadial.RadialPattern[radius];
        // If we are still in the same room, and under a roof
        if (RoomQuery.RoomAt(radCell) == room && Find.RoofGrid.Roofed(radCell)) {
          // Add all things on this tile to thingsInRoom
          List<Thing> thingsInRoom = radCell.GetThingList();
          for (int i = 0; i < thingsInRoom.Count; i++) {
            // If an acceptable bookcase is found, save it, then stop scanning
            if ((thingsInRoom[i].def.defName == "AJO_BookRack" ||
                 thingsInRoom[i].def.defName == "AJO_Bookshelf" ||
                 thingsInRoom[i].def.defName == "AJO_Bookcase") && 
                 pawn.CanReserve(thingsInRoom[i], 1)) {
              bookcase = thingsInRoom[i];
              goto Out;
            }
          }
        }
        // Increment the radius, moving to the next tile
        radius++;
      }
      Out:
      if (bookcase == null) {
        return null;
      }

      return new Job(this.def.jobDef, t, bookcase, chair);
    }
  }
}
