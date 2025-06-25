using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllKeyPanel : MonoBehaviour
{
    [Header("����Ű ���� �г�"), SerializeField] 
    private GameObject contorllKeyPanel;

    [SerializeField] private Button openCKPanel;
    [SerializeField] private Button closeCKPanel;

    private bool isFirstLogin = true;

    private void Start()
    {
        //int isFirstLogin = PlayerPrefs.GetInt("IsFirstLogin", 0);

        //if (isFirstLogin == 0)
        //{
        //    // �� ó�� �α��� �ÿ��� ����Ű ���� �г� �ڵ����� ���
        //    contorllKeyPanel.SetActive(true);

        //    // �� ù �α��� ����� ���� ( �������ʹ� �ڵ����� �� �ߵ��� )
        //    PlayerPrefs.SetInt("IsFirstLogin", 1);

        //    // �� PlayerPrefs �� ��� ����
        //    PlayerPrefs.Save();
        //}
        //else
        //{
        //    // ù �α����� �ƴ϶��, ����Ű ���� �г� �� ���·� ����
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

        // �� ��ư Ŭ�� �̺�Ʈ ����    
        openCKPanel.onClick.AddListener(OpenControllKeyPanel);
        closeCKPanel.onClick.AddListener(CloseControllKeyPanel);
    }

    // �� ����Ű ���� �г� ���� ��ư
    public void OpenControllKeyPanel()
    {
        contorllKeyPanel.SetActive(true);
    }

    // �� ����Ű ���� �г� �ݴ� ��ư
    public void CloseControllKeyPanel()
    {
        contorllKeyPanel.SetActive(false);
    }
}
