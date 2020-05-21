using System;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;

class MainClass {
  /**
	 * 商户编号 
	 */
	public static String MERCHANT_NO = "";

  /**
	* api key
  * 联系客服获取测试账户与域名
	*/
	public static String API_KEY = "";
	
	
	public static void Main()
	{
		//注1 !!! 所有请求参数格式均使用 首字母小写驼峰格式 eg: orderNo(订单编号), notifyUrl(通知地址), 
		//注2 !!! http|https请使用小写 (错误示例: HTTP, HTTPS) 回调地址正确示例: https://www.baidu.com  回调地址错误示例: HTTPS://www.baidu.com
		
		//创建订单
		//CreateOrder();
		
		
		//查询订单
		//QueryOrder();
		
		
		//回调验签
		//CallbackVerify();
	}
	
	
	/**
	 *	创建订单, 页面跳转形式
	 */
	public static void CreateOrder()
	{
		String ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) + "";
		
		SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
		parameters.Add("amount", "100");
		parameters.Add("orderNo", "00000000000001");
		parameters.Add("merchantNo", MERCHANT_NO);
		parameters.Add("orderType", "ISSUED");
		parameters.Add("payMode", "ebank");

		//以下两项, 下发订单必填, 充值单不填
		parameters.Add("fundAccountNo", "628888888888888888");
		parameters.Add("fundAccountUser", "测试资金人");

