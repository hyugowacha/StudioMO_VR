using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    [SerializeField] StagePanelType stagePanelType;

    [SerializeField] SelectStagePanel selectStagePanel;

    public void OnClickSelectStage()
    {
        selectStagePanel.OnClickStageButton(stagePanelType);
    }
}
