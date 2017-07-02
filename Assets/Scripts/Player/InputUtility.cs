using UnityEngine;
using System.Collections;
// http://wiki.unity3d.com/index.php?title=Xbox360Controller
// http://www.davevoyles.com/tutorial-connecting-xbox-one-360-gamepad-unity/
public class InputUtility
{
    public static string GetXAxisName(int player)
    {
        return "dPadXAxis_P" + player;
    }
    public static string GetYAxisName(int player)
    {
        return "dPadYAxis_P" + player;
    }
    public static string GetJumpButtonName(int player)
    {
        return "B_P" + player;
    }

    public static string GetShootButtonName(int player)
    {
        return "A_P" + player;
    }

    public static string GetPreviousWeaponButtonName(int player)
    {
        return "LeftBumper_P" + player;
    }

    public static string GetNextWeaponButtonName(int player)
    {
        return "RightBumper_P" + player;
    }
}