		parameters.Add("fundAccountName", "中国银行");
		parameters.Add("notifyUrl", "https://www.baidu.com/");
		parameters.Add("returnUrl", "https://www.baidu.com/");
		parameters.Add("ts", ts);

		
		//拼接参数
		String paramsOrder = GetParamsOrder(parameters);
		Console.WriteLine(paramsOrder);
		//签名
		String signStr = Sign(API_KEY, paramsOrder);
		//UrlEncode
		signStr = WebUtility.UrlEncode(signStr);
		//请求地址
    String s = "网关地址+/pk-order/#/create?" + paramsOrder + "&sign=" + signStr;
    Console.WriteLine(s);		
  
	}
	
	
	/*
	 * 查询订单
	 * 回调验签demo
	 * (仅供参考, 实际使用自行修改)
	 */
	public static void QueryOrder()
	{
		String host = "host/open/order/query";
		String ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) + "";
		SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
		parameters.Add("orderNo", "000000004");
		parameters.Add("merchantNo", MERCHANT_NO);
		parameters.Add("ts", ts);
		
		//拼接参数
		String paramsOrder = GetParamsOrder(parameters);
		Console.WriteLine(paramsOrder);
		//签名, sign不需要UrlEncode
		String signStr = Sign(API_KEY, paramsOrder);
		parameters.Add("sign", signStr);
		Console.WriteLine(signStr);
		
		//此处为字典转换后json数据, 实际使用引入json库 对parameters对象序列化即可
		String jsonData = "{\"orderNo\":\"000000004\",\"merchantNo\":\"20200507151719795154545655\",\"ts\":\"" + ts + "\",\"sign\":\"" + signStr + "\"}";
		
		var request = (HttpWebRequest)WebRequest.Create(host);
		request.Method = "POST";
		request.ContentType = "application/json;charset=UTF-8";
		var byteData = Encoding.UTF8.GetBytes(jsonData);
		var length = byteData.Length;
		request.ContentLength = length;
		var writer = request.GetRequestStream();
		writer.Write(byteData, 0, length);
		writer.Close();
		var response = (HttpWebResponse)request.GetResponse();
		//查询结果
		var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
		Console.WriteLine(responseString);
		//TODO 验签 --> 与回调验签一致

	}
	
	
	/*
	 *	回调验签
	 *	模拟代码, 仅供参考, 实际使用自行修改
	 */ 
	public static void CallbackVerify()
	{
		//模拟查询响应结果 与回调参数验签
		String result = "{\"amount\":1000,\"realAmount\":1000,\"orderNo\":\"000000004\",\"systemOrderNo\":\"20200513111313358109498205\",\"merchantNo\":\"20200507151719795154545655\",\"orderType\":\"ISSUED\",\"payMode\":\"ebank\",\"fundAccountNo\":\"628888888888888888\",\"fundAccountUser\":\"测试资金人\",\"fundAccountName\":\"中国银行\",\"fundAccountUrl\":null,\"voucherUrl\":\"https://timgsa.baidu.com/timg.jpg\",\"payTime\":1589340782,\"ts\":1589339592,\"payStatus\":30,\"orderStatus\":50,\"sign\":\"8beabab16d42f6920d0ba92b6f66d373\"}";

		//此处模拟回调(查询)响应参数, 实际使用引入json库序列化即可
		SortedDictionary<string, string> resultDictionary = new SortedDictionary<string, string>();

    resultDictionary.Add("amount", "1000");
    resultDictionary.Add("realAmount", "1000");
    resultDictionary.Add("orderNo", "000000004");
    resultDictionary.Add("systemOrderNo", "20200513111313358109498205");
    resultDictionary.Add("merchantNo", "20200507151719795154545655");
    resultDictionary.Add("orderType", "ISSUED");
    resultDictionary.Add("payMode", "ebank");
    resultDictionary.Add("fundAccountNo", "628888888888888888");
    resultDictionary.Add("fundAccountUser", "测试资金人");
    resultDictionary.Add("fundAccountName", "中国银行");
    //resultDictionary.Add("fundAccountUrl",null);
    resultDictionary.Add("voucherUrl","https://timgsa.baidu.com/timg.jpg");
	resultDictionary.Add("payTime", "1589340782");
    resultDictionary.Add("ts", "1589339592");
    resultDictionary.Add("payStatus", "30");
    resultDictionary.Add("orderStatus", "50");
		resultDictionary.Add("sign", "8beabab16d42f6920d0ba92b6f66d373");
		
		
		//拼接参数
		String resultParam = GetParamsOrder(resultDictionary);
		//签名
		String signStr = Sign(API_KEY, resultParam);
		//校验
		Console.WriteLine(CheckHash(resultDictionary["sign"], signStr));
		
	}
	
	
	
	
	/*
	 * 验签
	 */
	public static Boolean CheckHash(String sign, String encryptSign)
	{
		return String.Equals(sign, encryptSign, StringComparison.CurrentCultureIgnoreCase);
	}
	
	
	/*
	 * 签名
	 */
	public static String Sign(String apiKey, String paramsOrder)
	{
		String signStr = paramsOrder + "&key=" + API_KEY;
		return MD5Encrypt(signStr);
	}

  public static string MD5Encrypt(string str)
  {
    MD5 md5 = MD5.Create();
    // 将字符串转换成字节数组
    byte[] byteOld = Encoding.UTF8.GetBytes(str);
    // 调用加密方法
    byte[] byteNew =  md5.ComputeHash(byteOld);
    // 将加密结果转换为字符串
    StringBuilder sb = new StringBuilder();
    foreach (byte b in byteNew)
    {
        // 将字节转换成16进制表示的字符串，
        sb.Append(b.ToString("x2"));
    }
    // 返回加密的字符串
    return sb.ToString();
  }

	/*
	 * 排序拼接
	 */
	public static string GetParamsOrder(IDictionary<string, string> parameters)
	{
		//排序
		IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
		IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

		//拼接
		StringBuilder query = new StringBuilder("");
		while (dem.MoveNext())
		{
			string key = dem.Current.Key;
			string value = dem.Current.Value;
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !"sign".Equals(key))
			{
				query.Append(key).Append("=").Append(WebUtility.UrlEncode(value)).Append("&");				
			}
		}
		return query.ToString().Substring(0, query.Length - 1);
	}
}
