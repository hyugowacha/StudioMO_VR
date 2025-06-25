using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllKeyPanel : MonoBehaviour
{
    [Header("조작키 설명 패널"), SerializeField] 
    private GameObject contorllKeyPanel;

    [SerializeField] private Button openCKPanel;
    [SerializeField] private Button closeCKPanel;

    private bool isFirstLogin = true;

    private void Start()
    {
        //int isFirstLogin = PlayerPrefs.GetInt("IsFirstLogin", 0);

        //if (isFirstLogin == 0)
        //{
        //    // ▼ 처음 로그인 시에는 조작키 설명 패널 자동으로 띄움
        //    contorllKeyPanel.SetActive(true);

        //    // ▼ 첫 로그인 기록을 저장 ( 다음부터는 자동으로 안 뜨도록 )
        //    PlayerPrefs.SetInt("IsFirstLogin", 1);

        //    // ▼ PlayerPrefs 값 즉시 저장
        //    PlayerPrefs.Save();
        //}
        //else
        //{
        //    // 첫 로그인이 아니라면, 조작키 설명 패널 끈 상태로 시작
        //    contorllKeyPanel.SetActive(false);
        //}

        if (isFirstLogin)
        {
            contorllKeyPanel.SetActive(true);

            isFirstLogin = false;
        }
        else
        {
            contorllKeyPanel.SetActive(false);
        }

        // ▼ 버튼 클릭 이벤트 연결    
        openCKPanel.onClick.AddListener(OpenControllKeyPanel);
        closeCKPanel.onClick.AddListener(CloseControllKeyPanel);
    }

    // ▼ 조작키 설명 패널 여는 버튼
    public void OpenControllKeyPanel()
    {
        contorllKeyPanel.SetActive(true);
    }

    // ▼ 조작키 설명 패널 닫는 버튼
    public void CloseControllKeyPanel()
    {
        contorllKeyPanel.SetActive(false);
    }
}
