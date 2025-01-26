using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BackEnd;
using System.Threading.Tasks;

//뒤끝 차트에서 받은 상점 데이터 정보용 클래스
public class ShopItem
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public int ItemShopValue { get; set; }
    public string ItemInfo { get; set; }
}

public class BackendChart
{
    //인스턴스
    private static BackendChart _instance = null;
    private static readonly object _lock = new object();

    //상점 관련
    public List<ShopItem> shopItems = new List<ShopItem>(); //상점 데이터 저장 리스트
    private BackendReturnObject chartData; //차트 상점 데이터 저장 필드

    public static BackendChart Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new BackendChart();
                }

                return _instance;
            }
        }
    }

    public void InitializeShopInfo(System.Action onComplete)
    {
        GetChartAsync("160546");
        SetShopInfo();
        onComplete?.Invoke();

    }

    //차트 불러오기 함수
    private void GetChartAsync(string chartId)
    {
        Debug.Log($"{chartId}의 차트 불러오기를 요청합니다.");

        chartData = Backend.Chart.GetChartContents(chartId);
        

        if (chartData.IsSuccess() == false)
        {
            Debug.LogError($"{chartId}의 차트를 불러오는 중, 에러가 발생했습니다. : {chartData}");
            return;
        }

        Debug.Log("차트 불러오기에 성공했습니다. : {chartData}");
    }

    //상점 정보 가져오는 함수
    public void SetShopInfo()
    {
        if (chartData == null || chartData.IsSuccess() == false)
        {
            Debug.LogError("SetShopInfo 호출 전에 차트 데이터를 먼저 로드하세요.");
            return;
        }

        shopItems.Clear(); // 기존 데이터 초기화

        foreach (LitJson.JsonData gameData in chartData.FlattenRows())
        {
            try
            {
                // ShopItem 객체 생성 및 데이터 매핑
                ShopItem item = new ShopItem
                {
                    ItemId = gameData["item_id"].ToString(),
                    ItemName = gameData["item_name"].ToString(),
                    ItemShopValue = int.Parse(gameData["item_shopValue"].ToString()),
                    ItemInfo = gameData["item_info"].ToString()
                };

                // 리스트에 추가
                shopItems.Add(item);

                /*
                // 디버깅 로그 출력 (모든 데이터를 출력)
                StringBuilder content = new StringBuilder();
                content.AppendLine($"item_id: {item.ItemId}");
                content.AppendLine($"item_name: {item.ItemName}");
                content.AppendLine($"item_shopValue: {item.ItemShopValue}");
                content.AppendLine($"item_info: {item.ItemInfo}");
                Debug.Log(content.ToString());*/
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"데이터 파싱 중 오류 발생: {ex.Message}");
            }
        }

        Debug.Log($"총 {shopItems.Count}개의 아이템이 로드되었습니다.");
    }
}