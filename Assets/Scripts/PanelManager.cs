using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelManager : SingletonMono<PanelManager>
{

    public Transform menuPanel;
    private TMP_InputField columnInpuField, rowInputField, mineCountInputField;
    private Button gameStartBtn;

    public Transform gamePanel;
    private TMP_Text flagCountTmp, timeTmp;
    private void Start()
    {

        columnInpuField = menuPanel.Find(nameof(columnInpuField)).GetComponent<TMP_InputField>();
        mineCountInputField = menuPanel.Find(nameof(mineCountInputField)).GetComponent<TMP_InputField>();
        rowInputField = menuPanel.Find(nameof(rowInputField)).GetComponent<TMP_InputField>();
        gameStartBtn = menuPanel.Find(nameof(gameStartBtn)).GetComponent<Button>();

        flagCountTmp = gamePanel.Find(nameof(flagCountTmp)).GetComponent<TMP_Text>();
        timeTmp = gamePanel.Find(nameof(timeTmp)).GetComponent<TMP_Text>();

        gameStartBtn.onClick.AddListener(() => 
        {
            if (columnInpuField.text == "" || rowInputField.text == "" || mineCountInputField.text == "")
            {
                //TODO 提示不能为空
                Debug.Log("不能为空");
                return;
            }
            int x = int.Parse(columnInpuField.text);
            int y = int.Parse(rowInputField.text);
            int c = int.Parse(mineCountInputField.text);
           
            Vector2Int t = new Vector2Int(x,y);
            //GameManager.GetT().SetBoard(t,c);
            GameManager.GetT().GameStart(t,c);
           // menuPanel.GetComponent<Animator>().Play("Hide");
        });
        GameManager.GetT().eventCenter.TimerStart += () =>
        {
           timer= StartCoroutine(TimerCoroutine());
        };
    }
    Coroutine timer;
    WaitForSeconds ws = new WaitForSeconds(1f);
    IEnumerator TimerCoroutine()
    {
        int t = 0;
        while (true)
        {
            yield return ws;
            t++;
            timeTmp.text = t.ToString();
        }
    }
    public void ReSetTimer()
    {
        if (timer!=null)
        {
            StopCoroutine(timer);
        }
    }
    int flagCount = 0;
    public void SetFlagCount(int v,bool isInit=false)
    {
        if (isInit)
        {
            flagCount = v;           
        }
        else
        {
            flagCount += v;
        }
        Debug.Log(flagCount);
        flagCountTmp.text = flagCount.ToString();

    }
}
