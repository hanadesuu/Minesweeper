using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;

public class GameManager : SingletonMono<GameManager>
{
   
    public Transform cubeParent;
    //棋盘大小
    private Vector2Int size;
    public GameObject cube;
    public GameObject cubeIndex;
    //雷的数量
    private int mineCount;
    /// <summary>
    /// 所有的地雷坐标
    /// </summary>
    private List<Vector2Int> minePos = new List<Vector2Int>();
    private List<Vector2Int> rightFlagList = new List<Vector2Int>();
    /// <summary>
    ///对应格子上一个显示的数字信息,-1表示是雷，0表示空，其它大于0的数字表示周围雷的数量
    /// </summary>
    public int[,] info;
    public bool isFirstClick = true;

    public Camera mainCamera;
    public EventCenterSO eventCenter;
    #region Unity Event
    private void Start()
    {
       
        eventCenter.click += ClickPoint;
        eventCenter.flag += FlagPoint;
        eventCenter.rollback += RollBack;
    }
    private void ClickPoint(Vector2Int v)
    {
        if (v.x>size.x-1|| v.x < 0|| v.y > size.y - 1 || v.y < 0)
        {
            Debug.Log("超出棋盘范围");
            return;
        }
        if (isFirstClick)
        {
            GenerateMine(Vector2IntToPos(v));
        }
        ShowNum(Vector2IntToPos(v));
    }
    private void FlagPoint(Vector2Int v)
    {
        if (v.x > size.x - 1 || v.x < 0 || v.y > size.y - 1 || v.y < 0)
        {
            Debug.Log("超出棋盘范围");
            return;
        }
        int t = CubeManager.GetT().SetFlag(v);
        this.SetFlag(v, t);
    }
    private void RollBack()
    {

    }

