接口文档(v.200521)
=
文档内容最后更新于：2020-05-21 

<span style="color:red !important;"> 特别注意：</span>
-
1.**时间戳为秒级，非毫秒级，毫秒级请/1000**

2.**returnUrl/notifyUrl 为完整地址,含有协议+端口。如果回调通知地址（notifyUrl）不传，平台不会发起异步回调，需要调用查询接口确认订单状态。**

3.**金额为整数，非小数，以分为单位，不能包含有“·”号**,例：123 即 1.23 元。

4.**商户编号需要从商户后台首页获取，并非登陆账号**，商户密钥（apikey）每次刷新都会重新随机生成，保存好最后一次刷新的密钥进行对接即可。

5.**商户接收异步通知时，不要写死固定参数接收，请使用通用的json/map 对象接收，这样可接收完整参数，然后对json/map 里面的参数进行签名校验。如果只接收固定参数，会导致签名验证失败。后期如果通知增加参数，也可以不用修改代码**

6.**商户测试时如果要确认能不能回调以及验证签名是否成功，可生成订单后直接取消，取消后系统便会有通知。测试完取消通过后，再联系客服测试成功订单**


接口规范
-
1.字符编码：UTF-8

2.Content-Type：application/json

3.URL 传输参数需要对参数进行 UrlEncode

接口调用所需条件
-
1.网关地址：请联系客服

2.商户编号(merchantNo)

3.商户密钥(apiKey)

签名（sign）算法
-
DigestUtils.md5Hex(originalStr + "key=" + apiKey)

1.originalStr: 除sign参数外其他参数值非空（空值或者空字符串）的参数按参数名称字母正序排序然后以name=UrlEncode(value)形式组合， 通过&拼接得到结果将apiKey拼接在最后。<br>
i.注：空值（空值或者空字符串）不参与签名。<br>
ii.注：value需要进行UrlEncode编码

示例:
amount=100&fundAccountName=%E6%B5%8B%E8%AF%95%E5%95%86%E6%88%B7%E7%9A%84%E6%94%AF%E4%BB%98%E5%AE%9D&fundAccountNo=123456789&fundAccountUser=%E6%B5%8B%E8%AF%95%E5%95%86%E6%88%B7&merchantNo=20200507151719795154545655&notifyUrl=https%3A%2F%2Fwww.baidu.com%2F&orderNo=00000000000001&orderType=ISSUED&payMode=ebank&returnUrl=https%3A%2F%2Fwww.baidu.com%2F&ts=1590052086&sign=6785507C5026704FF24511345DFE687


2.DigestUtils.md5Hex(originalStr + "key=" + apiKey) <br>
i.用DigestUtils.md5Hex算法将“originalStr + "key=" + apiKey”进行加密得到签名信息

