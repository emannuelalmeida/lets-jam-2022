using System.Collections.Generic;

public class Timer
{
    private readonly Dictionary<int, bool> countdownActive = new Dictionary<int, bool>();
    private readonly Dictionary<int, int> countdownTimer = new Dictionary<int, int>();

    public bool countdown(int id, int ticksCount)
    {
        if (!countdownActive.ContainsKey(id))
        {
            countdownActive[id] = true;
            countdownTimer[id] = ticksCount;
        }

        if (!countdownActive[id])
        {
            countdownActive[id] = true;
            countdownTimer[id] = ticksCount;
        }

        if (--countdownTimer[id] == 0)
        {
            countdownActive[id] = false;
            return true;
        }

        return false;
    }

}
