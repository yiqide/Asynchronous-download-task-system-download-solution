using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Dow : MonoBehaviour
{
    private string url="https://dldir1.qq.com/qqfile/QQforMac/QQ_6.7.5.dmg";
    private string filepath="/Users/a0624/Downloads/qq.dmg";
    private float dow=0;
    public Text Text;

    private DowHanld.DowTask task=null;
    private Task<byte[]> t=null;

    private void Start()
    {
        FileHanldAsync.init("/Users/a0624/Downloads/test2.txt");
    }

    private void Update()
    {
        //Text.text = dow.ToString();
        if (Input.GetKeyDown(KeyCode.S))
        {
            FileHanldAsync.addString("埃罗芒阿老师_03【独家正版】.49774079.fl");
            //FileHanldAsync.WriteAppendAsync("/Users/a0624/Downloads/test.txt","4月】埃罗芒阿老师_03【独家正版】.49774079.flv");
            //FileHanldAsync.WriteAppendAsync("/Users/a0624/Downloads/test.txt","4月】埃罗芒阿老师_03【独家正版】.49774079.flv");
            //FileHanldAsync.WriteAppendAsync("/Users/a0624/Downloads/test.txt","4月】埃罗芒阿老师_03【独家正版】.49774079.flv");

            //t= ReadFileAsync("/Users/a0624/Downloads/【4月】埃罗芒阿老师_03【独家正版】.49774079.flv");
        }

        // if (t!=null && t.IsCompleted)
        // {
        //     Debug.Log("完成:"+t.Result.Length);
        // }
        // else
        // {
        //     Debug.Log("没有完成");
        // }
    }


    public async Task<byte[]> ReadFileAsync(string file)
    {
        FileStream fileStream = new FileStream(file,FileMode.Open);
        byte[] bytes = new byte[fileStream.Length]; 
        await fileStream.ReadAsync(bytes,0,(int)fileStream.Length);
        return bytes;
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

public static class FileHanldAsync
{
    public static async Task WritedFileAsync(string path,string data)
    {
        FileStream fileStream = new FileStream(path,FileMode.OpenOrCreate);
        byte[] bytes = UnicodeEncoding.UTF8.GetBytes(data);
        await fileStream.WriteAsync(bytes,0,bytes.Length);
    }

    public static async Task WriedFileAsync(string path, byte[] data)
    {
        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
        await fileStream.WriteAsync(data, 0, data.Length);
    }

    private static FileStream fileStream;
    public static async Task WriteAppendAsync(string path,string data)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        if (FileHanldAsync.fileStream==null)
        { 
            fileStream = new FileStream(path,FileMode.Append);
        }
        data = System.Environment.NewLine+data;
        byte[] bytes = UnicodeEncoding.UTF8.GetBytes(data);
        await fileStream.WriteAsync(bytes,0,bytes.Length);
        Debug.Log("完成");
    }

    private static dataT[] strings = new dataT[10];
    private  static int point=0;
    private static int addPoint = 0;
    public static void init(string path)
    {
        if (!File.Exists(path))
        {
            var f= File.Create(path);
            f.Close();
            f.Dispose();
        }
        Thread thread = new Thread(() =>
        {
            Thread.CurrentThread.Join(100);
            fileStream = new FileStream(path, FileMode.Append);
            while (true)
            {
                if (strings[point].data!=null)
                {
                    fileStream.Write(strings[point].data,0,strings[point].data.Length);
                    Debug.Log("写入完成");
                    strings[point].data = null;
                    if (point==9)
                    {
                        point = 0;
                    }
                    else
                    {
                        point++;
                    }
                }
            }
        });
        thread.Start();
    }

    public static void addString(string data)
    {
        data = Environment.NewLine + data;
        if (strings[addPoint].data==null)
        {
            strings[addPoint].data = UnicodeEncoding.UTF8.GetBytes(data);
            if (addPoint==9)
            {
                addPoint = 0;
            }
            else
            {
                addPoint++;
            }
        }
    }
    private struct dataT
    {
        public byte[] data;
    }
}


