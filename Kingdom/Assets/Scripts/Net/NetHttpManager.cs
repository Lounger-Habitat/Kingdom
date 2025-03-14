using System.Collections;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System;

public class NetHttpManager : MonoBehaviour
{
    public static NetHttpManager Instance;
    void Awake()
    {
        Instance = this;
    }
    private string sseUrl = "http://localhost:8000/test/multi"; // 替换为你的 SSE 服务地址
    public float reconnectInterval = 5f;

    string postData = "{\"model_id\": \"us.anthropic.claude-3-5-sonnet-20241022-v2:0\",\"messages\": [{\"role\":\"user\",\"content\": \"output 100 word!\"}],\"temperature\": 0,\"maxTokens\": 100,\"stream\": true}";

    private SSEDownloadHandler sseDownloadHandler;
    void Start()
    {
        //StartCoroutine(ConnectSSE());
    }
    public void POST(string url, string message, Action<string> callBack)
    {
        //string msg = "{\"model_id\": \"us.anthropic.claude-3-5-sonnet-20241022-v2:0\",\"messages\": [{\"role\":\"user\",\"content\":\""+message+"\"}],\"temperature\": 0,\"maxTokens\": 1000,\"stream\": false}";
        Debug.Log(message);
        //Debug.Log(msg);
        StartCoroutine(PostRequest($"http://127.0.0.1:8000/{url}", message, callBack));
    }

    IEnumerator PostRequest(string url, string jsonData, Action<string> callBack)
    {
        Debug.Log("start post");
        // 创建一个UnityWebRequest对象，设置请求方法为POST
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 将JSON数据转换为字节数组
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 设置请求的UploadHandler
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求的DownloadHandler
        request.downloadHandler = new DownloadHandlerBuffer();

        // 设置请求头，指定内容类型为application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        // 检查是否有错误
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // 请求成功，处理响应
            Debug.Log(request.downloadHandler.text);
            callBack?.Invoke(request.downloadHandler.text);
        }
    }
    IEnumerator PostAndListenSse()
    {
        Debug.Log("1");
        using (UnityWebRequest webRequest = new UnityWebRequest(sseUrl))// , "POST"))
        {
            // 设置请求头
            // webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "text/event-stream");

            // 设置 POST 数据
            // string postData = "{\"initialData\": \"Hello, SSE!\"}";
            // webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(postData));
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 发起请求
            var res = webRequest.SendWebRequest();
            Debug.Log("2");
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("3");
                // 逐行读取 SSE 数据
                while (true)
                {
                    Debug.Log("4");
                    string data = webRequest.downloadHandler.text;
                    if (!string.IsNullOrEmpty(data))
                    {
                        Debug.Log("Received: " + data);
                    }
                    Debug.Log("5");
                    // 模拟流式处理
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    public void ChatWithConnectSSE(string url,string data,Action<string> CallBack)
    {
        actionStr = CallBack;
        StartCoroutine(ConnectSSE(url,data,CallBack));
    }
    Action<string> actionStr;
    private IEnumerator ConnectSSE(string url,string data,Action<string> CallBack)
    {
        //StartCoroutine(ParseSSE(CallBack));
        while (true)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "text/event-stream");
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

                sseDownloadHandler = new SSEDownloadHandler();
                request.downloadHandler = sseDownloadHandler;
                request.disposeDownloadHandlerOnDispose = true;

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(reconnectInterval);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"SSE Error: {request.error}");
                }
                else
                {
                    Debug.Log("SSE connection closed by the server.");
                    sseDownloadHandler = null;
                    break;
                }
            }

            sseDownloadHandler = null;
            yield return new WaitForSeconds(reconnectInterval);
        }
        //StopCoroutine(ParseSSE(CallBack));
        actionStr = null;
        yield return new WaitForSeconds(2.0f);//等待2秒后通知对话结束
        CallBack.Invoke(@"{""event"": ""stop:end_SSE""}");
    }

    private void Update()
    {
        if (sseDownloadHandler != null)
        {
            //Debug.Log("123");
            string eventData;
            while (sseDownloadHandler.TryGetEvent(out eventData))
            {
                ProcessEvent(eventData, actionStr);
            }
        }
    }

    private void ProcessEvent(string eventData, Action<string> CallBack)
    {
        string[] lines = eventData.Split('\n');
        string eventType = "message";
        StringBuilder dataBuilder = new StringBuilder();
        string id = null;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            int colonIndex = line.IndexOf(':');
            if (colonIndex == -1) continue;

            string field = line.Substring(0, colonIndex).Trim();
            string value = line.Substring(colonIndex + 1).Trim();
            dataBuilder.AppendLine(value);
            switch (field)
            {
                case "event":
                    eventType = value;

                    break;
                case "data":
                    //dataBuilder.AppendLine(value);
                    break;
                case "id":
                    id = value;
                    break;

            }
        }

        string data = dataBuilder.ToString().TrimEnd();
        //Debug.Log($"Event Type: {eventType}, Data: {data}");
        CallBack?.Invoke(data);
    }
}

public class SSEDownloadHandler : DownloadHandlerScript
{
    private ConcurrentQueue<string> eventQueue = new ConcurrentQueue<string>();
    private StringBuilder buffer = new StringBuilder();

    public SSEDownloadHandler() : base(new byte[4096]) { }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || dataLength == 0) return false;

        string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
        chunk = chunk.Replace("\r\n", "\n").Replace('\r', '\n');
        buffer.Append(chunk);

        ProcessBuffer();
        return true;
    }

    private void ProcessBuffer()
    {
        int index;
        while ((index = buffer.ToString().IndexOf("\n\n")) != -1)
        {
            string eventData = buffer.ToString(0, index).Trim();
            buffer.Remove(0, index + 2);
            eventQueue.Enqueue(eventData);
        }
    }

    public bool TryGetEvent(out string eventData)
    {
        return eventQueue.TryDequeue(out eventData);
    }
}
