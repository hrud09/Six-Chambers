using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundChangePopUp : PopupBase
{
    public void EnableView()
    {
        SetView(true);
    }
    public void DisableView()
    {

        AudioManager.CallPlaySFX(Sound.RoundChangePopUp);
        SetView(false);
    }
    protected override void OnPopupEnabled()
    {
    }
    protected override void OnPopupDisabled()
    {
    }
}
