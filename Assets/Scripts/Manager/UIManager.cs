using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void PlayAgainButton ()
    {
        SoundManager.instance.PlaySound("start");
        GameManager.instance.RestartScene ();
    }
}
