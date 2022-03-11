

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public class GetDanMu
{
    string url = "https://api.live.bilibili.com/xlive/web-room/v1/dM/gethistory?roomid=1820641";
    HttpWebRequest request;
    public void SetRequest()
    {
        this.request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "application/json, text/javascript, */*; q=0.01";
        request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
    }
    public string Response()
    {
        HttpWebResponse response = (HttpWebResponse)this.request.GetResponse();
        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
}
public class DOb
{
    public int code;
    public DanMuJs data;
}
public class DanMuJs
{
    public List<Admin> admin;
    public List<Room> room;
}
public class Admin
{
    public string text;
    public int uid;
    public string nickname;
    public string timeline;
}
public class Room
{
    public string text;
    public int uid;
    public string nickname;
    public string timeline;
}