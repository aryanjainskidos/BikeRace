namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class PowerRatingManager : MonoBehaviour
{

    /**
	 * Cik reitingpunktus dos katra apgreida katrs límenis
	 * ir 11 límenji (nulltais + 10)
	 */

    private static int[] AccelerationRating = new int[] { 0, 10, 20, 30, 40, 50, 70, 95, 100, 120, 140 };
    private static int[] AccelerationStartRating = new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 50, 60 };
    private static int[] MaxSpeedRating = new int[] { 0, 20, 40, 60, 80, 100, 120, 130, 140, 170, 200 };
    private static int[] BreakSpeedRating = new int[] { 0, 10, 20, 30, 40, 50, 60, 70, 75, 90, 100 };



    public static int Calculate(int AccelerationLvl, int AccelerationStartLvl, int MaxSpeedLvl, int BreakSpeedLvl)
    {
        return AccelerationRating[AccelerationLvl] + AccelerationStartRating[AccelerationStartLvl] + MaxSpeedRating[MaxSpeedLvl] + BreakSpeedRating[BreakSpeedLvl];
    }

    //used to calculate temp upgrade values
    // AccelerationLvl, AccelerationStartLvl, MaxSpeedLvl, BreakSpeedLvl should not be < 0
    public static int[] IncreasePowerEvenlyBy(int amount, int AccelerationLvl, int AccelerationStartLvl, int MaxSpeedLvl, int BreakSpeedLvl)
    {
        int iterations = 0;
        while (amount > 0 && iterations < 60)
        {

            if (amount > 0 && AccelerationLvl + 1 < AccelerationRating.Length)
            {
                AccelerationLvl++;
                amount -= (AccelerationRating[AccelerationLvl] - AccelerationRating[AccelerationLvl - 1]); //decrese amount by power delta
            }

            if (amount > 0 && AccelerationStartLvl + 1 < AccelerationStartRating.Length)
            {
                AccelerationStartLvl++;
                amount -= (AccelerationStartRating[AccelerationStartLvl] - AccelerationStartRating[AccelerationStartLvl - 1]);
            }

            if (amount > 0 && MaxSpeedLvl + 1 < MaxSpeedRating.Length)
            {
                MaxSpeedLvl++;
                amount -= (MaxSpeedRating[MaxSpeedLvl] - MaxSpeedRating[MaxSpeedLvl - 1]);
            }

            if (amount > 0 && BreakSpeedLvl + 1 < BreakSpeedRating.Length)
            {
                BreakSpeedLvl++;
                amount -= (BreakSpeedRating[BreakSpeedLvl] - BreakSpeedRating[BreakSpeedLvl - 1]);
            }

            iterations++;//failsafe
        }

        return new int[] { -amount, AccelerationLvl, AccelerationStartLvl, MaxSpeedLvl, BreakSpeedLvl };
    }
}

}
