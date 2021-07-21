using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Dow : MonoBehaviour
{
    private string url="https://dldir1.qq.com/qqfile/QQforMac/QQ_6.7.5.dmg";
    private string filepath="/Users/a0624/Downloads/qq.dmg";
    private string url2 = "https://upos-sz-mirrorkodo.bilivideo.com/upgcxcode/79/40/49774079/49774079_da8-1-112.flv?e=ig8euxZM2rNcNbR37buBhwdlhW4j7zUVhoNvNC8BqJIzNbfqXBvEqxTEto8BTrNvN0GvT90W5JZMkX_YN0MvXg8gNEV4NC8xNEV4N03eN0B5tZlqNxTEto8BTrNvNeZVuJ10Kj_g2UB02J0mN0B5tZlqNCNEto8BTrNvNC7MTX502C8f2jmMQJ6mqF2fka1mqx6gqj0eN0B599M=&";

    private float dow=0;
    public Text Text;

    private DowHanld.DowTask task=null;

    private void Update()
    {
        Text.text = dow.ToString();
        if (Input.GetKeyDown(KeyCode.S))
        {
            task= DowHanld.CorectTask(url,filepath,"one");
            task.Start();
        }

        if (task!=null)
        {
            Text.text = task.DowDateSzie.ToString();
        }
    }


}
/// <summary>
/// 异步下载任务系统下载解决方案
/// </summary>
public static class DowHanld
{
    private static List<DowTask> dowTaskList_NotStarted=new List<DowTask>();
    private static List<DowTask> dowTaskList_UnderWay=new List<DowTask>();
    private static List<DowTask> dowTaskList_Done=new List<DowTask>();
    private static List<DowTask> dowTaskList_Defeat=new List<DowTask>();

    public static DowTask CorectTask(string url,string path,string taskName)
    {
        return new DowTask(path, url,taskName);
    }

    public static List<DowTask> DowTasks_NotStarte
    {
        get { return dowTaskList_NotStarted; }
    }

    public static List<DowTask> DowTaskList_UnderWay
    {
        get { return dowTaskList_UnderWay; }
    }

    public static List<DowTask> DowTaskList_Done
    {
        get { return dowTaskList_Done; }
    }

    public static List<DowTask> DowTaskList_Defeat
    {
        get { return dowTaskList_Defeat; }
    }

    
    public class DowTask
{
    public string path;
    public string url;
    private DowState state;
    public string name="";
    public DowState State
    {
        get { return state; }
        set
        {
            switch (value)
            {
                case DowState.Defeat:
                    RemoveOdlState(state);
                    break;
                case DowState.Done: 
                    RemoveOdlState(state);
                    break;
                case DowState.NotStarted: 
                    RemoveOdlState(state);
                    break;
                case DowState.UnderWay: 
                    RemoveOdlState(state);
                    break;
            }
            state = value;
        }
    }

    private void RemoveOdlState(DowState value)
    {
        switch (value)
        {
            case DowState.NotStarted:
                dowTaskList_NotStarted.Remove(this);
                break;
            case DowState.UnderWay:
                dowTaskList_UnderWay.Remove(this);
                break;
            case DowState.Done:
                dowTaskList_Done.Remove(this);
                break;
            case DowState.Defeat:
                dowTaskList_Defeat.Remove(this);
                break;
            default:
                return;
        }
    }

    public float DowDateSzie=0;
    public void Start()
    {
        State = DowState.UnderWay;
        Task t = new Task(() =>
        {
            var s= DownloadAssetAsync(url, path);
            s.Wait();
        });
        t.Start();
        
    }
    private async Task DownloadAssetAsync( string url,string filePath)
    {
        try
        {
            WebRequest Myrq =HttpWebRequest.Create(url);
            WebResponse myrp = Myrq.GetResponse();
            Stream st = myrp.GetResponseStream();
            Stream so = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            byte[] by = new byte[1024*16];
            int osize = st.Read(by, 0, (int)by.Length);
            int count = 0;
            int speed;
            while (osize > 0)
            {
                so.Write(by, 0, osize);
                osize = st.Read(by, 0, (int)by.Length);
                count++;
                DowDateSzie= count*(float)16/1024;
                
            }
            so.Close();
            so.Dispose();
            st.Close();
            st.Dispose();
            myrp.Close();
            myrp.Dispose();
            Myrq.Abort();
            State = DowState.Done;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            State = DowState.Defeat;
        }
    }

    public DowTask(string path,string url,string name)
    {
        this.path = path;
        this.url = url;
        this.name = name;
        state = DowState.NotStarted;
    }
}

    public enum DowState
{
    NotStarted,
    UnderWay,
    Done,
    Defeat
}
}


