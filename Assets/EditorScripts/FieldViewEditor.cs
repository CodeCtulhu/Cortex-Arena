using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (BotController))]
public class FieldViewEditor : Editor
{

    private void OnSceneGUI()
    {
        BotController bot = (BotController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(bot.transform.position,Vector3.back,Vector3.right,360,bot.viewRadius);

        Vector3 viewAngleA = bot.DirFromAngle(-bot.viewAngle / 2, false);
        Vector3 viewAngleB = bot.DirFromAngle(bot.viewAngle / 2, false);

        Handles.DrawLine(bot.transform.position, bot.transform.position + viewAngleA * bot.viewRadius);
        Handles.DrawLine(bot.transform.position, bot.transform.position + viewAngleB * bot.viewRadius);
    }
}
