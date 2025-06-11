using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ��Ƽ �÷��̿��� ���� ��Ȳ�� �����ִ� ��ŷ �г�
/// </summary>
public class RankingPanel : Panel
{
    private enum Index: byte
    {
        Member1,
        Member2,
        Member3,
        Member4,
        End
    }

    [SerializeField]
    private Color[] colors = new Color[(int)Index.End];

    [SerializeField]
    private Animator[] animators = new Animator[(int)Index.End];

    [SerializeField]
    private TMP_Text[] scoreTexts = new TMP_Text[(int)Index.End];

#if UNITY_EDITOR
    private void OnValidate()
    {
        Color[] colors = new Color[(int)Index.End];
        for (int i = 0; i < Mathf.Clamp(this.colors.Length, 0, (int)Index.End); i++)
        {
            colors[i] = this.colors[i];
        }
        this.colors = colors;
        ExtensionMethod.Sort(ref animators, (int)Index.End);
        ExtensionMethod.Sort(ref scoreTexts, (int)Index.End);
    }
#endif

    //���� �÷��̾���� ��ŷ�� �����ִ� �г�
    public void Show(IEnumerable<Character> characters)
    {
        if(characters != null)
        {

        }
        else
        {

        }
    }
}