    private void Update()
    {
        //检测点击格子
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ClickCommand clickCommand = new ClickCommand(PosToVector2Int(hit.collider.transform.position));
                CommandHandler.GetT().ExecuteCommand(clickCommand);
            }
        }
        //检测在格子插旗子或取消插旗
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                FlagCommand flagCommand = new FlagCommand(PosToVector2Int(hit.collider.transform.position));
                CommandHandler.GetT().ExecuteCommand(flagCommand);
            }
        }
        ScrollView();
        DragCamera();
    }
    float distance = 0;
    public float scrollSpeed;
    private void ScrollView()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        distance = Mathf.Clamp(distance, 8, 70);
        mainCamera.orthographicSize = distance;
    }
    private void DragCamera()
    {

        if (Input.GetMouseButton(2))
        {
            mainCamera.transform.Translate(Input.GetAxis("Mouse X") * -1, Input.GetAxis("Mouse Y") * -1, 0);

        }
    }
    #endregion
    #region 弹幕
    WaitForSeconds ws = new WaitForSeconds(2f);
    IEnumerator GetDanMuCoroutine()
    {
        GetDanMu getDanMu = new GetDanMu();
        string sendTime=string.Empty;
        getDanMu.SetRequest();
        string t0 = getDanMu.Response();
        DOb ob0 = JsonMapper.ToObject<DOb>(t0);
        sendTime = ob0.data.room[ob0.data.room.Count - 1].timeline;
        while (true)
        {
            yield return ws;
            getDanMu.SetRequest();
            string t = getDanMu.Response();
            DOb ob = JsonMapper.ToObject<DOb>(t);
            int index = ob.data.room.Count - 1;
            if (sendTime == ob.data.room[index].timeline)
            {
                
            }
            else
            {
                sendTime = ob.data.room[index].timeline;
                string strs = ob.data.room[index].text;
                // var strs = ob.data.admin.Where(item => item.nickname == "墨雨绯花");
                bool flag = Regex.IsMatch(strs, @"^flag[(][0-9]+,[0-9]+[)]$");
                bool click = Regex.IsMatch(strs, @"^click[(][0-9]+,[0-9]+[)]$");
                bool rollback = Regex.IsMatch(strs, @"^order:cancel$");


                //TODO 使用正则表达式验证输入是否违规
                if (click)
                {
                    GetCommandFromDanMu(strs, CommType.Click);
                }
                else
                if (flag)
                {
                    GetCommandFromDanMu(strs, CommType.Flag);
                }
                else if (rollback)
                {
                    GetCommandFromDanMu(strs, CommType.RollBack);
                }
                else
                {
                    Debug.Log("无法识别，请校验格式");
                }
            }
        }
    }
    private void GetCommandFromDanMu(string v, CommType commandType)
    {
        switch (commandType)
        {
            case CommType.Click:
                ClickCommand clickCommand = new ClickCommand(GetVector2Int(v));
                CommandHandler.GetT().ExecuteCommand(clickCommand);
                break;
            case CommType.Flag:
                FlagCommand flagCommand = new FlagCommand(GetVector2Int(v));
                CommandHandler.GetT().ExecuteCommand(flagCommand);
                break;
            case CommType.RollBack:
                CommandHandler.GetT().FlashBackCommand();
                break;
            default:
                break;
        }
    }

    private static Vector2Int GetVector2Int(string v)
    {
        string s = v;
        Match m = Regex.Match(s, "[(]");
        Match m1 = Regex.Match(s, ",");
        Match m2 = Regex.Match(s, "[)]");
        string x = s.Substring(m.Index + 1, m1.Index - m.Index - 1);
        string y = s.Substring(m1.Index + 1, m2.Index - m1.Index - 1);
        Vector2Int pos = new Vector2Int(int.Parse(x), int.Parse(y));
        return pos;
    }


    #endregion
    #region UI
    public void SetBoard(Vector2Int size, int count)
    {
        this.size = size;
        this.mineCount = count;
        minePos.Clear();
        rightFlagList.Clear();
        info = new int[size.x, size.y];
        isFirstClick = true;
        PanelManager.GetT().SetFlagCount(count,true);
        PanelManager.GetT().ReSetTimer();
    }
    public void GameStart(Vector2Int _size, int _count)
    {
        SetBoard(_size, _count);
        GenerateBoard();

    }
    #endregion

    #region Logic and Data


    /// <summary>
    /// 生成棋盘
    /// </summary>
    public void GenerateBoard()
    {
        StartCoroutine(GetDanMuCoroutine());

        if (cubeParent.childCount > 0)
        {
            List<GameObject> childs = new List<GameObject>();
            foreach (Transform item in cubeParent)
            {
                childs.Add(item.gameObject);
            }
            for (int i = 0; i < cubeParent.childCount; i++)
            {
                Destroy(childs[i]);
            }
        }
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject go = Instantiate(cube, Vector2IntToPos(new Vector2Int(i, j)), Quaternion.identity);
                go.transform.SetParent(cubeParent);
            }
        }
        for (int i = 0; i < size.x; i++)
        {
            GameObject gox = Instantiate(cubeIndex, Vector2IntToPos(new Vector2Int(i,-1)), Quaternion.identity);
            gox.GetComponent<CubeIndex>().SetIndex(i);
            gox.transform.SetParent(cubeParent);
           
        }
        for (int i = 0; i < size.y; i++)
        {
            GameObject goy = Instantiate(cubeIndex, Vector2IntToPos(new Vector2Int(-1, i)), Quaternion.identity);
            goy.GetComponent<CubeIndex>().SetIndex(i);
            goy.transform.SetParent(cubeParent);
        }
    }
    /// <summary>
    /// 生成地雷
    /// </summary>
    /// <param name="firstPos"></param>
    public void GenerateMine(Vector3 firstPos)
    {
        

        eventCenter.TimerStart.Invoke();

        isFirstClick = false;
        Vector2Int fPos = PosToVector2Int(firstPos);
        System.Random r = new System.Random();
        List<Vector2Int> rPos = new List<Vector2Int>();
        int count = 0;
        while (count < mineCount)
        {
            int rx = r.Next(0, size.x);
            int ry = r.Next(0, size.y);
            Vector2Int t = new Vector2Int(rx, ry);
            if (!rPos.Contains(t) && t != fPos)
            {
                rPos.Add(t);
                count++;
            }
        }
        minePos = rPos;
        for (int i = 0; i < rPos.Count; i++)
        {
            int rx = rPos[i].x;
            int ry = rPos[i].y;
            info[rx, ry] = -1;
            List<Vector2Int> dirList = new List<Vector2Int>();
            dirList.Add(rPos[i] + Vector2Int.up);
            dirList.Add(rPos[i] + Vector2Int.down);
            dirList.Add(rPos[i] + Vector2Int.left);
            dirList.Add(rPos[i] + Vector2Int.right);
            dirList.Add(rPos[i] + Vector2Int.up + Vector2Int.right);
            dirList.Add(rPos[i] + Vector2Int.up + Vector2Int.left);
            dirList.Add(rPos[i] + Vector2Int.left + Vector2Int.down);
            dirList.Add(rPos[i] + Vector2Int.right + Vector2Int.down);
            foreach (Vector2Int item in dirList)
            {
                if (item.x >= 0 && item.x <= size.x - 1 && item.y >= 0 && item.y <= size.y - 1)
                {
                    info[item.x, item.y] = info[item.x, item.y] == -1 ? -1 : info[item.x, item.y] + 1;
                }
            }
        }
    }

    /// <summary>
    /// 点击cube后更新数据 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="numTmp"></param>
    public void ShowNum(Vector3 pos)
    {
        Vector2Int t = PosToVector2Int(pos);
        if (info[t.x, t.y] == -1)
        {
            //TODO game over
            Debug.Log("Game over");
        }
        bool s = CubeManager.GetT().SetNum(t, info[t.x, t.y]);
        if (s)
        {
            Check(t);
        }
    }
    /// <summary>
    /// 检测周围八个位置是否为0，是则显示，直到显示出周围所有为0的格子及其边界格子
    /// </summary>
    /// <param name="current"></param>
    private void Check(Vector2Int current)
    {
        bool[,] isChecked = new bool[size.x, size.y];
        isChecked[current.x, current.y] = true;
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        if (info[current.x, current.y] != 0)
        {
            return;
        }
        q.Enqueue(current);
        while (q.Count > 0)
        {
            Vector2Int t = q.Dequeue();
            List<Vector2Int> dirList = new List<Vector2Int>();
            dirList.Add(t + Vector2Int.up);
            dirList.Add(t + Vector2Int.down);
            dirList.Add(t + Vector2Int.left);
            dirList.Add(t + Vector2Int.right);
            dirList.Add(t + Vector2Int.up + Vector2Int.right);
            dirList.Add(t + Vector2Int.up + Vector2Int.left);
            dirList.Add(t + Vector2Int.left + Vector2Int.down);
            dirList.Add(t + Vector2Int.right + Vector2Int.down);

            foreach (Vector2Int item in dirList)
            {
                if (item.x >= 0 && item.x <= size.x - 1 && item.y >= 0 && item.y <= size.y - 1)
                {
                    if (info[item.x, item.y] == 0)
                    {
                        if (!isChecked[item.x, item.y])
                        {
                            q.Enqueue(new Vector2Int(item.x, item.y));
                            isChecked[item.x, item.y] = true;
                        }
                    }
                    else
                    {
                        if (!isChecked[item.x, item.y])
                        {
                            isChecked[item.x, item.y] = true;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < isChecked.GetLength(0); i++)
        {
            for (int j = 0; j < isChecked.GetLength(1); j++)
            {
                if (isChecked[i, j])
                {
                    CubeManager.GetT().SetNum(new Vector2Int(i, j), info[i, j]);
                }
            }
        }
    }
    /// <summary>
    /// 插上棋子后更新数据
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="isFlag"></param>
    public void SetFlag(Vector2Int pos, int isFlag)
    {

        Vector2Int t = pos;
        var p = minePos.Where(i => i == t);
        if (p.Count() == 1)
        {
            if (isFlag==1)
            {
                rightFlagList.Add(t);
            }
            else if(isFlag==0)
            {
                rightFlagList.Remove(t);
            }
        }
        if (isFlag == 1)
        {
          
            PanelManager.GetT().SetFlagCount(-1);
        }
        else if (isFlag == 0)
        {
            
            PanelManager.GetT().SetFlagCount(1);
        }
        int temp = 0;
        if (rightFlagList.Count == mineCount)
        {
            foreach (Vector2Int item in rightFlagList)
            {
                if (info[item.x, item.y] == -1)
                {
                    temp++;
                }
            }
        }
        if (temp == mineCount)
        {
            //TODO game pass
            Debug.Log("Game Pass");
        }
    }
    public bool IfOverMineCout()
    {
        if (rightFlagList.Count >= mineCount)
        {
            return true;
        }
        return false;
    }
    public Vector2Int PosToVector2Int(Vector3 pos)
    {
        float x = pos.x + (float)size.x / 2 - 0.5f;
        float y = pos.y + (float)size.y / 2 - 0.5f;
        return new Vector2Int((int)x, (int)y);
    }
    public Vector3 Vector2IntToPos(Vector2Int pos)
    {
        float x = pos.x - (float)size.x / 2 + 0.5f;
        float y = pos.y - (float)size.y / 2 + 0.5f;
        return new Vector3(x, y, 0);
    }
    #endregion
    #region Test
    [ContextMenu("测试 0")]
    public void Test0()
    {
        GetDanMu getDanMu = new GetDanMu();
        getDanMu.SetRequest();
        string t = getDanMu.Response();

        DOb ob = JsonMapper.ToObject<DOb>(t);
        var strs = ob.data.admin.Where(item => item.nickname == "墨雨绯花");
        Debug.Log(strs.LastOrDefault().text);
    }
    #endregion
}