3.[c# demo](https://github.com/CloudProxyPay/document/blob/master/C%23/demo.cs)

4.[java demo](https://github.com/CloudProxyPay/document/blob/master/java/demo.java)

5.[php demo](https://github.com/CloudProxyPay/document/blob/master/php/demo.php)

同步通知 （returnUrl）
-
当创建订单时传入返回地址，订单结束后，用户点击“返回商户”，会在返回链接带上参数（returnUrl?urlparams）。参数内容参考[统一返回参数](https://github.com/CloudProxyPay/document/blob/master/README.md#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0)，可通过签名算法计算签名的正确性。例：<br>
returnUrl?<br>

    amount=10000&
    
    realAmount=10000&
    
    orderNo=000000004&
    
    systemOrderNo=20200513111313358109498205&
    
    merchantNo=20200507105901967155208220&
    
    orderType=ISSUED&
    
    payMode=ebank&
    
    fundAccountNo=6217003260005288000&
    
    fundAccountUser=张三&
    
    fundAccountName=中国银行&
    
    fundAccountUrl=https://timgsa.baidu.com/timg.jpg&
    
    voucherUrl=https://timgsa.baidu.com/timg.jpg&
    
    payTime=1581586702&
    
    ts=1589786373
    
    payStatus=30&
    
    orderStatus=50&

    sign=e88479b960c7193221c3e3eb1c32a5f4&
    
    
 异步回调 （notifyUrl）
 -
当创建订单时传入异步回调地址时，订单结束后（用户取消订单(-30)、用户支付超时（-40）、订单失败（-50）、订单已完成（50））进行通知，总共通知3次，每次间隔10 分钟，超时时间为10s，处理成功后返回 success，返回其他字符表示处理失败，会继续进行后续通知。通知内容参考[统一返回参数](https://github.com/CloudProxyPay/document/blob/master/README.md#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0)，可通过签名算法计算签名的正确性 例：<br>
curl -X POST "回调地址"<br>
  -H 'content-type: application/json' <br>
  -d '{<br>
  
    amount=10000&
    
    realAmount=10000&
    
    orderNo=000000004&
    
    systemOrderNo=20200513111313358109498205&
    
    merchantNo=20200507105901967155208220&
    
    orderType=ISSUED&
    
    payMode=ebank&
    
    fundAccountNo=6217003260005288000&
    
    fundAccountUser=张三&
    
    fundAccountName=中国银行&
    
    fundAccountUrl=https://timgsa.baidu.com/timg.jpg&
    
    voucherUrl=https://timgsa.baidu.com/timg.jpg&
    
    payTime=1581586702&
    
    ts=1589786373
    
    payStatus=30&
    
    orderStatus=50&

    sign=e88479b960c7193221c3e3eb1c32a5f4&

}'

订单接口内容
-
1.创建下发提现订单接口

i.使用场景：当商户创建时，根据下面参数，生成订单信息。<br>
ii.请求方式：页面跳转 <br>
iii.请求地址：网关地址+/cloud-order/#/create?urlparams  <br>
iv.请求参数

参数名称  | 必须  | 数据类型 | 示例| 参数说明
 ---- | ----- | ------  | ------    | ------
 amount  | 是 | 整数 | 10000 | 金额,以分为单位；最小值100，即1元
 orderNo  | 是 | 字符串(<50) | 1589786340457 | 商户订单编号
 merchantNo  | 是 | 字符串 | 20200507105901967155208220 | 商户编号
 orderType  | 是 | 字符串 | ISSUED | 订单类型，请参考订单类型枚举
 payMode  | 是 | 字符串 | ebank | 支付模式，请参考支付模式枚举
 fundAccountNo  | 下发是，提现否 | 字符串(<50) | 6217003260005288000 | 资金账户编号
 fundAccountUser  | 下发是，提现否 | 字符串(<50) | 张三 | 资金账户人
 fundAccountName  | 下发否，提现否 | 字符串(<50) | 张三 | 资金账户名称
 ts  | 是 | 整数 | 1589788853 | 商户订单时间戳（秒级）
 notifyUrl  | 否 | 字符串 | https://www.baidu.com/notify | 后台通知地址
 returnUrl  | 否 | 字符串 | https://www.baidu.com | 支付完成用户返回地址
 sign  | 是 | 字符串 | 1c3ae12c00b95dce4c253bb7... | 参数签名，请按照签名算法生成

统一返回参数
-
1.参数内容

 参数名称  | 必须  | 数据类型 | 示例| 参数说明
 ---- | ----- | ------  | ------    | ------
 amount  | 是 | 整数 | 10000 | 金额,以分为单位
 realAmount  | 是 | 整数 | 10000 | 实际金额,以分为单位
 orderNo  | 是 | 字符串 | 20200518151933726139693961 | 商户编号
 systemOrderNo  | 是 | 字符串 | 20200518151933726139693961 | 系统订单编号
 merchantNo  | 是 | 字符串 | 20200507105901967155208220 | 商户编号
 orderType  | 是 | 整数 | ISSUED | 订单类型，请参考订单类型枚举
 payMode  | 是 | 字符串 | ebank | 支付模式，请参考支付模式枚举
 fundAccountNo  | 否 | 字符串 | 6217003260005288000 | 资金账户编号
 fundAccountUser  | 否 | 字符串 | 张三 | 资金账户用户
 fundAccountName  | 否 | 字符串 | 中国银行 | 资金账户名称
 fundAccountUrl  | 否 | 字符串 |  | 资金账户地址
 voucherUrl  | 否 | 字符串 |  | 凭证地址
 payTime  | 否 | 整数 | 1575948756 | 支付时间（秒级）
 ts  | 是 | 整数 | 1575948756 | 商户订单时间戳（秒级）
 payStatus  | 是 | 整数 | 30 | 支付状态，请参考支付状态枚举
 orderStatus  | 是 | 整数 | 50 | 订单状态，请参考订单状态枚举
 sign  | 是 | 字符串 | e88479b960c7193221c3e3eb1c32a5f4 | 签名
 
2.订单状态（orderStatus）枚举

值  | 说明  
 ---- | -----
 20  | 等待接单
 30  | 支付等待中
 -30  | 用户取消订单
 40  | 支付中
 -40  | 用户支付超时
 -50  | 订单失败
 50  | 订单已完成
    
3.支付状态（payStatus）枚举
 
  值  | 说明  
 ---- | -----  
 5  | 等待接单中
 10  | 等待支付 
 -10  | 支付超时
 -20  | 支付取消
 20  | 支付中
 -20  | 支付取消
 30  | 支付成功
 -30  | 支付失败

4.订单类型（orderType）枚举
 
  值  | 说明  
 ---- | -----  
 ISSUED  | 下发
 RECHARGE  | 充值 
 
 5.支付模式（payMode）枚举
 
  值  | 说明  
 ---- | -----  
 ebank  | 网银
 alipay  | 支付宝 

**以订单状态为主进行判断，支付超时后状态可能会收到支付成功状态通知，请注意处理**
